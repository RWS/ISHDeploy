$VerbosePreference="Continue"
$secureString = "$password" | ConvertTo-SecureString -asPlainText -Force
$credential = New-Object System.Management.Automation.PSCredential ("$username", $secureString)
New-Service -Name "$name" -DisplayName "$displayName" -Description "$description" -BinaryPathName "$pathToExecutable" -StartupType Manual -Credential $credential
Write-Host "Start service is $startService"
if ("$startService" -like "True"){
	Start-Service -Name "$name"
	Write-Host "Service $name was started"
}