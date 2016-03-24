CLS
Import-Module ISHDeploy

# $DebugPreference = "SilentlyContinue"

$DebugPreference = "Continue"
$VerbosePreference = "Continue"
$WarningPreference = "Continue"

$info = @{
  "WebPath" = "F:\InfoShare\";
  "Suffix" = ''
}

#EXAMPLE

Enable-ISHUIQualityAssistant -ISHDeployment $info