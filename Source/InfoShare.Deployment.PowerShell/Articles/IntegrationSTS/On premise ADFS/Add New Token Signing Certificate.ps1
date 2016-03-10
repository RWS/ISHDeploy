#region Target ADFS information
$tokenSigningCertificateThumbprint=""
#Validation mode for token signing certificate
$tokenSigningCertificateValidationMode="None"
#endregion

# First query the available deployments
$ishDeployment=Get-ISHDeployment -Project "ORA12"
Set-ISHIntegrationSTSCertificate -Deployment $ishDeployment -Thumbprint $tokenSigningCertificateThumbprint -Issuer "ADFS20180215" -ValidationMode $tokenSigningCertificateValidationMode

