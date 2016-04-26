CLS
Import-Module ISHDeploy

# $DebugPreference = "SilentlyContinue"

$DebugPreference = "Continue"
$VerbosePreference = "Continue"
$WarningPreference = "Continue"

#EXAMPLE

$dict = New-Object "System.Collections.Generic.Dictionary``2[System.String,System.String]"
$dict.Add('webpath', 'E:\New folder')
$dict.Add('apppath', 'E:\New folder')
$dict.Add('projectsuffix', 'SQL2014')
$dict.Add('datapath', 'E:\New folder')
$version = New-Object System.Version -ArgumentList '1.0.0.0';

$deployment = New-Object ISHDeploy.Models.ISHDeployment -ArgumentList ($dict, $version)

Set-ISHDeployment $deployment

Disable-ISHUIContentEditor