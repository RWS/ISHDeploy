$hash=@{
    Label="Custom Search"
    Type="Publication"
    UserRole=@(
		"Administrator"
		"Author"
	)
	SearchXML="CustomPublicationSearch"
}
Set-ISHUISearchMenuBarItem -ISHDeployment $deploymentName @hash
Move-ISHUISearchMenuBarItem -ISHDeployment $deploymentName -Label $hash["Label"] -First
