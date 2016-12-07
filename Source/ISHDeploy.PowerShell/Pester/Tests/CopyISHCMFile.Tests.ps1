param(
    $session = $null,
    $testingDeploymentName = "InfoShare"
)
. "$PSScriptRoot\Common.ps1"


$moduleName = Invoke-CommandRemoteOrLocal -ScriptBlock { (Get-Module "ISHDeploy.*").Name } -Session $session
$packagePath = "C:\ProgramData\$moduleName\$($testingDeployment.Name)\Packages"
$computerName = $computerName.split(".")[0]
$uncPackagePath = "\\$computerName\" + ($packagePath.replace(":", "$"))
$filePath = Join-Path $testingDeployment.WebPath ("\Web{0}\Author\ASP" -f $suffix )
$customFileName = "test.config"
$customFile = Join-Path $filePath "\Custom\$customFileName"
$binFile = Join-Path $filePath "\Bin\$customFileName"


#endregion

#region Script Blocks 



# Script block for Set-ISHContentEditor
$scriptBlockCopyISHCMFile = {
    param (
        [Parameter(Mandatory=$true)]
        $ishDeployName ,
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
        Copy-ISHCMFile -ISHDeployment $ishDeploy -FileName $fileName -ToBin
    }
    elseif($switchState -eq "ToCustom"){
        Copy-ISHCMFile -ISHDeployment $ishDeploy -FileName $fileName -ToCustom
    }
    else {
        Copy-ISHCMFile -ISHDeployment $ishDeploy -FileName $fileName, $filename2 -ToCustom
    }
}

