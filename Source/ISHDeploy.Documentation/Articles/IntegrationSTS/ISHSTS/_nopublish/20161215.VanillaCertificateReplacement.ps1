#Certificate details
$certificateName="ISHSTS (20161201)"
$certificateThumbprint="20161201.Thumbprint"

# Set the ISHSTS token signing certificate
# Execute before Set-ISHIntegrationSTSCertificate
Set-ISHSTSConfiguration -ISHDeployment $deploymentName -TokenSigningCertificateThumbprint $certificateThumbprint

# Set Token signing certificate
# Execute after Set-ISHSTSConfiguration
Set-ISHIntegrationSTSCertificate -ISHDeployment $deploymentName -Issuer $certificateName -Thumbprint $certificateThumbprint

# Set service certificate
Set-ISHAPIWCFServiceCertificate -ISHDeployment $deploymentName -Thumbprint $certificateThumbprint