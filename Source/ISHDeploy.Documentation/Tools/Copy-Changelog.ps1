param (
    [Parameter(Mandatory=$true)]
    [string]
    $FilePath,
    [Parameter(Mandatory=$true)]
    [string]
    $ExportPath
)


#create new index.md file based on last version from CHANGELOG.md file
$subject = Get-Content "$FilePath\CHANGELOG.md"
$result = Get-Content "$FilePath\index.md"
$flag = $false
foreach ($str in $subject) {
    if ($str.StartsWith("##")){
        if($flag){
            break
        }
        $flag = $true
    }
    If($flag){
        $result += $str
    }
}
$result + "Please review the module's entire [history](CHANGELOG.md)." | Set-Content "$ExportPath\index.md"