Describe "Testing Copy-ISHCMFile"{
    BeforeEach {
		ArtifactCleaner -filePath $uncPackagePath -fileName "test.file"
        ArtifactCleaner -filePath $uncPackagePath -fileName "Trisoft.Web.dll"  
		ArtifactCleaner -filePath $uncPackagePath -fileName $customFileName   
        UndoDeploymentBackToVanila $testingDeploymentName $true
    }

    It "Copy-ISHCMFile copies to custom folder"{
		#Arrange
        New-Item -Path $uncPackagePath -Name $customFileName -Force -type file |Out-Null
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockCopyISHCMFile -Session $session -ArgumentList $testingDeploymentName, $customFileName, "ToCustom"
        #Assert
        RemotePathCheck $customFile | Should Be "True"
    }

    It "Copy-ISHCMFile copies to bin folder"{
		#Arrange
        New-Item -Path $uncPackagePath -Name $customFileName -Force -type file |Out-Null
        #Invoke-CommandRemoteOrLocal -ScriptBlock{Test-Path "$packagePath\test.file"} -Session $session -ArgumentList $packagePath | Should Be "True"
        
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockCopyISHCMFile -Session $session -ArgumentList $testingDeploymentName, $customFileName, "TobiN"
        #Assert
        RemotePathCheck $binFile | Should Be "True"
    }

    It "Copy-ISHCMFile owerwrites file in custom folder"{
		#Arrange
        New-Item -Path $uncPackagePath -Name $customFileName -Force -type file |Out-Null

        #Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockCopyISHCMFile -Session $session -ArgumentList $testingDeploymentName, "test.file", "ToCustom"
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockCopyISHCMFile -Session $session -ArgumentList $testingDeploymentName, $customFileName, "ToCustom" -WarningVariable Warning} | Should not Throw
        #need extra commandlet call here because otherwise Warning variable is empty
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockCopyISHCMFile -Session $session -ArgumentList $testingDeploymentName, $customFileName, "ToCustom" -WarningVariable Warning
        $Warning | should Match " has been overritten" 
        #Assert
        RemotePathCheck $customFile | Should Be "True"
    }

    It "Copy-ISHCMFile owerwrites custom file in bin folder"{
		#Arrange
        New-Item -Path $uncPackagePath -Name $customFileName -Force -type file |Out-Null
        
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockCopyISHCMFile -Session $session -ArgumentList $testingDeploymentName, $customFileName, "ToBin"
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockCopyISHCMFile -Session $session -ArgumentList $testingDeploymentName, $customFileName, "ToBin" -WarningVariable Warning} | Should not Throw
        #need extra commandlet call here because otherwise Warning variable is empty
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockCopyISHCMFile -Session $session -ArgumentList $testingDeploymentName, $customFileName, "ToBin" -WarningVariable Warning
        $Warning | should Match " has been overritten" 
        #Assert
        RemotePathCheck $binFile | Should Be "True"
    }
   
    It "Copy-ISHCMFile throws error when file does not exist"{
		#Arrange
        New-Item -Path $uncPackagePath -Name $customFileName -Force -type file |Out-Null
        
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockCopyISHCMFile -Session $session -ArgumentList $testingDeploymentName, "unexistingTest.file", "ToCustom"} | Should throw "InvalidPath for"
    }

    It "Copy-ISHCMFile does not owerwrite CM files in bin folder"{
		#Arrange
        New-Item -Path $uncPackagePath -Name "Trisoft.Web.dll" -Force -type file |Out-Null
        #Assert
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockCopyISHCMFile -Session $session -ArgumentList $testingDeploymentName, "unexistingTest.file", "ToCustom"} | Should throw 

    }
    
    It "Copy-ISHCMFile can copy multiple files"{
		#Arrange
        New-Item -Path $uncPackagePath -Name "Trisoft.Web.dll" -Force -type file |Out-Null
        New-Item -Path $uncPackagePath -Name $customFileName -Force -type file |Out-Null
        #Assert
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockCopyISHCMFile -Session $session -ArgumentList $testingDeploymentName, $customFileName, "copyMultiple", "Trisoft.Web.dll" 
        $secondCustomFile = Join-Path $filePath "\Custom\Trisoft.Web.dll"
        RemotePathCheck $customFile | Should Be "True"
        RemotePathCheck $secondCustomFile | Should Be "True"
    }
    
    It "Copy-ISHCMFile writes history"{
        #Act
        New-Item -Path $uncPackagePath -Name $customFileName -Force -type file |Out-Null
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockCopyISHCMFile -Session $session -ArgumentList $testingDeploymentName, $customFileName, "ToCustom"
        $history = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetHistory -Session $session -ArgumentList $testingDeploymentName
        
        #Assert
        $history.Contains('if($IncludeCustomFile)
{
     Copy-ISHCMFile -ISHDeployment $deploymentName -FileName @("test.config") -ToCustom 
}') | Should be "True"
    }

    It "Undo-ISHDeployment deletes copied files"{      
        #Act
		New-Item -Path $uncPackagePath -Name $customFileName -Force -type file |Out-Null
		Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockCopyISHCMFile -Session $session -ArgumentList $testingDeploymentName, $customFileName, "ToCustom"
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockCopyISHCMFile -Session $session -ArgumentList $testingDeploymentName, $customFileName, "ToBin"

        UndoDeploymentBackToVanila $testingDeploymentName $true
		RemotePathCheck $customFile | Should Be "False"
        RemotePathCheck $binFile | Should Be "False"
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

    It "Copy-ISHCMFile replace installtool input parameters"{
		#Arrange
        New-Item -Path $uncPackagePath -Name $customFileName -Force -type file -Value $testFileContent | Out-Null
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockCopyISHCMFile -Session $session -ArgumentList $testingDeploymentName, $customFileName, "ToCustom"
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

    It "Copy-ISHCMFile replace installtool input parameters with wrong placeholder"{
		#Arrange
        New-Item -Path $uncPackagePath -Name $customFileName -Force -type file -Value $testFileContentWithWrongPlaceholder | Out-Null
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockCopyISHCMFile -Session $session -ArgumentList $testingDeploymentName, $customFileName, "ToCustom" -WarningVariable Warning
        #Assert
        $content = Get-Content -Path $customFile
        $Warning | should Match "Input parameter trisoftxopuswebappname in placeholder #!#installtool:trisoftxopuswebappname#!# is not found." 
    }

    $ignoreTestFileContent = '<?xml version="1.0"?><ArrayOfString xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema"><string>#!#installtool:datapath#!#</string></ArrayOfString>'

    It "Copy-ISHCMFile does not replace palceholders in unknown files"{
		#Arrange
        New-Item -Path $uncPackagePath -Name "test.file" -Force -type file -Value $ignoreTestFileContent | Out-Null
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockCopyISHCMFile -Session $session -ArgumentList $testingDeploymentName, "test.file", "ToCustom"
        #Assert
        $pathToTestFile = Join-Path $filePath "\Custom\test.file"
        $content = Get-Content -Path $pathToTestFile
        $content | Should Match "<string>#!#installtool:datapath#!#</string>"
    }
}