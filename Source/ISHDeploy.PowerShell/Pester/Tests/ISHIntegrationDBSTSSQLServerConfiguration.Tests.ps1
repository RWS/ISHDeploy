param(
    $session = $null,
    $testingDeploymentName = "InfoShare"
)
. "$PSScriptRoot\Common.ps1"

$computerName = If ($session) {$session.ComputerName} Else {$env:COMPUTERNAME}


#region variables

$testFileName = "testFileName"

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
$suffix = GetProjectSuffix($testingDeployment.Name)
$moduleName = Invoke-CommandRemoteOrLocal -ScriptBlock { (Get-Module "ISHDeploy.*").Name } -Session $session
$packagePath = "C:\ProgramData\$moduleName\$($testingDeployment.Name)\Packages"
$computerName = $computerName.split(".")[0]
$uncPackagePath = "\\$computerName\" + ($packagePath.replace(":", "$"))
#endregion

#region Script Blocks 
$scriptBlockCleanTmpFolder = {
    param (
        [Parameter(Mandatory=$false)]
        $packagePath
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }

    if (!(Test-Path "$packagePath\tmp"))
    {
        New-Item "$packagePath\tmp" -ItemType directory
    }

    Get-ChildItem -Path "$packagePath\tmp" -Include *.* -File -Recurse | foreach { $_.Delete()}
}

$scriptGetPackageFolder = {
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

$scriptBlockSaveScript = {
    param (
        [Parameter(Mandatory=$false)]
        $ishDeployName,
        [Parameter(Mandatory=$false)]
        $filename,
        [Parameter(Mandatory=$false)]
        $OutputType
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    if($OutputType){
        Save-ISHIntegrationDBSTSSQLServerConfiguration -ISHDeployment $ishDeploy -Filename $filename -Type PS1
    }
    else{
        Save-ISHIntegrationDBSTSSQLServerConfiguration -ISHDeployment $ishDeploy -Filename $filename 
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

Describe "Testing ISHIntegrationDBSTSSQLServerConfiguration"{
    BeforeEach {
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockUndoDeployment -Session $session -ArgumentList $testingDeploymentName
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockCleanTmpFolder -Session $session -ArgumentList $packagePath
    }
    

    It "Save file"{
        $packagePath = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptGetPackageFolder -Session $session -ArgumentList $testingDeploymentName, $true
        RemotePathCheck $packagePath | Should be "True"
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSaveScript -Session $session -ArgumentList $testingDeploymentName, $testFileName
        
        RemotePathCheck "$packagePath\$testFileName" | Should be $true
        $Mdfile = Get-Content "$packagePath\$testFileName"
        $Mdfile -contains "USE [MASTER]" | Should be $true
        $Mdfile -contains "CREATE LOGIN [GLOBAL\infoshareserviceuser] FROM WINDOWS WITH DEFAULT_DATABASE=[ISH11_MECDVTRI14QA03]" | Should be $true
        $Mdfile -contains "CREATE LOGIN [GLOBAL\infoshareserviceuser] FROM WINDOWS WITH DEFAULT_DATABASE=[ISH11_MECDVTRI14QA03]" | Should be $true
    }

    It "Save same file"{
        $packagePath = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptGetPackageFolder -Session $session -ArgumentList $testingDeploymentName, $true
        RemotePathCheck $packagePath | Should be "True"
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSaveScript -Session $session -ArgumentList $testingDeploymentName, $testFileName
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSaveScript -Session $session -ArgumentList $testingDeploymentName, $testFileName
        
        RemotePathCheck "$packagePath\$testFileName" | Should be $true
    }

    It "Save PS1 file"{
        $packagePath = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptGetPackageFolder -Session $session -ArgumentList $testingDeploymentName, $true
        RemotePathCheck $packagePath | Should be "True"
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSaveScript -Session $session -ArgumentList $testingDeploymentName, $testFileName, $true
        
        RemotePathCheck "$packagePath\$testFileName" | Should be $true
        $Mdfile = Get-Content "$packagePath\$testFileName"

        $Mdfile = $Mdfile.Trim()

        $Mdfile -contains "if (-not (Get-Module -ListAvailable -Name SQLPS))" | Should be $true
    }

    It "Save same PS1 file"{
        $packagePath = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptGetPackageFolder -Session $session -ArgumentList $testingDeploymentName, $true
        RemotePathCheck $packagePath | Should be "True"
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSaveScript -Session $session -ArgumentList $testingDeploymentName, $testFileName, $true
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSaveScript -Session $session -ArgumentList $testingDeploymentName, $testFileName, $true
        
        RemotePathCheck "$packagePath\$testFileName" | Should be $true
    }
}