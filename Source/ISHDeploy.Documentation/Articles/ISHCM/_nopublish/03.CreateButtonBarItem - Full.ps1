$deploymentName="InfoShare"

$zipFileName="Custom.zip"

$fileRelativePaths=@(
    "Extensions\namespace.custom1.js"
    "Images\custom1.png"
)

#region Copy the package into the module's staging folder
$targetUploadPath=Get-ISHPackageFolderPath -ISHDeployment $deploymentName
Copy-Item $zipFileName $targetUploadPath
#endregion

#region Expand customization into ISHCM custom folder
Expand-ISHCMFile -ISHDeployment $deploymentName -Filename $zipFileName -ToCustom
#endregion

#region register extention
$extensionRelativePaths=@(
    "Extensions\namespace.custom1.js"
)

Set-ISHCMCUILResourceGroup -ISHDeployment $deploymentName -Path $extensionRelativePaths
#endregion

$hash=@{
    Name="Custom1"
    Icon="Custom/Images/custom1.png"
    JSFunction="namespace.custom1"
}

Set-ISHUIButtonBarItem -ISHDeployment $deploymentName @hash -Logical
Set-ISHUIButtonBarItem -ISHDeployment $deploymentName @hash -Version
Set-ISHUIButtonBarItem -ISHDeployment $deploymentName @hash -Language