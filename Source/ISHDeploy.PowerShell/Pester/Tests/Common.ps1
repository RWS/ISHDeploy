#Invokes command in session or locally, depending on -session parameter
Function Invoke-CommandRemoteOrLocal {
    param (
        [Parameter(Mandatory=$true)]
        $ScriptBlock,
        [Parameter(Mandatory=$false)]
        $Session=$null,
        [Parameter(Mandatory=$false)]
        $ArgumentList=$null
    ) 

    if($Session)
    {
        Invoke-Command -Session $Session -ScriptBlock $ScriptBlock -ArgumentList $ArgumentList -ErrorAction Stop
    }
    else
    {
        Invoke-Command -ScriptBlock $ScriptBlock -ArgumentList $ArgumentList -ErrorAction Stop 
    }
}

$scriptBlockGetDeployment = {
    param (
        [Parameter(Mandatory=$false)]
        $ishDeployName 
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
    Get-ISHDeployment -Name $ishDeployName 
}

$scriptBlockCreateCertificate = {
    $sslCertificate  = New-SelfSignedCertificate -DnsName "testDNS" -CertStoreLocation "cert:\LocalMachine\My"
    return $sslCertificate 
}
$computerName = If ($session) {$session.ComputerName} Else {$env:COMPUTERNAME}

$testingDeployment = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetDeployment -Session $session -ArgumentList $testingDeploymentName
$testCertificate = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockCreateCertificate -Session $session

#Gets suffix from project name
Function GetProjectSuffix($projectName)
{
    return $projectName.Replace("InfoShare", "")
}

$suffix = GetProjectSuffix($testingDeployment.Name)
$dbPath = ("\\$computerName\{0}\Web{1}\InfoShareSTS\App_Data\IdentityServerConfiguration-2.2.sdf" -f $testingDeployment.Webpath, $suffix).replace(":", "$")

#check path remotely
$scriptBlockTestPath = {
    param (
        [Parameter(Mandatory=$true)]
        $path 
    )
    Test-Path $path
}
Function RemotePathCheck {
    param (
        [Parameter(Mandatory=$true)]
        $path
    ) 
    $isExists = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockTestPath -Session $session -ArgumentList $path
    
    return $isExists
}


#Stop WebRequestToSTS
$scriptBlockWebRequest = {
    param (
        [Parameter(Mandatory=$true)]
        $url
    ) 
    
    $request = [System.Net.WebRequest]::Create($url)
    $request.Method = "GET";
    $request.KeepAlive = $false;
    try {
        [System.Net.HttpWebResponse]$response = $request.GetResponse()
        $status = $response.StatusCode
        Write-Debug "Status of web response of $url is: $status"
    } catch [System.Net.WebException] {
        Write-Error $_.Exception
    }
}

Function WebRequestToSTS
{
    param (
        [Parameter(Mandatory=$true)]
        $projectName
    ) 
    
    $result = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetInputParameters -Session $session -ArgumentList $projectName
    $baseurl = $result["baseurl"]
    $infosharestswebappname = $result["infosharestswebappname"]
    $url = "$baseurl/$infosharestswebappname"
    
    Write-Debug "Send Get request to STS server to init DB file creation"
    Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockWebRequest -Session $session -ArgumentList $url
}
#undo changes
$scriptBlockUndoDeployment = {
    param (
        [Parameter(Mandatory=$true)]
        $deployName,
        [Parameter(Mandatory=$false)]
        [bool]$skipRecycling = $false
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
    
    # Sets a value indicating whether skip recycle or not. For integration test perspective only. Please, see https://jira.sdl.com/browse/TS-11329
    [ISHDeploy.Business.Operations.ISHDeployment.UndoISHDeploymentOperation]::SkipRecycle = $skipRecycling

    $ishDeploy = Get-ISHDeployment -Name $deployName
    Undo-ISHDeployment -ISHDeployment $ishDeploy
}

Function UndoDeploymentBackToVanila {
    param (
        [Parameter(Mandatory=$true)]
        $deploymentName,
        [Parameter(Mandatory=$false)]
        [bool]$skipRecycling = $false
    ) 

    Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockUndoDeployment -Session $session -ArgumentList $deploymentName, $skipRecycling

    if ($skipRecycling -eq $false){

        $i = 0
        $doesDBFileExist = Test-Path $dbPath

        if ($doesDBFileExist -ne $true) {
            Write-Debug "$dbPath does not exist"
            WebRequestToSTS $testingDeploymentName
            Start-Sleep -Milliseconds 1000
        }

        $doesDBFileExist = Test-Path $dbPath
        while($doesDBFileExist -ne $true)
        {
            Start-Sleep -Milliseconds 7000
            $doesDBFileExist = Test-Path $dbPath

            if ($i -ge 2)
            {
                $doesDBFileExist | Should be $true
            }
            $i++
        }
    }
}

#remove item remotely
$scriptBlockRemoveItem = {
    param (
        [Parameter(Mandatory=$true)]
        $path 
    )
    Remove-Item $path
}
Function RemoteRemoveItem {
    param (
        [Parameter(Mandatory=$true)]
        $path
    ) 
    Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveItem -Session $session -ArgumentList $path
}

#rename item remotely
$scriptBlockRenameItem = {
    param (
        [Parameter(Mandatory=$true)]
        $path,
        [Parameter(Mandatory=$true)]
        $name
    )
    Rename-Item $path, $name
}
Function RemoteRenameItem {
    param (
        [Parameter(Mandatory=$true)]
        $path,
        [Parameter(Mandatory=$true)]
        $name
    ) 
    Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRenameItem -Session $session -ArgumentList $path $name
}

#retries command specified amount of times with 1 second delay between tries. Exits if command has expected response or tried to run specifeied amount of time
function RetryCommand {
    param([int]$numberOfRetries, $command, $expectedResult)

    for($i=0; $i -lt $numberOfRetries; $i++) {
     $actualResult = Invoke-Command -ScriptBlock $command
     if($actualResult -eq $expectedResult) {
        $i = $numberOfRetries
     }
     else {
        Start-Sleep -Milliseconds 1000
     }
    }

    return $actualResult
}

#Logs debug info
Function Log($Message)
{
    $date=Get-Date -Format "HHmmss.fff"
    $threadID=[System.Threading.Thread]::CurrentThread.ManagedThreadId
    $computerName=$env:COMPUTERNAME

    Write-Debug "$computerName $date [$threadID] - $Message"
}


function RemoteReadXML{
    param($xmlFile)

    [xml]$actualResult = Invoke-CommandRemoteOrLocal -ScriptBlock {param ($xmlFile) Get-Content $xmlFile} -Session $session -ArgumentList $xmlFile

    return $actualResult
}

#Gets InputParameters
$scriptBlockGetInputParameters = {
    param (
        [Parameter(Mandatory=$true)]
        $projectName 
    )

    $RegistryInstallToolPath = "SOFTWARE\\Trisoft\\InstallTool"
    if ([System.Environment]::Is64BitOperatingSystem)
    {
        $RegistryInstallToolPath = "SOFTWARE\\Wow6432Node\\Trisoft\\InstallTool"
    }
    [Microsoft.Win32.RegistryKey]$installToolRegKey = [Microsoft.Win32.Registry]::LocalMachine.OpenSubKey($RegistryInstallToolPath);
    $currentInstallValue = $installToolRegKey.OpenSubKey("InfoShare").OpenSubKey($projectName).GetValue("Current")
    $historyRegKey = $installToolRegKey.OpenSubKey("InfoShare").OpenSubKey($projectName).OpenSubKey("History")
    $installFolderRegKey =  $historyRegKey.GetSubKeyNames() | Where { $_ -eq $currentInstallValue } | Select -First 1
    
    $inputParametersPath = $historyRegKey.OpenSubKey($installFolderRegKey).GetValue("InstallHistoryPath")

    [System.Xml.XmlDocument]$inputParameters = new-object System.Xml.XmlDocument
    $inputParameters.load("$inputParametersPath\inputparameters.xml")

    $result = @{}
    $result["osuser"] = $inputParameters.SelectNodes("inputconfig/param[@name='osuser']/currentvalue")[0].InnerText
    $result["connectstring"] = $inputParameters.SelectNodes("inputconfig/param[@name='connectstring']/currentvalue")[0].InnerText
    $result["infoshareauthorwebappname"] = $inputParameters.SelectNodes("inputconfig/param[@name='infoshareauthorwebappname']/currentvalue")[0].InnerText
    $result["infosharewswebappname"] = $inputParameters.SelectNodes("inputconfig/param[@name='infosharewswebappname']/currentvalue")[0].InnerText
    $result["infosharestswebappname"] = $inputParameters.SelectNodes("inputconfig/param[@name='infosharestswebappname']/currentvalue")[0].InnerText
    $result["baseurl"] = $inputParameters.SelectNodes("inputconfig/param[@name='baseurl']/currentvalue")[0].InnerText

    return $result
    
}
Function Get-InputParameters
{
    param (
        [Parameter(Mandatory=$true)]
        $projectName
    ) 
    Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetInputParameters -Session $session -ArgumentList $projectName
}

$scriptBlockRemoveCertificate= {
    param (
        [Parameter(Mandatory=$true)]
        $thumbprint
    )
    certutil -delstore my $thumbprint
}

#Stop WebAppPool
$scriptBlockStopWebAppPool = {
    param (
        [Parameter(Mandatory=$true)]
        $infoshareauthorwebappname,
        [Parameter(Mandatory=$true)]
        $infosharewswebappname,
        [Parameter(Mandatory=$true)]
        $infosharestswebappname 
    )
    

    if((Get-WebAppPoolState "TrisoftAppPool$infoshareauthorwebappname").Value -ne 'Stopped')
    {
	    Stop-WebAppPool -Name "TrisoftAppPool$infoshareauthorwebappname"
    }
    if((Get-WebAppPoolState "TrisoftAppPool$infosharewswebappname").Value -ne 'Stopped')
    {
	    Stop-WebAppPool -Name "TrisoftAppPool$infosharewswebappname"
    }
    if((Get-WebAppPoolState "TrisoftAppPool$infosharestswebappname").Value -ne 'Stopped')
    {
	    Stop-WebAppPool -Name "TrisoftAppPool$infosharestswebappname"
    }
}
Function StopPool
{
    param (
        [Parameter(Mandatory=$true)]
        $projectName
    ) 
    $result = Get-InputParameters $projectName
    Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockStopWebAppPool -Session $session -ArgumentList $result["infoshareauthorwebappname"], $result["infosharewswebappname"], $result["infosharestswebappname"]
}

Function UndoDeployment
{
    param (
        [Parameter(Mandatory=$true)]
        $testingDeploymentName
    ) 
    UndoDeployment
}

# Get Test Data variable
Function Get-TestDataValue
{
    param (
        [Parameter(Mandatory=$true)]
        $valuePath
    ) 

    # Global variables
    $testData = Get-Variable -Name "TestData" -Scope Global -ValueOnly
    $value = $testData.$valuePath;
    if (-not $value)
    {
        throw "Value `$valuePath` does not exists"
    }

    return $value
}

Function ArtifactCleaner
{
    param (
        [Parameter(Mandatory=$true)]
        $filePath,
		[Parameter(Mandatory=$true)]
        $fileName
    ) 
    
     if(RemotePathCheck "$filePath\_$fileName")
        {
            if (RemotePathCheck "$filePath\$fileName")
            {
                RemoteRemoveItem "$filePath\$fileName"
            }
            RemoteRenameItem "$filePath\_$fileName" "$fileName"
        }
}