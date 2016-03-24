param([Parameter(Position=0,Mandatory=$true)][string] $artifactPath)

#$artifactPath ='C:\Program Files (x86)\Jenkins\workspace\Build ISHDeploy develop branch on commit\Source\ISHDeploy\bin\Release\ISHDeploy.dll'

$targetPath = '\\10.91.5.34\C$\Users\infoshareserviceuser\Documents\WindowsPowerShell\Modules\ISHDeploy'


$password =  convertto-securestring -AsPlainText -Force -String "!nfoshar3"
$cred = new-object -typename System.Management.Automation.PSCredential -argumentlist "global\infoshareserviceuser", $password
$session = New-PSSession -ComputerName WIN-BCMCO6U3OI4.global.sdl.corp -Credential $cred


Invoke-Command -ScriptBlock {C:\Users\infoshareserviceuser\Documents\AutomatedTests\kill_powershell.bat} -Session $session

$session = New-PSSession -ComputerName WIN-BCMCO6U3OI4.global.sdl.corp -Credential $cred
Copy-Item  $artifactPath $targetPath -force

Invoke-Command -ScriptBlock {C:\Users\infoshareserviceuser\Documents\AutomatedTests\TestInitializer.ps1} -Session $session

$testResults = gci '\\10.91.5.34\C$\Automated_deployment' -Filter "Test*"

foreach ($testResult in $testResults) {

    Copy-Item \\10.91.5.34\C$\Automated_deployment\$testResult 'C:\Users\Public\Documents'

}

Remove-PSSession -Session $session
