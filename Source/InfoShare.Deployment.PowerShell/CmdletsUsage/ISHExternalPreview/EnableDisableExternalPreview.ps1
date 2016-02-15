CLS
Import-Module InfoShare.Deployment

# $DebugPreference = "SilentlyContinue"

$DebugPreference = "Continue"
$VerbosePreference = "Continue"
$WarningPreference = "Continue"

#EXAMPLE


$info = @{
  "WebPath" = "F:\InfoShare\";
  "Suffix" = ''
}

#Pause

#Enable-ISHExternalPreview -ISHDeployment $info -ExternalId "qweqrqwe"

#Pause

Disable-ISHExternalPreview -ISHDeployment $info 

