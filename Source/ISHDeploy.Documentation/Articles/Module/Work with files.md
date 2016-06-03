# Work with files
 
This article explains how to work with cmdlets that use files as input or output. All cmdlets use a **FileName** parameter and do not accept values with paths. 
The location of the files is abstracted away and to copy form from/to it the module offers [Get-ISHPackageFolderPath](..\commands\Get-ISHPackageFolderPath.md).
 
## Cmdlet working with files
Some of the cmdlets of the module depend on a file for their operation or their output is too complicated to write into the console.

The module uses a specific folder within `$env:ProgramData`: 
- All saved files from cmdlets are written in this location.
- All files that drive a cmdlet must first be uploaded in this location.

To abstract the location away, the module provides the `Get-ISHPackageFolderPath` that returns the location in two possible formats:
- Local folder format e.g. `C:\ProgramData\ISHDeploy.X.0.Y\InfoShareSQL\Packages`
- UNC path format e.g. `\\COMPUTER\C$\ProgramData\ISHDeploy.X.0.Y\InfoShareSQL\Packages`. This format is focused on scripts with remote targets.

## Script target is local
First get a reference to a deployment and get the package location.

```powershell
$deployment=Get-ISHDeployment -Name InfoShare
$location=Get-ISHPackageFolderPath -ISHDeployment $deployment
```

To upload a file from path `$inputFilePath` into the module package directory execute 
```powershell
Copy-Item $inputFilePath $location
```

To download a file with name `$fileName` saved by a module cmdlet to the temp directory execute
```
$sourcePath=Join-Path $location $fileName
Copy-Item $sourcePath $env:TEMP
```

## Script target is remote
In this case we work with a remote computer `$computerName="SERVER"` where the module is deployed. 
To keep the examples simple when using `Invoke-Command` the `$Using` pattern will be used to access local variables from the script block. 

PowerShell v5.0 offers new features with regards to working with files and remote sessions. Depending on the version there are two options.

### PowerShell v4.0
With PowerShell v4.0 we must work with UNC paths.

First get a reference to a deployment and get the package location in UNC format.

```powershell
$deployment=Invoke-Command -ComputerName $computerName -ScriptBlock {Get-ISHDeployment -Name InfoShare}
$location=Invoke-Command -ComputerName $computerName -ScriptBlock {Get-ISHPackageFolderPath -ISHDeployment $Using:deployment -UNC}
```

To upload a file from path `$inputFilePath` into the module package directory execute 
```powershell
Copy-Item $inputFilePath $location
```

To download a file with name `$fileName` saved by a module cmdlet to the temp directory execute
```
$sourcePath=Join-Path $location $fileName
Copy-Item $sourcePath $env:TEMP
```

### PowerShell v5.0
With PowerShell v5.0 we get the ability to drive a cmdlet such as `Copy-Item` with `-FromSession` and `-ToSession`. When using these parameters, also file paths are local to the from/to computer referenced by the session.

First create a `PSSession`, then create a reference to a deployment and get the package location in local format.

```powershell
$session=New-PSSession -ComputerName $computerName
$deployment=Invoke-Command -Session $session -ScriptBlock {Get-ISHDeployment -Name InfoShare}
$location=Invoke-Command -Session $session -ScriptBlock {Get-ISHPackageFolderPath -ISHDeployment $Using:deployment}
```

To upload a file from path `$inputFilePath` into the module package directory execute 
```powershell
Copy-Item $inputFilePath $location -ToSession $session
```

To download a file with name `$fileName` saved by a module cmdlet to the temp directory execute
```
$sourcePath=Join-Path $location $fileName
Copy-Item $sourcePath $env:TEMP -FromSession $session
```