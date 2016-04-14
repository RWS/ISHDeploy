param(
	$session = $null,
	$testingDeployment = "InfoShare",
	$outputFile
	)

$DebugPreference="SilentlyContinue"
$VerbosePreference="SilentlyContinue"

$executingScriptDirectory = Split-Path -Path $MyInvocation.MyCommand.Definition -Parent
$testsFolder = Join-Path $executingScriptDirectory "Tests"

Invoke-Pester -Script @{Path = $testsFolder;Parameters = @{'testingDeploymentName' = $testingDeployment; 'session' = $session} } -OutputFormat NUnitXml -OutputFile $outputFile