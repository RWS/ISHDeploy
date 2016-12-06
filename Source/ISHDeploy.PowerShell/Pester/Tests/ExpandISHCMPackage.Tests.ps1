param(
    $session = $null,
    $testingDeploymentName = "InfoShare"
)
. "$PSScriptRoot\Common.ps1"


$moduleName = Invoke-CommandRemoteOrLocal -ScriptBlock { (Get-Module "ISHDeploy.*").Name } -Session $session
$packagePath = "C:\ProgramData\$moduleName\$($testingDeployment.Name)\Packages"
$zipTempPath = "C:\ProgramData\$moduleName\$($testingDeployment.Name)"

$computerName = $computerName.split(".")[0]
$uncZipPath = "\\$computerName\" + ($zipTempPath.replace(":", "$"))
$uncPackagePath = "\\$computerName\" + ($packagePath.replace(":", "$"))
$filePath = Join-Path $testingDeployment.WebPath ("\Web{0}\Author\ASP" -f $suffix )
$customFileName = "test.config"
$customFile = Join-Path $filePath "\Custom\$customFileName"
$binFile = Join-Path $filePath "\Bin\$customFileName"

$zipName = "test.zip"
$zipPath = Join-Path $uncZipPath $zipName
$secondZip = "test2.zip"
$secondZipPath = Join-Path $uncZipPath $secondZip

#endregion

#region Script Blocks 



# Script block for Set-ISHContentEditor
$scriptBlockExpandISHCMPackage = {
    param (
        [Parameter(Mandatory=$true)]
        $ishDeployName,
        [Parameter(Mandatory=$true)]
        $fileName,
        [Parameter(Mandatory=$true)]
        $switchState,
        [Parameter(Mandatory=$false)]
        $filename2
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }  
    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    if ($switchState -eq "ToBin"){
        Expand-ISHCMPackage -ISHDeployment $ishDeploy -FileName $fileName -ToBin
    }
    elseif($switchState -eq "ToCustom"){
        Expand-ISHCMPackage -ISHDeployment $ishDeploy -FileName $fileName -ToCustom
    }
    else {
        Expand-ISHCMPackage -ISHDeployment $ishDeploy -FileName $fileName, $filename2 -ToCustom
    }
}

