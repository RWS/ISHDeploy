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

$moduleName = $moduleName = Invoke-CommandRemoteOrLocal -ScriptBlock { (Get-Module "ISHDeploy.*").Name } -Session $session
$packagePath = "C:\ProgramData\$moduleName\$($testingDeployment.Name)\Packages"
$computerName = $computerName.split(".")[0]
$uncPackagePath = "\\$computerName\" + ($packagePath.replace(":", "$"))

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

$scriptBlockGet = {
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

Describe "Testing Get-ISHPackageFolderPath"{
    BeforeEach {
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockUndoDeployment -Session $session -ArgumentList $testingDeploymentName
    }

    It "Get package folder path"{
        $checkPath = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGet -Session $session -ArgumentList $testingDeploymentName  
        $checkPath -eq $packagePath | Should be "True"
    }
    
    It "Get package folder unc path"{
        $checkPath = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGet -Session $session -ArgumentList $testingDeploymentName, $true
        $checkPath -eq $uncPackagePath| Should be "True"
    }

    It "Get package folder creates Package folder"{
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGet -Session $session -ArgumentList $testingDeploymentName, $true} | Should Not Throw
        RetryCommand -numberOfRetries 10 -command {Test-Path ($uncPackagePath)} -expectedResult $true | Should Be "True"
        
    }
}

