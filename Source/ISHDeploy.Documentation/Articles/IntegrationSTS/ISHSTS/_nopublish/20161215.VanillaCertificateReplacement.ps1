#Certificate details
$certificateName="ISHSTS (20161201)"
$certificateThumbprint="20161201.Thumbprint"

# Set the ISHSTS token signing certificate
Set-ISHSTSConfiguration -ISHDeployment $deploymentName -Thumbprint $certificateThumbprint

# Set Token signing certificate
Set-ISHIntegrationSTSCertificate -ISHDeployment $deploymentName -Issuer $certificateName -Thumbprint $certificateThumbprint

# Set service certificate
Set-ISHAPIWCFServiceCertificate -ISHDeployment $deploymentName -Thumbprint $certificateThumbprint