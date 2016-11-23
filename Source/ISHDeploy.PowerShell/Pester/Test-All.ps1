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

$result = Invoke-Pester -Script @{Path = $testsFolder;Parameters = @{'testingDeploymentName' = $testingDeployment; 'session' = $session} } -OutputFormat NUnitXml -OutputFile $outputFile -PassThru

#Switch $ENV:PublishPackageToTest variable to prevent publishing on Nexus
if ($result.FailedCount -ne 0) {
    Write-Host ""
    Write-Host "------------------------------------------------------------------------------------------------"
    Write-HOST "No Publishing to Nexus will be made because some tests failed"
	Write-Host "------------------------------------------------------------------------------------------------"
	Write-Host ""
    throw "Test errors $result.FailedCount detected"
}


if ($session) {
    Remove-PSSession $session
}

return $result.FailedCount