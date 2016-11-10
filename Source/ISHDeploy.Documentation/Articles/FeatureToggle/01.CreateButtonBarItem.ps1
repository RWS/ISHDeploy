$hash=@{
    Name="Custom1"
    Icon="UIFramework/custom1.png"
    Action="custom1()"
}

Set-ISHUIButtonBarItem -ISHDeployment $deploymentName @hash -Logical
Set-ISHUIButtonBarItem -ISHDeployment $deploymentName @hash -Version
Set-ISHUIButtonBarItem -ISHDeployment $deploymentName @hash -Language