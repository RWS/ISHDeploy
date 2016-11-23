$extensionRelativePaths=@(
    "Extensions\namespace.custom1.js"
)

Set-ISHCMCUILResourceGroup -ISHDeployment $deploymentName -Path $extensionRelativePaths
