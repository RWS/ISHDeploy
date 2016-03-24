Import-Module ISHDeploy
. "$PSScriptRoot\Common.ps1"


$htmlStyle = Set-Style

$logFile = "C:\Automated_deployment\Test6.htm"


$WarningPreference = “Continue"
$VerbosePreference = "SilentlyCOntinue"
$global:logArray = @()




function GetIshDeploymentWithDeploymentParameter_test(){
    #Action
    $deploy = Get-ISHDeployment -Name "InfoShareSQL2014"

    $checkResult = (($deploy.WebPath -eq "C:\InfoShare") -and($deploy.Count -eq 1))
    # Assert
    Assert_IsTrue $checkResult $MyInvocation.MyCommand.Name "1"
 }

 function GetIshDeploymentWithoutDeploymentParameter_test(){
    #Action
    $deploy = Get-ISHDeployment

    $checkResult = $deploy.Count -eq 2
     # Assert
    Assert_IsTrue $checkResult $MyInvocation.MyCommand.Name "2"

 }

 function GetIshDeploymentWithWrongDeploymentParameter_test(){
        
        #Action
        try
        {
            $deploy = Get-ISHDeployment -Name "InfoShareTestDummySuffix" -WarningVariable Warning -ErrorAction Stop
        }
        catch 
        {
            $ErrorMessage = $_.Exception.Message
        }

        $checkResult = $ErrorMessage -match "Deployment with suffix testDummySuffix is not found on the system"
         # Assert
        Assert_IsTrue $checkResult $MyInvocation.MyCommand.Name "3"

 }
 


 GetIshDeploymentWithDeploymentParameter_test
 GetIshDeploymentWithoutDeploymentParameter_test
 GetIshDeploymentWithWrongDeploymentParameter_test 

$global:logArray | ConvertTo-HTML -Head $htmlStyle | Out-File $logFile


Edit-LogHtml -targetHTML $logFile


Invoke-Expression $logFile