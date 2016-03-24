CLS
Import-Module ISHDeploy

$DebugPreference = "Continue"
$VerbosePreference = "Continue"
$WarningPreference = "Continue"

Get-ISHDeployment -Deployment "" | Set-ISHDeployment

