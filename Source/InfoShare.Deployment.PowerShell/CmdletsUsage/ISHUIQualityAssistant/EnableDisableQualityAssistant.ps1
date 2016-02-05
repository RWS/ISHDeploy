CLS
Import-Module InfoShare.Deployment

# $DebugPreference = "SilentlyContinue"

$DebugPreference = "Continue"
$VerbosePreference = "Continue"
$WarningPreference = "Continue"

#EXAMPLE

Set-ISHProject "C:\Trisoft\RnDProjects\Trisoft\Test\Server.Web\Websites\Trisoft.InfoShare.Web" sites

Pause

Enable-ISHUIQualityAssistant

Pause

Disable-ISHUIQualityAssistant