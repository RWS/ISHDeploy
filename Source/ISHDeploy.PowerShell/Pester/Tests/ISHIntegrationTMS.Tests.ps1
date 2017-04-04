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
$Uri = "testUri"
$testUsername = "testUserName"
$testPassword = "testPassword"
$secpasswd = ConvertTo-SecureString $testPassword -AsPlainText -Force
$Credential = New-Object System.Management.Automation.PSCredential ($testUsername, $secpasswd)
$MaxJobSize = 250
$RetriesOnTimeout = 2
$TMSLocaleId = "english"
$TrisoftLanguage = "en"
$mappingParameters = @{ISHLanguage = $TrisoftLanguage; TMSLanguage = $TMSLocaleId}
$Mapping = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockNewISHIntegrationTMSMapping -Session $session -ArgumentList $mappingParameters
$testId = "testId"
$testName = "testName"
$templateParameters = @{TemplateID = $testId; Name = $testName }
$Template = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockNewISHIntegrationTMSTemplate -Session $session -ArgumentList $templateParameters
$requestedMetadataFieldName = "testRequestedMetadataName"
$requestedMetadataFieldLevel = "logical"
$requestedMetadataFieldValueType = "value"
$requestedMetadataParameters = @{Name =$requestedMetadataFieldName; Level=$requestedMetadataFieldLevel; ValueType=$requestedMetadataFieldValueType} 
$requestedMetadataParameters2 =  @{Name ="$requestedMetadataFieldName 2"; Level="lng"; ValueType="id"} 
$requestedMetadata = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockNewISHFieldMetadata -Session $session -ArgumentList $requestedMetadataParameters
$requestedMetadata2 = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockNewISHFieldMetadata -Session $session -ArgumentList $requestedMetadataParameters2
$groupingMetadataFieldName = "testGroupingMetadataName"
$groupingMetadataFieldLevel = "version"
$groupingMetadataFieldValueType = "element"
$groupingMetadataParameters = @{Name =$groupingMetadataFieldName; Level=$groupingMetadataFieldLevel; ValueType=$groupingMetadataFieldValueType} 
$groupingMetadata = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockNewISHFieldMetadata -Session $session -ArgumentList $groupingMetadataParameters
$testAPIKey = "testApKey"
$testSecretKey = "testSecretKey"
#endregion

#region Script Blocks
$scriptBlockSetISHIntegrationTMS = {
    param (
        $ishDeployName,
        $parameters

    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }

    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Set-ISHIntegrationTMS -ISHDeployment $ishDeploy @parameters

}

