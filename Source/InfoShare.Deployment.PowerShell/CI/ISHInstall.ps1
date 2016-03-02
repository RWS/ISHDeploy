Param( 
    [Parameter(Position=0,Mandatory=$true)][string] $deploymentFolder,
	[Parameter(Position=1,Mandatory=$true)][string] $inputparameters
	)



$folder = Get-ChildItem $deploymentFolder | where {$_.Attributes -eq 'Directory'}

$installToolPath = Join-Path $deploymentFolder $folder 
$installToolPath = Join-Path $installToolPath '\__InstallTool'

Write-Host $installToolPath

$commandInstall = 'cmd.exe /C '+ $installToolPath + '\InstallTool.exe -Install -cdroot '+ "C:\Automated_deployment\" + $folder.ToString() +' -installplan ' + $installToolPath + '\installplan.xml -inputparameters ' + "$inputparameters"

Write-Host $commandInstall
Invoke-Expression $commandInstall
