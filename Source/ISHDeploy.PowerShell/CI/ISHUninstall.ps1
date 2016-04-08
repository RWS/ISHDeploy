Param( 
    [Parameter(Position=0,Mandatory=$true)][string] $deployment,
	[Parameter(Position=1,Mandatory=$true)][string] $inputparameters,
    [Parameter(Position=2,Mandatory=$true)][string] $deploymentfolder
)

$folder = Get-ChildItem $deploymentFolder | where {$_.Attributes -eq 'Directory'}

$installToolPath = Join-Path $deploymentFolder $folder 
$installToolPath = Join-Path $installToolPath '\__InstallTool'

Write-Host $installToolPath

$commandUninstall = 'cmd.exe /C '+ $installToolPath + '\InstallTool.exe -Uninstall -project '+ $deployment +' -inputparameters "C:\Users\infoshareserviceuser\Documents\inputparameters_ORA12.xml" '
Write-Host $commandUninstall

Invoke-Expression $commandUninstall