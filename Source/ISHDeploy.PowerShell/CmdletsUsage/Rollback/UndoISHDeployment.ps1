CLS

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
$version = New-Object System.Version -ArgumentList '13.0.0.0';

$deployment = New-Object ISHDeploy.Models.ISHDeployment -ArgumentList ($dict, $version)

Undo-ISHDeployment -ISHDeployment $deployment