param (
    [Parameter(Mandatory=$true)]
    [string]
    $ModuleDir,
    [Parameter(Mandatory=$true)]
    [string]
    $ModuleName,
    [Parameter(Mandatory=$true)]
    [string]
    $ExportPath,
    [Parameter(Mandatory=$true)]
    [string]
    $FilePath
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

#create new index.md file based on last version from CHANGELOG.md file
$subject = [IO.File]::ReadAllText("$FilePath\CHANGELOG.md")
$regex = [regex] '##((.|\n)*?)(?=##)'
$match = $regex.Match($subject);
[IO.File]::ReadAllText("$FilePath\IndexTemplate.txt") + $match + "Please review the module's entire [history](CHANGELOG.md)." | Set-Content "$FilePath\index.md"

try
{
	Import-Module platyPS
	Get-Command -Module platyPS | Select-Object -ExpandProperty Name

	Write-Verbose "Processing maml file ($MamlFilePath)"
	# Generating markdown form maml file
	[string]$mamlContent = Get-Content $MamlFilePath
	Get-PlatyPSMarkdown -maml $mamlContent -OneFilePerCommand -OutputFolder $ExportPath

	if(!(Test-Path $ExportPath ))
	{
		Write-Host "Creating Module Folder at '$ExportPath'."
		New-Item -ItemType directory -Path $ExportPath
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