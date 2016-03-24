CLS
Import-Module "C:\Stash Projects\Infoshare Deployment\Source\ISHDeploy\bin\Debug\ISHDeploy.dll"

# $DebugPreference = "SilentlyContinue"

$DebugPreference = "Continue"
$VerbosePreference = "Continue"
$WarningPreference = "Continue"

#EXAMPLE

$dict = New-Object "System.Collections.Generic.Dictionary``2[System.String,System.String]"
$dict.Add('webpath', 'C:\Trisoft\RnDProjects\Trisoft\Test\Server.Web')
$dict.Add('apppath', 'C:\Trisoft\RnDProjects\Trisoft\Test\Server.Web')
$dict.Add('projectsuffix', 'sites')
$dict.Add('datapath', 'C:\Trisoft\RnDProjects\Trisoft\Test\Server.Web')
$version = New-Object System.Version -ArgumentList '1.0.0.0';

$deploy = New-Object ISHDeploy.Models.ISHDeployment -ArgumentList ($dict, $version)

Set-ISHUIEventMonitorTab -ISHDeployment $deploy -Label "2NewTab" -Icon "~/UIFramework/new-tab.job.32x32.png" -EventTypesFilter "TRANSLATIONJOB, NON-TRANSLATIONJOB" -StatusFilter "All" -SelectedMenuItemTitle "New Tab" -ModifiedSinceMinutesFilter "3600" -SelectedButtonTitle "New Tab Button" -UserRole "Administrator" -Description "New tab added"