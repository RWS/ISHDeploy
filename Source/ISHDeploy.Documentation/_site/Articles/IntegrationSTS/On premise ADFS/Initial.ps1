#region Target ADFS information
#WS Federation endpoint
$wsFederationUri=""
#WS Trust endpoint
$wsTrustUri=""
#The authentication type
$bindingType="WindowsMixed"
#Token signing thumbprint
$tokenSigningCertificateThumbprint=""
#Validation mode for token signing certificate
$tokenSigningCertificateValidationMode="None"
#endregion

# First query the available deployments
$ishDeployment=Get-ISHDeployment -Project "ORA12"
# Configure the STS integration values
Set-ISHIntegrationSTSWSFederation -Deployment $ishDeployment -Endpoint $wsFederationUri
Set-ISHIntegrationSTSWSTrust -Deployment $ishDeployment -Endpoint $wsTrustUri -Mex $mexUri -BindingType $bindingType -ActorUsername $actor -IncludeInternalClients
Set-ISHIntegrationSTSCertificate -Deployment $ishDeployment -Thumbprint $tokenSigningCertificateThumbprint -Issuer "ADFS20170215" -ValidationMode $tokenSigningCertificateValidationMode

# Get a package on how to configure ADFS
$integrationPackage=Get-ISHIntegrationSTSConfigurationPackage -Deployment $ishDeployment  -ADFS
