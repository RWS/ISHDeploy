$VerbosePreference="Continue"
$secureString = "$password" | ConvertTo-SecureString -asPlainText -Force
$credential = New-Object System.Management.Automation.PSCredential ("$username", $secureString)
New-Service -Name "$name" -DisplayName "$displayName" -Description "$description" -BinaryPathName "$pathToExecutable" -StartupType Manual -Credential $credential

#sc.exe failure "$name" reset= 86400 reset= 0 actions= "restart/60000"
#sc.exe config "$name" start= demand/delayed-auto