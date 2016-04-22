#$deployment = Get-ISHDeployment

Set-ISHIntegrationSTSWSTrust -ISHDeployment $deployment[0] -Endpoint "https://mecdev12qa02.global.sdl.corp/ISHSTSORA12/issue/wstrust/mixed/username" -MexEndpoint "https://mecdev12qa02.global.sdl.corp/ISHSTSORA12/issue/wstrust/mex" -BindingType "UserNameMixed" -ActorUsername "admin" -ActorPassword "admin" -Verbose -Debug