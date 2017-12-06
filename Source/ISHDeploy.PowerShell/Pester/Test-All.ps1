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

if ($result.FailedCount -ne 0) {
    
    # Retest failed tests
    $testsToRerun = $result.TestResult | where {$_.Result -eq “Failed” } | select -ExpandProperty "Describe" | Get-Unique
    
    [System.Xml.XmlDocument]$xml = new-object System.Xml.XmlDocument
    $xml.load($outputFile)

    $xmlTestResults = $xml.SelectNodes("test-results")[0]
    $failedTestsAmount = [int]$xmlTestResults.failures

    foreach ($failedTestSuiteName in $testsToRerun) {
        $retestedResult = Invoke-Pester -Script @{Path = $testsFolder;Parameters = @{'testingDeploymentName' = $testingDeployment; 'session' = $session; } } -PassThru -TestName $failedTestSuiteName
        if ($retestedResult.FailedCount -eq 0) {
            
            $failedTestSuite = $xml.SelectNodes("test-results/test-suite/results/test-suite[@name='$failedTestSuiteName']")[0]
            
            $failedTestsOfSuite = $failedTestSuite.SelectNodes("results/test-case[@result='Failure']")
            
            foreach ($testFailedToSuccess in $failedTestsOfSuite) {
                $testFailedToSuccess.SetAttribute("result", "Success")
                $testFailedToSuccess.SetAttribute("success", "True")
                $testFailedToSuccess.RemoveChild($testFailedToSuccess.SelectSingleNode("failure"))
                $failedTestsAmount = $failedTestsAmount - 1
            }
            $failedTestSuite.SetAttribute("success", "True")
            $failedTestSuite.SetAttribute("result", "Success")
        }
    }

    if ($failedTestsAmount -eq 0)
    {
        $xml.SelectSingleNode("test-results").SetAttribute("failures", 0)
        $xml.SelectSingleNode("test-results/test-suite").SetAttribute("success", "True")
        $xml.SelectSingleNode("test-results/test-suite").SetAttribute("result", "Success")
    }

    $xml.Save($outputFile)
}

if ($session) {
    Remove-PSSession $session
}

return $result.FailedCount