#Issuer name
$issuerName="Example STS (20160101)"
#Token signing thumbprint
$tokenSigningCertificateThumbprint="20160101.Thumbprint"

# Set Token signing certificate
Set-ISHIntegrationSTSCertificate -ISHDeployment $deployment -Issuer $issuerName -Thumbprint $tokenSigningCertificateThumbprint
