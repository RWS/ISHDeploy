#Variables
$directory = $args[0];
$branch = "Dev"
if($directory.contains("\Test\") -eq $true)
{
	$branch = "Test"
} elseif ($directory.contains("\Prod\") -eq $true) 
{
	$branch = "Prod"
}

#Get PS module path
[array]$modulepath = $env:PSModulePath.Split(";")
if ($modulepath) {
	$modulepath = $modulepath[0]
}

#Create directory if necessary
if ((Test-Path -path "$modulepath\Trisoft.Configuration.Automation.$branch") -ne $True)
{
	New-Item "$modulepath\Trisoft.Configuration.Automation.$branch" -type directory
}

#Copy files
Copy-Item ($directory + "Trisoft.Configuration.Automation.dll") "$modulepath/Trisoft.Configuration.Automation.$branch" -Force
Copy-Item ($directory + "Trisoft.Configuration.Automation.pdb") "$modulepath/Trisoft.Configuration.Automation.$branch" -Force

#TODO: uncomment when help will be available
#Copy-Item ($directory + "Trisoft.Configuration.Automation.dll-help.xml") "$modulepath/Trisoft.Configuration.Automation.$branch" -Force
#Copy-Item ($directory + "Trisoft.Configuration.Automation.psd1") "$modulepath/Trisoft.Configuration.Automation.$branch/Trisoft.Configuration.Automation.$branch.psd1" -Force
#Copy-Item ($directory + "Trisoft.Configuration.Automation.Format.ps1xml") "$modulepath/Trisoft.Configuration.Automation.$branch" -Force