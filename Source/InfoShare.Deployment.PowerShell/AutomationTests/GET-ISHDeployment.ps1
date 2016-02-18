Import-Module InfoShare.Deployment
. "$PSScriptRoot\Common.ps1"
CLS

$htmlStyle = Set-Style

$logFile = "C:\Automated_deployment\Test6.htm"


$WarningPreference = “Continue"
$VerbosePreference = "SilentlyCOntinue"
$global:logArray = @()


function GetIshDeploymentWithDeploymentParameter_test(){

    $deploy = Get-ISHDeployment -Deployment "SQL2014"
 if (($deploy.WebPath -eq "C:\InfoShare") -and($deploy.Count -eq 1)) {
            Write-Host $MyInvocation.MyCommand.Name -NoNewline
            Write-Host " Passed" -foregroundcolor "green" 
            logger "1" $MyInvocation.MyCommand.Name "Passed" " "
        }

        else {
             Write-Host $MyInvocation.MyCommand.Name -NoNewline 
             Write-Host " Failed"  -foregroundcolor "red"
             logger "1" $MyInvocation.MyCommand.Name "Failed" " "
        }

 }

 function GetIshDeploymentWithoutDeploymentParameter_test(){

    $deploy = Get-ISHDeployment
 if ($deploy.Count -eq 2) {
            Write-Host $MyInvocation.MyCommand.Name -NoNewline
            Write-Host " Passed" -foregroundcolor "green" 
            logger "2" $MyInvocation.MyCommand.Name "Passed" " "
        }

        else {
             Write-Host $MyInvocation.MyCommand.Name -NoNewline 
             Write-Host " Failed"  -foregroundcolor "red"
             logger "2" $MyInvocation.MyCommand.Name "Failed" " "
        }

 }

 function GetIshDeploymentWithWrongDeploymentParameter_test(){

     try
        {
            $deploy = Get-ISHDeployment -Deployment "testDummySuffix" -WarningVariable Warning -ErrorAction Stop
        }
        catch 
        {
            $ErrorMessage = $_.Exception.Message
        }
        if ($ErrorMessage -match "Deployment with suffix testDummySuffix is not found on the system") {
            Write-Host $MyInvocation.MyCommand.Name -NoNewline
            Write-Host " Passed" -foregroundcolor "green" 
            logger "3" $MyInvocation.MyCommand.Name "Passed" " "
        }

        else {
             Write-Host $MyInvocation.MyCommand.Name -NoNewline 
             Write-Host " Failed"  -foregroundcolor "red"
             logger "3" $MyInvocation.MyCommand.Name "Failed" " "
        }

 }
 


 GetIshDeploymentWithDeploymentParameter_test
 GetIshDeploymentWithoutDeploymentParameter_test
 GetIshDeploymentWithWrongDeploymentParameter_test 

$global:logArray | ConvertTo-HTML -Head $htmlStyle | Out-File $logFile


Edit-LogHtml -targetHTML $logFile


Invoke-Expression $logFile