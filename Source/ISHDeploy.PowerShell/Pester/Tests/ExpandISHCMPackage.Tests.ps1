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
$customFile = Join-Path $filePath "\Custom\test.file"
$binFile = Join-Path $filePath "\Bin\test.file"

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
        ArtifactCleaner -filePath $uncPackagePath -fileName $zipName    
        UndoDeploymentBackToVanila $testingDeploymentName $true
    }

    It "Expand-ISHCMPackage copies to custom folder"{
		#Arrange
        New-Item -Path $uncPackagePath -Name "test.file" -Force -type file |Out-Null
        ZipFolder -zipfile $zipPath -folderPath $uncPackagePath
        Move-Item -Path $zipPath -Destination $uncPackagePath -Force
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockExpandISHCMPackage -Session $session -ArgumentList $testingDeploymentName, $zipName, "ToCustom"
        #Assert
        RemotePathCheck $customFile | Should Be "True"
    }

    It "Expand-ISHCMPackage copies to bin folder"{
		#Arrange
        New-Item -Path $uncPackagePath -Name "test.file" -Force -type file |Out-Null
        ZipFolder -zipfile $zipPath -folderPath $uncPackagePath
        Move-Item -Path $zipPath -Destination $uncPackagePath -Force
        #Invoke-CommandRemoteOrLocal -ScriptBlock{Test-Path "$packagePath\test.file"} -Session $session -ArgumentList $packagePath | Should Be "True"
        
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockExpandISHCMPackage -Session $session -ArgumentList $testingDeploymentName, $zipName, "ToBiN"
        #Assert
        RemotePathCheck $binFile | Should Be "True"
    }

    It "Expand-ISHCMPackage owerwrites file in custom folder"{
		#Arrange
        New-Item -Path $uncPackagePath -Name "test.file" -Force -type file |Out-Null
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
        New-Item -Path $uncPackagePath -Name "test.file" -Force -type file |Out-Null
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
        New-Item -Path $uncPackagePath -Name "test.file" -Force -type file |Out-Null
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
        New-Item -Path $uncPackagePath -Name "test.file" -Force -type file |Out-Null

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
        New-Item -Path $uncPackagePath -Name "test.file" -Force -type file |Out-Null

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
        New-Item -Path $uncPackagePath -Name "test.file" -Force -type file |Out-Null
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

    

    UndoDeploymentBackToVanila $testingDeploymentName $true
}