# $deployment is acquired by Get-ISHDeployment
 
# Set the license and enable the Content Editor
Set-ISHContentEditor -ISHDeployment $deployment -LicenseKey "licensekey" -Domain "ish.example.com"
Enable-ISHUIContentEditor -ISHDeployment $deployment
 
# Enable the Quality Assistant
Enable-ISHUIQualityAssistant -ISHDeployment $deployment
 
# Enable the External Preview using externalid
Enable-ISHExternalPreview -ISHDeployment $deployment -ExternalId "externalid"

# Create a new tab for CUSTOM event types
$hash=@{
    Label="Custom Event"
    Description="Show all custom events"
    EventTypesFilter=@("CUSTOM1","CUSTOM2")
}
Set-ISHUIEventMonitorTab -ISHDeployment $deployment @hash
Move-ISHUIEventMonitorTab -ISHDeployment $deployment -Label $hash["Label"] -First
