# Integrating with a database
 
This article explains how to work with cmdlets that get or set the target database. 
 
## Set deploymentName variable
First set deploymentName variable.

```powershell
$deploymentName="InfoShare"
```

## Getting the database integration

```
Get-ISHIntegrationDB -ISHDeployment $deploymentName
```

## Testing the database integration

```
Test-ISHIntegrationDB -ISHDeployment $deploymentName
```

## Setting the database integration

### SQL Server 

In this example we want to connect with a database e.g. `ISHDatabase` hosted on a SQL Server e.g `MSSQLServer`. The example has two variations, one with username/password security and one using SQL Server's integrated security.

CopyCodeBlock(_nopublish\Example.Set-ISHIntegrationDB.SQL.Raw.ps1)

The exact same is possible with using explicit parameters

CopyCodeBlock(_nopublish\Example.Set-ISHIntegrationDB.SQL.ps1)

### Oracle

In this example we want to connect with a serrice e.g. `ISHDatabase` hosted on an Oracle Server e.g `OracleServer`.

CopyCodeBlock(_nopublish\Example.Set-ISHIntegrationDB.Oracle.Raw.ps1)

The exact same is possible with using explicit parameters

CopyCodeBlock(_nopublish\Example.Set-ISHIntegrationDB.Oracle.ps1)