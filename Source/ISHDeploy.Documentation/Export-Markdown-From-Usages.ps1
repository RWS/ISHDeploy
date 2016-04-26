param (
    [Parameter(Mandatory=$true)]
    [string]
    $UsagesDir,
    [Parameter(Mandatory=$true)]
    [string]
    $ExportPath
)

#$DebugPreference="Continue"
#$VerbosePreference="Continue"

Write-Debug "Usage Directory: $UsagesDir"
Write-Debug "Export Path: $ExportPath"

try
{
    $cmdletsUsage = @{};

    $ymlFile = Join-Path $ExportPath "toc.yml"

    If (Test-Path $ymlFile){
	    Remove-Item $ymlFile
    }
    
    Get-ChildItem -Path $UsagesDir | ForEach-Object {

        $folderName = $_.Name
        $folderPath = Join-Path $ExportPath $folderName

        New-Item -ItemType Directory -Force -Path $folderPath

        $mdFile = Join-Path $folderPath ($folderName + ".md")

        "- name: $folderName"| Out-File $ymlFile -Encoding utf8 -Append
        "  href: " + "../obj/doc/using/$folderName/$folderName.md"| Out-File $ymlFile -Encoding utf8 -Append

        "# Using the $folderName commandlets" | Out-File $mdFile -Encoding utf8

        Get-ChildItem $_.FullName -Filter *.ps1  | ForEach-Object {
            Copy-Item $_.FullName $folderPath -Force
            $name = $_.BaseName
            $cmdletName = $name -creplace "^([A-Z]+)([a-z]+)", '$1$2-'
            $fileName = $_.Name
            ""
            "##  $cmdletName"
            "CopyCodeBlockAndLink($fileName)"
        } | Out-String | Out-File $mdFile -Encoding utf8 -Append
    }
}
catch
{
	Write-Error $_.Exception
	exit 1
}