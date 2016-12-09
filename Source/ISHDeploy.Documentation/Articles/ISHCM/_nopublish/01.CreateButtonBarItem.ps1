$hash=@{
    Name="Custom1"
    Icon="Custom/Images/custom1.png"
    JSFunction="namespace.custom1"
<# 
	# if the custom1 javascript function accepts parameters of string,boolean,int then add the following
	JSArgumentsList=@(
		"string"
		$true
		[int]0
	)
#>
}

Set-ISHUIButtonBarItem -ISHDeployment $deploymentName @hash -Logical
Set-ISHUIButtonBarItem -ISHDeployment $deploymentName @hash -Version
Set-ISHUIButtonBarItem -ISHDeployment $deploymentName @hash -Language