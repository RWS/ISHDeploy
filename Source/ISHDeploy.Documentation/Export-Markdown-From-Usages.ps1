param (
    #[Parameter(Mandatory=$true)]
    [string]
    $InputDir,
    #[Parameter(Mandatory=$true)]
    [string]
    $ExportPath
)

$InputDir ="C:\Stash Projects\ishdeploy\Source\ISHDeploy.PowerShell\CmdletsUsage"
$ExportPath ="C:\Stash Projects\ishdeploy\Source\ISHDeploy.Documentation\Using"

#$DebugPreference="Continue"
#$VerbosePreference="Continue"

Write-Debug "Usage Directory: $InputDir"
Write-Debug "Export Path: $ExportPath"

try
{
    # Celaning Export path
    Remove-Item  $ExportPath\* -Recurse -Force

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
        $ymlFileContent += "  href: " + "../obj/doc/using/$folderName/$folderName.md"
    }

    $ymlFileContent | Out-String | Out-File (Join-Path $ExportPath "toc.yml") -Encoding utf8 
}
catch
{
	Write-Error $_.Exception
	exit 1
}