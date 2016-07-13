param(
    $session = $null,
    $testingDeploymentName = "InfoShare"
)

. "$PSScriptRoot\Common.ps1"


$moduleName = Invoke-CommandRemoteOrLocal -ScriptBlock { (Get-Module "ISHDeploy.*").Name } -Session $session
$backupPath = "\\$computerName\C$\ProgramData\$moduleName\$($testingDeployment.Name)\Backup"

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

UndoDeploymentBackToVanila $testingDeploymentName $true

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

