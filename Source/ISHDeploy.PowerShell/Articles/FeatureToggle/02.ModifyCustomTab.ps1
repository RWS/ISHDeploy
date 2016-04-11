$hash=@{
    Label="Custom Event"
    Description="Show all custom events"
    EventTypesFilter=@("CUSTOM1","CUSTOM2")
}
Set-ISHUIEventMonitorTab -ISHDeployment $deployment @hash
Move-ISHUIEventMonitorTab -ISHDeployment $deployment -Label $hash["Label"] -First
