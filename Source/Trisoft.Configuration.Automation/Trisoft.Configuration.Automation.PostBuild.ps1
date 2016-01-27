#Variables
$directory = $args[0];

#Get PS module path
[array]$modulepath = $env:PSModulePath.Split(";")
if ($modulepath) {
	$modulepath = $modulepath[0]
}

#Create directory if necessary
if ((Test-Path -path "$modulepath\Trisoft.Configuration.Automation") -ne $True)
{
	New-Item "$modulepath\Trisoft.Configuration.Automation" -type directory
}

#Copy files
Copy-Item ($directory + "Trisoft.Configuration.Automation.dll") "$modulepath/Trisoft.Configuration.Automation" -Force
Copy-Item ($directory + "Trisoft.Configuration.Automation.pdb") "$modulepath/Trisoft.Configuration.Automation" -Force

#TODO: uncomment when help will be available
#Copy-Item ($directory + "Trisoft.Configuration.Automation.dll-help.xml") "$modulepath/Trisoft.Configuration.Automation" -Force
#Copy-Item ($directory + "Trisoft.Configuration.Automation.psd1") "$modulepath/Trisoft.Configuration.Automation/Trisoft.Configuration.Automation.psd1" -Force
#Copy-Item ($directory + "Trisoft.Configuration.Automation.Format.ps1xml") "$modulepath/Trisoft.Configuration.Automation" -Force