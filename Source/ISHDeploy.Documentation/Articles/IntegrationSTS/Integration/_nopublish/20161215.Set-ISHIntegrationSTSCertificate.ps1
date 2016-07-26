#Issuer name
$issuerName="Example STS (20161201)"
#Token signing thumbprint
$tokenSigningCertificateThumbprint="20161201.Thumbprint"

# Set Token signing certificate
Set-ISHIntegrationSTSCertificate -ISHDeployment $deploymentName -Issuer $issuerName -Thumbprint $tokenSigningCertificateThumbprint