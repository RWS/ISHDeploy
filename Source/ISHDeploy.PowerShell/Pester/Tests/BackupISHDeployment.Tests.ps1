param(
    $session = $null,
    $testingDeploymentName = "InfoShare"
)
. "$PSScriptRoot\Common.ps1"


$moduleName = Invoke-CommandRemoteOrLocal -ScriptBlock { (Get-Module "ISHDeploy.*").Name } -Session $session
$backupPath = "C:\ProgramData\$moduleName\$($testingDeployment.Name)\Backup"
$computerName = $computerName.split(".")[0]
$uncPackagePath = "\\$computerName\" + ($backupPath.replace(":", "$"))
$pathToAppFolder = Join-Path $testingDeployment.WebPath ("\App{0}" -f $suffix )
$pathToDataFolder = Join-Path $testingDeployment.WebPath ("\Data{0}" -f $suffix )
$pathToWebFolder = Join-Path $testingDeployment.WebPath ("\Web{0}" -f $suffix )
$pathToBackupAppFolder = Join-Path $backupPath ("\App{0}" -f $suffix )
$pathToBackupDataFolder = Join-Path $backupPath ("\Data{0}" -f $suffix )
$pathToBackupWebFolder = Join-Path $backupPath ("\Web{0}" -f $suffix )

#endregion

#region Script Blocks 

$scriptBlockBackupISHDeployment = {
    param (
        [Parameter(Mandatory=$true)]
        $ishDeployName ,
        [Parameter(Mandatory=$true)]
        $paths,
        [Parameter(Mandatory=$true)]
        $switch
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }  
    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    if ($switch -eq "Web"){
        Backup-ISHDeployment -ISHDeployment $ishDeploy -Path $paths -Web
    }
    elseif($switch -eq "App"){
        Backup-ISHDeployment -ISHDeployment $ishDeploy -Path $paths -App
    }
    elseif($switch -eq "Data") {
        Backup-ISHDeployment -ISHDeployment $ishDeploy -Path $paths -Data
    }
    else
    {
        Write-Error "Wrong switch parameter"
    }
}

function GetListOfFiles {
    param (
        [Parameter(Mandatory=$true)]
        $path,
        [Parameter(Mandatory=$true)]
        $filter
    ) 
    $listOfFiles = (Get-ChildItem -Path $path –File -Filter $filter -Recurse | sort FullName | ForEach-Object { $_.FullName.Replace($path, "").ToLower() })
    return $listOfFiles
}

Describe "Testing Backup-ISHDeployment"{
    BeforeEach {
        UndoDeploymentBackToVanila $testingDeploymentName $true
    }

    It "Backup-ISHDeployment backup all *.dll in Web files"{
		#Arrange
        $listOfOriginalFiles = GetListOfFiles $pathToWebFolder "*.dll"
        #Action
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockBackupISHDeployment -Session $session -ArgumentList $testingDeploymentName, "*.dll", "Web"
        #Assert
        $listOfBackupFiles = GetListOfFiles $pathToBackupWebFolder "*.dll"

        Compare-Object $listOfBackupFiles $listOfOriginalFiles | Should be $null
    }

    It "Backup-ISHDeployment backup only Trisoft.InfoShare.dll in Web files"{
		#Arrange
        $listOfOriginalFiles = GetListOfFiles $pathToWebFolder "Trisoft.InfoShare.dll"
        #Action
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockBackupISHDeployment -Session $session -ArgumentList $testingDeploymentName, "Trisoft.InfoShare.dll", "Web"
        #Assert
        $listOfBackupFiles = GetListOfFiles $pathToBackupWebFolder "Trisoft.InfoShare.dll"

        Compare-Object $listOfBackupFiles $listOfOriginalFiles | Should be $null
    }
}