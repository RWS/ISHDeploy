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

$filterTypes = @("JOB", "NON-JOB", "UN-JOB")

# Using all parameters
Set-ISHUIEventMonitorTab -ISHDeployment $deploy  -Label "2NewTab" -Icon "~/UIFramework/new-tab.job.32x32.png" -EventTypesFilter $filterTypes -SelectedStatusFilter "All" -ModifiedSinceMinutesFilter "3600" -UserRole "Administrator" -Description "New tab added"

# Using default parameters
Set-ISHUIEventMonitorTab -ISHDeployment $deploy -Label "9 New Tab" -EventTypesFilter $filterTypes  -Description "New tab added"

#Checking existing Icon
Set-ISHUIEventMonitorTab -ISHDeployment $deploy -Label "9 New Tab" -EventTypesFilter $filterTypes  -Description "New tab added" -Icon "C:\Trisoft\RnDProjects\Trisoft\Dev\Resources\UIFramework\warning.32x32.png"
