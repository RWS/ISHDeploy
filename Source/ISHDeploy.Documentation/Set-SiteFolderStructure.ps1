param(
    [Parameter(Mandatory=$true)]
    [String]$DocPath, 
    [Parameter(Mandatory=$true)]
    [String]$DocVersion,
    [String]$WebSitePath = "$Env:systemdrive\inetpub\ishdeploy-doc",
    [String]$WebSiteName = "ISHDeploy Documentation Portal",
    [String]$WebSitePort = 8081
)

Import-Module WebAdministration

# Create web folder for documentation portal
function CreateWebFolderStructure($indexName, $folderPath)
{
	if(!(Test-Path $folderPath ))
	{
		Write-Host "Creating root folder'$hostname'."
		New-Item -ItemType directory -Path $folderPath
	}

	$indexHtmlFilePath = "$folderPath\index.html"
	if(!(Test-Path -Path $indexHtmlFilePath ))
	{
		Write-Host "Creating 'index.html' file."
	    $htmlContent = "<!DOCTYPE html>
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
	<ul></ul>
</body></html>"

		New-Item $indexHtmlFilePath -ItemType file -Value $htmlContent -Force
	}
}

#Import the IIS Web Administration module
function AddDocPortalSite($hostname, $siteRoot, $port)
{
	$iisAppPool = "iis:\AppPools\$hostname"
	$iisSite = "iis:\Sites\$hostname"

	#Check if the application pool already exists
	if (!(Test-Path $iisAppPool)) 
	{
		Write-Host "'$hostname' Application Pool is not registered, registering it."

		New-Item $iisAppPool

		$NewPool = Get-Item $iisAppPool
  
		#Set the application pool's account to NETWORK SERVICE

		$NewPool.ProcessModel.IdentityType = 2
		$NewPool | Set-Item
 
		#Set the application pool to .NET 4
		Set-ItemProperty $iisAppPool managedRuntimeVersion v4.0
	}
 
	#Check the site exists
	if (!(Test-Path $iisSite)) 
	{
		#Create the site and bind it to the hostname, port and folder on disk
		Write-Host "'$hostname' Site does not exists, creating it."
		New-Item $iisSite -bindings @{protocol="http";bindingInformation=":{0}:" -f $port} -physicalPath "$siteRoot"

		#Set the site's application pool
		Set-ItemProperty $iisSite -name applicationPool -value "$hostname"
	}

	#Checking if web structure is in place
	if(!(Test-Path $siteRoot ))
	{
		Write-Host "Creating web site structure"
		CreateWebFolderStructure $hostname $siteRoot
	}
}

#Import the IIS Web Administration module
function AddVFAndUpdateIndexFile($vfName, $DocPath)
{
	#Check the site exists
	if (!(Test-Path iis:\Sites\$WebSiteName\$vfName)) 
	{
		Write-Host "Creating wirtual directory '$vfName' for '$DocPath'"
		New-WebVirtualDirectory -Site $WebSiteName -Name $vfName -PhysicalPath $DocPath -Force
	}

	$indexHtml = "$WebSitePath\index.html";
	$indexContent = (Get-Content $indexHtml);
	$signature = "<li id=\'$([Regex]::Escape($vfName))'.*?\/li>"
	$liRow = "<li id='$vfName'><a href='./$vfName'>$vfName</a></li>"

	if ([string]$indexContent -Match $signature)
	{
		# Need to be replaced
		Write-Host "Index.html has definition for '$vfName', udating it."
		$indexContent -replace $signature, "$liRow" | Set-Content $indexHtml	
	}
	else
	{
		# Need to be added
		Write-Host "Index.html does not have definition for '$vfName', adding it."
		$indexContent -replace '</ul>', "$liRow</ul>" | Set-Content $indexHtml	
	}
}

AddDocPortalSite $WebSiteName $WebSitePath $WebSitePort

AddVFAndUpdateIndexFile $DocVersion $DocPath


