CLS
Import-Module InfoShare.Deployment

# $DebugPreference = "SilentlyContinue"

$DebugPreference = "Continue"
$VerbosePreference = "Continue"
$WarningPreference = "Continue"

$info = @{
  "WebPath" = "F:\InfoShare\";
  "Suffix" = ''
}

#EXAMPLE

#Enable-ISHUIQualityAssistant -ISHDeployment $info

#Pause

Disable-ISHUIQualityAssistant -ISHDeployment $info