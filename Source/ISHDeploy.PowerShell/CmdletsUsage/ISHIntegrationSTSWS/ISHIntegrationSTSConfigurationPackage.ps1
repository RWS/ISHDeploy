#$deployment = Get-ISHDeployment -Name InfoShare

Set-ISHIntegrationSTSConfigurationPackage -ISHDeployment $deployment -FileName package.zip

Set-ISHIntegrationSTSConfigurationPackage -ISHDeployment $deployment -ADFS -FileName package.zip