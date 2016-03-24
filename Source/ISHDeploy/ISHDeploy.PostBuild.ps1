#Variables
$directory = $args[0].trim('"');

#Get PS module path
[array]$modulepath = $env:PSModulePath.Split(";")
if ($modulepath) {
	$modulepath = $modulepath[0]
}

#Create directory if necessary
if ((Test-Path -path "$modulepath\ISHDeploy") -ne $True)
{
	New-Item "$modulepath\ISHDeploy" -type directory
}

#Copy files
Copy-Item (Join-Path $directory "ISHDeploy.dll") "$modulepath/ISHDeploy" -Force
Copy-Item (Join-Path $directory "ISHDeploy.pdb") "$modulepath/ISHDeploy" -Force
Copy-Item (Join-Path $directory "ISHDeploy.dll-help.xml") "$modulepath/ISHDeploy" -Force
#Copy-Item (Join-Path $directory "ISHDeploy.psd1") "$modulepath/ISHDeploy/ISHDeploy.psd1" -Force
