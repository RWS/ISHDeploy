# Uri and credentials for SDL TMS
$uri="http://tms.example.com/"
$credential=Get-Credential -Message "SDL TMS integration credential" 

# Set the integration with required parameters
Set-ISHIntegrationTMS -ISHDeployment $ishDeployment -Name WorldServer -Uri $uri -Credential $credential -Mappings $mapping -Templates $templates

# Set the integration with extra parameters
Set-ISHIntegrationTMS -ISHDeployment $ishDeployment -Name WorldServer -Uri $uri -Credential $credential -Mappings $mapping -Templates $templates -RequestMetadata $requestMetadata -GroupMetadata $groupMetadata