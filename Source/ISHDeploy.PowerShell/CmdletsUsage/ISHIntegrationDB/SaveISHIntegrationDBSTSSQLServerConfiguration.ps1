$deployment = Get-ISHDeployment

Save-ISHIntegrationDBSTSSQLServerConfiguration -ISHDeployment $deployment[0] -FileName GrantPermissions.sql -Verbose -Debug

Save-ISHIntegrationDBSTSSQLServerConfiguration -ISHDeployment $deployment[0] -FileName GrantPermissions.ps1 -Type PS1 -Verbose -Debug