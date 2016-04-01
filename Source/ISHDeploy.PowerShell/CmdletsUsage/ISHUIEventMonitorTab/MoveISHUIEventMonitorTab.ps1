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

Move-ISHUIEventMonitorTab -ISHDeployment $deploy -Label "Publish" -First

Move-ISHUIEventMonitorTab -ISHDeployment $deploy -Label "Publish" -Last

Move-ISHUIEventMonitorTab -ISHDeployment $deploy -Label "P1" -After "Publish"