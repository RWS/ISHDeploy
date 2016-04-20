#Import-Module F:\Projects\ishdeploy\Source\ISHDeploy\bin\Debug\ISHDeploy.12.0.0.dll

#$deploy = Get-ISHDeployment

Set-ISHIntegrationSTSWSTrust -ISHDeployment $deploy[0] -Endpoint "google.com" -MexEndpoint "google.com.ua" -BindingType "WindowsMixed" -Verbose