#$deployment = Get-ISHDeployment -Name InfoShare

Set-ISHIntegrationSTSConfigurationPackage -ISHDeployment $deployment -FileName package.zip