CLS

$DebugPreference = "Continue"
$VerbosePreference = "Continue"
$WarningPreference = "Continue"

$dict = New-Object "System.Collections.Generic.Dictionary``2[System.String,System.String]"
$dict.Add('webpath', 'C:\ISHSandbox')
$dict.Add('apppath', 'C:\ISHSandbox')
$dict.Add('projectsuffix', 'SQL2014')
$dict.Add('datapath', 'C:\ISHSandbox')
$version = New-Object System.Version -ArgumentList '1.0.0.0';

$deployment = New-Object ISHDeploy.Models.ISHDeployment -ArgumentList ($dict, $version)

Set-ISHIntegrationSTSCertificate -ISHDeployment $deployment -Thumbprint "T1" -Issuer "20151028ADFS"

Set-ISHIntegrationSTSCertificate -ISHDeployment $deployment -Thumbprint "T2" -Issuer "20151028ASDFT2" -ValidationMode "None"

