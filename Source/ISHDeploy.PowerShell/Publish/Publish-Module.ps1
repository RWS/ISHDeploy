param(
[string] $modulePath,
[string] $apiKey,
[string] $repositoryName,
[string] $repositoryPath
)

#Install the packages
try
{
	Write-Host "Checking if repository '$repositoryName' is registered"

	# Check if repository exists
	$repository = Get-PSRepository $repositoryName -ErrorAction SilentlyContinue

	if ($repository.Count -eq 0)
	{
		Write-Host "Repository is not registered"
        Write-Host "Registering repository '$repositoryName' at '$repositoryPath'"
        
		Register-PSRepository -Name $repositoryName -SourceLocation $repositoryPath -PublishLocation $repositoryPath -InstallationPolicy Trusted

        $repository = Get-PSRepository $repositoryName -ErrorAction SilentlyContinue
        if ($repository.Count -eq 0) {
            throw "Cannot register repository $repositoryName"
        }
	}

    Write-Host "Repository is registered"

	# Creating temporary Module directory

	$manifestFile = Get-ChildItem "$modulePath\*.psd1"

	if (-not $manifestFile)
	{
		throw [System.IO.FileNotFoundException] "Manifest file ""$manifestFile"" was not found."
	}
	$manifestFileVersion = (Test-ModuleManifest -Path $manifestFile).Version
	Write-Host "Manifest files is found at module location."

	# Copying files in temporary directory. As manifest name should match the name of the folder it`s in.
	$moduleName = [System.IO.Path]::GetFileNameWithoutExtension($manifestFile)
	$tmpModulePath = New-Item -Force -Path "$modulePath\$moduleName" -ItemType directory

	Copy-Item "$modulePath\*" $tmpModulePath -recurse

	if (-not (Get-Module -ListAvailable -Name PSScriptAnalyzer)) 
	{
		Write-Host "PSScriptAnalyzer module is not installed."
		Write-Host "Installing PSScriptAnalyzer module.."

		Install-Module -Name PSScriptAnalyzer -force
	} 
	else
	{
		Write-Host "PSScriptAnalyzer module is installed."
	}

    # Run PSScriptAnalyzer against the module to make sure it's not failing any tests
    Invoke-ScriptAnalyzer -Path $tmpModulePath

	# Publishing module to Powershell gallery
	Write-Host "Publishing Module '$moduleName' to repository '$($repository.Name)'";

	$initialDebugPreference = $DebugPreference
	$initialVerbosePreference = $VerbosePreference
	$DebugPreference = "Continue"
	$VerbosePreference = "Continue"

    Publish-Module -Path $tmpModulePath -NuGetApiKey $apiKey -Repository $repository.Name

	$DebugPreference = $initialDebugPreference
	$VerbosePreference = $initialVerbosePreference
    # The module is now listed on the PowerShell Gallery
	$repositoryModuleVersion = (Find-Module -Name $moduleName -Repository $repository.Name).Version
    Find-Module -Name $moduleName -Repository $repository.Name
	$compareVersions = Compare-Object -ReferenceObject $manifestFileVersion -DifferenceObject $repositoryModuleVersion
	if($compareVersions.Count -eq 0) { 
		Write-Host "Published"
	}
	else{
		Write-Host "Version in manifest file: $manifestFileVersion" 
		Write-Host "Version in repository: $repositoryModuleVersion" 
		Write-Host "Publishing failed."
		throw "Publishing of module failed"
	}


}
catch
{
	if ($tmpModulePath)
	{
		# Removing temporary Module directory
		Remove-Item $tmpModulePath -Recurse -Force
	}

    Write-Error $_
}