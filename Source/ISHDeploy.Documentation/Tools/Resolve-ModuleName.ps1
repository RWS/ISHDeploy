param (
    [Parameter(Mandatory=$true)]
    [string]
    $ReleaseYear,
    [Parameter(Mandatory=$true)]
    [string]
    $SupportedCMVersion
)

try
{
    $moduleName="ISHDeploy.$SupportedCMVersion"
	Write-Verbose "moduleName=$moduleName"
    
    $sourcePath=Resolve-Path "$PSScriptRoot\..\"
	$objPath=Resolve-Path "$PSScriptRoot\..\obj\doc"
	Write-Verbose "sourcePath=$sourcePath"
	Write-Verbose "objPath=$objPath"
	
	$rootItems=Get-ChildItem $sourcePath -Filter "*.md" 
	$rootItems| ForEach-Object{ 
        Write-Debug "Copying $($_.FullName) to $objPath"
        Copy-Item ($_.FullName) $objPath -Force
        Write-Verbose "Copyied $($_.FullName) to $objPath"
    }

	$rootItems=Get-ChildItem $objPath -Filter "*.md"

	$rootItems | ForEach-Object {
		Write-Debug "Loading $($_.FullName)"
		$content = $_|Get-Content
		Write-Verbose "Loaded $($_.FullName)"
		$content=$content -replace "{ModuleName}",$moduleName
        $content=$content -replace "{ReleaseYear}",$ReleaseYear
        $content=$content -replace "{SupportedCMVersion}",$SupportedCMVersion
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




