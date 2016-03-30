param(
[string] $targetPC,
[string] $artifactPath
)

#Create session to remote PC
$session = New-PSSession -ComputerName $targetPC

#Set up target path for dll
$targetPath = "\\$targetPC\C$\Users\$env:USERNAME\Documents\WindowsPowerShell\Modules\ISHDeploy"

#In case if there is no folder for module - create it
if (!(Test-Path -path $targetPath)) {New-Item $targetPath -Type Directory}

#Create session to remote PC
$session = New-PSSession -ComputerName $targetPC 

#Kill all powershell instances of powershell on remote PC. It will allow to copy dll. Also kills session
Invoke-Command -ScriptBlock {"$targetPath\kill_powershell.bat"} -Session $session

#Create session again
$session = New-PSSession -ComputerName $targetPC 

#Get all artifacts in folder
$artifacts = gci $artifactPath

#Copy artifacts, that match pattern "ISHDeploy*" to remote pc
foreach ($artifact in $artifacts){
    Copy-Item  "$artifact" "$targetPath" -force
	
}


#Get local folder with scripts
$executingScriptDirectory = Split-Path -Path $MyInvocation.MyCommand.Definition -Parent
#Copy scripts for installing
Copy-Item "$executingScriptDirectory\ISHInstall.ps1" $targetPath -force 
Copy-Item "$executingScriptDirectory\ISHReinstaller.ps1" $targetPath -force 
Copy-Item "$executingScriptDirectory\ISHUninstall.ps1" $targetPath -force
Copy-Item "$executingScriptDirectory\kill_powershell.bat" $targetPath -force

#Get folder with automation tests 
$testsFolder = Join-Path (get-item $executingScriptDirectory ).parent.FullName "AutomationTests"
$testScripts = gci $testsFolder

#Copy test scripts from CI to remote PC

foreach($testScript in $testScripts){
	
	Copy-Item "$testsFolder\$testScript" "\\$targetPC\C$\Users\$env:USERNAME\Documents\AutomatedTests" -Force
    
}
 
Invoke-Command -ScriptBlock { & "C:\Users\$env:USERNAME\Documents\WindowsPowerShell\Modules\InfoShare.Deployment\ISHReinstaller.ps1"} -Session $session

#Kill session
Remove-PSSession -Session $session