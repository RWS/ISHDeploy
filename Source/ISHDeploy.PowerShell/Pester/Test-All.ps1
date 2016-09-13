param(
	$targetPC = $null,
	$testingDeployment = "InfoShare",
    [Parameter(Mandatory=$true)]
    $testDataFilePath,
	$outputFile
	)

& "$PSScriptRoot\Helpers\Init-TestData.ps1" -dataFilePath $testDataFilePath

$DebugPreference = "SilentlyContinue"
$VerbosePreference = "SilentlyContinue"

$session = $null
if($targetPC){
	$session = New-PSSession -ComputerName $targetPC
}
$executingScriptDirectory = Split-Path -Path $MyInvocation.MyCommand.Definition -Parent
$testsFolder = Join-Path $executingScriptDirectory "Tests" 

Invoke-Pester -Script @{Path = $testsFolder;Parameters = @{'testingDeploymentName' = $testingDeployment; 'session' = $session} } -OutputFormat NUnitXml -OutputFile $outputFile

if ($session) {
    Remove-PSSession $session
}