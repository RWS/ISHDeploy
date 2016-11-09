$hash=@{
    Label="Custom"
    Action="custom.V2.html"
    UserRole=@(
		"Administrator"
		"User"
	)
}
Set-ISHUIMainMenuBarItem -ISHDeployment $deploymentName @hash
Move-ISHUIMainMenuBarItem -ISHDeployment $deploymentName -Label $hash["Label"] -First
