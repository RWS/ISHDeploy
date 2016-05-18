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
$filepath = "$xmlPath\Web{0}\InfoShareWS" -f $testingDeployment.OriginalParameters.projectsuffix
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

$scriptBlockRemoveISHIntegrationSTSCertificate = {
    param (
        $ishDeployName,
        $issuer
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }

    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Remove-ISHIntegrationSTSCertificate -ISHDeployment $ishDeploy -Issuer $issuer

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

$authorWebConfigFilePath


# Function reads target files and their content, searches for specified nodes in xml
$scriptBlockReadTargetXML = {
    param(
        $Issuer,
        $ValidationMode,
        $xmlPath,
        $testingDeployment
    )
    #read all files that are touched with commandlet
    [System.Xml.XmlDocument]$authorWebConfig = new-object System.Xml.XmlDocument
    $authorWebConfig.load("$xmlPath\Web{0}\Author\ASP\Web.config" -f $testingDeployment.OriginalParameters.projectsuffix)
    [System.Xml.XmlDocument]$wsWebConfig = new-object System.Xml.XmlDocument
    $wsWebConfig.load("$xmlPath\Web{0}\InfoShareWS\Web.config" -f $testingDeployment.OriginalParameters.projectsuffix)
    [System.Xml.XmlDocument]$stsWebConfig = new-object System.Xml.XmlDocument
    $stsWebConfig.load("$xmlPath\Web{0}\InfoShareSTS\Web.config" -f $testingDeployment.OriginalParameters.projectsuffix )
    
    $result =  @{}
    #get variables and nodes from files
    $result["authorWebConfigNodes"] = $authorWebConfig.SelectNodes("configuration/system.identityModel/identityConfiguration/issuerNameRegistry/trustedIssuers/add[@name='$Issuer']")
    $result["authorWebConfigValidationMode"] = $authorWebConfig.SelectNodes("configuration/system.identityModel/identityConfiguration/certificateValidation[@certificateValidationMode='$ValidationMode']")
    $result["wsWebConfigNodes"] = $wsWebConfig.SelectNodes("configuration/system.identityModel/identityConfiguration/issuerNameRegistry/trustedIssuers/add[@name='$Issuer']")
    $result["wsWebConfigNodesValidationMode"] = $wsWebConfig.SelectNodes("configuration/system.identityModel/identityConfiguration/certificateValidation[@certificateValidationMode='$ValidationMode']")
    $result["stswebConfigNodes"] = $stsWebConfig.SelectNodes("configuration/system.serviceModel/behaviors/serviceBehaviors/behavior/addActAsTrustedIssuer[@name='$Issuer']")

    return $result
}
function remoteReadTargetXML() {
    param(
        $Issuer,
        $ValidationMode
    )
    #read all files that are touched with commandlet
    $result = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockReadTargetXML -Session $session -ArgumentList $Issuer, $ValidationMode, $xmlPath, $testingDeployment
    
    #get variables and nodes from files
    $global:authorWebConfigNodes = $result["authorWebConfigNodes"]
    $global:authorWebConfigValidationMode = $result["authorWebConfigValidationMode"]    
    $global:wsWebConfigNodes = $result["wsWebConfigNodes"]
    $global:wsWebConfigNodesValidationMode = $result["wsWebConfigNodesValidationMode"]
    $global:stswebConfigNodes = $result["stswebConfigNodes"]
}


Describe "Testing ISHIntegrationSTSCertificate"{
    BeforeEach {
        if(RemotePathCheck "$filepath\_Web.config")
        {
            if (RemotePathCheck "$filepath\Web.config")
            {
                RemoteRemoveItem "$filepath\Web.config"
            }
            RemoteRenameItem "$filepath\_Web.config" "Web.config"
        }

        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockUndoDeployment -Session $session -ArgumentList $testingDeploymentName
    }

    It "Set ISHIntegrationSTSCertificate"{       
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationSTSCertificate -Session $session -ArgumentList $testingDeploymentName, "testThumbprint", "testIssuer", "PeerOrChainTrust" -WarningVariable Warning
        #Assert
        remoteReadTargetXML -Issuer "testIssuer" -ValidationMode "PeerOrChainTrust"
        
        $authorWebConfigNodes.Count | Should be 1
        $authorWebConfigNodes[0].thumbprint | Should be "testThumbprint"
        $authorWebConfigValidationMode.Count | Should be 1
        $wsWebConfigNodes.Count | Should be 1
        $wsWebConfigNodes[0].thumbprint | Should be "testThumbprint"
        $wsWebConfigNodesValidationMode.Count | Should be 1
        $stswebConfigNodes.Count | Should be 1
        $Warning | Should be $null 
    }


    It "Set ISHIntegrationSTSCertificate with wrong XML"{
        #Arrange
        # Running valid scenario commandlet to out files into backup folder before they will ba manually modified in test
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationSTSCertificate -Session $session -ArgumentList $testingDeploymentName, "testThumbprint", "testIssuer", "PeerOrChainTrust"
        Rename-Item "$filepath\Web.config"  "_Web.config"
        New-Item "$filepath\Web.config" -type file |Out-Null
        
        #Act/Assert
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationSTSCertificate -Session $session -ArgumentList $testingDeploymentName, "testThumbprint", "testIssuer", "PeerOrChainTrust" -ErrorAction Stop }| Should Throw "Root element is missing"
        #Rollback
        Remove-Item "$filepath\Web.config"
        Rename-Item "$filepath\_Web.config" "Web.config"
    }

    It "Set ISHIntegrationSTSCertificate with no XML"{
        #Arrange
        Rename-Item "$filepath\Web.config"  "_Web.config"

        #Act/Assert
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationSTSCertificate -Session $session -ArgumentList $testingDeploymentName, "testThumbprint", "testIssuer", "PeerOrChainTrust" -ErrorAction Stop }| Should Throw "Could not find file"
        #Rollback
        Rename-Item "$filepath\_Web.config" "Web.config"
    }

    It "Set ISHIntegrationSTSCertificate several times"{        
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationSTSCertificate -Session $session -ArgumentList $testingDeploymentName, "testThumbprint", "testIssuer", "PeerOrChainTrust"
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationSTSCertificate -Session $session -ArgumentList $testingDeploymentName, "testThumbprint222", "testIssuer", "PeerOrChainTrust"} | Should not Throw
        
        #Assert
        remoteReadTargetXML -Issuer "testIssuer" -ValidationMode "PeerOrChainTrust"
        
        $authorWebConfigNodes.Count | Should be 1
        $authorWebConfigValidationMode.Count | Should be 1
        $authorWebConfigNodes[0].thumbprint | Should be "testThumbprint222"
        $wsWebConfigNodes.Count | Should be 1
        $wsWebConfigNodesValidationMode.Count | Should be 1
        $wsWebConfigNodes[0].thumbprint | Should be "testThumbprint222"
        $stswebConfigNodes.Count | Should be 1
    }

    It "Set ISHIntegrationSTSCertificate normalizes thumbprint"{       
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationSTSCertificate -Session $session -ArgumentList $testingDeploymentName, "test T h u m b p rint  2", "testIssuer", "PeerOrChainTrust" -WarningVariable Warning
        
        #Assert
        $Warning | Should Match "has been normalized to 'testThumbprint2'"

        remoteReadTargetXML -Issuer "testIssuer" -ValidationMode "PeerOrChainTrust"
        
        $authorWebConfigNodes.Count | Should be 1
        $authorWebConfigValidationMode.Count | Should be 1
        $authorWebConfigNodes[0].thumbprint | Should be "testThumbprint2"
        $wsWebConfigNodes.Count | Should be 1
        $wsWebConfigNodesValidationMode.Count | Should be 1
        $wsWebConfigNodes[0].thumbprint | Should be "testThumbprint2"
        $stswebConfigNodes.Count | Should be 1
       
    }

    It "Set ISHIntegrationSTSCertificate normalizes thumbprint with wrong symbols"{       
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationSTSCertificate -Session $session -ArgumentList $testingDeploymentName, "test ,T h u> m<{ b! [p] rint  3", "testIssuer", "PeerOrChainTrust" -WarningVariable Warning
        
        #Assert
        ($Warning-join -'') | Should Match "has been normalized to 'testThumbprint3'"

        remoteReadTargetXML -Issuer "testIssuer" -ValidationMode "PeerOrChainTrust"
        
        $authorWebConfigNodes.Count | Should be 1
        $authorWebConfigValidationMode.Count | Should be 1
        $authorWebConfigNodes[0].thumbprint | Should be "testThumbprint3"
        $wsWebConfigNodes.Count | Should be 1
        $wsWebConfigNodesValidationMode.Count | Should be 1
        $wsWebConfigNodes[0].thumbprint | Should be "testThumbprint3"
        $stswebConfigNodes.Count | Should be 1
    }

    It "Set ISHIntegrationSTSCertificate writes proper history"{        
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationSTSCertificate -Session $session -ArgumentList $testingDeploymentName, "testThumbprint", "testIssuer", "PeerOrChainTrust"
        $history = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetHistory -Session $session -ArgumentList $testingDeploymentName
        
        #Assert
        $history.Contains('Set-ISHIntegrationSTSCertificate -ISHDeployment $deployment -Thumbprint "testThumbprint" -Issuer "testIssuer" -ValidationMode PeerOrChainTrust') | Should be "True"     
    }
    
    It "Set ISHIntegrationSTSCertificate change ValidationMode"{       
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationSTSCertificate -Session $session -ArgumentList $testingDeploymentName, "testThumbprint", "testIssuer", "Custom"
        
        #Assert
        remoteReadTargetXML -Issuer "testIssuer" -ValidationMode "Custom" 
        
        $authorWebConfigValidationMode.Count | Should be 1
        $authorWebConfigValidationMode[0].certificateValidationMode | Should be "Custom"
        $wsWebConfigNodesValidationMode.Count | Should be 1
        $wsWebConfigNodesValidationMode[0].certificateValidationMode | Should be "Custom"
    }

    It "Remove ISHIntegrationSTSCertificate"{       
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationSTSCertificate -Session $session -ArgumentList $testingDeploymentName, "testThumbprint", "testIssuer", "PeerOrChainTrust"
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationSTSCertificate -Session $session -ArgumentList $testingDeploymentName, "testThumbprint2", "testIssuer", "PeerOrChainTrust"
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationSTSCertificate -Session $session -ArgumentList $testingDeploymentName, "testThumbprint3", "testIssuer2", "PeerOrChainTrust"
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveISHIntegrationSTSCertificate -Session $session -ArgumentList $testingDeploymentName, "testIssuer"
        
        #Assert
        remoteReadTargetXML -Issuer "testIssuer" -ValidationMode "PeerOrChainTrust"
        $authorWebConfigNodes.Count | Should be 0
        $authorWebConfigValidationMode.Count | Should be 1
        $wsWebConfigNodes.Count | Should be 0
        $wsWebConfigNodesValidationMode.Count | Should be 1
        $stswebConfigNodes.Count | Should be 0

        
        remoteReadTargetXML -Issuer "testIssuer2" -ValidationMode "PeerOrChainTrust"
        $authorWebConfigNodes.Count | Should be 1
        $authorWebConfigValidationMode.Count | Should be 1
        $authorWebConfigNodes[0].thumbprint | Should be "testThumbprint3"
        $wsWebConfigNodes.Count | Should be 1
        $wsWebConfigNodesValidationMode.Count | Should be 1
        $wsWebConfigNodes[0].thumbprint | Should be "testThumbprint3"
        $stswebConfigNodes.Count | Should be 1
    }

    It "Remove ISHIntegrationSTSCertificate with wrong XML"{
        #Arrange
        # Running valid scenario commandlet to out files into backup folder before they will ba manually modified in test
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveISHIntegrationSTSCertificate -Session $session -ArgumentList $testingDeploymentName, "testIssuer"
        Rename-Item "$filepath\Web.config"  "_Web.config"
        New-Item "$filepath\Web.config" -type file |Out-Null
        
        #Act/Assert
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveISHIntegrationSTSCertificate -Session $session -ArgumentList $testingDeploymentName, "testIssuer" -ErrorAction Stop }| Should Throw "Root element is missing"
        #Rollback
        Remove-Item "$filepath\Web.config"
        Rename-Item "$filepath\_Web.config" "Web.config"
    }

    It "Remove ISHIntegrationSTSCertificate with no XML"{
        #Arrange
        Rename-Item "$filepath\Web.config"  "_Web.config"

        #Act/Assert
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveISHIntegrationSTSCertificate -Session $session -ArgumentList $testingDeploymentName, "testIssuer" -ErrorAction Stop }| Should Throw "Could not find file"
        #Rollback
        Rename-Item "$filepath\_Web.config" "Web.config"
    }

    It "Set ISHIntegrationSTSCertificate writes proper history"{        
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationSTSCertificate -Session $session -ArgumentList $testingDeploymentName, "testThumbprint", "testIssuer", "PeerOrChainTrust"
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveISHIntegrationSTSCertificate -Session $session -ArgumentList $testingDeploymentName, "testIssuer"
        $history = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetHistory -Session $session -ArgumentList $testingDeploymentName
        
        #Assert
        $history.Contains('Remove-ISHIntegrationSTSCertificate -ISHDeployment $deployment -Issuer "testIssuer"') | Should be "True"
              
    }

    It "Remove ISHIntegrationSTSCertificate - simple remove"{       
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationSTSCertificate -Session $session -ArgumentList $testingDeploymentName, "testThumbprint", "testIssuer", "PeerOrChainTrust"
        #Assert
        remoteReadTargetXML -Issuer "testIssuer" -ValidationMode "PeerOrChainTrust"
        $authorWebConfigNodes.Count | Should be 1
        $authorWebConfigValidationMode.Count | Should be 1
        $authorWebConfigNodes[0].thumbprint | Should be "testThumbprint"
        $wsWebConfigNodes.Count | Should be 1
        $wsWebConfigNodesValidationMode.Count | Should be 1
        $wsWebConfigNodes[0].thumbprint | Should be "testThumbprint"
        $stswebConfigNodes.Count | Should be 1

        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveISHIntegrationSTSCertificate -Session $session -ArgumentList $testingDeploymentName, "testIssuer"
        remoteReadTargetXML -Issuer "testIssuer" -ValidationMode "PeerOrChainTrust"
        
        $authorWebConfigNodes.Count | Should be 0
        $authorWebConfigValidationMode.Count | Should be 1
        $wsWebConfigNodes.Count | Should be 0
        $wsWebConfigNodesValidationMode.Count | Should be 1
        $stswebConfigNodes.Count | Should be 0
    }

    It "Set-ISHIntegrationSTSCertificate works after last issuer was removed"{       
      #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveISHIntegrationSTSCertificate -Session $session -ArgumentList $testingDeploymentName, "Issuer"
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationSTSCertificate -Session $session -ArgumentList $testingDeploymentName, "testThumbprint222", "Issuer", "PeerOrChainTrust"} | Should not Throw
        #Assert
        remoteReadTargetXML -Issuer "Issuer" -ValidationMode "PeerOrChainTrust"
        
        $authorWebConfigNodes.Count | Should be 1
        $authorWebConfigValidationMode.Count | Should be 1
        $authorWebConfigNodes[0].thumbprint | Should be "testThumbprint222"
        $wsWebConfigNodes.Count | Should be 1
        $wsWebConfigNodesValidationMode.Count | Should be 1
        $wsWebConfigNodes[0].thumbprint | Should be "testThumbprint222"
        $stswebConfigNodes.Count | Should be 1
    }
}