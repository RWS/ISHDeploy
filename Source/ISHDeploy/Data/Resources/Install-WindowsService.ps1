$VerbosePreference="Continue"
$secureString = "$password" | ConvertTo-SecureString -asPlainText -Force
$credential = New-Object System.Management.Automation.PSCredential ("$username", "$secureString")
new-service -Name "$name" -DisplayName "$displayName" -Description "$description" -BinaryPathName "$pathToExecutable" -StartupType Manual -Credential $credential