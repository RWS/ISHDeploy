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

#Set-ISHIntegrationSTSCertificate -ISHDeployment $deployment -Thumbprint "T17" -Issuer "20151028ASDFT9"
#Set-ISHIntegrationSTSCertificate -ISHDeployment $deployment -Thumbprint "T18" -Issuer "20151028ASDFT9"
#Set-ISHIntegrationSTSCertificate -ISHDeployment $deployment -Thumbprint "T19" -Issuer "20151028ASDFT9"

Remove-ISHIntegrationSTSCertificate -ISHDeployment $deployment -Issuer "20151028ASDFT0"

