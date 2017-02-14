param(
    $session = $null,
    $testingDeploymentName = "InfoShare"
)
. "$PSScriptRoot\Common.ps1"

#region variables
$xmlPath = $webPath.ToString().replace(":", "$")
$xmlPath = "\\$computerName\$xmlPath"
$filePath = $appPath.ToString().replace(":", "$")
$filePath = "\\$computerName\$filePath"
$filePath = Join-Path $filePath "TranslationOrganizer\Bin"

$Name = "testName"
$MaxJobSize = 250
$ExportFolder = "c:\testExportFolder"
#endregion

#region Script Blocks
$scriptBlockSetISHTranslationFileSystemExport = {
    param (
        $ishDeployName,
        $parameters

    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }

    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Set-ISHTranslationFileSystemExport -ISHDeployment $ishDeploy @parameters

}

$scriptBlockRemoveISHIntegrationFileSystem = {
    param (
        $ishDeployName

    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }

    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Remove-ISHIntegrationFileSystem -ISHDeployment $ishDeploy 

}


#endregion


# Function reads target files and their content, searches for specified nodes in xml
$scriptBlockReadTargetXML = {
    param(

        $filePath
        
    )
    #read all files that are touched with commandlet
    
    [System.Xml.XmlDocument]$OrganizerConfig = new-object System.Xml.XmlDocument
    $OrganizerConfig.load("$filePath\TranslationOrganizer.exe.config")
    
    $result =  @{}
    #get variables and nodes from files
    $result["Name"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/fileSystem/instances/add").alias
    $result["MaxJobSize"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/fileSystem/instances/add").externalJobMaxTotalUncompressedSizeBytes
    $result["ExportFolder"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/fileSystem/instances/add").exportFolder

    return $result
}
function remoteReadTargetXML() {

    #read all files that are touched with commandlet
    $result = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockReadTargetXML -Session $session -ArgumentList $filePath
    
    #get variables and nodes from files


   $global:NameFromFile = $result["Name"]  
   $global:MaxJobSizeFromFile = $result["MaxJobSize"]
   $global:ExportFolderFromFile = $result["ExportFolder"]

}


Describe "Testing ISHTranslationFileSystemExport"{
    BeforeEach {
        ArtifactCleaner -filePath $filePath -fileName "TranslationOrganizer.exe.config"
        UndoDeploymentBackToVanila $testingDeploymentName $true
    }

    It "Set ISHTranslationFileSystemExport with all parameters"{       
        #Act

        $params = @{
        Name=$Name;
        MaximumJobSize=$MaxJobSize;
        ExportFolder = $ExportFolder
        }
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHTranslationFileSystemExport -Session $session -ArgumentList $testingDeploymentName, $params
        
        #Assert
        remoteReadTargetXML
        

        $NameFromFile | Should be $Name
        $MaxJobSizeFromFile | Should be $MaxJobSize
        $ExportFolderFromFile | Should be $ExportFolder
    }
   
   It "Set ISHTranslationFileSystemExport with wrong XML"{
        #Arrange
        # Running valid scenario commandlet to out files into backup folder before they will ba manually modified in test
        $params = @{
        Name=$Name;
        MaximumJobSize=$MaxJobSize;
        ExportFolder = $ExportFolder
        }
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHTranslationFileSystemExport -Session $session -ArgumentList $testingDeploymentName, $params

        Rename-Item "$filepath\TranslationOrganizer.exe.config"  "_TranslationOrganizer.exe.config"
        New-Item "$filepath\TranslationOrganizer.exe.config" -type file |Out-Null
        
        #Act/Assert
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHTranslationFileSystemExport -Session $session -ArgumentList $testingDeploymentName, $params -ErrorAction Stop }| Should Throw "Root element is missing"
        #Rollback
        Remove-Item "$filepath\TranslationOrganizer.exe.config"
        Rename-Item "$filepath\_TranslationOrganizer.exe.config" "TranslationOrganizer.exe.config"
    }

    It "Set ISHTranslationFileSystemExport with no XML"{
        #Arrange
        Rename-Item "$filepath\TranslationOrganizer.exe.config"  "_TranslationOrganizer.exe.config"

        #Act/Assert
        $params = @{
        Name=$Name;
        MaximumJobSize=$MaxJobSize;
        ExportFolder = $ExportFolder
        }

        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHTranslationFileSystemExport -Session $session -ArgumentList $testingDeploymentName, $params -ErrorAction Stop }| Should Throw "Could not find file"
        #Rollback
        Rename-Item "$filepath\_TranslationOrganizer.exe.config" "TranslationOrganizer.exe.config"
    }

	It "Remove ISHTranslationFileSystemExport"{       
        #Act

        $params = @{
        Name=$Name;
        MaximumJobSize=$MaxJobSize;
        ExportFolder = $ExportFolder
        }
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHTranslationFileSystemExport -Session $session -ArgumentList $testingDeploymentName, $params
        
        remoteReadTargetXML
        $NameFromFile | Should be $Name
        $MaxJobSizeFromFile | Should be $MaxJobSize
        $ExportFolderFromFile | Should be $ExportFolder

		Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveISHIntegrationFileSystem -Session $session -ArgumentList $testingDeploymentName
		remoteReadTargetXML
		#Assert
		$NameFromFile | Should be $null
        $MaxJobSizeFromFile | Should be $null
        $ExportFolderFromFile | Should be $null
    }

    It "Remove ISHTranslationFileSystemExport removes default"{       
        #Act


		{Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveISHIntegrationFileSystem -Session $session -ArgumentList $testingDeploymentName} | Should not Throw

    }

    It "Remove ISHTranslationFileSystemExport with wrong XML"{
        #Arrange
        # Running valid scenario commandlet to out files into backup folder before they will ba manually modified in test
        $params = @{
        Name=$Name;
        MaximumJobSize=$MaxJobSize;
        ExportFolder = $ExportFolder
        }
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHTranslationFileSystemExport -Session $session -ArgumentList $testingDeploymentName, $params

        Rename-Item "$filepath\TranslationOrganizer.exe.config"  "_TranslationOrganizer.exe.config"
        New-Item "$filepath\TranslationOrganizer.exe.config" -type file |Out-Null
        
        #Act/Assert
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveISHIntegrationFileSystem -Session $session -ArgumentList $testingDeploymentName -ErrorAction Stop }| Should Throw "Root element is missing"
        #Rollback
        Remove-Item "$filepath\TranslationOrganizer.exe.config"
        Rename-Item "$filepath\_TranslationOrganizer.exe.config" "TranslationOrganizer.exe.config"
    }

    It "Remove ISHTranslationFileSystemExport with no XML"{
        #Arrange
        Rename-Item "$filepath\TranslationOrganizer.exe.config"  "_TranslationOrganizer.exe.config"

        #Act/Assert
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveISHIntegrationFileSystem -Session $session -ArgumentList $testingDeploymentName -ErrorAction Stop }| Should Throw "Could not find file"
        #Rollback
        Rename-Item "$filepath\_TranslationOrganizer.exe.config" "TranslationOrganizer.exe.config"
    }
    <#
    It "Set ISHTranslationFileSystemExport writes proper history"{        
       #Act
        $params = @{
        Name=$Name;
        MaximumJobSize=$MaxJobSize;
        ExportFolder = $ExportFolder
        }
        
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHTranslationFileSystemExport -Session $session -ArgumentList $testingDeploymentName, $params
        $history = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetHistory -Session $session -ArgumentList $testingDeploymentName

        #Assert
        $history.Contains('Set-ISHTranslationFileSystemExport -ISHDeployment $deploymentName -MaximumJobSize 250 -Name "testName" -Credential (New-Object System.Management.Automation.PSCredential ("testUserName", (ConvertTo-SecureString "testPassword" -AsPlainText -Force))) -RetriesOnTimeout 2 -Uri "testUri" -Mappings @((New-ISHTranslationFileSystemExportMapping -ISHLanguage en -WSLocaleID 192))') | Should be "True"     
    } 
	#>
     UndoDeploymentBackToVanila $testingDeploymentName $true
}