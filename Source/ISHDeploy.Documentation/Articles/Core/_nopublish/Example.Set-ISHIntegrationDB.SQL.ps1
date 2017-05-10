$initialCatalog="ISHDatabase"
$dataSource="MSSQLServer"

# SQL example with user/password security
$userId="isource"
$password=""
$securePassword = ConvertTo-SecureString -String $Password -AsPlainText -Force

$credentials=New-Object System.Management.Automation.PSCredential($UserName, $SecurePassword)
Set-ISHIntegrationDB -ISHDeployment $deploymentName -SQLServer -DataSource $dataSource -InitialCatalog $initialCatalog -Credentials $credentials

# SQL example with integrated security
Set-ISHIntegrationDB -ISHDeployment $deploymentName -SQLServer -DataSource $dataSource -InitialCatalog $initialCatalog
