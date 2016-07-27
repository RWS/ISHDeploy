# $deploymentName is the name of Deployment
 
# Set the license and enable the Content Editor
Set-ISHContentEditor -ISHDeployment $deploymentName -LicenseKey "licensekey" -Domain "ish.example.com"
Enable-ISHUIContentEditor -ISHDeployment $deploymentName
 
# Enable the Quality Assistant
Enable-ISHUIQualityAssistant -ISHDeployment $deploymentName
 
# Enable the External Preview using externalid
Enable-ISHExternalPreview -ISHDeployment $deploymentName -ExternalId "externalid"

# Create a new tab for CUSTOM event types
$hash=@{
    Label="Custom Event"
    Description="Show all custom events"
    EventTypesFilter=@("CUSTOM1","CUSTOM2")
}
Set-ISHUIEventMonitorTab -ISHDeployment $deploymentName @hash
Move-ISHUIEventMonitorTab -ISHDeployment $deploymentName -Label $hash["Label"] -First
