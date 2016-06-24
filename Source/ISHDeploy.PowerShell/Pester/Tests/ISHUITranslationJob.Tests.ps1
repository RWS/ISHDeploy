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
$configPath = Join-Path $testingDeployment.WebPath ("\Web{0}\Author\ASP\" -f $suffix)
$configPath = $configPath.ToString().replace(":", "$")
$configPath = "\\$computerName\$configPath"
$xmlPath = Join-Path $configPath "\XSL"

#endregion

#region Script Blocks 

# Script block for Enable-ISHUITranslationJob
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
        Enable-ISHUITranslationJob -ISHDeployment $ishDeploy
  
}

# Script block for Disable-ISHUITranslationJob
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
    Disable-ISHUITranslationJob -ISHDeployment $ishDeploy
}

#endregion



# Function reads target files and their content, searches for specified nodes in xm
function readTargetXML() {
    [xml]$XmlEventMonitorBar= Get-Content "$xmlPath\EventMonitorMenuBar.xml"  -ErrorAction SilentlyContinue
    [xml]$XmlTopDocumentButtonbar = Get-Content "$xmlPath\TopDocumentButtonbar.xml" -ErrorAction SilentlyContinue
    $TreeHtm = Get-Content "$configPath\Tree.htm" -ErrorAction SilentlyContinue

    $global:textEventMenuBar = $XmlEventMonitorBar.menubar.menuitem | ? {$_.label -eq "Translation Jobs"}
    $global:textTopDocumentButtonbar = $XmlTopDocumentButtonbar.BUTTONBAR.BUTTON.INPUT | ? {$_.NAME -eq "TranslationJob"}
	$global:textTopDocumentButtonbarTranslationButton = $XmlTopDocumentButtonbar.BUTTONBAR.BUTTON.INPUT | ? {$_.NAME -eq "Translation"}
   
    $global:textTreeHtm = $TreeHtm | Select-String '"Translation Jobs"'
    $global:textFunctionTreeHtm = $TreeHtm | Select-String 'function HighlightTranslationJobs()'
	
    $commentCheck = $global:textTreeHtm.ToString().StartsWith("//")-and $global:textFunctionTreeHtm.ToString().StartsWith("//")
    if (!$commentCheck -and $global:textEventMenuBar -and $global:textTopDocumentButtonbar -and $global:textTopDocumentButtonbarTranslationButton.Count -eq 2){
        Return "Enabled"
    }
    elseif  ($commentCheck -and !$global:textEventMenuBar -and !$global:textTopDocumentButtonbar -and $global:textTopDocumentButtonbarTranslationButton.Count -eq 2){
        Return "Disabled" 
    }

}


