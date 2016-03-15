Import-Module InfoShare.Deployment
. "$PSScriptRoot\Common.ps1"


$executingScriptDirectory = Split-Path -Path $MyInvocation.MyCommand.Definition -Parent
$testComparingFilePath = Join-Path $executingScriptDirectory "History_test.ps1"

#region Variables initoialization
$htmlStyle = Set-Style

$logFile = "C:\Automated_deployment\Test7.htm"

$WarningPreference = “Continue"
$VerbosePreference = "SilentlyCOntinue"
$global:logArray = @()

$deploy = Get-ISHDeployment -Deployment "SQL2014"
Write-Host = 

$historyPath = "C:\ProgramData\InfoShare.Deployment\v"
$historyPath =  $historyPath  + $deploy.Version

$historyPath =  $historyPath + "\ISH" + $deploy.Suffix

$historyPath = Join-Path $historyPath "\History.ps1"

function getIshHisory_test(){

   

    Remove-Item $historyPath

    Enable-ISHUIContentEditor -ISHDeployment $deploy
    Enable-ISHUIQualityAssistant -ISHDeployment $deploy
   
    $historyFile = Get-ISHDeploymentHistory -ISHDeployment $deploy
    
    $historyTestFile = Get-Content $testComparingFilePath
    
    
    $compareResult = Compare-Object $historyFile $historyTestFile

       
   $checkResult = $c.Count -eq 0

  
   
    
    # Assert
    Assert_IsTrue $checkResult $MyInvocation.MyCommand.Name "1"
}

function getIshHisoryCommandFails_test(){
    
    $xmlPath = $deploy.WebPath
    $xmlPath = $xmlPath + "\Web" 
    $xmlPath = $xmlPath + $deploy.Suffix 
    $xmlPath = $xmlPath + "\Author\ASP"

    Rename-Item "$xmlPath\Web.config" "_Web.config"
    New-Item "$xmlPath\Web.config" -type file |Out-Null

    Remove-Item $historyPath


    Enable-ISHUIContentEditor -ISHDeployment $deploy
    Enable-ISHUIQualityAssistant -ISHDeployment $deploy

    
    try
    {
         Enable-ISHExternalPreview -ISHDeployment $deploy -ErrorAction Stop 
        
    }
    catch 
    {
        $ErrorMessage = $_.Exception.Message
    }

    
   
    $historyFile = Get-Content $historyPath
    
    $historyTestFile = Get-Content $testComparingFilePath
    
    
    $compareResult = Compare-Object $historyFile $historyTestFile

       
    $checkResult = $c.Count -eq 0

  
   
    
    # Assert
    Assert_IsTrue $checkResult $MyInvocation.MyCommand.Name "2"

     Remove-Item "$xmlPath\Web.config"
    Rename-Item "$xmlPath\_Web.config" "Web.config"
}

function getIshHisoryCommandletsThatImplementsBaseClassWriteNoHistory_test(){

   

    Remove-Item $historyPath

    Enable-ISHUIContentEditor -ISHDeployment $deploy
    Enable-ISHUIQualityAssistant -ISHDeployment $deploy
    Get-ISHDeployment
    Get-ISHDeploymentHistory -ISHDeployment $deploy
    Test-ISHContentEditor -ISHDeployment $deploy -Hostname "global.sdl.corp"
   
    $historyFile = Get-Content $historyPath
    
    $historyTestFile = Get-Content $testComparingFilePath
    
    
    $compareResult = Compare-Object $historyFile $historyTestFile

       
   $checkResult = $c.Count -eq 0

  
   
    
    # Assert
    Assert_IsTrue $checkResult $MyInvocation.MyCommand.Name "3"
}


getIshHisory_test
getIshHisoryCommandFails_test
getIshHisoryCommandletsThatImplementsBaseClassWriteNoHistory_test


$global:logArray | ConvertTo-HTML -Head $htmlStyle | Out-File $logFile


Edit-LogHtml -targetHTML $logFile


Invoke-Expression $logFile
