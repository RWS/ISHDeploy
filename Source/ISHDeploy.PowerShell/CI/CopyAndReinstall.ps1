﻿param(
[string] $targetPC
)

#Create session to remote PC
$session = New-PSSession -ComputerName $targetPC

#Set up target path for infoshareserviceuser for dll
$targetPath = "\\$targetPC\C$\Users\infoshareserviceuser\Documents\WindowsPowerShell\Modules\InfoShare.Deployment"
#Get local folder with scripts
$executingScriptDirectory = Split-Path -Path $MyInvocation.MyCommand.Definition -Parent
#Copy scripts for installing
Copy-Item "$executingScriptDirectory\ISHInstall.ps1" $targetPath -force 
Copy-Item "$executingScriptDirectory\ISHReinstaller.ps1" $targetPath -force 
Copy-Item "$executingScriptDirectory\ISHUninstall.ps1" $targetPath -force

#Copy test scripts from CI to remote PC
$testsFolder = Join-Path (get-item $executingScriptDirectory ).parent.FullName "AutomationTests"
$testScripts = gci $testsFolder
Copy-Item "$executingScriptDirectory\kill_powershell.bat" $testsFolder -force

foreach($testScript in $testScripts){
	
	Copy-Item $testsFolder\$testScript \\$targetPC\C$\Users\infoshareserviceuser\Documents\AutomatedTests -Force
    
}


Invoke-Command -ScriptBlock {C:\Users\infoshareserviceuser\Documents\WindowsPowerShell\Modules\InfoShare.Deployment\ISHReinstaller.ps1} -Session $session