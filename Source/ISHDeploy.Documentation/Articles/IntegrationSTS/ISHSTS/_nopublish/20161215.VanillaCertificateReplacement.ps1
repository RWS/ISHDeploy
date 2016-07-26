#Certificate details
$certificateName="ISHSTS (20161201)"
$certificateThumbprint="20161201.Thumbprint"

# Set Token signing certificate
Set-ISHIntegrationSTSCertificate -ISHDeployment $deployment -Issuer $certificateName -Thumbprint $certificateThumbprint

# Set the ISHSTS token signing certificate
Set-ISHSTSConfiguration -ISHDeployment $deployment -Thumbprint $certificateThumbprint

# Set service certificate
Set-ISHAPIWCFServiceCertificate -ISHDeployment $deployment -Thumbprint $certificateThumbprint