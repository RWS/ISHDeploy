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
$requestedMetadataFieldName = "testRequestedMetadataName"
$requestedMetadataFieldLevel = "logical"
$requestedMetadataFieldValueType = "value"
$requestedMetadataParameters = @{Name =$requestedMetadataFieldName; Level=$requestedMetadataFieldLevel; ValueType=$requestedMetadataFieldValueType} 
$requestedMetadataParameters2 =  @{Name ="$requestedMetadataFieldName 2"; Level="lng"; ValueType="id"} 
$requestedMetadata = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockNewISHFieldMetadata -Session $session -ArgumentList $requestedMetadataParameters
$requestedMetadata2 = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockNewISHFieldMetadata -Session $session -ArgumentList $requestedMetadataParameters2
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

$scriptBlockRemoveISHTranslationFileSystemExport  = {
    param (
        $ishDeployName

    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }

    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Remove-ISHTranslationFileSystemExport -ISHDeployment $ishDeploy 

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
    
    $result["RequestedName"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/fileSystem/instances/add/requestedMetadata/ishfields/ishfield").name
    $result["RequestedLevel"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/fileSystem/instances/add/requestedMetadata/ishfields/ishfield").level
    $result["RequestedIshvaluetype"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/fileSystem/instances/add/requestedMetadata/ishfields/ishfield").ishvaluetype

    return $result
}
function remoteReadTargetXML() {

    #read all files that are touched with commandlet
    $result = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockReadTargetXML -Session $session -ArgumentList $filePath
    
    #get variables and nodes from files

    $global:NameFromFile = $result["Name"]  
    $global:MaxJobSizeFromFile = $result["MaxJobSize"]
    $global:ExportFolderFromFile = $result["ExportFolder"]

    $global:RequestedNameFromFile = $result["RequestedName"]
    $global:RequestedLevelFromFile = $result["RequestedLevel"]
    $global:RequestedIshvaluetypeFromFile = $result["RequestedIshvaluetype"]
}
function remoteReadReadArrays() {
    $scriptBlockReadArrays = {
        param(
            $filePath
        )
        #read all files that are touched with commandlet
    
        [System.Xml.XmlDocument]$OrganizerConfig = new-object System.Xml.XmlDocument
        $OrganizerConfig.load("$filePath\TranslationOrganizer.exe.config")
    
        $result =  @{}
        #get variables and nodes from files

        $result["RequestedMetadataCount"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/fileSystem/instances/add/requestedMetadata/ishfields/ishfield").Count

        return $result
    }

    #read all files that are touched with commandlet
    $result = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockReadArrays -Session $session -ArgumentList $filePath
    
    #get variables and nodes from files

    $global:RequestedMetadataCount = $result["RequestedMetadataCount"]
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
            ExportFolder = $ExportFolder;
            RequestedMetadata = $requestedMetadata
        }
		Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveISHTranslationFileSystemExport  -Session $session -ArgumentList $testingDeploymentName
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHTranslationFileSystemExport -Session $session -ArgumentList $testingDeploymentName, $params
        
        #Assert
        remoteReadTargetXML        

        $NameFromFile | Should be $Name
        $MaxJobSizeFromFile | Should be $MaxJobSize
        $ExportFolderFromFile | Should be $ExportFolder
        $RequestedNameFromFile | Should be $requestedMetadataFieldName
        $RequestedLevelFromFile | Should be $requestedMetadataFieldLevel
        $RequestedIshvaluetypeFromFile | Should be $requestedMetadataFieldValueType
    }
    

    It "Set ISHTranslationFileSystemExport with default MaximumJobSize"{       
        #Act
        $params = @{
            Name=$Name;
            ExportFolder = $ExportFolder;
            RequestedMetadata = $requestedMetadata
        }
		Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveISHTranslationFileSystemExport  -Session $session -ArgumentList $testingDeploymentName
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHTranslationFileSystemExport -Session $session -ArgumentList $testingDeploymentName, $params
        
        #Assert
        remoteReadTargetXML

        $MaxJobSizeFromFile | Should be 5242880
    }

    It "Set ISHTranslationFileSystemExport with do not update MaximumJobSize if it is not specified"{       
        #Act
        
        $params = @{
            Name=$Name;
            MaximumJobSize=$MaxJobSize;
            ExportFolder = $ExportFolder;
            RequestedMetadata = $requestedMetadata
        }
		Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveISHTranslationFileSystemExport  -Session $session -ArgumentList $testingDeploymentName
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHTranslationFileSystemExport -Session $session -ArgumentList $testingDeploymentName, $params
        
        $params = @{
            Name=$Name;
            ExportFolder = $ExportFolder;
            RequestedMetadata = $requestedMetadata
        }
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHTranslationFileSystemExport -Session $session -ArgumentList $testingDeploymentName, $params
        
        #Assert
        remoteReadTargetXML

        $MaxJobSizeFromFile | Should be $MaxJobSize
    }
   
    It "Set ISHTranslationFileSystemExport with wrong XML"{
        #Arrange
        # Running valid scenario commandlet to out files into backup folder before they will ba manually modified in test
        $params = @{
            Name=$Name;
            MaximumJobSize=$MaxJobSize;
            ExportFolder = $ExportFolder;
            RequestedMetadata = $requestedMetadata
        }
        { Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHTranslationFileSystemExport -Session $session -ArgumentList $testingDeploymentName, $params } | Should throw "TranslationOrganizer.exe.config already contains settings for FileSystem. You should remove FileSystem configuration section first. To do this you can use Remove-ISHTranslationFileSystemExport cmdlet."

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
            ExportFolder = $ExportFolder;
            RequestedMetadata = $requestedMetadata
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
            ExportFolder = $ExportFolder;
            RequestedMetadata = $requestedMetadata
        }
		Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveISHTranslationFileSystemExport -Session $session -ArgumentList $testingDeploymentName
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHTranslationFileSystemExport -Session $session -ArgumentList $testingDeploymentName, $params
        remoteReadTargetXML
        $NameFromFile | Should be $Name
        $MaxJobSizeFromFile | Should be $MaxJobSize
        $ExportFolderFromFile | Should be $ExportFolder

		Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveISHTranslationFileSystemExport -Session $session -ArgumentList $testingDeploymentName
		remoteReadTargetXML
		#Assert
		$NameFromFile | Should be $null
        $MaxJobSizeFromFile | Should be $null
        $ExportFolderFromFile | Should be $null
    }

    It "Remove ISHTranslationFileSystemExport removes default"{       
        #Act
		{Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveISHTranslationFileSystemExport  -Session $session -ArgumentList $testingDeploymentName} | Should not Throw
    }

    It "Remove ISHTranslationFileSystemExport with wrong XML"{
        #Arrange
        
        Rename-Item "$filepath\TranslationOrganizer.exe.config"  "_TranslationOrganizer.exe.config"
        #New-Item "$filepath\TranslationOrganizer.exe.config" -type file |Out-Null
        
        #Act/Assert
        #{ Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveISHTranslationFileSystemExport  -Session $session -ArgumentList $testingDeploymentName -ErrorAction Stop }| Should Throw "Root element is missing"
        #Rollback
        #Remove-Item "$filepath\TranslationOrganizer.exe.config"
        #Rename-Item "$filepath\_TranslationOrganizer.exe.config" "TranslationOrganizer.exe.config"
    }

    It "Remove ISHTranslationFileSystemExport with no XML"{
        #Arrange
        Rename-Item "$filepath\TranslationOrganizer.exe.config"  "_TranslationOrganizer.exe.config"

        #Act/Assert
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveISHTranslationFileSystemExport  -Session $session -ArgumentList $testingDeploymentName -ErrorAction Stop }| Should Throw "Could not find file"
        #Rollback
        Rename-Item "$filepath\_TranslationOrganizer.exe.config" "TranslationOrganizer.exe.config"
    }
    
	It "Set ISHTranslationFileSystemExport sets multiple metadata"{
		#Arrange
        $params = @{
            Name=$Name;
            MaximumJobSize=$MaxJobSize;
            ExportFolder = $ExportFolder;
            RequestedMetadata = @($requestedMetadata, $requestedMetadata2);
        }
        #Act
		Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveISHTranslationFileSystemExport  -Session $session -ArgumentList $testingDeploymentName
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHTranslationFileSystemExport -Session $session -ArgumentList $testingDeploymentName, $params -ErrorAction Stop }| Should Not Throw
        
        #Assert
        remoteReadReadArrays
        $RequestedMetadataCount| Should be 2
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