$hash=@{
    Label="Custom Event"
    Description="Show all custom events"
    EventTypesFilter=@("CUSTOM1","CUSTOM2")
}
Set-ISHUIEventMonitorTab -ISHDeployment $deploymentName @hash
Move-ISHUIEventMonitorTab -ISHDeployment $deploymentName -Label $hash["Label"] -First
