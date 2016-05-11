param(
    $session = $null,
    $testingDeploymentName = "InfoShare"
)
. "$PSScriptRoot\Common.ps1"

$computerName = If ($session) {$session.ComputerName} Else {$env:COMPUTERNAME}


#region variables

$packageFileName = "testFileName"

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

$moduleName = Invoke-CommandRemoteOrLocal -ScriptBlock { (Get-Module "ISHDeploy.*").Name } -Session $session
$packagePath = "C:\ProgramData\$moduleName\$($testingDeployment.Name)\Packages"
$computerName = $computerName.split(".")[0]
$uncPackagePath = "\\$computerName\" + ($packagePath.replace(":", "$"))
$tempFolder = $PSScriptRoot
#endregion

#region Script Blocks 
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

$scriptBlockGetISHPackageFolder = {
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

$scriptBlockGetISHPackage = {
    param (
        [Parameter(Mandatory=$false)]
        $ishDeployName,
        [Parameter(Mandatory=$false)]
        $filename,
        [Parameter(Mandatory=$false)]
        $ADFSSwitch
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    if($ADFSSwitch){
        Save-ISHIntegrationSTSConfigurationPackage -ISHDeployment $ishDeploy -Filename $filename -ADFS
    }
    else{
        Save-ISHIntegrationSTSConfigurationPackage -ISHDeployment $ishDeploy -Filename $filename 
    }  
}

$scriptBlockGetHistory = {
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

#endregion


Add-Type -AssemblyName System.IO.Compression.FileSystem
function Unzip
{
    param([string]$zipfile, [string]$outpath)

    [System.IO.Compression.ZipFile]::ExtractToDirectory($zipfile, $outpath)
}

Describe "Testing ISHIntegrationSTSConfigurationPackage"{
    BeforeEach {
        New-Item "$tempFolder\tmp" -ItemType directory
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockUndoDeployment -Session $session -ArgumentList $testingDeploymentName
    }
    

    It "Save package"{
        $tempFolder = $PSScriptRoot
        $packagePath = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHPackageFolder -Session $session -ArgumentList $testingDeploymentName, $true
        Test-Path $packagePath | Should be "True"
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHPackage -Session $session -ArgumentList $testingDeploymentName, $packageFileName
        Unzip "$packagePath\$packageFileName" "$tempFolder\tmp"
        Start-Sleep -Milliseconds 7000
        Test-Path "$tempFolder\tmp\ishws.cer" | Should be $true
        Test-Path "$tempFolder\tmp\CM Security Token Service Requirements.md" | Should be $true
        $Mdfile = Get-Content "$tempFolder\tmp\CM Security Token Service Requirements.md"
        $Mdfile -contains "https://$computerName.global.sdl.corp/ISHWSSQL2014/Wcf/API25/Application.svc" | Should be $true
        $Mdfile -contains "https://$computerName.global.sdl.corp/ISHWSSQL2014/Wcf/API/ConditionManagement.svc" | Should be $true
    }

    It "Save same package"{
        $tempFolder = $PSScriptRoot
        $packagePath = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHPackageFolder -Session $session -ArgumentList $testingDeploymentName, $true
        Test-Path $packagePath | Should be "True"
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHPackage -Session $session -ArgumentList $testingDeploymentName, $packageFileName
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHPackage -Session $session -ArgumentList $testingDeploymentName, $packageFileName
        Unzip "$packagePath\$packageFileName" "$tempFolder\tmp"
        Start-Sleep -Milliseconds 7000
        Test-Path "$tempFolder\tmp\ishws.cer" | Should be $true
        Test-Path "$tempFolder\tmp\CM Security Token Service Requirements.md" | Should be $true
    }



    AfterEach {
        if(Test-Path ("$tempFolder\tmp")){
            Remove-Item "$tempFolder\tmp" -Recurse -Force 
        }
    }

}