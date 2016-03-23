Import-Module ISHDeploy

$executingScriptDirectory = Split-Path -Path $MyInvocation.MyCommand.Definition -Parent

$ISHInstall = Join-Path $executingScriptDirectory "ISHInstall.ps1"
$ISHUninstall = Join-Path $executingScriptDirectory "ISHUninstall.ps1"

$ISHUninstallSQL2014= $ISHUninstall + ' InfoShareSQL2014 C:\Users\infoshareserviceuser\Documents\inputparameters_SQL2014.xml C:\Automated_deployment'
$ISHUninstallORA12= $ISHUninstall + ' InfoShareORA12 C:\Users\infoshareserviceuser\Documents\inputparameters_ORA12.xml C:\Automated_deployment'

$ISHInstallSQL2014 = $ISHInstall +' C:\Automated_deployment C:\Users\infoshareserviceuser\Documents\inputparameters_SQL_2014.xml'
$ISHInstallORA12 = $ISHInstall +' C:\Automated_deployment C:\Users\infoshareserviceuser\Documents\inputparameters_ORA12.xml'

$ORA = Get-ISHDeployment -Deployment "ORA12"
$SQL = Get-ISHDeployment -Deployment "SQL2014"
if($ORA){
Invoke-Expression $ISHUninstallORA12
}

Invoke-Expression $ISHInstallORA12

if($SQL){
Invoke-Expression $ISHUninstallSQL2014
}

Invoke-Expression $ISHInstallSQL2014