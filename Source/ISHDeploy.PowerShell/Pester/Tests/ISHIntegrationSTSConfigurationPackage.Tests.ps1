param(
    $session = $null,
    $testingDeploymentName = "InfoShare"
)
. "$PSScriptRoot\Common.ps1"

$computerName = If ($session) {$session.ComputerName} Else {$env:COMPUTERNAME}

#region variables
$packageFileName = "testFileName"

# Test variables
$domain = Get-TestDataValue "testDomain"

# Script block for getting ISH deployment
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

# Generating file pathes to remote PC files
$testingDeployment = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetDeployment -Session $session -ArgumentList $testingDeploymentName
$suffix = GetProjectSuffix($testingDeployment.Name)
$moduleName = Invoke-CommandRemoteOrLocal -ScriptBlock { (Get-Module "ISHDeploy.*").Name } -Session $session
$packagePath = "C:\ProgramData\$moduleName\$($testingDeployment.Name)\Packages"
$computerName = $computerName.split(".")[0]
$uncPackagePath = "\\$computerName\" + ($packagePath.replace(":", "$"))
#endregion

#region Script Blocks 
$scriptBlockCleanTmpFolder = {
    param (
        [Parameter(Mandatory=$false)]
        $packagePath
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }

    if (!(Test-Path "$packagePath\tmp"))
    {
        New-Item "$packagePath\tmp" -ItemType directory
    }

    Get-ChildItem -Path "$packagePath\tmp" -Include *.* -File -Recurse | foreach { $_.Delete()}
}

