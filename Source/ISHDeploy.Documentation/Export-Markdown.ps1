param (
    [Parameter(Mandatory=$true)]
    [string]
    $ModuleDir,
    [Parameter(Mandatory=$true)]
    [string]
    $ModuleName,
    [Parameter(Mandatory=$true)]
    [string]
    $ExportPath
)
#$DebugPreference="Continue"
#$VerbosePreference="Continue"
$MamlFilePath = Join-Path $ModuleDir ($ModuleName + ".dll-Help.xml")

Write-Debug "Module Directory: $ModuleDir"
Write-Debug "Module Name: $ModuleName"
Write-Debug "Module Maml File Path: $MamlFilePath"
Write-Debug "Export Path: $ExportPath"


# https://blogs.msdn.microsoft.com/powershell/2016/02/05/platyps-write-external-help-files-in-markdown/
if (-not (Get-Module -ListAvailable -Name platyPS)) 
{
	$mode="32bit"
	if([System.Environment]::Is64BitProcess -eq $true)
	{
		$mode="64bit"
	}
	Write-Error "PlatyPS module is not available for $mode. 
	Please follow instructions on https://blogs.msdn.microsoft.com/powershell/2016/02/05/platyps-write-external-help-files-in-markdown/
	For powershell version smaller or equal to 4 you need to install PackageManagement https://www.microsoft.com/en-us/download/details.aspx?id=49186"
    exit 1
}

try
{
	$modulePath=Join-Path $ModuleDir "$ModuleName.psd1"
	Import-Module $modulePath
	Write-Verbose "Imported module from $modulePath"

	if(Test-Path $ExportPath)
	{
		Remove-Item "$ExportPath" -Recurse -Force	
	}
	New-Item -ItemType Directory -Path $ExportPath 
	Write-Verbose "Reset export path at $ExportPath."
	
	# Generating markdown from module
	New-MarkdownHelp -Module  $ModuleName -OutputFolder $ExportPath -NoMetadata -Force
	Write-Verbose "Exported markdown files from $ModuleName in $ExportPath."

	# From schema https://github.com/PowerShell/platyPS/blob/master/platyPS.schema.md
	# platyPS generates link elements like [{link name}]({link url}) but the {link url} is always empty resulting in ugly links
	# The following block reads each file and promotes the {link name} to {link url}
	Get-ChildItem -Path $ExportPath -Filter "*.md" | ForEach-Object {
		$content=Get-Content -Path $_.FullName
		$content -replace "\[(?<linkname>.+)\]\(\)",'[${linkname}](${linkname}.md)' |Out-File $_.FullName -Force
	}

	# Generating context for all markdown files generated from maml
	Get-ChildItem -Path $ExportPath -Filter "*.md" | ForEach-Object {
		"- name: " + $_.BaseName;
		"  href: " + $_.Name
	} | Out-File "$ExportPath/toc.yml" -Encoding utf8
}
catch
{
	Write-Error $_.Exception
	exit 1
}