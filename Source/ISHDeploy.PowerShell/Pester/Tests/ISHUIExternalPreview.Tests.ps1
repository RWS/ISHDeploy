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
$suffix = GetProjectSuffix($testingDeployment.Name)
$xmlPath = Join-Path $testingDeployment.WebPath ("\Web{0}\Author\ASP" -f $suffix )
$xmlPath = $xmlPath.ToString().replace(":", "$")
$xmlPath = "\\$computerName\$xmlPath"

$customId = "testID"
#endregion

#region Script Blocks 

# Script block for Enable-ISHExternalPreview
$scriptBlockEnable = {
    param (
        [Parameter(Mandatory=$true)]
        $ishDeployName,
        [Parameter(Mandatory=$false)]
        $externalId
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
    if(!$externalId){
        $ishDeploy = Get-ISHDeployment -Name $ishDeployName
        Enable-ISHExternalPreview -ISHDeployment $ishDeploy
    }
    else{
        $ishDeploy = Get-ISHDeployment -Name $ishDeployName
        Enable-ISHExternalPreview -ISHDeployment $ishDeploy -ExternalId $externalId
    }
  
}

# Script block for Disable-ISHExternalPreview
$scriptBlockDisable = {
    param (
        [Parameter(Mandatory=$true)]
        $ishDeployname 
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Disable-ISHExternalPreview -ISHDeployment $ishDeploy
}

#endregion


# Function reads target files and their content, searches for specified nodes in xml
$scriptBlockReadTargetXML = {
    param(
        $xmlPath,
        $customID
    )
    [xml]$XmlWebConfig = Get-Content "$xmlPath\Web.config" #-ErrorAction SilentlyContinue
    $textConfig = $XmlWebConfig.configuration.'trisoft.infoshare.web.externalpreviewmodule'.identity | ? {$_.externalId -eq "ServiceUser"}
    $configSection = $XmlWebConfig.configuration.configSections.section | ? {$_.name -eq "trisoft.infoshare.web.externalpreviewmodule"}
    $module = $XmlWebConfig.configuration.'system.webServer'.modules.add  | ? {$_.name -eq "TrisoftExternalPreviewModule"}
    $configCustomID = $XmlWebConfig.configuration.'trisoft.infoshare.web.externalpreviewmodule'.identity | ? {$_.externalId -eq $customID}

	if($textConfig -and $configSection -and $module -and !$configCustomID){
		Return "Enabled"
	}
    elseif(!$textConfig -and $configSection -and $module -and $configCustomID){
        Return "CustomIdEnabled"
    }
	else{
		Return "Disabled"
	}
}
function readTargetXML() {
	$result = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockReadTargetXML -Session $session -ArgumentList $xmlPath, $customID

    return $result
}


Describe "Testing ISHExternalPreview"{
    BeforeEach {    
        StopPool -projectName $testingDeploymentName
		ArtifactCleaner -filePath $xmlPath -fileName "web.config"
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockUndoDeployment -Session $session -ArgumentList $testingDeploymentName
    }

    It "enables Disabled External Preview"{
		#Arrange
        # Make sure, that External Preview is disabled, otherwise - make it disabled 
        $precondition = readTargetXML
        if ($precondition -eq "Enabled"){Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisable -Session $session -ArgumentList $testingDeploymentName }

        # Now External Preview should be fo sure disabled. Otherwise test fails
        
        $precondition = RetryCommand -numberOfRetries 10 -command {readTargetXML} -expectedResult "Disabled"
        $precondition | Should Be "Disabled"
		#Act
        # Try enabling External Preview
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnable -Session $session -ArgumentList $testingDeploymentName
        $result = RetryCommand -numberOfRetries 10 -command {readTargetXML} -expectedResult "Enabled"
		#Assert
        $result | Should Be "Enabled"
    }

    It "disables enabled External Preview" {
		#Arrange
        # Make sure, that External Preview is disabled, otherwise - make it disabled 
        $precondition = readTargetXML
        
		if ($precondition -eq "Disabled") {
			Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnable -Session $session -ArgumentList $testingDeploymentName
		}

        # Now External Preview should be fo sure disabled. Otherwise test fails
        $precondition = RetryCommand -numberOfRetries 10 -command {readTargetXML} -expectedResult "Enabled"
        $precondition | Should Be "Enabled"
		#Act
        # Try disabling External Preview
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisable -Session $session -ArgumentList $testingDeploymentName
		$result = RetryCommand -numberOfRetries 10 -command {readTargetXML} -expectedResult "Disabled"
		#Assert
        $result | Should Be "Disabled"
    }

    It "enables External Preview with no XML"{
        #Arange
        Rename-Item "$xmlPath\Web.config" "_Web.config"
		#Act/Assert
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnable -Session $session -ArgumentList $testingDeploymentName -WarningVariable Warning -ErrorAction Stop }| Should Throw "Could not find file"
        #Rollback
        Rename-Item "$xmlPath\_Web.config" "Web.config"
    }

    It "disables External Preview with no XML"{
        #Arange
        Rename-Item "$xmlPath\Web.config" "_Web.config"
		#Act/Assert
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisable -Session $session -ArgumentList $testingDeploymentName -WarningVariable Warning -ErrorAction Stop }| Should Throw "Could not find file"
        #Rollback
        Rename-Item "$xmlPath\_Web.config" "Web.config"
    }

    It "enables External Preview with wrong XML"{
        #Arrange
        # Running valid scenario commandlet to out files into backup folder before they will ba manually modified in test
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnable -Session $session -ArgumentList $testingDeploymentName
        Rename-Item "$xmlPath\Web.config" "_Web.config"
        New-Item "$xmlPath\Web.config" -type file |Out-Null

        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnable -Session $session -ArgumentList $testingDeploymentName -WarningVariable Warning -ErrorAction Stop }| Should Throw "Root element is missing"

        #Rollback
        Remove-Item "$xmlPath\Web.config"
        Rename-Item "$xmlPath\_Web.config" "Web.config"
     }

    It "disables External Preview with wrong XML"{
        #Arrange
        # Running valid scenario commandlet to out files into backup folder before they will ba manually modified in test
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnable -Session $session -ArgumentList $testingDeploymentName
        Rename-Item "$xmlPath\Web.config" "_Web.config"
        New-Item "$xmlPath\Web.config" -type file |Out-Null
		
		#Act/Assert
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisable -Session $session -ArgumentList $testingDeploymentName -WarningVariable Warning -ErrorAction Stop }| Should Throw "Root element is missing"

        #Rollback
        Remove-Item "$xmlPath\Web.config"
        Rename-Item "$xmlPath\_Web.config" "Web.config"
     }

    It "enables enabled External Preview"{
		#Arrange
        # Make sure, that External Preview is disabled, otherwise - make it disabled 
        $precondition = readTargetXML
        if ($precondition -eq "Disabled"){Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnable -Session $session -ArgumentList $testingDeploymentName }

        # Now External Preview should be fo sure disabled. Otherwise test fails
        $precondition = RetryCommand -numberOfRetries 10 -command {readTargetXML} -expectedResult "Enabled"
        $precondition | Should Be "Enabled"
		#Act
        # Try enabling External Preview
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnable -Session $session -ArgumentList $testingDeploymentName
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnable -Session $session -ArgumentList $testingDeploymentName -ErrorAction Stop } | Should Not Throw
        $result = RetryCommand -numberOfRetries 10 -command {readTargetXML} -expectedResult "Enabled"
		#Assert
        $result | Should Be "Enabled"
    }

    It "disables disabled External Preview"{
		#Arrange
        # Make sure, that External Preview is disabled, otherwise - make it disabled 
        $precondition = readTargetXML
        if ($precondition -eq "Enabled"){Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisable -Session $session -ArgumentList $testingDeploymentName }

        # Now External Preview should be fo sure disabled. Otherwise test fails
        $precondition = RetryCommand -numberOfRetries 10 -command {readTargetXML} -expectedResult "Disabled"
        $precondition | Should Be "Disabled"
		#Act
        # Try disabling External Preview
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisable -Session $session -ArgumentList $testingDeploymentName
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisable -Session $session -ArgumentList $testingDeploymentName -ErrorAction Stop } | Should Not Throw
        $result = RetryCommand -numberOfRetries 10 -command {readTargetXML} -expectedResult "Disabled"
		#Assert
        $result | Should Be "Disabled"
    }

    It "enables External Preview for custom ID"{
		#Arrange
        # Make sure, that External Preview is disabled, otherwise - make it disabled 
        $precondition = readTargetXML
        if ($precondition -eq "Disabled"){Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnable -Session $session -ArgumentList $testingDeploymentName}

        # Now External Preview should be fo sure disabled. Otherwise test fails
        $precondition = RetryCommand -numberOfRetries 10 -command {readTargetXML} -expectedResult "Enabled"
        $precondition | Should Be "Enabled"
		#Act
        # Try enabling External Preview
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnable -Session $session -ArgumentList $testingDeploymentName, $customID  -ErrorAction Stop
        $result = RetryCommand -numberOfRetries 10 -command {readTargetXML} -expectedResult "CustomIdEnabled"
		#Assert
        $result | Should Be "CustomIdEnabled"
    }

     It "disables enabled for custom ID External Preview" {
		#Arrange
        # Make sure, that External Preview is disabled, otherwise - make it disabled 
        $precondition = readTargetXML
        
		if ($precondition -eq "Disabled") {
			Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnable -Session $session -ArgumentList $testingDeploymentName, $customID
		}

        # Now External Preview should be fo sure disabled. Otherwise test fails
        $precondition = RetryCommand -numberOfRetries 10 -command {readTargetXML} -expectedResult "CustomIdEnabled"
        $precondition | Should Be "CustomIdEnabled"
		#Act
        # Try disabling External Preview
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisable -Session $session -ArgumentList $testingDeploymentName
		$result = RetryCommand -numberOfRetries 10 -command {readTargetXML} -expectedResult "Disabled"
		#Assert
        $result | Should Be "Disabled"
    }

	Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockUndoDeployment -Session $session -ArgumentList $testingDeploymentName
    
}