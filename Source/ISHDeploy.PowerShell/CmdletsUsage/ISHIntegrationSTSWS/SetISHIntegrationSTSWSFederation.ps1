#$deployment = Get-ISHDeployment

Set-ISHIntegrationSTSWSFederation -ISHDeployment $deployment[0] -Endpoint "https://test.global.sdl.corp/InfoShareSTS/issue/wsfed"