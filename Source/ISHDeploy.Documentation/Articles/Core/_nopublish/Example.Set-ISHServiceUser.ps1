$username="ServiceUser"
$password="password"

$securePassword = ConvertTo-SecureString -String $password -AsPlainText -Force
$credential=New-Object System.Management.Automation.PSCredential($username, $securePassword)

Set-ISHServiceUser -ISHDeployment $deploymentName -Credential $credential