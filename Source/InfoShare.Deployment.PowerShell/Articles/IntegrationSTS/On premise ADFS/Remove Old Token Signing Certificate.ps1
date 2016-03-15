# First query the available deployments
$ishDeployment=Get-ISHDeployment -Project "ORA12"
Remove-ISHIntegrationSTSCertificate -Deployment $ishDeployment -Issuer "ADFS20170215"

