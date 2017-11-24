param(
	$targetPC = $null,
	$testingDeployment = "InfoShare",
    [Parameter(Mandatory=$true)]
    $testDataFileUri,
	$outputFile
	)


$DebugPreference = "SilentlyContinue"
$VerbosePreference = "SilentlyContinue"

$client = New-Object System.Net.Webclient
$uri = [System.Uri]$testDataFileUri
$json = $client.DownloadString($uri.AbsoluteUri) 

$jsonData = $json | ConvertFrom-Json;

Set-Variable TestData -Value $jsonData -Scope Global -Force

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
	if ($session) {
		Remove-PSSession $session
	}
    throw "Test errors $result.FailedCount detected"
}


if ($session) {
    Remove-PSSession $session
}

return $result.FailedCount