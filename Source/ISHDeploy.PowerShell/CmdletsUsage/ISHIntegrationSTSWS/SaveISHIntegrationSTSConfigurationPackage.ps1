$deployment = Get-ISHDeployment

Save-ISHIntegrationSTSConfigurationPackage -ISHDeployment $deployment[0] -FileName "testfile.zip" -Verbose -Debug

Save-ISHIntegrationSTSConfigurationPackage -ISHDeployment $deployment[0] -ADFS -FileName "testfile.zip" -Verbose -Debug