Describe "Testing ISHUITranslationJob"{
    BeforeEach {
	if(RemotePathCheck "$xmlPath\_EventMonitorMenuBar.xml")
        {
            if (RemotePathCheck "$xmlPath\EventMonitorMenuBar.xml")
            {
                RemoteRemoveItem "$xmlPath\EventMonitorMenuBar.xml"
            }
            RemoteRenameItem "$xmlPath\_EventMonitorMenuBar.xml" "EventMonitorMenuBar.xml"
        }
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockUndoDeployment -Session $session -ArgumentList $testingDeploymentName
    }

    It "enables Disabled Translation Job"{
		#Arrange
        # Make sure, that Translation Job is disabled, otherwise - make it disabled 
        $precondition = readTargetXML
        if ($precondition -eq "Enabled"){Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisable -Session $session -ArgumentList $testingDeploymentName }

        # Now Translation Job should be fo sure disabled. Otherwise test fails
        $precondition = readTargetXML
        $precondition | Should Be "Disabled"
		#Act
        # Try enabling Translation Job
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnable -Session $session -ArgumentList $testingDeploymentName
        $result = readTargetXML
		#Assert
        $result | Should Be "Enabled"
    }

    It "disables enabled Translation Job" {
		#Arrange
        # Make sure, that Translation Job is disabled, otherwise - make it disabled 
        $precondition = readTargetXML
        
		if ($precondition -eq "Disabled") {
			Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnable -Session $session -ArgumentList $testingDeploymentName
		}

        # Now Translation Job should be fo sure disabled. Otherwise test fails
        $precondition = readTargetXML
        $precondition | Should Be "Enabled"
		#Act
        # Try disabling Translation Job
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisable -Session $session -ArgumentList $testingDeploymentName
		$result = readTargetXML
		#Assert
        $result | Should Be "Disabled"
    }

    It "enables Translation Job with no XML"{
        #Arange
        Rename-Item "$xmlPath\EventMonitorMenuBar.xml" "_EventMonitorMenuBar.xml"
		#Act/Assert
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnable -Session $session -ArgumentList $testingDeploymentName -WarningVariable Warning -ErrorAction Stop }| Should Throw "Could not find file"
        #Rollback
        Rename-Item "$xmlPath\_EventMonitorMenuBar.xml" "EventMonitorMenuBar.xml"
    }

    It "disables Translation Job with no XML"{
        #Arange
        Rename-Item "$xmlPath\EventMonitorMenuBar.xml" "_EventMonitorMenuBar.xml"
		#Act/Assert
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisable -Session $session -ArgumentList $testingDeploymentName -WarningVariable Warning -ErrorAction Stop }| Should Throw "Could not find file"
        #Rollback
        Rename-Item "$xmlPath\_EventMonitorMenuBar.xml" "EventMonitorMenuBar.xml"
    }

    It "enables Translation Job with wrong XML"{
        #Arrange
        # Running valid scenario commandlet to out files into backup folder before they will ba manually modified in test
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnable -Session $session -ArgumentList $testingDeploymentName
        Rename-Item "$xmlPath\EventMonitorMenuBar.xml" "_EventMonitorMenuBar.xml"
        New-Item "$xmlPath\EventMonitorMenuBar.xml" -type file |Out-Null

        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnable -Session $session -ArgumentList $testingDeploymentName -WarningVariable Warning -ErrorAction Stop }| Should Throw "Root element is missing"

        #Rollback
        Remove-Item "$xmlPath\EventMonitorMenuBar.xml"
        Rename-Item "$xmlPath\_EventMonitorMenuBar.xml" "EventMonitorMenuBar.xml"
     }

    It "disables Translation Job with wrong XML"{
        #Arrange
        # Running valid scenario commandlet to out files into backup folder before they will ba manually modified in test
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnable -Session $session -ArgumentList $testingDeploymentName
        Rename-Item "$xmlPath\EventMonitorMenuBar.xml" "_EventMonitorMenuBar.xml"
        New-Item "$xmlPath\EventMonitorMenuBar.xml" -type file |Out-Null
		
		#Act/Assert
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisable -Session $session -ArgumentList $testingDeploymentName -WarningVariable Warning -ErrorAction Stop }| Should Throw "Root element is missing"

        #Rollback
        Remove-Item "$xmlPath\EventMonitorMenuBar.xml"
        Rename-Item "$xmlPath\_EventMonitorMenuBar.xml" "EventMonitorMenuBar.xml"
     }

     It "enables enabled Translation Job"{
		#Arrange
        # Make sure, that Translation Job is disabled, otherwise - make it disabled 
        $precondition = readTargetXML
        if ($precondition -eq "Disabled"){Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnable -Session $session -ArgumentList $testingDeploymentName }

        # Now Translation Job should be fo sure disabled. Otherwise test fails
        $precondition = readTargetXML
        $precondition | Should Be "Enabled"
		#Act
        # Try enabling Translation Job
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnable -Session $session -ArgumentList $testingDeploymentName
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnable -Session $session -ArgumentList $testingDeploymentName -ErrorAction Stop } | Should Not Throw
        $result = readTargetXML
		#Assert
        $result | Should Be "Enabled"
    }

    It "disables disabled Translation Job"{
		#Arrange
        # Make sure, that Translation Job is disabled, otherwise - make it disabled 
        $precondition = readTargetXML
        if ($precondition -eq "Enabled"){Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisable -Session $session -ArgumentList $testingDeploymentName }

        # Now Translation Job should be fo sure disabled. Otherwise test fails
        $precondition = readTargetXML
        $precondition | Should Be "Disabled"
		#Act
        # Try disabling Translation Job
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisable -Session $session -ArgumentList $testingDeploymentName
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisable -Session $session -ArgumentList $testingDeploymentName -ErrorAction Stop } | Should Not Throw
        $result = readTargetXML
		#Assert
        $result | Should Be "Disabled"
    }
	
	Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockUndoDeployment -Session $session -ArgumentList $testingDeploymentName
    
}