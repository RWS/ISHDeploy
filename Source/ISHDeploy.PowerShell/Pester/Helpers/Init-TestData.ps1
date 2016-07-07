param(
    [Parameter(Mandatory=$true)]
    $dataFilePath
	)

Function Get-FileJSON
{
    param (
        [Parameter(Mandatory=$true)]
        $filePath
    ) 

    $client = New-Object System.Net.Webclient
    $uri = [System.Uri]$filePath

    $json = $client.DownloadString($uri.AbsoluteUri) 

    return $json | ConvertFrom-Json;
}

$jsonData = Get-FileJSON $dataFilePath

Set-Variable TestData -Value $jsonData -Scope Global -Force