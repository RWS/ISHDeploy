CLS
Import-Module InfoShare.Deployment

# $DebugPreference = "SilentlyContinue"

$DebugPreference = "Continue"
$VerbosePreference = "Continue"
$WarningPreference = "Continue"

#EXAMPLE


$info = @{
  "InstallPath" = "F:\InfoShare\";
  "Suffix" = ''
}

#Pause

#Enable-ISHExternalPreview -IshProject $info -ExternalId "qweqrqwe"

#Pause

Disable-ISHExternalPreview -IshProject $info 

