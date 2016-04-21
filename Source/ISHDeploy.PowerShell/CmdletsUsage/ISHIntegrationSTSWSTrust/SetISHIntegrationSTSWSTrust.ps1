#$deployment = Get-ISHDeployment

Set-ISHIntegrationSTSWSTrust -ISHDeployment $deployment[0] -Endpoint "google.com" -MexEndpoint "google.com.ua" -BindingType "WindowsMixed" -ActorUsername "Test User Name" -ActorPassword "123123123123" -Verbose -Debug