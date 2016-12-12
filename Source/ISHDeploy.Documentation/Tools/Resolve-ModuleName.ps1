param (
    [Parameter(Mandatory=$true)]
    [string]
    $ReleaseYear,
    [Parameter(Mandatory=$true)]
    [string]
    $SupportedCMVersion,
    [Parameter(Mandatory=$true)]
    [string]
    $SoftwareCMVersion
)

try
{
    $moduleName="ISHDeploy.$SupportedCMVersion"
	Write-Verbose "moduleName=$moduleName"

	$supportedCMVersionMajor=($SupportedCMVersion -split '.')[0]
	$supportedCMVersionRevision=($SupportedCMVersion -split '.')[2]
    
	$objPath=Resolve-Path "$PSScriptRoot\..\obj\doc"
	Write-Verbose "objPath=$objPath"

	$rootItems=Get-ChildItem $objPath -Filter "*.md" -Recurse

	$rootItems | ForEach-Object {
		Write-Debug "Loading $($_.FullName)"
		$content = $_|Get-Content
		Write-Verbose "Loaded $($_.FullName)"
		$content=$content -replace "{ModuleName}",$moduleName
        $content=$content -replace "{ReleaseYear}",$ReleaseYear
        $content=$content -replace "{SupportedCMVersion}",$SupportedCMVersion
        $content=$content -replace "{SupportedCMVersionMajor}",$supportedCMVersionMajor
        $content=$content -replace "{SupportedCMVersionMinor}",$supportedCMVersionMinor
        $content=$content -replace "{SoftwareCMVersion}",$SoftwareCMVersion
		Write-Verbose "Processed $($_.FullName)"
		
		Write-Debug "Saving $($_.FullName)"
		$content | Out-File $_.FullName -Force
		Write-Verbose "Saved $($_.FullName)"
	}
}
catch
{
	Write-Error $_.Exception
	exit 1
}




