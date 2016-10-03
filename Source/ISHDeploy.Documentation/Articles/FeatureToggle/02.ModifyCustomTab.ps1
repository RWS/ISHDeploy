$hash=@{
    Label="Custom Event"
    Description="Show all custom events"
    EventTypesFilter=@("CUSTOM1","CUSTOM2")
}
Set-ISHUIEventMonitorMenuBarItem -ISHDeployment $deploymentName @hash
Move-ISHUIEventMonitorMenuBarItem -ISHDeployment $deploymentName -Label $hash["Label"] -First
