CLS
Import-Module ISHDeploy

$DebugPreference = "Continue"
$VerbosePreference = "Continue"
$WarningPreference = "Continue"

$deployment = Get-ISHDeployment

Get-ISHPackageFolderPath -ISHDeployment $deployment[0] -UNC
