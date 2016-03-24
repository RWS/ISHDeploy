param (
    [Parameter(Mandatory=$true)]
    [string]
    $SourcePath,
    [Parameter(Mandatory=$true)]
    [string]
    $ExportPath
)

$DebugPreference="Continue"
$VerbosePreference="Continue"

#$SourcePath="C:\Stash\infoshare-deployment\Source\ISHDeploy.PowerShell\Using"
#$ExportPath="C:\Stash\infoshare-deployment\Source\ISHDeploy.Documentation\obj\doc\using"

#$SourcePath="C:\Stash\infoshare-deployment\Source\ISHDeploy.PowerShell\Articles"
#$ExportPath="C:\Stash\infoshare-deployment\Source\ISHDeploy.Documentation\obj\doc\articles"

Write-Debug "SourcePath=$SourcePath"
Write-Debug "ExportPath=$ExportPath"


try
{
	if(-not (Test-Path $ExportPath))
	{
		New-Item $ExportPath -ItemType Directory
	}
	Get-ChildItem -Path $ExportPath -Include * | Remove-Item -recurse 

    Copy-Item "$SourcePath\*" $ExportPath -Recurse

    $pathsGroupedByContainer=Get-ChildItem $ExportPath -Recurse -Include @("*.ps1","*.md") | Group-Object Directory
    foreach($container in $pathsGroupedByContainer)
    {
        $containerPath=$container.Name
        $files=$container.Group
		#Write-Verbose "Processing ${($files.Count) files in $containerPath"
        $mdFiles=$files|Where-Object {$_.Extension -eq ".md"}
        $copyCodeBlockAndLinkMatchEvaluator = 
        {  
			param($m) 
			$targetFileName=$m.Groups["file"].Value
            $targetFilePath=Join-Path $containerPath $targetFileName
            Write-Verbose "Loading $targetFilePath"
			$codeContent=Get-Content $targetFilePath | Out-String
            $syntaxType=""
            if($targetFilePath.ToLowerInvariant().EndsWith(".ps1"))
            {
                $syntaxType="powershell"
            }
            $newContent="
``````$syntaxType
$codeContent``````
[Download]($targetFileName)
"
            return $newContent

        }
        $copyCodeBlockMatchEvaluator = 
        {  
			param($m) 
			$targetFileName=$m.Groups["file"].Value
            $targetFilePath=Join-Path $containerPath $targetFileName
            Write-Verbose "Loading $targetFilePath"
			$codeContent=Get-Content $targetFilePath | Out-String
            $syntaxType=""
            if($targetFilePath.ToLowerInvariant().EndsWith(".ps1"))
            {
                $syntaxType="powershell"
            }
            $newContent="
``````$syntaxType
$codeContent``````
"
            return $newContent

        }
        foreach($mdFile in $mdFiles)
        {
			Write-Verbose "Processing $($mdFile.Name)"
            $mdContent=$mdFile |Get-Content|Out-String
            $mdContent=[regex]::replace($mdContent,'CopyCodeBlockAndLink\((?<file>.*)\)',$copyCodeBlockAndLinkMatchEvaluator)
            $mdContent=[regex]::replace($mdContent,'CopyCodeBlock\((?<file>.*)\)',$copyCodeBlockMatchEvaluator)
            $mdContent|Out-File $mdFile -Force
        }
    }
}
catch
{
	Write-Error $_.Exception
	exit 1
}




