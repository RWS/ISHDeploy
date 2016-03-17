Import-Module ISHBackdoor
Import-Module InfoShare.Deployment
. "$PSScriptRoot\Common.ps1"

#region Initializing Variables
$htmlStyle = Set-Style

$logFile = "C:\Automated_deployment\Test10.htm"
$global:logArray = @()

$WarningPreference = “Continue"
$VerbosePreference = "SilentlyCOntinue"

$deploy = Get-ISHDeployment -Name "InfoShareSQL2014"

$backupPath = "C:\ProgramData\InfoShare.Deployment\v{0}\{1}\Backup" -f $deploy.SoftwareVersion,  $deploy.Name

#endregion


function checkBackupFolderIsEmpty{
    $directoryInfo = Get-ChildItem $backupPath
    if ($directoryInfo.Count -eq 0){
        return $true
    }
    else {
        return $false

    }    
}

#region Tests

function clearHistoryWhenItExists_test(){
    #Arrange
    Enable-ISHUIContentEditor -ISHDeployment $deploy
    Disable-ISHUIContentEditor -ISHDeployment $deploy
    $folderEmptyState = checkBackupFolderIsEmpty
    #Action
    if ($folderEmptyState -eq $false){
        Clear-ISHDeploymentHistory -ISHDeployment $deploy

        $folderEmptyState = checkBackupFolderIsEmpty

        $checkResult = $folderEmptyState -eq $true

        # Assert
        Assert_IsTrue $checkResult $MyInvocation.MyCommand.Name "1"
    
    }
    else{
        TestIsBlocked $MyInvocation.MyCommand.Name "1" "History folder is empty"
        
    }
 
}

function clearHistoryWhenNoHistoryGivesNoError_test(){
    #Arrange
    Clear-ISHDeploymentHistory -ISHDeployment $deploy
    $folderEmptyState = checkBackupFolderIsEmpty
    #Action
    if ($folderEmptyState -eq $true){

        try
        {
              Clear-ISHDeploymentHistory -ISHDeployment $deploy -WarningVariable Warning -ErrorAction Stop 
        
        }
        catch 
        {
            $ErrorMessage = $_.Exception.Message
        }

        $checkResult = !$ErrorMessage

        # Assert
        Assert_IsTrue $checkResult $MyInvocation.MyCommand.Name "2"
    
    }
    else{
        TestIsBlocked $MyInvocation.MyCommand.Name "2" "History folder is not empty"
        
    }
 
}

#endregion

#region Test Calls
clearHistoryWhenItExists_test
clearHistoryWhenNoHistoryGivesNoError_test

#endregion

#region Log and HTML Output
$global:logArray | ConvertTo-HTML -Head $htmlStyle | Out-File $logFile

Edit-LogHtml -targetHTML $logFile

Invoke-Expression $logFile
#endregion

