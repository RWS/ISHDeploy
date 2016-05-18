# $deployment is acquired by Get-ISHDeployment
 
# Set the license and enable the Content Editor
Set-ISHContentEditor -ISHDeployment $deployment -LicenseKey "licensekey" -Domain "ish.example.com"
Enable-ISHUIContentEditor -ISHDeployment $deployment
 
# Enable the Quality Assistant
Enable-ISHUIQualityAssistant -ISHDeployment $deployment
 
# Enable the External Preview using externalid
Enable-ISHExternalPreview -ISHDeployment $deployment -ExternalId "externalid"