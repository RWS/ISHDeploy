$hash=@{
    Label="Custom Event"
    Description="Show all custom events"
    UserRole="Administrator"
	EventTypesFilter="CUSTOM"
}
Set-ISHUIEventMonitorMenuBarItem -ISHDeployment $deploymentName @hash
