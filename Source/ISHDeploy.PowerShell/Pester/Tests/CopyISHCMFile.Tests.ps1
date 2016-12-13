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

    It "Copy-ISHCMFile replace installtool input parameters"{
		#Arrange
        New-Item -Path $uncPackagePath -Name $customFileName -Force -type file | Out-Null
        $testFileContent = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetParameters -Session $session -ArgumentList $testingDeploymentName, @{Original = $true; } | Select-Object -ExpandProperty Name|ForEach-Object {"#!#installtool:$($_.ToUpperInvariant())#!#"}|Out-File (Join-path $uncPackagePath $customFileName) -Force
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockCopyISHCMFile -Session $session -ArgumentList $testingDeploymentName, $customFileName, "ToCustom"
        #Assert
        $pathToTestFile = Join-Path $filePath "\Custom\$customFileName"
        $content = Invoke-CommandRemoteOrLocal -ScriptBlock {param($path) Get-Content $path} -Session $session -ArgumentList $pathToTestFile
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
        $Warning | should Match "Input parameter trisoftxopuswebappname in placeholder #!#installtool:trisoftxopuswebappname#!# is not found." 
    }

    $ignoreTestFileContent = '<?xml version="1.0"?><ArrayOfString xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema"><string>#!#installtool:datapath#!#</string></ArrayOfString>'

    It "Copy-ISHCMFile does not replace palceholders in unknown files"{
		#Arrange
        New-Item -Path $uncPackagePath -Name "test.file" -Force -type file -Value $ignoreTestFileContent | Out-Null
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockCopyISHCMFile -Session $session -ArgumentList $testingDeploymentName, "test.file", "ToCustom"
        #Assert
        $pathToTestFile = Join-Path $filePath "\Custom\test.file"
        $content = Invoke-CommandRemoteOrLocal -ScriptBlock {param($path) Get-Content $path} -Session $session -ArgumentList $pathToTestFile
        $content | Should Match "<string>#!#installtool:datapath#!#</string>"
    }
}