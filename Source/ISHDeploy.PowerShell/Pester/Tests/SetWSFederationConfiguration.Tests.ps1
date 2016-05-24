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
$xmlPath =Join-Path $testingDeployment.WebPath ("\Web{0}\Author\ASP" -f $testingDeployment.OriginalParameters.projectsuffix )
$xmlPath = $xmlPath.ToString().replace(":", "$")
$xmlPath = "\\$computerName\$xmlPath"
#endregion

#region Script Blocks 

$scriptBlockSetWSFederation = {
    param (
        $ishDeployName,
        $endpoint
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }

    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    
    Set-ISHIntegrationSTSWSFederation -ISHDeployment $ishDeploy  -Endpoint $endpoint
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



# Function reads target files and their content, searches for specified nodes in xml
$scriptBlockReadTargetXML = {
    param(
        $xmlPath,
        $endpoint
    )
    #read all files that are touched with commandlet
    [xml]$WebConfig = get-content "$xmlPath\Web.config"  

    #get variables and nodes from files
   
    $webConfigNode = $WebConfig.configuration.'system.identityModel.services'.federationConfiguration.wsFederation  | ? {$_.issuer -eq $endpoint }
    
    #return
	if($webConfigNode)
    {
		Return $true
	}
	    
    Return $false
}
function readTargetXML() {
    param(
        $endpoint
    )
	$result = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockReadTargetXML -Session $session -ArgumentList $xmlPath, $endpoint

    return $result
}


Describe "Testing ISHIntegrationSTSWSFederation"{
    BeforeEach {
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockUndoDeploymentWithoutRestartingAppPools -Session $session -ArgumentList $testingDeploymentName
    }

    It "Set ISHIntegrationSTSWSFederation with full parameters"{   
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetWSFederation -Session $session -ArgumentList $testingDeploymentName, "testEndpoint"
        #Assert
        readTargetXML -endpoint "testEndpoint" | Should be $true
    }


    It "Set ISHIntegrationSTSWSFederation with wrong XML"{
        # Running valid scenario commandlet to out files into backup folder before they will ba manually modified in test
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetWSFederation -Session $session -ArgumentList $testingDeploymentName, "testEndpoint"
        Rename-Item "$xmlPath\Web.config"  "_web.config"
        New-Item "$xmlPath\Web.config" -type file |Out-Null
        
        #Act/Assert
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetWSFederation -Session $session -ArgumentList $testingDeploymentName, "testEndpoint" -ErrorAction Stop }| Should Throw "Root element is missing"
        #Rollback
        Remove-Item "$xmlPath\Web.config"
        Rename-Item "$xmlPath\_Web.config" "web.config"
    }

    It "Set ISHIntegrationSTSWSFederation with no XML"{
        #Arrange
        Rename-Item "$xmlPath\Web.config"  "_web.config"
        #Act/Assert
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetWSFederation -Session $session -ArgumentList $testingDeploymentName, "testEndpoint" -ErrorAction Stop }| Should Throw "Could not find file"
        #Rollback
        Rename-Item "$xmlPath\_Web.config" "web.config"
    }

    It "Set ISHIntegrationSTSWSFederation several times"{       
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetWSFederation -Session $session -ArgumentList $testingDeploymentName, "testEndpoint"
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetWSFederation -Session $session -ArgumentList $testingDeploymentName, "testEndpoint"} | Should not Throw
        #Assert
        readTargetXML -endpoint "testEndpoint" | Should be $true
    }

    It "Set ISHIntegrationSTSWSFederation writes proper history"{
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetWSFederation -Session $session -ArgumentList $testingDeploymentName, "testEndpoint"
        $history = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetHistory -Session $session -ArgumentList $testingDeploymentName
        
        #Assert
        $history.Contains('Set-ISHIntegrationSTSWSFederation -ISHDeployment $deployment -Endpoint testEndpoint') | Should be "True"
              
    }
}