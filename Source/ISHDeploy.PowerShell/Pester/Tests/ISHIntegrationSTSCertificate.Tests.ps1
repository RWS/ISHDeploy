param(
    $session = $null,
    $testingDeploymentName = "InfoShare"
)
. "$PSScriptRoot\Common.ps1"

$computerName = If ($session) {$session.ComputerName} Else {$env:COMPUTERNAME}

#region variables
# Script block for getting ISH deployment
$scriptBlockGetDeployment = {
    param (
        [Parameter(Mandatory=$false)]
        $ishDeployName 
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
    Get-ISHDeployment -Name $ishDeployName 
}

# Generating file pathes to remote PC files
$testingDeployment = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetDeployment -Session $session -ArgumentList $testingDeploymentName
$xmlPath = $testingDeployment.WebPath
$xmlPath = $xmlPath.ToString().replace(":", "$")
$xmlPath = "\\$computerName\$xmlPath"
#endregion

#region Script Blocks 
$scriptBlockUndoDeployment = {
    param (
        [Parameter(Mandatory=$false)]
        $ishDeployName 
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Undo-ISHDeployment -ISHDeployment $ishDeploy
}

$scriptBlockSetISHIntegrationSTSCertificate = {
    param (
        $ishDeployName,
        $thumbprint,
        $issuer,
        $validationMode
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }

    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Set-ISHIntegrationSTSCertificate -ISHDeployment $ishDeploy -Thumbprint $thumbprint -Issuer $issuer -ValidationMode $validationMode

}

$scriptBlockGetHistory = {
    param (
        [Parameter(Mandatory=$false)]
        $ishDeployName 
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Get-ISHDeploymentHistory -ISHDeployment $ishDeploy
}

#endregion



# Function reads target files and their content, searches for specified nodes in xm
function readTargetXML() {
    param(
        $Thumbprint,
        $Issuer, 
        $ValidationMode  
    )
    #read all files that are touched with commandlet
    [xml]$authorWebConfig = get-content ("$xmlPath\Web{0}\Author\ASP\Web.config" -f $testingDeployment.OriginalParameters.projectsuffix ) 
    [xml]$wsWebConfig = get-content ("$xmlPath\Web{0}\InfoShareWS\Web.config" -f $testingDeployment.OriginalParameters.projectsuffix ) 
    [xml]$stsWebConfig = get-content ("$xmlPath\Web{0}\InfoShareSTS\Web.config" -f $testingDeployment.OriginalParameters.projectsuffix )  

    #get variables and nodes from files
    $authorwebConfigNodes = $wsWebConfig.configuration.'system.identityModel'.identityConfiguration.issuerNameRegistry.trustedIssuers.add | ? {$_.thumbprint -eq $Thumbprint -and $_.name -eq $Issuer }
    $authorwebConfigValidationMode = $wsWebConfig.configuration.'system.identityModel'.identityConfiguration.certificateValidation | ? {$_.certificateValidationMode -eq $ValidationMode}

    $WSwebConfigNodes = $wsWebConfig.configuration.'system.identityModel'.identityConfiguration.issuerNameRegistry.trustedIssuers.add  | ? {$_.thumbprint -eq $Thumbprint -and $_.name -eq $Issuer }
    $WSwebConfigNodesValidationMode = $wsWebConfig.configuration.'system.identityModel'.identityConfiguration.certificateValidation | ? {$_.certificateValidationMode -eq $ValidationMode}

    $stswebConfigNodes = $stsWebConfig.configuration.'system.serviceModel'.behaviors.serviceBehaviors.behavior.addActAsTrustedIssuer| ? {$_.thumbprint -eq $Thumbprint -and $_.name -eq $Issuer }


    #check conditions
    $authorCheck = $false
    if($authorwebConfigNodes -and $authorwebConfigValidationMode){
        $authorCheck = $true
    }

    $wsCheck = $false
    if($WSwebConfigNodes -and $WSwebConfigNodesValidationMode){
        $wsCheck = $true
    }
    
    #return
	if($authorCheck -and $stswebConfigNodes -and $wsCheck)
    {
		Return $true	
    }

    Return $false
}


Describe "Testing ISHIntegrationSTSCertificate"{
    BeforeEach {
            Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockUndoDeployment -Session $session -ArgumentList $testingDeploymentName
    }

    It "Set ISHIntegrationSTSCertificate with full parameters"{       
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationSTSCertificate -Session $session -ArgumentList $testingDeploymentName, "testThumbprint", "testIssuer", "PeerOrChainTrust"
        #Assert
        Start-Sleep -Milliseconds 7000
        readTargetXML -Thumbprint "testThumbprint" -Issuer "testIssuer" -ValidationMode "PeerOrChainTrust" | Should be $true
       
    }


    It "Set ISHIntegrationSTSCertificate with wrong XML"{
        #Arrange
        $filepath = "$xmlPath\Web{0}\InfoShareWS" -f $testingDeployment.OriginalParameters.projectsuffix
        # Running valid scenario commandlet to out files into backup folder before they will ba manually modified in test
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationSTSCertificate -Session $session -ArgumentList $testingDeploymentName, "testThumbprint", "testIssuer", "PeerOrChainTrust"
        Rename-Item "$filepath\Web.config"  "_web.config"
        New-Item "$filepath\Web.config" -type file |Out-Null
        
        #Act/Assert
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationSTSCertificate -Session $session -ArgumentList $testingDeploymentName, "testThumbprint", "testIssuer", "PeerOrChainTrust" -ErrorAction Stop }| Should Throw "Root element is missing"
        #Rollback
        Remove-Item "$filepath\Web.config"
        Rename-Item "$filepath\_Web.config" "web.config"
    }

    It "Set ISHIntegrationSTSCertificate with no XML"{
        #Arrange
        $filepath = "$xmlPath\Web{0}\InfoShareWS" -f $testingDeployment.OriginalParameters.projectsuffix
        Rename-Item "$filepath\Web.config"  "_web.config"

        #Act/Assert
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationSTSCertificate -Session $session -ArgumentList $testingDeploymentName, "testThumbprint", "testIssuer", "PeerOrChainTrust" -ErrorAction Stop }| Should Throw "Could not find file"
        #Rollback
        Rename-Item "$filepath\_Web.config" "web.config"
    }

    It "Set ISHIntegrationSTSCertificate several times"{        
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationSTSCertificate -Session $session -ArgumentList $testingDeploymentName, "testThumbprint", "testIssuer", "PeerOrChainTrust"
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationSTSCertificate -Session $session -ArgumentList $testingDeploymentName, "testThumbprint", "testIssuer", "PeerOrChainTrust"} | Should not Throw
        #Assert
        Start-Sleep -Milliseconds 7000
        readTargetXML -Thumbprint "testThumbprint" -Issuer "testIssuer" -ValidationMode "PeerOrChainTrust" | Should be $true
       
    }

    It "Set ISHIntegrationSTSCertificate normalizes thumbprint"{       
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationSTSCertificate -Session $session -ArgumentList $testingDeploymentName, "test T h u m b p rint", "testIssuer", "PeerOrChainTrust"
        #Assert
        Start-Sleep -Milliseconds 7000
        readTargetXML -Thumbprint "testThumbprint" -Issuer "testIssuer" -ValidationMode "PeerOrChainTrust" | Should be $true
       
    }

    It "Set ISHIntegrationSTSCertificate writes proper history"{        
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationSTSCertificate -Session $session -ArgumentList $testingDeploymentName, "testThumbprint", "testIssuer", "PeerOrChainTrust"
        $history = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetHistory -Session $session -ArgumentList $testingDeploymentName
        
        #Assert
        $history.Contains('Set-ISHIntegrationSTSCertificate -ISHDeployment $deployment -Thumbprint "testThumbprint" -Issuer "testIssuer" -ValidationMode PeerOrChainTrust') | Should be "True"
              
    }
}