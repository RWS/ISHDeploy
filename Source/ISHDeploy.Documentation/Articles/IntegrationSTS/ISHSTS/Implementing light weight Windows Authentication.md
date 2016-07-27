# Implementing light weight Windows Authentication
 
This article explains how to integrate with Active Directory the out of the box **Security Token Service** provided in the Vanilla deployment, also referred to as **ISHSTS**.

# Acknowledgements

Content Manager is installed on `ish.example.com` and ISHSTS is installed on `ish.example.com/ISHSTS`. 

Set deploymentName variable.
```powershell
$deploymentName="InfoShare"
```

# Enable Windows Authentication

Then configure **ISHSTS** to provide windows authentication
CopyCodeBlock(_nopublish\Set-ISHSTSConfiguration.Authentication.ps1)

`Set-ISHSTSConfiguration` will raise a warning when the connection combines one all of the following conditions:

- That integrated database is a SQL Server
- The connection string used integrated authentication instead of a username/password combination.

The warning is raised because some additional actions are required on the database to complete this configuration. 

# Configure the SQL Server database

We need to grant permissions to the server and database for the principal that matches the computer account that hosts ISHSTS.

`Save-ISHIntegrationDBSTSSQLServerConfiguration` will export a **sql** or **PowerShell** script based on the value provided to the `-Type` parameter.

For example
CopyCodeBlock(_nopublish\Save-ISHIntegrationDBSTSSQLServerConfiguration.ps1)

If you saved a sql script then execute that on the same SQL Server referenced by the connection string. 

The saved PowerShell script embeds the SQL server name and database and uses windows authentication to access the database. 
To execute it there are two conditions:

- The current user has access to the target database. The script uses windows authentication and for this reason `Save-ISHIntegrationDBSTSSQLServerConfiguration` generates a warning.

> WARNING: Current implementation of .ps1 works only with windows authentication.

- [SQLPS](https://msdn.microsoft.com/en-us/library/hh245198.aspx) PowerShell modules is installed. The module is normally installed allong **SQL Server Management Studio**.

If all conditions are satisfied then executing the script completes the task.

```powershell
& .\Invoke-GrantPermissionsOnSQLServer.ps1
```

