$hash=@{
    Label="Custom"
    Action="Custom/HTML/custom..V2.html"
    UserRole=@(
		"Administrator"
		"Author"
	)
}
Set-ISHUIMainMenuBarItem -ISHDeployment $deploymentName @hash
Move-ISHUIMainMenuBarItem -ISHDeployment $deploymentName -Label $hash["Label"] -First
