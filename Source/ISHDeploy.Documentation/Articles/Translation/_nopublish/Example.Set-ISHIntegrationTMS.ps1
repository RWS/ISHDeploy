# Uri and credentials for SDL TMS
$uri="http://tms.example.com/"

# Set the integration with required parameters
Set-ISHIntegrationTMS -ISHDeployment $deploymentName -Name TMS -Uri $uri -Mappings $mappings -Templates $templates

# Set the integration with extra parameters
Set-ISHIntegrationTMS -ISHDeployment $deploymentName -Name TMS -Uri $uri -Mappings $mappings -Templates $templates -RequestMetadata $requestMetadata -GroupMetadata $groupMetadata