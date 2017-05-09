$initialCatalog="ISHDatabase"
$dataSource="MSSQLServer"

$userId="isource"
$password=""

# SQL example with user/password security
$connectionString="Provider=SQLOLEDB.1;Password=$password;Persist Security Info=True;User ID=$userId;Initial Catalog=$initialCatalog;Data Source=$dataSource"
Set-ISHIntegrationDB -ISHDeployment $deploymentName -ConnectionString $connectionString -Engine sqlserver2014 -Raw

# SQL example with integrated security
$connectionString="Provider=SQLOLEDB.1;Integrated Security=SSPI;Persist Security Info=True;Initial Catalog=$initialCatalog;Data Source=$dataSource"
Set-ISHIntegrationDB -ISHDeployment $deploymentName -ConnectionString $connectionString -Engine sqlserver2014 -Raw
