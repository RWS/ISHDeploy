# Save the powershell script to configure the SQL database
Save-ISHIntegrationDBSTSSQLServerConfiguration -ISHDeployment $deploymentName -Type PS1 -FileName "Invoke-GrantPermissionsOnSQLServer.ps1"

# Save the sql script to run directly on SQL Server
Save-ISHIntegrationDBSTSSQLServerConfiguration -ISHDeployment $deploymentName -Type SQL -FileName "GrantPermissions.sql"