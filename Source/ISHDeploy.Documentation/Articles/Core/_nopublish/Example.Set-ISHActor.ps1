$username="ServiceUser"
$password="password"

$securePassword = ConvertTo-SecureString -String $password -AsPlainText -Force
$credential=New-Object System.Management.Automation.PSCredential($username, $securePassword)

Set-ISHActor -ISHDeployment $deploymentName -Credential $credential