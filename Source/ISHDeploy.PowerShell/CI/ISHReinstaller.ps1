$executingScriptDirectory = Split-Path -Path $MyInvocation.MyCommand.Definition -Parent
$installationPath = "C:\Automated_deployment"

$installScript = Join-Path $executingScriptDirectory "ISHInstall.ps1"
$uninstallScript = Join-Path $executingScriptDirectory "ISHUninstall.ps1"

$inputParametersDir = Join-Path $executingScriptDirectory "InputParameters"

# For each inputparameters.xml files unistall and install environment
Get-ChildItem $inputParametersDir | ForEach-Object {
	$instance = Get-ISHDeployment -Name $_.BaseName

	if($instance) {
		Invoke-Command -ScriptBlock { & ([ScriptBlock]::Create("$uninstallScript -deployment $($_.BaseName) -inputparameters $($_.FullName) -deploymentfolder $installationPath")) }
	}

	Invoke-Command -ScriptBlock { & ([ScriptBlock]::Create("$installScript -deploymentFolder $installationPath -inputparameters $($_.FullName)")) }
}