#Issuer name
$issuerName="Example STS (20160101)"

# Set Token signing certificate
Remove-ISHIntegrationSTSCertificate -ISHDeployment $deployment -Issuer $issuerName
