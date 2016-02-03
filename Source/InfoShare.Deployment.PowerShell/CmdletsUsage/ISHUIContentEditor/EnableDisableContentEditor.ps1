CLS
Import-Module InfoShare.Deployment

# $DebugPreference = "SilentlyContinue"

$DebugPreference = "Continue"
$VerbosePreference = "Continue"
$WarningPreference = "Continue"

#EXAMPLE

Set-ISHProject "E:\Projects\RnDProjects\Trisoft\Dev\Server.Web" sites

Pause

Enable-ISHUIContentEditor

Pause

Disable-ISHUIContentEditor