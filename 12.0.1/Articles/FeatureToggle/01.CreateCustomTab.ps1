$hash=@{
    Label="Custom Event"
    Description="Show all custom events"
    EventTypesFilter="CUSTOM"
}
Set-ISHUIEventMonitorTab -ISHDeployment $deploymentName @hash
