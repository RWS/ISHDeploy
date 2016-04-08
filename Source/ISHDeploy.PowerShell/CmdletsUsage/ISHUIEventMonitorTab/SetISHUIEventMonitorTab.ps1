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

$deployment = New-Object ISHDeploy.Models.ISHDeployment -ArgumentList ($dict, $version)

$filterTypes = @("JOB", "NON-JOB", "UN-JOB")

# Using all parameters
Set-ISHUIEventMonitorTab -ISHDeployment $deployment -Label "All Parameters" -Icon "~/UIFramework/new-tab.job.32x32.png" -EventTypesFilter $filterTypes -SelectedStatusFilter "All" -ModifiedSinceMinutesFilter "3600" -UserRole "Administrator" -Description "Tab using all available parameters"

# Using default parameters with filter types
Set-ISHUIEventMonitorTab -ISHDeployment $deployment -Label "Defaults and Filter" -EventTypesFilter $filterTypes -Description "Using default parameters with filtered types"

# Using default parameters
Set-ISHUIEventMonitorTab -ISHDeployment $deployment -Label "Defaults" -Description "Using default parameters"