#Issuer name
$issuerName="Example STS (20160101)"

# Remove Token signing certificate
Remove-ISHIntegrationSTSCertificate -ISHDeployment $deployment -Issuer $issuerName
