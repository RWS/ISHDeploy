CLS
Import-Module InfoShare.Deployment

$DebugPreference = "Continue"
$VerbosePreference = "Continue"
$WarningPreference = "Continue"

Get-ISHDeployment -Deployment "" | Set-ISHDeployment

