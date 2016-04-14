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
$backupPath = "C:\ProgramData\ISHDeploy\v{0}\{1}\Backup" -f $testingDeployment.SoftwareVersion,  $testingDeployment.Name
$backupPath = $backupPath.ToString().replace(":", "$")
$backupPath = "\\$computerName\$backupPath"

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

# Script block for Enable-ISHUIQualityAssistant. Added here for generating backup files
$scriptBlockDisable = {
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
   

function checkBackupFolderIsEmpty{
    $directoryInfo = Get-ChildItem $backupPath
    if ($directoryInfo.Count -eq 0){
        return $true
    }
    else {
        return $false

    }    
}

# Restoring system to vanila state for not loosing files, touched in previous tests
Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockUndoDeployment -Session $session -ArgumentList $testingDeploymentName

Describe "Testing Clear-ISHDeploymentHistory"{
    It "Clear ish deploy history"{
		# Try enabling Quality Assistant for generating backup files
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisable -Session $session -ArgumentList $testingDeploymentName
        # Check if backup files where created 
        $folderEmptyState = checkBackupFolderIsEmpty
        $folderEmptyState | Should be $false
        # Clear Deployment history
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockClean -Session $session -ArgumentList $testingDeploymentName
        $folderEmptyState = checkBackupFolderIsEmpty
        $folderEmptyState | Should be $true
    }

    It "Clear-ISHDeploymentHistory should not throw, when history is empty"{
		Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockClean -Session $session -ArgumentList $testingDeploymentName
        $folderEmptyState = checkBackupFolderIsEmpty
        $folderEmptyState | Should be $true
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockClean -Session $session -ArgumentList $testingDeploymentName} | Should not Throw
    }

}

