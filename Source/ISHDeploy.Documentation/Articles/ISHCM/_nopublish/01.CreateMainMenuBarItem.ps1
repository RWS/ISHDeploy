$hash=@{
    Label="Custom"
    Action="Custom/HTML/custom.html"
    UserRole="Administrator"
}
Set-ISHUIMainMenuBarItem -ISHDeployment $deploymentName @hash