param([String]$outputpath, [String]$modulename)

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
Get-ChildItem -Path $outputpath -Filter ("$modulename*") | Copy-Item -Destination "$modulepath\$modulename" -Force

