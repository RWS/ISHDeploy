CLS
Import-Module ISHDeploy

# $DebugPreference = "SilentlyContinue"

$DebugPreference = "Continue"
$VerbosePreference = "Continue"
$WarningPreference = "Continue"

#EXAMPLE


$info = @{
  "WebPath" = "F:\InfoShare\";
  "Suffix" = ''
}

Disable-ISHExternalPreview -ISHDeployment $info 

