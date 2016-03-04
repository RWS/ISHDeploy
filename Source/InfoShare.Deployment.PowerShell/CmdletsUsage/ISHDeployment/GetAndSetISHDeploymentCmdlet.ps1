CLS
Import-Module InfoShare.Deployment

$DebugPreference = "Continue"
$VerbosePreference = "Continue"
$WarningPreference = "Continue"

#Enable-ISHExternalPreview -ISHDeployment $info -ExternalId "qweqrqwe"

#Pause

Get-ISHDeployment -Deployment "" | Set-ISHDeployment

