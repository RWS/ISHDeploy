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
$customFile = Join-Path $filePath "\Custom\test.file"
$binFile = Join-Path $filePath "\Bin\test.file"


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
        UndoDeploymentBackToVanila $testingDeploymentName $true
    }

    It "Copy-ISHCMFile copies to custom folder"{
		#Arrange
        New-Item -Path $uncPackagePath -Name "test.file" -Force -type file |Out-Null
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockCopyISHCMFile -Session $session -ArgumentList $testingDeploymentName, "test.file", "ToCustom"
        #Assert
        RemotePathCheck $customFile | Should Be "True"
    }

    It "Copy-ISHCMFile copies to bin folder"{
		#Arrange
        New-Item -Path $uncPackagePath -Name "test.file" -Force -type file |Out-Null
        #Invoke-CommandRemoteOrLocal -ScriptBlock{Test-Path "$packagePath\test.file"} -Session $session -ArgumentList $packagePath | Should Be "True"
        
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockCopyISHCMFile -Session $session -ArgumentList $testingDeploymentName, "test.file", "ToBiN"
        #Assert
        RemotePathCheck $binFile | Should Be "True"
    }

    It "Copy-ISHCMFile owerwrites file in custom folder"{
		#Arrange
        New-Item -Path $uncPackagePath -Name "test.file" -Force -type file |Out-Null

        #Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockCopyISHCMFile -Session $session -ArgumentList $testingDeploymentName, "test.file", "ToCustom"
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockCopyISHCMFile -Session $session -ArgumentList $testingDeploymentName, "test.file", "ToCustom" -WarningVariable Warning} | Should not Throw
        #need extra commandlet call here because otherwise Warning variable is empty
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockCopyISHCMFile -Session $session -ArgumentList $testingDeploymentName, "test.file", "ToCustom" -WarningVariable Warning
        $Warning | should Match " has been overritten" 
        #Assert
        RemotePathCheck $customFile | Should Be "True"
    }

    It "Copy-ISHCMFile owerwrites custom file in bin folder"{
		#Arrange
        New-Item -Path $uncPackagePath -Name "test.file" -Force -type file |Out-Null
        
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockCopyISHCMFile -Session $session -ArgumentList $testingDeploymentName, "test.file", "ToBin"
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockCopyISHCMFile -Session $session -ArgumentList $testingDeploymentName, "test.file", "ToBin" -WarningVariable Warning} | Should not Throw
        #need extra commandlet call here because otherwise Warning variable is empty
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockCopyISHCMFile -Session $session -ArgumentList $testingDeploymentName, "test.file", "ToBin" -WarningVariable Warning
        $Warning | should Match " has been overritten" 
        #Assert
        RemotePathCheck $binFile | Should Be "True"
    }
   
    It "Copy-ISHCMFile throws error when file does not exist"{
		#Arrange
        New-Item -Path $uncPackagePath -Name "test.file" -Force -type file |Out-Null
        
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockCopyISHCMFile -Session $session -ArgumentList $testingDeploymentName, "unexistingTest.file", "ToCustom"} | Should throw "Could not find file"
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
        New-Item -Path $uncPackagePath -Name "test.file" -Force -type file |Out-Null
        #Assert
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockCopyISHCMFile -Session $session -ArgumentList $testingDeploymentName, "Test.file", "copyMultiple", "Trisoft.Web.dll" 
        $secondCustomFile = Join-Path $filePath "\Custom\Trisoft.Web.dll"
        RemotePathCheck $customFile | Should Be "True"
        RemotePathCheck $secondCustomFile | Should Be "True"
    }
    
    #It "Copy-ISHCMFile writes history"{
    #    #Act
    #    New-Item -Path $uncPackagePath -Name "test.file" -Force
    #    Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockCopyISHCMFile -Session $session -ArgumentList $testingDeploymentName, "test.file", "ToCustom"
    #    $history = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetHistory -Session $session -ArgumentList $testingDeploymentName
        
    #    #Assert
    #    $history.Contains('Copy-ISHCMFile -ISHDeployment $deploymentName -FileName $test.file -ToCustom') | Should be "True"
    #}

    It "Undo-ISHDeployment deletes copied files"{      
        #Act
		New-Item -Path $uncPackagePath -Name "test.file" -Force -type file |Out-Null
		Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockCopyISHCMFile -Session $session -ArgumentList $testingDeploymentName, "test.file", "ToCustom"
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockCopyISHCMFile -Session $session -ArgumentList $testingDeploymentName, "test.file", "ToBin"

        UndoDeploymentBackToVanila $testingDeploymentName $true
		RemotePathCheck $customFile | Should Be "False"
        RemotePathCheck $binFile | Should Be "False"
    }
}