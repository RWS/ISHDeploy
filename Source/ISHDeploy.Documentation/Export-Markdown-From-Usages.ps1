param (
    [Parameter(Mandatory=$true)]
    [string]
    $InputDir,
    [Parameter(Mandatory=$true)]
    [string]
    $ExportPath
)

#$DebugPreference="Continue"
#$VerbosePreference="Continue"

Write-Debug "Usage Directory: $InputDir"
Write-Debug "Export Path: $ExportPath"

try
{
	if(!(Test-Path $ExportPath ))
	{
		Write-Host "Creating Using Folder at '$ExportPath'."
		New-Item -ItemType directory -Path $ExportPath
	}

    # Copy Cmdlet samples to Export folder
    Copy-Item  $InputDir\* $ExportPath -Recurse -Force

    $ymlFileContent = @();

    Get-ChildItem -Path $ExportPath | % {

        $folderName = $_.Name
        $folderPath = Join-Path $ExportPath $folderName

        $mdFileContent = @("# Using the $folderName commandlets");

        Get-ChildItem $_.FullName -Filter *.ps1  | % {
            $name = $_.BaseName
            $cmdletName = $name -creplace "^([A-Z]+)([a-z]+)", '$1$2-'
            $fileName = $_.Name
            $mdFileContent += ""
            $mdFileContent += "##  $cmdletName"
            $mdFileContent += "CopyCodeBlockAndLink($fileName)"
        } 
        $mdFileContent | Out-String | Out-File (Join-Path $folderPath "$folderName.md") -Encoding utf8

        $ymlFileContent += "- name: $folderName"
        $ymlFileContent += "  href: $folderName/$folderName.md"
    }

    $ymlFileContent | Out-String | Out-File (Join-Path $ExportPath "toc.yml") -Encoding utf8 -Force

    . "$PSScriptRoot\Resolve-Markdown.ps1" -SourcePath "$ExportPath"
}
catch
{
	Write-Error $_.Exception
	exit 1
}