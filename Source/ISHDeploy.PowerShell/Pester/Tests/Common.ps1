#Invokes command in session or locally, depending on -session parameter
#Import-Module $PSScriptRoot.Replace(".powershell\pester\tests", "\bin\DebugSkipVersion\ISHDeploy.12.0.0.dll")

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
#undo changes
$scriptBlockUndoDeployment = {
    param (
        [Parameter(Mandatory=$false)]
        $ishDeployName 
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Undo-ISHDeployment -ISHDeployment $ishDeploy
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
    Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRenameItem -Session $session -ArgumentList $path, $name
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

#Gets suffix from project name
Function GetProjectSuffix($projectName)
{
    return $projectName.Replace("InfoShare", "")
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
    
    $inputParametersPath = $historyRegKey.OpenSubKey($installFolderRegKey).GetValue("InstallHistoryPath")#.ToString()

    [System.Xml.XmlDocument]$inputParameters = new-object System.Xml.XmlDocument
    $inputParameters.load("$inputParametersPath\inputparameters.xml")

    $result = @{}
    $result["osuser"] = $inputParameters.SelectNodes("inputconfig/param[@name='osuser']/currentvalue")[0].InnerText
    $result["connectstring"] = $inputParameters.SelectNodes("inputconfig/param[@name='connectstring']/currentvalue")[0].InnerText
    $result["infoshareauthorwebappname"] = $inputParameters.SelectNodes("inputconfig/param[@name='infoshareauthorwebappname']/currentvalue")[0].InnerText
    $result["infosharewswebappname"] = $inputParameters.SelectNodes("inputconfig/param[@name='infosharewswebappname']/currentvalue")[0].InnerText
    $result["infosharestswebappname"] = $inputParameters.SelectNodes("inputconfig/param[@name='infosharestswebappname']/currentvalue")[0].InnerText

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

$scriptBlockCreateCertificate = {
    $sslCertificate  = New-SelfSignedCertificate -DnsName "testDNS" -CertStoreLocation "cert:\LocalMachine\My"
    return $sslCertificate 
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
    Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockUndoDeployment -Session $session -ArgumentList $testingDeploymentName
}