#Issuer name
$issuerName="Example STS (20160101)"

# Remove Token signing certificate
Remove-ISHIntegrationSTSCertificate -ISHDeployment $deploymentName -Issuer $issuerName
