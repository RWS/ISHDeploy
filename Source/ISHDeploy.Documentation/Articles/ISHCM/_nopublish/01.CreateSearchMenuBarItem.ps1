$hash=@{
    Label="Custom Search"
    Type="Default"
    UserRole="Administrator"
	SearchXML="CustomDefaultSearch"
}
Set-ISHUISearchMenuBarItem -ISHDeployment $deploymentName @hash
