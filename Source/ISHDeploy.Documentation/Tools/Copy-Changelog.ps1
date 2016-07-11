param (
    [Parameter(Mandatory=$true)]
    [string]
    $ExportPath
)


#create new index.md file based on last version from CHANGELOG.md file
$changelogFilePath="$ExportPath\CHANGELOG.md"
$indexFilePath="$ExportPath\index.md"
$changelogContent = Get-Content $changelogFilePath
$indexContent = Get-Content $indexFilePath
$flag = $false
foreach ($line in $changelogContent) {
    if ($line.StartsWith("##")){
        if($flag){
            break
        }
        $flag = $true
    }
    If($flag){
        $indexContent += $line
    }
}
$indexContent+="Please review the module's entire [history](CHANGELOG.md)." 
$indexContent | Set-Content $indexFilePath
