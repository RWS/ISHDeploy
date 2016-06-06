param(
    [Parameter(Mandatory=$true)]
    [String]$ModulePath, 
    [Parameter(Mandatory=$true)]
    [String]$ModuleName,
    [String]$WebSitePath = "$Env:systemdrive\inetpub\ishdeploy-doc"
)

$moduleWebPath = Join-Path $WebSitePath $ModuleName

#Check the site exists
if (Test-Path $moduleWebPath) 
{
	Write-Host "Module '$ModuleName' is already exists, we need to replacing it"

    Remove-Item $moduleWebPath -Recurse -Force
}

Copy-Item $ModulePath $moduleWebPath -Recurse -Force

Get-ChildItem -Directory $WebSitePath | ?{ $_.PSIsContainer }

$lis = "";
Get-ChildItem -Directory $WebSitePath | Select-Object BaseName | % {
    $n = $_.BaseName;
    $lis += "<li id='$n'><a title='$n' href='./$n'>$n</a></li>"
}

Write-Host "Creating 'index.html' file."

"<!DOCTYPE html>
<html><head>
    <meta http-equiv='Content-Type' content='text/html; charset=UTF-8'>
    <title>ISHDeploy Documentation Portal</title>
	<STYLE TYPE='text/css'>
		body,html{font:110%/2.2em Trebuchet MS,Tahoma,sans-serif;padding:0;border:0;margin:3em auto;width:800px;overflow:hidden;color:#444}li{list-style-type:none}body{background-size:cover;background-position:top center;background-repeat:no-repeat;background-attachment:fixed;mix-blend-mode:hard-light}ul{background-color:#676767;border-radius:10px;}
	</STYLE>
	<script type=""text/javascript"">
		function processData(a) { if (a = (a = a.match(/data-wallpaper-id=""(\d+)""/i)) && a[1]) document.body.style.backgroundImage = ""url('"" + (""http://wallpapers.wallhaven.cc/wallpapers/full/wallhaven-"" + a + "".jpg"") + ""')"" } var imgServerUrl = 'http://alpha.wallhaven.cc/search?q=snow+mountains&sorting=random', url = 'http://cors.io/?u=' + encodeURIComponent(imgServerUrl);
		document.addEventListener('DOMContentLoaded', function (a) { a = new XMLHttpRequest; a.onload = function () { 200 === this.status && null != this.response && processData(this.response) }; a.open('GET', url); a.send() });
	</script>
</head>
<body>
	<h1>ISHDeploy Documentation Portal</h1>
	<ul>$lis</ul>
</body></html>" | Out-File (Join-Path $WebSitePath "index.html") -Encoding utf8 -Force