$scriptBlockGetISHPackageFolder = {
    param (
        [Parameter(Mandatory=$false)]
        $ishDeployName,
        [Parameter(Mandatory=$false)]
        $uncSwitch 
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    if($uncSwitch){
        Get-ISHPackageFolderPath -ISHDeployment $ishDeploy -UNC
    }
    else{
        Get-ISHPackageFolderPath -ISHDeployment $ishDeploy
    }
}

$scriptBlockGetISHPackage = {
    param (
        [Parameter(Mandatory=$false)]
        $ishDeployName,
        [Parameter(Mandatory=$false)]
        $filename,
        [Parameter(Mandatory=$false)]
        $ADFSSwitch
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    if($ADFSSwitch){
        Save-ISHIntegrationSTSConfigurationPackage -ISHDeployment $ishDeploy -Filename $filename -ADFS
    }
    else{
        Save-ISHIntegrationSTSConfigurationPackage -ISHDeployment $ishDeploy -Filename $filename 
    }  
}

$scriptBlockGetHistory = {
    param (
        [Parameter(Mandatory=$false)]
        $ishDeployName 
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Get-ISHDeploymentHistory -ISHDeployment $ishDeploy
}

#endregion


Add-Type -AssemblyName System.IO.Compression.FileSystem
function Unzip
{
    param([string]$zipfile, [string]$outpath)

    [System.IO.Compression.ZipFile]::ExtractToDirectory($zipfile, $outpath)
}

Describe "Testing ISHIntegrationSTSConfigurationPackage"{
    BeforeEach {
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockUndoDeployment -Session $session -ArgumentList $testingDeploymentName
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockCleanTmpFolder -Session $session -ArgumentList $packagePath
    }
    

    It "Save package"{
        $packagePath = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHPackageFolder -Session $session -ArgumentList $testingDeploymentName, $true
        RemotePathCheck $packagePath | Should be "True"
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHPackage -Session $session -ArgumentList $testingDeploymentName, $packageFileName
        Unzip "$packagePath\$packageFileName" "$packagePath\tmp"
        
        RemotePathCheck "$packagePath\tmp\ishws.cer" | Should be $true
        RemotePathCheck "$packagePath\tmp\CM Security Token Service Requirements.md" | Should be $true
        $Mdfile = Get-Content "$packagePath\tmp\CM Security Token Service Requirements.md"
        $Mdfile -contains "https://$computerName.$domain/"+$testingDeployment.WebAppNameWS+"/Wcf/API25/Application.svc" | Should be $true
        $Mdfile -contains "https://$computerName.$domain/"+$testingDeployment.WebAppNameWS+"/Wcf/API/ConditionManagement.svc" | Should be $true
    }

    It "Save same package"{
        $packagePath = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHPackageFolder -Session $session -ArgumentList $testingDeploymentName, $true
        RemotePathCheck $packagePath | Should be "True"
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHPackage -Session $session -ArgumentList $testingDeploymentName, $packageFileName
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHPackage -Session $session -ArgumentList $testingDeploymentName, $packageFileName
        Unzip "$packagePath\$packageFileName" "$packagePath\tmp"
        
        RemotePathCheck "$packagePath\tmp\ishws.cer" | Should be $true
        RemotePathCheck "$packagePath\tmp\CM Security Token Service Requirements.md" | Should be $true
    }

    It "Save package ADFS"{
        $packagePath = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHPackageFolder -Session $session -ArgumentList $testingDeploymentName, $true
        RemotePathCheck $packagePath | Should be "True"
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHPackage -Session $session -ArgumentList $testingDeploymentName, $packageFileName, $true
        Unzip "$packagePath\$packageFileName" "$packagePath\tmp"
        
        RemotePathCheck "$packagePath\tmp\ishws.cer" | Should be $true
        RemotePathCheck "$packagePath\tmp\CM Security Token Service Requirements.md" | Should be $true
        RemotePathCheck "$packagePath\tmp\Invoke-ADFSIntegrationISH.ps1" | Should be $true
        $Mdfile = Get-Content "$packagePath\tmp\CM Security Token Service Requirements.md"
        $Mdfile -contains "https://$computerName.$domain/"+$testingDeployment.WebAppNameWS+"/Wcf/API25/Application.svc" | Should be $true
        $Mdfile -contains "https://$computerName.$domain/"+$testingDeployment.WebAppNameWS+"/Wcf/API/ConditionManagement.svc" | Should be $true
        $scriptFile = Get-Content "$packagePath\tmp\Invoke-ADFSIntegrationISH.ps1"
        
        $scriptFile -contains '$projectsuffix="' + $suffix +'"' | Should be $true
        #$scriptFile -contains '$osuser="' + $testingDeployment.OriginalParameters.osuser +'"' | Should be $true
    }

    It "Save same package with adfs switch"{
        $packagePath = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHPackageFolder -Session $session -ArgumentList $testingDeploymentName, $true
        RemotePathCheck $packagePath | Should be "True"
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHPackage -Session $session -ArgumentList $testingDeploymentName, $packageFileName, $true
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHPackage -Session $session -ArgumentList $testingDeploymentName, $packageFileName, $true
        Unzip "$packagePath\$packageFileName" "$packagePath\tmp"
        
        RemotePathCheck "$packagePath\tmp\ishws.cer" | Should be $true
        RemotePathCheck "$packagePath\tmp\CM Security Token Service Requirements.md" | Should be $true
    }

    It "Save package has certificate in md file"{
        $packagePath = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHPackageFolder -Session $session -ArgumentList $testingDeploymentName, $true
        RemotePathCheck $packagePath | Should be "True"
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHPackage -Session $session -ArgumentList $testingDeploymentName, $packageFileName
        Unzip "$packagePath\$packageFileName" "$packagePath\tmp"
        
        RemotePathCheck "$packagePath\tmp\ishws.cer" | Should be $true
        RemotePathCheck "$packagePath\tmp\CM Security Token Service Requirements.md" | Should be $true
        $Mdfile = Get-Content "$packagePath\tmp\CM Security Token Service Requirements.md"
        $certFile = Get-Content "$packagePath\tmp\ishws.cer"
        $Mdfile -contains "https://$computerName.$domain/"+$testingDeployment.WebAppNameWS+"/Wcf/API25/Application.svc" | Should be $true
        $Mdfile -contains "https://$computerName.$domain/"+$testingDeployment.WebAppNameWS+"/Wcf/API/ConditionManagement.svc" | Should be $true
        $Mdfile.ToString() -contains $certFile.ToString() | Should be $true
    }

    It "Save package with ADFS switch has certificate in md file"{
        $packagePath = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHPackageFolder -Session $session -ArgumentList $testingDeploymentName, $true
        RemotePathCheck $packagePath | Should be "True"
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHPackage -Session $session -ArgumentList $testingDeploymentName, $packageFileName, $true
        Unzip "$packagePath\$packageFileName" "$packagePath\tmp"
        
        RemotePathCheck "$packagePath\tmp\ishws.cer" | Should be $true
        RemotePathCheck "$packagePath\tmp\CM Security Token Service Requirements.md" | Should be $true
        $Mdfile = Get-Content "$packagePath\tmp\CM Security Token Service Requirements.md"
        $certFile = Get-Content "$packagePath\tmp\ishws.cer"
        $Mdfile -contains "https://$computerName.$domain/"+$testingDeployment.WebAppNameWS+"/Wcf/API25/Application.svc" | Should be $true
        $Mdfile -contains "https://$computerName.$domain/"+$testingDeployment.WebAppNameWS+"/Wcf/API/ConditionManagement.svc" | Should be $true
        $Mdfile.ToString() -contains $certFile.ToString() | Should be $true
    }

    It "Save package writes proper history"{
        $packagePath = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHPackageFolder -Session $session -ArgumentList $testingDeploymentName, $true
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHPackage -Session $session -ArgumentList $testingDeploymentName, "package.zip"
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHPackage -Session $session -ArgumentList $testingDeploymentName, "package_adfs.zip", $true
        $history = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetHistory -Session $session -ArgumentList $testingDeploymentName
        $history -contains 'Save-ISHIntegrationSTSConfigurationPackage -FileName "package.zip" -ISHDeployment $deployment
Save-ISHIntegrationSTSConfigurationPackage -FileName "package_adfs.zip" -ISHDeployment $deployment -ADFS'
       
    }
}