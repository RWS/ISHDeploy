param(
[string] $modulePath,
[string] $apiKey,
[string] $repositoryName,
[string] $repositoryPath
)

#Install the packages
try
{
	# Check if repository exists
	$repository = Get-PSRepository $repositoryName -ErrorAction SilentlyContinue

	Write-Host "Checking if repository is registered"

	if ($repository.Count -eq 0)
	{
		Write-Host "Repository is not registered"

		$registerScriptBlock=
		{
			$sourceName = $repositoryName
			$sourceLocation = $repositoryPath + "nuget/"
			$publishLocation = $repositoryPath

			Write-Host "Registering repository.."
			$repository = Register-PSRepository -Name $sourceName -SourceLocation $sourceLocation -PublishLocation  $publishLocation -InstallationPolicy Trusted
		}

        Invoke-Command -ScriptBlock $registerScriptBlock

        Write-Host ".. registered!"
	}
	else 
	{
		Write-Host "Repository is registered"
	}

	# Creating temporary Module directory

	$manifestFile = Get-ChildItem "$modulePath\*.psd1"

	if (-not $manifestFile)
	{
		throw [System.IO.FileNotFoundException] "Manifest file ""$manifestFile"" was not found."
	}

	Write-Host "Manifest files is found at module location."

	# Copying files in temporary directory. As manifest name should match the name of the folder it`s in.
	$moduleName = [System.IO.Path]::GetFileNameWithoutExtension($manifestFile)
	$tmpModulePath = New-Item -Force -Path "$modulePath\$moduleName" -ItemType directory

	Copy-Item "$modulePath\$moduleName.*" $tmpModulePath

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
    Publish-Module -Path $tmpModulePath -NuGetApiKey $apiKey -Repository $repository.Name

    # The module is now listed on the PowerShell Gallery
    Find-Module $moduleName

    Write-Host "Published"
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