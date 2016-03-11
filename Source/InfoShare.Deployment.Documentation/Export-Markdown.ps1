param (
    [Parameter(Mandatory=$true)]
    [string]
    $ModulePath,
    [Parameter(Mandatory=$true)]
    [string]
    $ExportPath
)
#$DebugPreference="Continue"
#$VerbosePreference="Continue"

Write-Debug "ModulePath=$ModulePath"
Write-Debug "ExportPath=$ExportPath"


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
	Import-Module platyPS
	Get-Command -Module platyPS | Select-Object -ExpandProperty Name

	Import-Module –Name $ModulePath -Verbose
	$commandletNames=Get-Command -Module InfoShare.Deployment | Select-Object -ExpandProperty Name 
	Write-Verbose "Processing $($commandletNames.Count) commandlets"

	if(-not (Test-Path $ExportPath))
	{
		New-Item $ExportPath -ItemType Directory
	}
	Get-ChildItem -Path $ExportPath -Include * | Remove-Item -recurse 

	$commandletNames | ForEach-Object {
		$mdPath=Join-Path $ExportPath "$_.md"
		Write-Verbose "Processing $_"
		$markDown=Get-platyPSMarkdown -Command $_
		Write-Debug $markDown
		Write-Verbose "Exporting to $mdPath"     
		$markDown |Out-File  $mdPath -Encoding UTF8
	}
	
	#To generate the toc.yml we can use some thing like this. But there is a problem with the generated output
	#Get-Command -Module InfoShare.Deployment |Select-Object -ExpandProperty Name | ForEach-Object {"- name: $_";"- href: $_.md"} |Out-File C:\docfx_walkthrough\docfx_project\infoshare.deployment\tox.yml -Encoding utf8
	Get-Command -Module InfoShare.Deployment |Select-Object -ExpandProperty Name | ForEach-Object {
		"- name: $_";
		"  href: " + "../obj/doc/module/infoshare.deployment/" + "$_.md"
	} |Out-File $PSScriptRoot/Module/toc.yml -Encoding utf8
}
catch
{
	Write-Error $_.Exception
	exit 1
}


