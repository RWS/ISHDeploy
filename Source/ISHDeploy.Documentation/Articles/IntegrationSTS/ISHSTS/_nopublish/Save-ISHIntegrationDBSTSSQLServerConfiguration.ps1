# Save the powershell script to configure the SQL database
Save-ISHIntegrationDBSTSSQLServerConfiguration -ISHDeployment $deployment -Type PS1 -FileName "Invoke-GrantPermissionsOnSQLServer.ps1"

# Save the sql script to run directly on SQL Server
Save-ISHIntegrationDBSTSSQLServerConfiguration -ISHDeployment $deployment -Type SQL -FileName "GrantPermissions.sql"