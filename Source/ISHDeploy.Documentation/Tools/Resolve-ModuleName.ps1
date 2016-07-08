param (
    [Parameter(Mandatory=$true)]
    [string]
    $ModulePath,
    [Parameter(Mandatory=$true)]
    [string]
    $Year
)

try
{
	Write-Verbose "ModulePath=$ModulePath"
	Write-Debug "Loading module from $ModulePath"
    $psd1Files=Get-ChildItem $ModulePath  -Filter "*.psd1"
    if($psd1Files.Count -eq 0)
    {
        throw "Could not fine module manifest in $ModulePath"
    }
    if($psd1Files.Count -gt 1)
    {
        throw "Too many module manifest in $ModulePath"
    }
    $moduleName=($psd1Files|Select-Object -ExpandProperty Name -First 1).Replace(".psd1","")
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

    . "$PSScriptRoot\Copy-Changelog.ps1" -FilePath "$sourcePath" -ExportPath "$objPath"

	$rootItems=Get-ChildItem $objPath -Filter "*.md"

	$rootItems | ForEach-Object {
		Write-Debug "Loading $($_.FullName)"
		$content = $_|Get-Content
		Write-Verbose "Loaded $($_.FullName)"
        $containModuleName=$content -match "{ModuleName}"
        $containYear=$content -match "{Year}"
		if( $containModuleName -Or $containYear)
		{
            if($containModuleName){
			    $content=$content -replace "{ModuleName}",$moduleName
            }
            if($containYear){
                $content=$content -replace "{Year}",$Year
            }

			Write-Verbose "Processed $($_.FullName)"
		
			Write-Debug "Saving $($_.FullName)"
			$content | Out-File $_.FullName -Force
			Write-Verbose "Saved $($_.FullName)"
		}
		else
		{
			Write-Verbose "Not processed $($_.FullName)"
		}

	}
}
catch
{
	Write-Error $_.Exception
	exit 1
}




