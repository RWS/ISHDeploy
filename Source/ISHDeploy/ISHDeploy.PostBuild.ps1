param([String]$outputpath, [String]$modulename)

<<<<<<< HEAD
=======
# Remove quotes
$outputpath = $outputpath.trim("'");

>>>>>>> Added file Build.props that contains all the variables that might be different for all branches.
# Get PS module path
[array]$modulepath = $env:PSModulePath.Split(";")
if ($modulepath) {
	$modulepath = $modulepath[0]
}

# Create directory if necessary
if ((Test-Path -path "$modulepath\$modulename") -ne $True)
{
	New-Item "$modulepath\$modulename" -type directory
}

# Copy files to WindowsPowerShell directory
<<<<<<< HEAD
#Get-ChildItem -Path $outputpath -Filter ("$modulename*") | Copy-Item -Destination "$modulepath\$modulename" -Force
=======
Get-ChildItem -Path $outputpath -Filter ("$modulename*") | Copy-Item -Destination "$modulepath\$modulename" -Force

>>>>>>> Added file Build.props that contains all the variables that might be different for all branches.
