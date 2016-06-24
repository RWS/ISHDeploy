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
$inputParameters = Get-InputParameters $testingDeploymentName
[System.Data.OleDb.OleDbConnection]$connection = New-Object "System.Data.OleDb.OleDbConnection" $inputParameters["connectstring"]
$database = $connection.Database
$datasource = $connection.DataSource
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

$scriptBlockGetNetBIOSDomain = {
    $domain =@(nbtstat -n | select-string -Pattern '^\s*(\w+)\s*<(00|1[BCDE]){1}>\s+GROUP' -AllMatches | % { $_.Matches.Groups[1].Value} | select -Unique)[0]
    $principal = $domain+"\"+$env:computername+"$"
    return $principal
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

Describe "Testing ISHIntegrationDBSTSSQLServerConfiguration"{
    BeforeEach {
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockUndoDeployment -Session $session -ArgumentList $testingDeploymentName
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockCleanTmpFolder -Session $session -ArgumentList $packagePath
        $principal= Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetNetBIOSDomain -Session $session
    }
    

    It "Save-ISHIntegrationDBSTSSQLServerConfiguration Save file"{
        $packagePath = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptGetPackageFolder -Session $session -ArgumentList $testingDeploymentName, $true
        RemotePathCheck $packagePath | Should be "True"
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSaveScript -Session $session -ArgumentList $testingDeploymentName, $testFileName
        
        RemotePathCheck "$packagePath\$testFileName" | Should be $true
        
        $Mdfile = Get-Content "$packagePath\$testFileName"
        [System.String]$content = [System.String]::Join("", $Mdfile)
        $content.Contains("USE [MASTER]") | Should be $true
        $content.Contains("CREATE LOGIN [$principal] FROM WINDOWS WITH DEFAULT_DATABASE=[$database]") | Should be $true
    }

    It "Save-ISHIntegrationDBSTSSQLServerConfiguration Save same file"{
        $packagePath = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptGetPackageFolder -Session $session -ArgumentList $testingDeploymentName, $true
        RemotePathCheck $packagePath | Should be "True"
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSaveScript -Session $session -ArgumentList $testingDeploymentName, $testFileName
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSaveScript -Session $session -ArgumentList $testingDeploymentName, $testFileName
        
        RemotePathCheck "$packagePath\$testFileName" | Should be $true
    }

    It "Save-ISHIntegrationDBSTSSQLServerConfiguration Save PS1 file"{
        $packagePath = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptGetPackageFolder -Session $session -ArgumentList $testingDeploymentName, $true -WarningVariable Warning
        RemotePathCheck $packagePath | Should be "True"
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSaveScript -Session $session -ArgumentList $testingDeploymentName, $testFileName, $true
        
        RemotePathCheck "$packagePath\$testFileName" | Should be $true
        $Mdfile = Get-Content "$packagePath\$testFileName"
        [System.String]$content = [System.String]::Join("", $Mdfile)
        $content.Contains("`$server=`"$datasource`"") | Should be $true
        $content.Contains("`$principal=`"$principal`"") | Should be $true
        $Warning | Should be $null         
    }

    It "Save-ISHIntegrationDBSTSSQLServerConfiguration Save same PS1 file"{
        $packagePath = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptGetPackageFolder -Session $session -ArgumentList $testingDeploymentName, $true
        RemotePathCheck $packagePath | Should be "True"
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSaveScript -Session $session -ArgumentList $testingDeploymentName, $testFileName, $true
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSaveScript -Session $session -ArgumentList $testingDeploymentName, $testFileName, $true
        
        RemotePathCheck "$packagePath\$testFileName" | Should be $true
    }
}