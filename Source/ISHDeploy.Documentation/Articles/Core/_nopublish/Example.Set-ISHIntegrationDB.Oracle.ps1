$serviceName="ISHDatabase"
$host="OracleServer"

$userId="isource"
$password=""
$securePassword = ConvertTo-SecureString -String $Password -AsPlainText -Force

# Oracle example
Set-ISHIntegrationDB -ISHDeployment $deploymentName -SQLServer -DataSource $dataSource -InitialCatalog $serviceName -Credentials $credentials