Describe "Testing Expand-ISHCMPackage"{
    BeforeEach {
		ArtifactCleaner -filePath $uncPackagePath -fileName "test.file"
        ArtifactCleaner -filePath $uncPackagePath -fileName "Trisoft.Web.dll"  
		ArtifactCleaner -filePath $uncPackagePath -fileName $customFileName  
        ArtifactCleaner -filePath $uncPackagePath -fileName $zipName    
        UndoDeploymentBackToVanila $testingDeploymentName $true
    }

    It "Expand-ISHCMPackage copies to custom folder"{
		#Arrange
        New-Item -Path $uncPackagePath -Name $customFileName -Force -type file |Out-Null
        ZipFolder -zipfile $zipPath -folderPath $uncPackagePath
        Move-Item -Path $zipPath -Destination $uncPackagePath -Force
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockExpandISHCMPackage -Session $session -ArgumentList $testingDeploymentName, $zipName, "ToCustom"
        #Assert
        RemotePathCheck $customFile | Should Be "True"
    }

    It "Expand-ISHCMPackage copies to bin folder"{
		#Arrange
        New-Item -Path $uncPackagePath -Name $customFileName -Force -type file |Out-Null
        ZipFolder -zipfile $zipPath -folderPath $uncPackagePath
        Move-Item -Path $zipPath -Destination $uncPackagePath -Force
        #Invoke-CommandRemoteOrLocal -ScriptBlock{Test-Path "$packagePath\test.file"} -Session $session -ArgumentList $packagePath | Should Be "True"
        
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockExpandISHCMPackage -Session $session -ArgumentList $testingDeploymentName, $zipName, "ToBiN"
        #Assert
        RemotePathCheck $binFile | Should Be "True"
    }

    It "Expand-ISHCMPackage owerwrites file in custom folder"{
		#Arrange
        New-Item -Path $uncPackagePath -Name $customFileName -Force -type file |Out-Null
        ZipFolder -zipfile $zipPath -folderPath $uncPackagePath
        Move-Item -Path $zipPath -Destination $uncPackagePath -Force
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockExpandISHCMPackage -Session $session -ArgumentList $testingDeploymentName, $zipName, "ToCustom"
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockExpandISHCMPackage -Session $session -ArgumentList $testingDeploymentName, $zipName, "ToCustom" -WarningVariable Warning} | Should not Throw
        #need extra commandlet call here because otherwise Warning variable is empty
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockExpandISHCMPackage -Session $session -ArgumentList $testingDeploymentName, $zipName, "ToCustom" -WarningVariable Warning
        $Warning | should Match " has been overritten" 
        #Assert
        RemotePathCheck $customFile | Should Be "True"
    }

    It "Expand-ISHCMPackage owerwrites custom file in bin folder"{
		#Arrange
        New-Item -Path $uncPackagePath -Name $customFileName -Force -type file |Out-Null
        ZipFolder -zipfile $zipPath -folderPath $uncPackagePath
        Move-Item -Path $zipPath -Destination $uncPackagePath -Force
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockExpandISHCMPackage -Session $session -ArgumentList $testingDeploymentName, $zipName, "ToBin"
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockExpandISHCMPackage -Session $session -ArgumentList $testingDeploymentName, $zipName, "ToBin" -WarningVariable Warning} | Should not Throw
        #need extra commandlet call here because otherwise Warning variable is empty
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockExpandISHCMPackage -Session $session -ArgumentList $testingDeploymentName, $zipName, "ToBin" -WarningVariable Warning
        $Warning | should Match " has been overritten" 
        #Assert
        RemotePathCheck $binFile | Should Be "True"
    }
   
   It "Expand-ISHCMPackage throws error when file does not exist"{
		#Arrange
        New-Item -Path $uncPackagePath -Name $customFileName -Force -type file |Out-Null
        ZipFolder -zipfile $zipPath -folderPath $uncPackagePath
        Move-Item -Path $zipPath -Destination $uncPackagePath -Force
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockExpandISHCMPackage -Session $session -ArgumentList $testingDeploymentName, "unexistingTest.file", "ToCustom"} | Should throw "InvalidPath for"
    }

    It "Expand-ISHCMPackage does not owerwrite CM files in bin folder"{
		#Arrange
        New-Item -Path $uncPackagePath -Name "Trisoft.Web.dll" -Force -type file |Out-Null
        ZipFolder -zipfile $zipPath -folderPath $uncPackagePath
        Move-Item -Path $zipPath -Destination $uncPackagePath -Force
        #Assert
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockExpandISHCMPackage -Session $session -ArgumentList $testingDeploymentName, $zipName, "ToBin" -WarningVariable Warning
        $Warning | should Match "Skip file"
    }
    
    It "Expand-ISHCMPackage can copy multiple files"{
		#Arrange
        New-Item -Path $uncPackagePath -Name "Trisoft.Web.dll" -Force -type file |Out-Null
        New-Item -Path $uncPackagePath -Name $customFileName -Force -type file |Out-Null

        ZipFolder -zipfile $zipPath -folderPath $uncPackagePath
        Move-Item -Path $zipPath -Destination $uncPackagePath -Force 

        ZipFolder -zipfile $secondZipPath -folderPath $uncPackagePath
        Move-Item -Path $secondZipPath -Destination $uncPackagePath -Force 

        #Assert
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockExpandISHCMPackage -Session $session -ArgumentList $testingDeploymentName, $zipName, "copyMultiple", $secondZip  
        $secondCustomFile = Join-Path $filePath "\Custom\Trisoft.Web.dll"
        RemotePathCheck $customFile | Should Be "True"
        RemotePathCheck $secondCustomFile | Should Be "True"
    }

    It "Expand-ISHCMPackage saves file tree structure after expand"{
		#Arrange
        
        New-Item -Path $uncPackagePath -Name "TestFolder" -ItemType directory -Force
        New-Item -Path "$uncPackagePath\TestFolder" -Name "Trisoft.Web.dll" -Force -type file |Out-Null
        New-Item -Path $uncPackagePath -Name $customFileName -Force -type file |Out-Null

        ZipFolder -zipfile $zipPath -folderPath $uncPackagePath
        Move-Item -Path $zipPath -Destination $uncPackagePath -Force

        #Assert
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockExpandISHCMPackage -Session $session -ArgumentList $testingDeploymentName, $zipName, "ToCustom"
        $secondCustomFile = Join-Path $filePath "\Custom\TestFolder\Trisoft.Web.dll"
        RemotePathCheck $customFile | Should Be "True"
        RemotePathCheck $secondCustomFile | Should Be "True"
    }
    
     It "Expand-ISHCMPackage writes history"{
        #Act
        New-Item -Path $uncPackagePath -Name $customFileName -Force -type file |Out-Null
        ZipFolder -zipfile $zipPath -folderPath $uncPackagePath
        Move-Item -Path $zipPath -Destination $uncPackagePath -Force
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockExpandISHCMPackage -Session $session -ArgumentList $testingDeploymentName, $zipName, "ToCustom"
        $history = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetHistory -Session $session -ArgumentList $testingDeploymentName
        
        #Assert
        $history.Contains('if($IncludeCustomFile)
{
     Expand-ISHCMPackage -ISHDeployment $deploymentName -FileName @("test.zip") -ToCustom 
}') | Should be "True"
              
   } 
   

$testFileContent = '<?xml version="1.0"?>
<ArrayOfString xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <string>#!#installtool:datapath#!#</string>
  <string>#!#installtool:projectsuffix#!#</string>
  <string>#!#installtool:apppath#!#</string>
  <string>#!#installtool:machinename#!#</string>
  <string>#!#installtool:webpath#!#</string>
  <string>#!#installtool:localservicehostname#!#</string>
  <string>#!#installtool:infosharewswebappname#!#</string>
  <string>#!#installtool:servicecertificatevalidationmode#!#</string>
  <string>#!#installtool:issuerwstrustbindingtype#!#</string>
  <string>#!#installtool:issuerwstrustendpointurl#!#</string>
  <string>#!#installtool:basehostname#!#</string>
  <string>#!#installtool:infoshareauthorwebappname#!#</string>
  <string>#!#installtool:serviceusername#!#</string>
  <string>#!#installtool:servicepassword#!#</string>
  <string>#!#installtool:ps_java_home#!#</string>
  <string>#!#installtool:softwareversion#!#</string>
  <string>#!#installtool:osuser#!#</string>
  <string>#!#installtool:issuercertificatethumbprint#!#</string>
  <string>#!#installtool:infosharestswebappname#!#</string>
  <string>#!#installtool:issuerwstrustendpointurl_normalized#!#</string>
  <string>#!#installtool:ps_javahelp_home#!#</string>
  <string>#!#installtool:ps_fo_processor_dir#!#</string>
  <string>#!#installtool:ps_htmlhelp_processor_dir#!#</string>
  <string>#!#installtool:ps_webworks_automap_application#!#</string>
  <string>#!#installtool:baseurl#!#</string>
  <string>#!#installtool:databasetype#!#</string>
  <string>#!#installtool:connectstring#!#</string>
  <string>#!#installtool:databaseserverversion#!#</string>
  <string>#!#installtool:installtoolversion#!#</string>
  <string>#!#installtool:datetime#!#</string>
  <string>#!#installtool:databasename#!#</string>
  <string>#!#installtool:netbiosdomainname#!#</string>
  <string>#!#installtool:databaseuser#!#</string>
  <string>#!#installtool:databasesource#!#</string>
  <string>#!#installtool:databasepassword#!#</string>
  <string>#!#installtool:issueractorusername#!#</string>
  <string>#!#installtool:issueractorpassword#!#</string>
  <string>#!#installtool:issuercertificatevalidationmode#!#</string>
  <string>#!#installtool:servicecertificatethumbprint#!#</string>
  <string>#!#installtool:issuerwsfederationendpointurl#!#</string>
  <string>#!#installtool:authenticationtype#!#</string>
  <string>#!#installtool:issuerwstrustmexurl#!#</string>
  <string>#!#installtool:ospassword#!#</string>
  <string>#!#installtool:solrlucene_service_port#!#</string>
  <string>#!#installtool:solrlucene_stop_port#!#</string>
  <string>#!#installtool:websitename#!#</string>
  <string>#!#installtool:infosharestswindowsauthenticationenabled#!#</string>
  <string>#!#installtool:srcpath#!#</string>
  <string>#!#installtool:workspacepath#!#</string>
</ArrayOfString>'

    It "Expand-ISHCMPackage replace installtool input parameters"{
		#Arrange
        New-Item -Path $uncPackagePath -Name $customFileName -Force -type file -Value $testFileContent |Out-Null
        ZipFolder -zipfile $zipPath -folderPath $uncPackagePath
        Move-Item -Path $zipPath -Destination $uncPackagePath -Force
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockExpandISHCMPackage -Session $session -ArgumentList $testingDeploymentName, $zipName, "ToCustom"
        #Assert
        $content = Get-Content -Path $customFile
        $content -Match "#!#" | Should Be $null
    }

$testFileContentWithWrongPlaceholder = '<?xml version="1.0"?>
<ArrayOfString xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <string>#!#installtool:infosharestswindowsauthenticationenabled#!#</string>
  <string>#!#installtool:trisoftxopuswebappname#!#</string>
  <string>#!#installtool:srcpath#!#</string>
  <string>#!#installtool:workspacepath#!#</string>
</ArrayOfString>'


    It "Expand-ISHCMPackage replace installtool input parameters with wrong placeholder"{
		#Arrange
        New-Item -Path $uncPackagePath -Name $customFileName -Force -type file -Value $testFileContentWithWrongPlaceholder |Out-Null
        ZipFolder -zipfile $zipPath -folderPath $uncPackagePath
        Move-Item -Path $zipPath -Destination $uncPackagePath -Force
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockExpandISHCMPackage -Session $session -ArgumentList $testingDeploymentName, $zipName, "ToCustom" -WarningVariable Warning
        #Assert
        $content = Get-Content -Path $customFile
        $Warning | should Match "Input parameter trisoftxopuswebappname in placeholder #!#installtool:trisoftxopuswebappname#!# is not found." 
    }
    
    $ignoreTestFileContent = '<?xml version="1.0"?><ArrayOfString xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema"><string>#!#installtool:datapath#!#</string></ArrayOfString>'

    It "Expand-ISHCMPackage does not replace palceholders in unknown files"{
		#Arrange
        New-Item -Path $uncPackagePath -Name "test.file" -Force -type file -Value $ignoreTestFileContent |Out-Null
        ZipFolder -zipfile $zipPath -folderPath $uncPackagePath
        Move-Item -Path $zipPath -Destination $uncPackagePath -Force
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockExpandISHCMPackage -Session $session -ArgumentList $testingDeploymentName, $zipName, "ToCustom"
        #Assert
        $pathToTestFile = Join-Path $filePath "\Custom\test.file"
        $content = Get-Content -Path $pathToTestFile
        $content | Should Match "<string>#!#installtool:datapath#!#</string>"
    }

    UndoDeploymentBackToVanila $testingDeploymentName $true
}