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
