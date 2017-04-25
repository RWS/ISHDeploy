$serviceName="ISHDatabase"
$host="OracleServer"

$userId="isource"
$password=""

# Oracle example
$connectionString="Provider=OraOLEDB.Oracle.1;Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=$host=)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=$serviceName)));User Id=$userId;Password=$password"
Set-ISHIntegrationDB -ISHDeployment $deploymentName -ConnectionString $connectionString -Raw

