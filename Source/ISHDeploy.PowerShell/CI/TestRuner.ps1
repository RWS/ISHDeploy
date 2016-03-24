param(
[string] $artifactPath,
[string] $targetPC
)


#Set up target path for infoshareserviceuser for dll
$targetPath = "\\$targetPC\C$\Users\$env:USERNAME\Documents\WindowsPowerShell\Modules\InfoShare.Deployment"

#Create session to remote PC
$session = New-PSSession -ComputerName $targetPC 

#Kill all powershell instances of powershell on remote PC. It will allow to copy dll. Also kills session
Invoke-Command -ScriptBlock {"$targetPath\kill_powershell.bat"} -Session $session

#Create session again
$session = New-PSSession -ComputerName $targetPC 

#Copy dll to remote pc
Copy-Item  $artifactPath $targetPath -force

#Run tests
Invoke-Command -ScriptBlock {C:\Users\$env:USERNAME\Documents\AutomatedTests\TestInitializer.ps1} -Session $session

#Get test results
$testResults = gci "\\$targetPC\C$\Automated_deployment\" -Filter "Test*"

foreach ($testResult in $testResults) {

    Copy-Item \\$targetPC\C$\Automated_deployment\$testResult 'C:\Users\Public\Documents'

}

#Close session
Remove-PSSession -Session $session