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
$configPath = Join-Path $testingDeployment.WebPath ("\Web{0}\Author\ASP\Editors\Xopus" -f $testingDeployment.OriginalParameters.projectsuffix )
$configPath = $configPath.ToString().replace(":", "$")
$configPath = "\\$computerName\$configPath"
$xmlPath = Join-Path $configPath "config"

#endregion

#region Script Blocks 

# Script block for Enable-ISHUIQualityAssistant
$scriptBlockEnable = {
    param (
        [Parameter(Mandatory=$true)]
        $ishDeployName 
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
        $ishDeploy = Get-ISHDeployment -Name $ishDeployName
        Enable-ISHUIQualityAssistant -ISHDeployment $ishDeploy
  
}

# Script block for Disable-ISHUIQualityAssistant
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
    Disable-ISHUIQualityAssistant -ISHDeployment $ishDeploy
}

#endregion



# Function reads target files and their content, searches for specified nodes in xm
function readTargetXML() {
	[xml]$XmlConfig = Get-Content "$xmlPath\config.xml" -ErrorAction SilentlyContinue
Write-Debug "COnfig path $xmlPath\config.xml"
    [xml]$XmlBlueLionConfig = Get-Content "$xmlPath\bluelion-config.xml" -ErrorAction SilentlyContinue
    Write-Debug "COnfig path $xmlPath\bluelion-config.xml"
    [xml]$XmlEnrichWebConfig = Get-Content "$configPath\BlueLion-Plugin\web.config" -ErrorAction SilentlyContinue

    $global:textConfig = $XmlConfig.config.javascript | ? {$_.src -eq "../BlueLion-Plugin/Bootstrap/bootstrap.js"}
    $global:textBlueLionConfig = $XmlBlueLionConfig.SelectNodes("*/*[local-name()='import'][@src='../BlueLion-Plugin/create-toolbar.xml']")
    $global:textEnrichBluelionWebConfigJsonMimeMapNodes = $XmlEnrichWebConfig.SelectNodes("configuration/system.webServer/staticContent/mimeMap[@fileExtension='.json']")

	if($textConfig -and $textBlueLionConfig.Count -eq 1 -and $textEnrichBluelionWebConfigJsonMimeMapNodes.Count -eq 1){
		Return "Enabled"
	}
	else{
		Return "Disabled"
	}

}
$precondition = readTargetXML


Describe "Testing ISHUIQualityAssistant"{
    BeforeEach {
            Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockUndoDeployment -Session $session -ArgumentList $testingDeploymentName
    }

    It "enables Disabled Quality Assistant"{
		#Arrange
        # Make sure, that Quality Assistant is disabled, otherwise - make it disabled 
        $precondition = readTargetXML
        if ($precondition -eq "Enabled"){Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisable -Session $session -ArgumentList $testingDeploymentName }

        # Now Quality Assistant should be fo sure disabled. Otherwise test fails
        $precondition = readTargetXML
        $precondition | Should Be "Disabled"
		#Act
        # Try enabling Quality Assistant
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnable -Session $session -ArgumentList $testingDeploymentName
        $result = readTargetXML
		#Assert
        $result | Should Be "Enabled"
    }

    It "disables enabled Quality Assistant" {
		#Arrange
        # Make sure, that Quality Assistant is disabled, otherwise - make it disabled 
        $precondition = readTargetXML
        
		if ($precondition -eq "Disabled") {
			Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnable -Session $session -ArgumentList $testingDeploymentName
		}

        # Now Quality Assistant should be fo sure disabled. Otherwise test fails
        $precondition = readTargetXML
        $precondition | Should Be "Enabled"
		#Act
        # Try disabling Quality Assistant
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisable -Session $session -ArgumentList $testingDeploymentName
		$result = readTargetXML
		#Assert
        $result | Should Be "Disabled"
    }

    It "enables Quality Assistant with no XML"{
        #Arange
        Rename-Item "$xmlPath\config.xml" "_config.xml"
		#Act/Assert
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnable -Session $session -ArgumentList $testingDeploymentName -WarningVariable Warning -ErrorAction Stop }| Should Throw "Could not find file"
        #Rollback
        Rename-Item "$xmlPath\_config.xml" "config.xml"
    }

    It "disables Quality Assistant with no XML"{
        #Arange
        Rename-Item "$xmlPath\config.xml" "_config.xml"
		#Act/Assert
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisable -Session $session -ArgumentList $testingDeploymentName -WarningVariable Warning -ErrorAction Stop }| Should Throw "Could not find file"
        #Rollback
        Rename-Item "$xmlPath\_config.xml" "config.xml"
    }

    It "enables Quality Assistant with wrong XML"{
        #Arrange
        # Running valid scenario commandlet to out files into backup folder before they will ba manually modified in test
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnable -Session $session -ArgumentList $testingDeploymentName
        Rename-Item "$xmlPath\config.xml" "_config.xml"
        New-Item "$xmlPath\config.xml" -type file |Out-Null

        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnable -Session $session -ArgumentList $testingDeploymentName -WarningVariable Warning -ErrorAction Stop }| Should Throw "Root element is missing"

        #Rollback
        Remove-Item "$xmlPath\config.xml"
        Rename-Item "$xmlPath\_config.xml" "config.xml"
     }

    It "disables Quality Assistant with wrong XML"{
        #Arrange
        # Running valid scenario commandlet to out files into backup folder before they will ba manually modified in test
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnable -Session $session -ArgumentList $testingDeploymentName
        Rename-Item "$xmlPath\config.xml" "_config.xml"
        New-Item "$xmlPath\config.xml" -type file |Out-Null
		
		#Act/Assert
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisable -Session $session -ArgumentList $testingDeploymentName -WarningVariable Warning -ErrorAction Stop }| Should Throw "Root element is missing"

        #Rollback
        Remove-Item "$xmlPath\config.xml"
        Rename-Item "$xmlPath\_config.xml" "config.xml"
     }

     It "enables enabled Quality Assistant"{
		#Arrange
        # Make sure, that Quality Assistant is disabled, otherwise - make it disabled 
        $precondition = readTargetXML
        if ($precondition -eq "Disabled"){Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnable -Session $session -ArgumentList $testingDeploymentName }

        # Now Quality Assistant should be fo sure disabled. Otherwise test fails
        $precondition = readTargetXML
        $precondition | Should Be "Enabled"
		#Act
        # Try enabling Quality Assistant
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnable -Session $session -ArgumentList $testingDeploymentName
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnable -Session $session -ArgumentList $testingDeploymentName -ErrorAction Stop } | Should Not Throw
        $result = readTargetXML
		#Assert
        $result | Should Be "Enabled"
    }

    It "disables disabled Quality Assistant"{
		#Arrange
        # Make sure, that Quality Assistant is disabled, otherwise - make it disabled 
        $precondition = readTargetXML
        if ($precondition -eq "Enabled"){Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisable -Session $session -ArgumentList $testingDeploymentName }

        # Now Quality Assistant should be fo sure disabled. Otherwise test fails
        $precondition = readTargetXML
        $precondition | Should Be "Disabled"
		#Act
        # Try disabling Quality Assistant
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisable -Session $session -ArgumentList $testingDeploymentName
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisable -Session $session -ArgumentList $testingDeploymentName -ErrorAction Stop } | Should Not Throw
        $result = readTargetXML
		#Assert
        $result | Should Be "Disabled"
    }

    
}