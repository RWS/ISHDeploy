$hash=@{
    Name="Custom1"
    Icon="UIFramework/custom1.png"
    Action="custom1()"
	ISHType=@(
		ISHIllustration
		ISHLibrary
		ISHMasterDoc
	)
}

Set-ISHUIButtonBarItem -ISHDeployment $deploymentName @hash -Logical
Set-ISHUIButtonBarItem -ISHDeployment $deploymentName @hash -Version
Set-ISHUIButtonBarItem -ISHDeployment $deploymentName @hash -Language

Move-ISHUIButtonBarItem -ISHDeployment $deploymentName -Name $hash.Name -Logical -First
Move-ISHUIButtonBarItem -ISHDeployment $deploymentName -Name $hash.Name -Version -First
Move-ISHUIButtonBarItem -ISHDeployment $deploymentName -Name $hash.Name -Language -First