param(
[string] $targetPC
)

#Create session to remote PC
$session = New-PSSession -ComputerName $targetPC 

#Run tests
Invoke-Command -ScriptBlock {C:\Users\$env:USERNAME\Documents\AutomatedTests\TestInitializer.ps1} -Session $session

#Get test results
$testResults = gci "\\$targetPC\C$\Automated_deployment\" -Filter "Test*"

foreach ($testResult in $testResults) {

    Copy-Item \\$targetPC\C$\Automated_deployment\$testResult 'C:\Users\Public\Documents'

}

#Close session
Remove-PSSession -Session $session