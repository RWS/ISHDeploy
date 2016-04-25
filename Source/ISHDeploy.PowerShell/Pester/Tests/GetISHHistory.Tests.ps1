param(
    $session = $null,
    $testingDeploymentName = "InfoShare"
)

. "$PSScriptRoot\Common.ps1"
$computerName = If ($session) {$session.ComputerName} Else {$env:COMPUTERNAME}

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

$testingDeployment = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetDeployment -Session $session -ArgumentList $testingDeploymentName

$moduleName = Invoke-CommandRemoteOrLocal -ScriptBlock { (Get-Module "ISHDeploy.*").Name } -Session $session
$backupPath = "\\$computerName\C$\ProgramData\$moduleName\$($testingDeployment.Name)\Backup"

$xmlPath = Join-Path ($testingDeployment.WebPath.replace(":", "$")) ("Web{0}\Author\ASP\XSL" -f $testingDeployment.OriginalParameters.projectsuffix )
$xmlPath = "\\$computerName\$xmlPath"

$scriptBlockGet = {
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

$scriptBlockClean = {
    param (
        [Parameter(Mandatory=$false)]
        $ishDeployName 
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Clear-ISHDeploymentHistory -ISHDeployment $ishDeploy
}

# Script block for Disable-ISHUIQualityAssistant. Added here for generating backup files
$scriptBlockDisableQA = {
    param (
        [Parameter(Mandatory=$true)]
        $ishDeployName 
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
        $ishDeploy = Get-ISHDeployment -Name $ishDeployName
        Disable-ISHUIQualityAssistant -ISHDeployment $ishDeploy
  
}

$scriptBlockEnableContentEditor = {
    param (
        [Parameter(Mandatory=$true)]
        $ishDeployName 
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
        $ishDeploy = Get-ISHDeployment -Name $ishDeployName
        Enable-ISHUIContentEditor -ISHDeployment $ishDeploy
  
}

$scriptBlockGetPackageFolder = {
    param (
        [Parameter(Mandatory=$false)]
        $ishDeployName,
        [Parameter(Mandatory=$false)]
        $uncSwitch 
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    if($uncSwitch){
        Get-ISHPackageFolderPath -ISHDeployment $ishDeploy -UNC
    }
    else{
        Get-ISHPackageFolderPath -ISHDeployment $ishDeploy
    }
}

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
   

 function createFakeHistory{
$text = '$deployment = Get-ISHDeployment -Name ''InfoShareSQL2014''
Disable-ISHUIQualityAssistant -ISHDeployment $deployment
'
  return $text
}

# Restoring system to vanila state for not loosing files, touched in previous tests
Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockUndoDeployment -Session $session -ArgumentList $testingDeploymentName

Describe "Testing Get-ISHDeploymentHistory"{
    BeforeEach {
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockClean -Session $session -ArgumentList $testingDeploymentName
    }

    It "Get ish deploy history"{
		# Try enabling Quality Assistant for generating backup files
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisableQA -Session $session -ArgumentList $testingDeploymentName
        $history = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGet -Session $session -ArgumentList $testingDeploymentName
        $fakeHistory =  createFakeHistory
        $history.EndsWith($fakeHistory) | Should be "True"
    }

    It "Failed commandlets write no history"{
        Rename-Item "$xmlPath\FolderButtonbar.xml" "_FolderButtonbar.xml"
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisableQA -Session $session -ArgumentList $testingDeploymentName
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnableContentEditor -Session $session -ArgumentList $testingDeploymentName -WarningVariable Warning -ErrorAction Stop }| Should Throw "Could not find file"
        
		$history = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGet -Session $session -ArgumentList $testingDeploymentName
        $fakeHistory =  createFakeHistory
        $history.EndsWith($fakeHistory) | Should be "True"
        
		Rename-Item "$xmlPath\_FolderButtonbar.xml" "FolderButtonbar.xml"
    }

    It "Commandlets that implements base class write no history"{
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisableQA -Session $session -ArgumentList $testingDeploymentName 
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGet -Session $session -ArgumentList $testingDeploymentName
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetDeployment -Session $session -ArgumentList $testingDeploymentName
		Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetPackageFolder -Session $session -ArgumentList $testingDeploymentName, $true
		Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetPackageFolder -Session $session -ArgumentList $testingDeploymentName

        $history = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGet -Session $session -ArgumentList $testingDeploymentName
        $fakeHistory =  createFakeHistory
        $history.EndsWith($fakeHistory) | Should be "True"
    }

    It "Get-IshHistory works when there is no history"{
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGet -Session $session -ArgumentList $testingDeploymentName -WarningVariable Warning -ErrorAction Stop }| Should Not Throw

        $history = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGet -Session $session -ArgumentList $testingDeploymentName
        !$history | Should be "True"
    }
}

