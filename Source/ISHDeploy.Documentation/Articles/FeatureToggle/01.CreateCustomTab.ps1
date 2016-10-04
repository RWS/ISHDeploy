$hash=@{
    Label="Custom Event"
    Description="Show all custom events"
    EventTypesFilter="CUSTOM"
}
Set-ISHUIEventMonitorMenuBarItem -ISHDeployment $deploymentName @hash
