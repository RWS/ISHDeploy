param(
	[string] $targetPC,
	[string] $moduleFilePath, # comes like this : C:\SomeDirectoryPath\ISHDeploy*.dll
	[string] $repositoryName,
	[string] $repositoryPath
)

# Defining variables
$automationTestsFolderName = "Pester"
$remoteBaseDir = "\\$targetPC\C$\Users\$env:USERNAME\Documents\"
$remoteAutomationTestsDir = Join-Path $remoteBaseDir $automationTestsFolderName
$remoteReinstallScriptsDir = Join-Path $remoteBaseDir "ReinstallScripts"
$executingScriptDirectory = Split-Path -Path $MyInvocation.MyCommand.Definition -Parent
$localAutomationTests = Join-Path (get-item $executingScriptDirectory ).parent.FullName $automationTestsFolderName
$moduleName = Get-ChildItem $moduleFilePath

Write-Host "RemoteAutomationTestsDir: $remoteAutomationTestsDir"
Write-Host "RemoteReinstallScriptsDir: $remoteReinstallScriptsDir"
Write-Host "ExecutingScriptDirectory: $executingScriptDirectory"
Write-Host "LocalAutomationTests: $localAutomationTests"
Write-Host "Module Name: $moduleName"

# In case if there is no folder for module - create it
if (!(Test-Path -path $remoteAutomationTestsDir)) { New-Item $remoteAutomationTestsDir -Type Directory }
if (!(Test-Path -path $remoteReinstallScriptsDir)) { New-Item $remoteReinstallScriptsDir -Type Directory }

# Copy all scripts for uninstalling and installing Content Manager
Get-ChildItem $executingScriptDirectory | Copy-Item -Destination $remoteReinstallScriptsDir -Recurse -Force

# Copy all automation tests to target pc
Get-ChildItem $localAutomationTests | Copy-Item -Destination $remoteAutomationTestsDir -Recurse -Force

# ------------------------------------------------------------------------------------------------
# ------------------------------ Reinstall ISHDeploy.xx.x module ---------------------------------
# ------------------------------------------------------------------------------------------------
$scriptBlock = {
	param(
		[string] $moduleName,
		[string] $repositoryName,
		[string] $repositoryPath
	)

	# Uninstall previous ISHDeploy modules
	$installedModules = Get-Module -ListAvailable | Select-Object Name | ? Name -like "ISHDeploy*"
	if ($installedModules.Count -ge 1) {
		$installedModules | ForEach-Object { Write-Host "Uninstalling " $_.Name; Uninstall-Module -Name $_.Name }
	}

	# Registry repository if it's not registered
	$repository = Get-PSRepository $repositoryName -ErrorAction SilentlyContinue

	if ($repository.Count -eq 0) {
		Write-Host "Repository is not registered"
		Write-Host "Registering repository $repositoryName"

		$sourceLocation = $repositoryPath + "nuget/"
		Register-PSRepository -Name $repositoryName -SourceLocation $sourceLocation -PublishLocation  $repositoryPath -InstallationPolicy Trusted

		$repository = Get-PSRepository $repositoryName -ErrorAction SilentlyContinue
        if ($repository.Count -eq 0) {
            throw "Cannot register repository $repositoryName"
        }
	}

	Write-Host "Repository is registered!"
	
	# Install new module
	Write-Host "Installing " $moduleName
	Install-Module -Name $moduleName -Force
}

# Create session to targetPC, kill all powershell instances, that might lock ISHDeploy
$session = New-PSSession -ComputerName $targetPC 
Invoke-Command -ScriptBlock {"$targetPath\kill_powershell.bat"} -Session $session
$session = New-PSSession -ComputerName $targetPC

# Reinstall ISHDeploy module
Invoke-Command -ScriptBlock $scriptBlock -ArgumentList $moduleName.BaseName, $repositoryName, $repositoryPath -Session $session 

# ------------------------------------------------------------------------------------------------
# --------------- Reinstall Content Manager instances on the test environment --------------------
# ------------------------------------------------------------------------------------------------
Invoke-Command -ScriptBlock { & "C:\Users\$env:USERNAME\Documents\ReinstallScripts\ISHReinstaller.ps1" } -Session $session

#Kill session
Remove-PSSession -Session $session