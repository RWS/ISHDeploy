# Integrating with a database
 
This tutorial explains how to work with cmdlets that get or set the target database. 
 
## Set deploymentName variable
First set deploymentName variable.

```powershell
$deploymentName="InfoShare"
```

## Getting the database integration

`Get-ISHIntegrationDB` cmdlet extracts information regarding the deployment's database integration.

```
Get-ISHIntegrationDB -ISHDeployment $deploymentName
```

would output 

```text
RawConnectionString     Engine
-------------------     ------
connectionstring        sqlserver2014
```

## Setting the database integration

The `Set-ISHIntegrationDB` sets the deployment's database integration. Valid target databases are:

- Oracle
- SQLServer 2012
- SQLServer 2014

### SQL Server 

In this example we want to connect with a database e.g. `ISHDatabase` hosted on a SQL Server 2014 e.g `MSSQLServer`. The example has two variations, one with username/password security and one using SQL Server's integrated security.

CopyCodeBlock(_nopublish\Example.Set-ISHIntegrationDB.SQL.Raw.ps1)

### Oracle

In this example we want to connect with a service e.g. `ISHDatabase` hosted on an Oracle Server e.g `OracleServer`.

CopyCodeBlock(_nopublish\Example.Set-ISHIntegrationDB.Oracle.Raw.ps1)

## Testing the database integration

`Test-ISHIntegrationDB` cmdlet tests if the deployment's database integration is valid. **Notice** that when an `oracle` engine is defined, then the `Test-ISHIntegrationDB` will skip and just raise a warning.

```
Test-ISHIntegrationDB -ISHDeployment $deploymentName
```