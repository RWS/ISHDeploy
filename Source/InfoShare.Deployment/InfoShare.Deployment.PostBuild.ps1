#Variables
$directory = $args[0].trimend('"');

#Get PS module path
[array]$modulepath = $env:PSModulePath.Split(";")
if ($modulepath) {
	$modulepath = $modulepath[0]
}

#Create directory if necessary
if ((Test-Path -path "$modulepath\InfoShare.Deployment") -ne $True)
{
	New-Item "$modulepath\InfoShare.Deployment" -type directory
}

#Copy files
Copy-Item (Join-Path $directory "InfoShare.Deployment.dll") "$modulepath/InfoShare.Deployment" -Force
Copy-Item (Join-Path $directory "InfoShare.Deployment.pdb") "$modulepath/InfoShare.Deployment" -Force

#TODO: uncomment when help will be available
#Copy-Item ($directory + "InfoShare.Deployment.dll-help.xml") "$modulepath/InfoShare.Deployment" -Force
#Copy-Item ($directory + "InfoShare.Deployment.psd1") "$modulepath/InfoShare.Deployment/InfoShare.Deployment.psd1" -Force
#Copy-Item ($directory + "InfoShare.Deployment.Format.ps1xml") "$modulepath/InfoShare.Deployment" -Force