$scriptBlockRemoveISHIntegrationTMS = {
    param (
        $ishDeployName

    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }

    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Remove-ISHIntegrationTMS -ISHDeployment $ishDeploy 

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
    $result["Name"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/tms/instances/add").alias
    $result["Uri"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/tms/instances/add").uri
    $result["Username"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/tms/instances/add").userName
    $result["Password"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/tms/instances/add").password
    $result["RetriesOnTimeout"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/tms/instances/add").retriesOnTimeout 
    $result["MaxJobSize"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/tms/instances/add").externalJobMaxTotalUncompressedSizeBytes

    $result["TrisoftLanguage"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/tms/instances/add/mappings/add").trisoftLanguage
    $result["tmsLocaleId"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/tms/instances/add/mappings/add").tmsLanguage

    $result["TemplateId"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/tms/instances/add/templates/add").templateId
    $result["TemplateName"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/tms/instances/add/templates/add").templateName
    $result["ApiKey"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/tms/instances/add").apiKey
    $result["Secret"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/tms/instances/add").secret
    $result["HttpTimeout"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/tms/instances/add").httpTimeout

    $result["RequestedName"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/tms/instances/add/requestedMetadata/ishfields/ishfield").name
    $result["RequestedLevel"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/tms/instances/add/requestedMetadata/ishfields/ishfield").level
    $result["RequestedIshvaluetype"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/tms/instances/add/requestedMetadata/ishfields/ishfield").ishvaluetype

    $result["GroupingName"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/tms/instances/add/groupingMetadata/ishfields/ishfield").name
    $result["GroupingLevel"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/tms/instances/add/groupingMetadata/ishfields/ishfield").level
    $result["GroupingIshvaluetype"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/tms/instances/add/groupingMetadata/ishfields/ishfield").ishvaluetype

    return $result
}
function remoteReadTargetXML() {

    #read all files that are touched with commandlet
    $result = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockReadTargetXML -Session $session -ArgumentList $filePath
    
    #get variables and nodes from files


   $global:NameFromFile = $result["Name"]  
   $global:UriFromFile = $result["Uri"]  
   $global:UserNameFromFile = $result["Username"]
   $global:PasswordFromFile = $result["Password"]  
   $global:RetriesOnTimeutFromFile = $result["RetriesOnTimeout"] 
   $global:MaxJobSizeFromFile = $result["MaxJobSize"]

   $global:TrisoftLanguageFromFile = $result["TrisoftLanguage"] 
   $global:TMSLocaleIDFromFile = $result["TMSLocaleId"]

   $global:TemplateIdFromFile = $result["TemplateId"]
   $global:TemplateNameFromFile = $result["TemplateName"] 
   
   $global:ApiKeyFromFile = $result["ApiKey"]
   $global:SecretFromFile = $result["Secret"]
   $global:HttpTimeoutFromFile = $result["HttpTimeout"]

   $global:RequestedNameFromFile = $result["RequestedName"]
   $global:RequestedLevelFromFile = $result["RequestedLevel"]
   $global:RequestedIshvaluetypeFromFile = $result["RequestedIshvaluetype"]

   $global:GroupingNameFromFile = $result["GroupingName"]
   $global:GroupingLevelFromFile = $result["GroupingLevel"] 
   $global:GroupingIshvaluetypeFromFile = $result["GroupingIshvaluetype"]

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

        $result["MappingsCount"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/tms/instances/add/mappings/add").Count

        $result["TemplateCount"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/tms/instances/add/templates/add").Count

        $result["RequestedMetadataCount"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/tms/instances/add/requestedMetadata/ishfields/ishfield").Count

        $result["GroupingMetadataCount"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/tms/instances/add/groupingMetadata/ishfields/ishfield").Count

        return $result
    }

    #read all files that are touched with commandlet
    $result = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockReadArrays -Session $session -ArgumentList $filePath
    
    #get variables and nodes from files

    $global:MappingsCount = $result["MappingsCount"]  
    $global:TemplateCount = $result["TemplateCount"]  
    $global:RequestedMetadataCount = $result["RequestedMetadataCount"]
    $global:GroupingMetadataCount = $result["GroupingMetadataCount"]  
}

Describe "Testing ISHIntegrationTMS"{
    BeforeEach {
        ArtifactCleaner -filePath $filePath -fileName "TranslationOrganizer.exe.config"
        UndoDeploymentBackToVanila $testingDeploymentName $true
    }

    It "Set ISHIntegrationTMS with tricky arrays"{       
        #Act
        $scriptBlockTrickyArrays = {
            param (
                $ishDeployName

            )
            if($PSSenderInfo) {
                $DebugPreference=$Using:DebugPreference
                $VerbosePreference=$Using:VerbosePreference 
            }

            $ishDeploy = Get-ISHDeployment -Name $ishDeployName
            $credential = New-Object System.Management.Automation.PSCredential("SomeUserName", (ConvertTo-SecureString "SomePass" -AsPlainText -Force))
            $mappings = @((New-ISHIntegrationTMSMapping -ISHLanguage en -TmsLanguage "en-GB"), (New-ISHIntegrationTMSMapping -ISHLanguage us -TmsLanguage "en-US"), (New-ISHIntegrationTMSMapping -ISHLanguage ua -TmsLanguage "ua-UA"))

            $templates=@(
                New-ISHIntegrationTMSTemplate -Name "Template1" -TemplateId "81143C38-0C96-4A8C-9BBB-87C1CF464FE3"
                New-ISHIntegrationTMSTemplate -Name "Template2" -TemplateId "70407FBC-86FA-4A9D-8E6D-35E1AE85DB73"
            )

            $requestMetadata=@(
                New-ISHFieldMetadata -Name FAUTHOR -Level lng -ValueType value
                New-ISHFieldMetadata -Name DOC-LANGUAGE -Level lng -ValueType value
            )

            # Create an array of group metadata

            $groupMetadata=@(
                New-ISHFieldMetadata -Name FAUTHOR -Level lng -ValueType value
                New-ISHFieldMetadata -Name DOC-LANGUAGE -Level lng -ValueType value
            )

            Set-ISHIntegrationTMS -ISHDeployment $ishDeploy -Name WorldServer -Uri "https:\\tms1.sd.com" -Credential $credential -MaximumJobSize 5242880 -RetriesOnTimeout 3 -ApiKey "someApiKey" -SecretKey "someSecretKey" -Mappings $mappings -Templates $templates -RequestMetadata $requestMetadata -GroupMetadata $groupMetadata
        }

        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockTrickyArrays -Session $session -ArgumentList $testingDeploymentName
        
        #Assert
        remoteReadReadArrays

        $MappingsCount | Should be 3
        $TemplateCount | Should be 2
        $RequestedMetadataCount| Should be 2
        $GroupingMetadataCount | Should be 2
    }

    It "Set ISHIntegrationTMS with all parameters"{       
        #Act

        $params = @{
            Name=$Name;
            Uri=$Uri;
            Credential=$Credential;
            MaximumJobSize=$MaxJobSize;
            RetriesOnTimeout=$RetriesOnTimeout;
            Mapping=$Mapping;
            Templates = $Template;
            ApiKey = $testAPIKey;
            SecretKey = $testSecretKey;
            RequestMetadata = $requestedMetadata;
            GroupMetadata = $groupingMetadata 
        }
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationTMS -Session $session -ArgumentList $testingDeploymentName, $params
        
        #Assert
        remoteReadTargetXML
        

        $NameFromFile | Should be $Name
        $UriFromFile | Should be $Uri
        $UserNameFromFile | Should be $testUsername
        $PasswordFromFile | Should be $testPassword 
        $RetriesOnTimeutFromFile | Should be $RetriesOnTimeout 
        $MaxJobSizeFromFile | Should be $MaxJobSize

        $TrisoftLanguageFromFile | Should be $TrisoftLanguage 
        $TMSLocaleIDFromFile | Should be $TMSLocaleId

        $TemplateIdFromFile | Should be $testId
        $TemplateNameFromFile | Should be $testName
        
        $ApiKeyFromFile | Should be $testAPIKey
        $SecretFromFile | Should be $testSecretKey

        $RequestedNameFromFile | Should be $requestedMetadataFieldName
        $RequestedLevelFromFile | Should be $requestedMetadataFieldLevel
        $RequestedIshvaluetypeFromFile | Should be $requestedMetadataFieldValueType

        $GroupingNameFromFile | Should be $groupingMetadataFieldName
        $GroupingLevelFromFile | Should be $groupingMetadataFieldLevel
        $GroupingIshvaluetypeFromFile | Should be $groupingMetadataFieldValueType
    }

    It "Set ISHIntegrationTMS with default MaximumJobSize"{       
        #Act

        $params = @{
            Name=$Name;
            Uri=$Uri;
            Credential=$Credential;
            RetriesOnTimeout=$RetriesOnTimeout;
            Mapping=$Mapping;
            Templates = $Template;
            ApiKey = $testAPIKey;
            SecretKey = $testSecretKey;
            RequestMetadata = $requestedMetadata;
            GroupMetadata = $groupingMetadata 
        }
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationTMS -Session $session -ArgumentList $testingDeploymentName, $params
        
        #Assert
        remoteReadTargetXML

        $MaxJobSizeFromFile | Should be 5242880
    }
    
    It "Set ISHIntegrationTMS with HttpTimeout"{       
        #Act

        $params = @{
            Name=$Name;
            Uri=$Uri;
            Credential=$Credential;
            RetriesOnTimeout=$RetriesOnTimeout;
            Mapping=$Mapping;
            Templates = $Template;
            ApiKey = $testAPIKey;
            SecretKey = $testSecretKey;
            HttpTimeout = "00:03:00.000";
            RequestMetadata = $requestedMetadata;
            GroupMetadata = $groupingMetadata 
        }
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationTMS -Session $session -ArgumentList $testingDeploymentName, $params
        
        #Assert
        remoteReadTargetXML

        $HttpTimeoutFromFile | Should be "00:03:00.000"
    }

    It "Set ISHIntegrationTMS with do not update MaximumJobSize if it is not specified"{       
        #Act
        $params = @{
            Name=$Name;
            Uri=$Uri;
            Credential=$Credential;
            MaximumJobSize=$MaxJobSize;
            RetriesOnTimeout=$RetriesOnTimeout;
            Mapping=$Mapping;
            Templates = $Template;
            ApiKey = $testAPIKey;
            SecretKey = $testSecretKey;
            RequestMetadata = $requestedMetadata;
            GroupMetadata = $groupingMetadata 
        }
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationTMS -Session $session -ArgumentList $testingDeploymentName, $params
        
        $params = @{
            Name=$Name;
            Uri=$Uri;
            Credential=$Credential;
            RetriesOnTimeout=$RetriesOnTimeout;
            Mapping=$Mapping;
            Templates = $Template;
            ApiKey = $testAPIKey;
            SecretKey = $testSecretKey;
            RequestMetadata = $requestedMetadata;
            GroupMetadata = $groupingMetadata 
        }
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationTMS -Session $session -ArgumentList $testingDeploymentName, $params
        
        #Assert
        remoteReadTargetXML

        $MaxJobSizeFromFile | Should be $MaxJobSize
    }

    It "Set ISHIntegrationTMS with default RetriesOnTimeout"{       
        #Act

        $params = @{
            Name=$Name;
            Uri=$Uri;
            Credential=$Credential;
            MaximumJobSize=$MaxJobSize;
            Mapping=$Mapping;
            Templates = $Template;
            ApiKey = $testAPIKey;
            SecretKey = $testSecretKey;
            RequestMetadata = $requestedMetadata;
            GroupMetadata = $groupingMetadata 
        }
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationTMS -Session $session -ArgumentList $testingDeploymentName, $params
        
        #Assert
        remoteReadTargetXML
        
        $RetriesOnTimeutFromFile | Should be 3 
    }

    It "Set ISHIntegrationTMS with do not update RetriesOnTimeout if it is not specified"{       
        #Act
        $params = @{
            Name=$Name;
            Uri=$Uri;
            Credential=$Credential;
            MaximumJobSize=$MaxJobSize;
            RetriesOnTimeout=$RetriesOnTimeout;
            Mapping=$Mapping;
            Templates = $Template;
            ApiKey = $testAPIKey;
            SecretKey = $testSecretKey;
            RequestMetadata = $requestedMetadata;
            GroupMetadata = $groupingMetadata 
        }
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationTMS -Session $session -ArgumentList $testingDeploymentName, $params
        
        $params = @{
            Name=$Name;
            Uri=$Uri;
            Credential=$Credential;
            MaximumJobSize=$MaxJobSize;
            Mapping=$Mapping;
            Templates = $Template;
            ApiKey = $testAPIKey;
            SecretKey = $testSecretKey;
            RequestMetadata = $requestedMetadata;
            GroupMetadata = $groupingMetadata 
        }
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationTMS -Session $session -ArgumentList $testingDeploymentName, $params
        
        #Assert
        remoteReadTargetXML
        
        $RetriesOnTimeutFromFile | Should be $RetriesOnTimeout 
    }
 
    It "Set ISHIntegrationTMS with wrong XML"{
        #Arrange
        # Running valid scenario commandlet to out files into backup folder before they will ba manually modified in test
        $params = @{
            Name=$Name;
            Uri=$Uri;
            Credential=$Credential;
            MaximumJobSize=$MaxJobSize;
            RetriesOnTimeout=$RetriesOnTimeout;
            Mapping=$Mapping;
            Templates = $Template;
            ApiKey = $testAPIKey;
            SecretKey = $testSecretKey;
            RequestMetadata = $requestedMetadata;
            GroupMetadata = $groupingMetadata 
        }
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationTMS -Session $session -ArgumentList $testingDeploymentName, $params

        Rename-Item "$filepath\TranslationOrganizer.exe.config"  "_TranslationOrganizer.exe.config"
        New-Item "$filepath\TranslationOrganizer.exe.config" -type file | Out-Null
        
        #Act/Assert
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationTMS -Session $session -ArgumentList $testingDeploymentName, $params -ErrorAction Stop }| Should Throw "Root element is missing"
        #Rollback
        Remove-Item "$filepath\TranslationOrganizer.exe.config"
        Rename-Item "$filepath\_TranslationOrganizer.exe.config" "TranslationOrganizer.exe.config"
    }
    
    It "Set ISHIntegrationTMS with no XML"{
        #Arrange
        Rename-Item "$filepath\TranslationOrganizer.exe.config"  "_TranslationOrganizer.exe.config"

        #Act/Assert
         $params = @{
            Name=$Name;
            Uri=$Uri;
            Credential=$Credential;
            MaximumJobSize=$MaxJobSize;
            RetriesOnTimeout=$RetriesOnTimeout;
            Mapping=$Mapping;
            Templates = $Template;
            ApiKey = $testAPIKey;
            SecretKey = $testSecretKey;
            RequestMetadata = $requestedMetadata;
            GroupMetadata = $groupingMetadata 
        }

        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationTMS -Session $session -ArgumentList $testingDeploymentName, $params -ErrorAction Stop }| Should Throw "Could not find file"
        #Rollback
        Rename-Item "$filepath\_TranslationOrganizer.exe.config" "TranslationOrganizer.exe.config"
    }
    
	It "Remove ISHIntegrationTMS"{       
        #Act

         $params = @{
            Name=$Name;
            Uri=$Uri;
            Credential=$Credential;
            MaximumJobSize=$MaxJobSize;
            RetriesOnTimeout=$RetriesOnTimeout;
            Mapping=$Mapping;
            Templates = $Template;
            ApiKey = $testAPIKey;
            SecretKey = $testSecretKey;
            RequestMetadata = $requestedMetadata;
            GroupMetadata = $groupingMetadata 
        }
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationTMS -Session $session -ArgumentList $testingDeploymentName, $params
        
        

		Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveISHIntegrationTMS -Session $session -ArgumentList $testingDeploymentName
		remoteReadTargetXML
		#Assert
		$NameFromFile | Should be $null
        $UriFromFile | Should be $null
        $UserNameFromFile | Should be $null
        $PasswordFromFile | Should be $null 
        $RetriesOnTimeutFromFile | Should be $null 
        $MaxJobSizeFromFile | Should be $null
        $TrisoftLanguageFromFile | Should be $null 
        $TMSLocaleIDFromFile | Should be $null
    }

    It "Remove ISHIntegrationTMS when no WS integration"{       
        #Act


		{Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveISHIntegrationTMS -Session $session -ArgumentList $testingDeploymentName} | Should not Throw
	
    }

    It "Remove ISHIntegrationTMS with wrong XML"{
        #Arrange
        # Running valid scenario commandlet to out files into backup folder before they will ba manually modified in test
        $params = @{
            Name=$Name;
            Uri=$Uri;
            Credential=$Credential;
            MaximumJobSize=$MaxJobSize;
            RetriesOnTimeout=$RetriesOnTimeout;
            Mapping=$Mapping;
            Templates = $Template;
            ApiKey = $testAPIKey;
            SecretKey = $testSecretKey;
            RequestMetadata = $requestedMetadata;
            GroupMetadata = $groupingMetadata 
        }
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationTMS -Session $session -ArgumentList $testingDeploymentName, $params

        Rename-Item "$filepath\TranslationOrganizer.exe.config"  "_TranslationOrganizer.exe.config"
        New-Item "$filepath\TranslationOrganizer.exe.config" -type file |Out-Null
        
        #Act/Assert
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveISHIntegrationTMS -Session $session -ArgumentList $testingDeploymentName -ErrorAction Stop }| Should Throw "Root element is missing"
        #Rollback
        Remove-Item "$filepath\TranslationOrganizer.exe.config"
        Rename-Item "$filepath\_TranslationOrganizer.exe.config" "TranslationOrganizer.exe.config"
    }

    It "Remove ISHIntegrationTMS with no XML"{
        #Arrange
        Rename-Item "$filepath\TranslationOrganizer.exe.config"  "_TranslationOrganizer.exe.config"

        #Act/Assert
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveISHIntegrationTMS -Session $session -ArgumentList $testingDeploymentName -ErrorAction Stop }| Should Throw "Could not find file"
        #Rollback
        Rename-Item "$filepath\_TranslationOrganizer.exe.config" "TranslationOrganizer.exe.config"
    }

	It "Set ISHIntegrationTMS sets multiple metadata"{

		#Arrange
         $params = @{
            Name=$Name;
            Uri=$Uri;
            Credential=$Credential;
            MaximumJobSize=$MaxJobSize;
            RetriesOnTimeout=$RetriesOnTimeout;
            Mapping=$Mapping;
            Templates = $Template;
            ApiKey = $testAPIKey;
            SecretKey = $testSecretKey;
            RequestMetadata = @($requestedMetadata, $requestedMetadata2);
            GroupMetadata = $groupingMetadata 
        }
        #Act
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationTMS -Session $session -ArgumentList $testingDeploymentName, $params -ErrorAction Stop }| Should Not Throw
        
        #Assert
        remoteReadReadArrays
        $RequestedMetadataCount| Should be 2
    }
    <#
    It "Set ISHIntegrationTMS writes proper history"{        
       #Act
       $params = @{
            Name=$Name;
            Uri=$Uri;
            Credential=$Credential;
            MaximumJobSize=$MaxJobSize;
            RetriesOnTimeout=$RetriesOnTimeout;
            Mapping=$Mapping;
            Templates = $Template;
            ApiKey = $testAPIKey;
            SecretKey = $testSecretKey;
            DestinationPortNumber =$DestinationPortNumber;
            IsapiFilterLocation = $IsapiFilterLocation;
            UseCompression = $UseCompression;
            UseSsl =$UseSsl;
            UseDefaultProxyCredentials = $UseDefaultProxyCredentials;
            ProxyServer = $ProxyServer;
            ProxyPort = $ProxyPort;
            RequestMetadata = $requestedMetadata;
            GroupMetadata = $groupingMetadata 
        }
        
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationTMS -Session $session -ArgumentList $testingDeploymentName, $params
        $history = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetHistory -Session $session -ArgumentList $testingDeploymentName

        #Assert
        $history.Contains('Set-ISHIntegrationTMS -ISHDeployment $deploymentName -MaximumJobSize 250 -Name "testName" -Credential (New-Object System.Management.Automation.PSCredential ("testUserName", (ConvertTo-SecureString "testPassword" -AsPlainText -Force))) -RetriesOnTimeout 2 -Uri "testUri" -Mappings @((New-ISHIntegrationTMSMapping -ISHLanguage en -WSLocaleID 192))') | Should be "True"     
    } 
    #>
     UndoDeploymentBackToVanila $testingDeploymentName $true
}