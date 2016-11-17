﻿param(
    $session = $null,
    $testingDeploymentName = "InfoShare"
)
. "$PSScriptRoot\Common.ps1"


$moduleName = Invoke-CommandRemoteOrLocal -ScriptBlock { (Get-Module "ISHDeploy.*").Name } -Session $session

$computerName = $computerName.split(".")[0]

$filePath = Join-Path $testingDeployment.WebPath ("\Web{0}\Author\ASP" -f $suffix )

$configPath = Join-Path $testingDeployment.WebPath ("\Web{0}\Author\ASP\UI\Extensions\_config.xml" -f $suffix )
$uncConfigPath = "\\$computerName\" + ($configPath.replace(":", "$"))
$extentionLoaderPath = Join-Path $testingDeployment.WebPath ("\Web{0}\Author\ASP\UI\Helpers\ExtensionsLoader.js" -f $suffix )

$customFile = Join-Path $filePath "\Custom"
$uncCustomFolderPath = "\\$computerName\" + ($filePath.replace(":", "$"))

#endregion

#region Script Blocks 



# Script block for Set-ISHContentEditor
$scriptBlockSetISHCMCUILResourceGroup = {
    param (
        [Parameter(Mandatory=$true)]
        $ishDeployName ,
        [Parameter(Mandatory=$true)]
        $name,
        [Parameter(Mandatory=$true)]
        $path
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }  
    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Set-ISHCMCUILResourceGroup -Name $name -Path $path -ISHDeployment $ishDeploy
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

function readTargetXML() {
    param (
            $recourceGroupName,
            $recourceGroupPath
    )
	[xml]$config = Get-Content $uncConfigPath -ErrorAction SilentlyContinue
	
	$global:resourceGroup = $config.configuration.resourceGroups.ResourceGroup | ? {$_.NAME -eq $recourceGroupName}
    $count = $config.SelectNodes("configuration/resourceGroups/ResourceGroup[@name='$recourceGroupName']").Count
    $file = $resourceGroup.files.file | ? {$_.NAME -eq "../../Custom/$recourceGroupPath"}
    if($count -eq 1){
	    if($file){
		    Return "Set"
	    }
	    else{
		    Return "NoRecourceGroup"
	    }
    }
    else{
        Return "$count similar recource groups detected"
    }
}

Describe "Testing Set-ISHCMCUILResourceGroup"{
    BeforeEach {
		ArtifactCleaner -filePath $customFile -fileName "test.js"
        UndoDeploymentBackToVanila $testingDeploymentName $true
    }

    It "Set-ISHCMCUILResourceGroup creates entry in config filer"{
		#Arrange
        New-Item -Path $uncCustomFolderPath -Name "Custom" -ItemType directory -Force
        New-Item -Path "$uncCustomFolderPath\Custom" -Name "test.js" -Force -type file |Out-Null
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHCMCUILResourceGroup -Session $session -ArgumentList $testingDeploymentName, "test", "test.js"
        #Assert
        RemotePathCheck $extentionLoaderPath | Should Be "True"

        $result = readTargetXML -recourceGroupName test -recourceGroupPath "test.js"
        $result | Should be "Set"

    }

    It "Set-ISHCMCUILResourceGroup throws when no custom file"{
		#Arrange
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHCMCUILResourceGroup -Session $session -ArgumentList $testingDeploymentName, "test", "test.js"} | Should Throw "Could not find file"

    }

    It "Set-ISHCMCUILResourceGroup updates entry in config filer"{
		#Arrange
        New-Item -Path $uncCustomFolderPath -Name "Custom" -ItemType directory -Force
        New-Item -Path "$uncCustomFolderPath\Custom" -Name "test.js" -Force -type file |Out-Null
        New-Item -Path "$uncCustomFolderPath\Custom" -Name "test2.js" -Force -type file |Out-Null
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHCMCUILResourceGroup -Session $session -ArgumentList $testingDeploymentName, "test", "test.js"
        $result = readTargetXML -recourceGroupName test -recourceGroupPath "test.js"
        $result | Should be "Set"
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHCMCUILResourceGroup -Session $session -ArgumentList $testingDeploymentName, "test", "test2.js"
        #Assert
        $result = readTargetXML -recourceGroupName test -recourceGroupPath "test2.js"
        $result | Should be "Set"

    }

    It "Set-ISHCMCUILResourceGroup set same multiple times"{
		#Arrange
        New-Item -Path $uncCustomFolderPath -Name "Custom" -ItemType directory -Force
        New-Item -Path "$uncCustomFolderPath\Custom" -Name "test.js" -Force -type file |Out-Null
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHCMCUILResourceGroup -Session $session -ArgumentList $testingDeploymentName, "test", "test.js"
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHCMCUILResourceGroup -Session $session -ArgumentList $testingDeploymentName, "test", "test.js"
        #Assert
        RemotePathCheck $extentionLoaderPath | Should Be "True"

        $result = readTargetXML -recourceGroupName test -recourceGroupPath "test.js"
        $result | Should be "Set"

    }

    #For 12.x only

    if($moduleName -like "*12*"){
        It "Undo-ISHDeployment deletes extentionLoader file on 12 version"{      
            #Act
		    #Arrange
            New-Item -Path $uncCustomFolderPath -Name "Custom" -ItemType directory -Force
            New-Item -Path "$uncCustomFolderPath\Custom" -Name "test.js" -Force -type file |Out-Null
            Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHCMCUILResourceGroup -Session $session -ArgumentList $testingDeploymentName, "test", "test.js"
            #Assert
            RemotePathCheck $extentionLoaderPath | Should Be "True"

            UndoDeploymentBackToVanila $testingDeploymentName $true
		    RemotePathCheck $extentionLoaderPath | Should Be "False"
        }
    }
    else{
        It "Undo-ISHDeployment does not delete extentionLoader file on 13 version"{      
            #Act
		    #Arrange
            New-Item -Path $uncCustomFolderPath -Name "Custom" -ItemType directory -Force
            New-Item -Path "$uncCustomFolderPath\Custom" -Name "test.js" -Force -type file |Out-Null
            Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHCMCUILResourceGroup -Session $session -ArgumentList $testingDeploymentName, "test", "test.js"
            #Assert
            RemotePathCheck $extentionLoaderPath | Should Be "True"

            UndoDeploymentBackToVanila $testingDeploymentName $true
		    RemotePathCheck $extentionLoaderPath | Should Be "True"
        }
    }

}