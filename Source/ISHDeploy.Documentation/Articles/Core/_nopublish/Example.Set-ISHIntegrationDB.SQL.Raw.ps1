$initialCatalog="ISHDatabase"
$dataSource="MSSQLServer"

$userId="isource"
$password=""

# SQL example with user/password security
$connectionString="Provider=SQLOLEDB.1;Password=isource;Persist Security Info=True;User ID=isource;Initial Catalog=$initialCatalog;Data Source=$dataSource"
Set-ISHIntegrationDB -ISHDeployment $deploymentName -ConnectionString $connectionString -Raw

# SQL example with integrated security
$connectionString="Provider=SQLOLEDB.1;Integrated Security=SSPI;Persist Security Info=True;Initial Catalog=$initialCatalog;Data Source=$dataSource"
Set-ISHIntegrationDB -ISHDeployment $deploymentName -ConnectionString $connectionString -Raw
