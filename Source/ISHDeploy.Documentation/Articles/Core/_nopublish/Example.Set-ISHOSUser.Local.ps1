$username="$($env:COMPUTERNAME)\localuser"
$password="password"

$securePassword = ConvertTo-SecureString -String $password -AsPlainText -Force
$credential=New-Object System.Management.Automation.PSCredential($username, $securePassword)

Set-ISHOSUser -ISHDeployment $deploymentName -Credential $credential