$hash=@{
    Label="Custom"
    Action="custom.html"
    UserRole="Administrator"
}
Set-ISHUIMainMenuBarItem -ISHDeployment $deploymentName @hash