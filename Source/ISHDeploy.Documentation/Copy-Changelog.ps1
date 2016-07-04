param (
    [Parameter(Mandatory=$true)]
    [string]
    $FilePath
)
#$DebugPreference="Continue"
#$VerbosePreference="Continue"

#create new index.md file based on last version from CHANGELOG.md file
$subject = [IO.File]::ReadAllText("$FilePath\CHANGELOG.md")
$regex = [regex] '##((.|\n)*?)(?=##)'
$match = $regex.Match($subject);
[IO.File]::ReadAllText("$FilePath\index.md") + $match + "Please review the module's entire [history](CHANGELOG.md)." | Set-Content "$FilePath\obj\doc\index.md"
