param(
    $session = $null,
    $testingDeploymentName = "InfoShare"
)

. "$PSScriptRoot\Common.ps1"

$moduleName = Invoke-CommandRemoteOrLocal -ScriptBlock { (Get-Module "ISHDeploy.*").Name } -Session $session
$packagePath = "C:\ProgramData\$moduleName\$($testingDeployment.Name)\Packages"
$computerName = $computerName.split(".")[0]
$uncPackagePath = "\\$computerName\" + ($packagePath.replace(":", "$"))

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
        UndoDeploymentBackToVanila $testingDeploymentName $true
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
        
        RemotePathCheck $uncPackagePath | Should Be "True"
    }
}

