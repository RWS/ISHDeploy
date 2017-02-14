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
$templateParameters = @{TemplateID = $testId; TemplateName = $testName }
$Template = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockNewISHTMSTemplate -Session $session -ArgumentList $templateParameters
$fieldName = "testMetadataName"
$fieldLevel = "testMetadataLevel"
$fieldValueType = "testMetadataValueType"
$metadataParameters = @{Name =$fieldName; Level=$fieldLevel; ValueType=$fieldValueType} 
$Metadata = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockNewISHFieldMetadata -Session $session -ArgumentList $metadataParameters
$DestinationPortNumber = 445
$IsapiFilterLocation = "testLocation"
$UseCompression = $true
$UseSsl = $true
$UseDefaultProxyCredentials = $true
$ProxyServer = "testserver"
$ProxyPort = 450

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
    $result["DestinationPortNumber"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/tms/instances/add").destinationPortNumber
    $result["IsapiFilterLocation"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/tms/instances/add").isapiFilterLocation
    $result["UseCompression"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/tms/instances/add").useCompression
    $result["UseSsl"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/tms/instances/add").useSsl
    $result["UseDefaultProxyCredentials"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/tms/instances/add").useDefaultProxyCredentials
    $result["ProxyServer"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/tms/instances/add").proxyServer
    $result["ProxyPort"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/tms/instances/add").proxyPort

    $result["TrisoftLanguage"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/tms/instances/add/mappings/add").trisoftLanguage
    $result["tmsLocaleId"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/tms/instances/add/mappings/add").tmsLanguage

    $result["TemplateId"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/tms/instances/add/templates/add").templateId
    $result["TemplateName"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/tms/instances/add/templates/add").templateName

    $result["RequestedName"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/tms/instances/add/requestedMetadata").name
    $result["RequestedLevel"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/tms/instances/add/requestedMetadata").level
    $result["RequestedIshvaluetype"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/tms/instances/add/requestedMetadata").ishvaluetype

    $result["GroupingName"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/tms/instances/add/groupingMetadata").name
    $result["GroupingLevel"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/tms/instances/add/groupingMetadata").level
    $result["GroupingIshvaluetype"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/tms/instances/add/groupingMetadata").ishvaluetype

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
   $global:DestinationPortNumberFromFile =$result["DestinationPortNumber"] 
   $global:IsapiFilterLocationFromFile = $result["IsapiFilterLocation"]
   $global:UseCompressionFromFile = $result["UseCompression"]
   $global:UseSslFromFile = $result["UseSsl"]
   $global:UseDefaultProxyCredentialsFromFile = $result["UseDefaultProxyCredentials"]
   $global:ProxyServerFromFile = $result["ProxyServer"] 
   $global:ProxyPortFromFile = $result["ProxyPort"]

   $global:TrisoftLanguageFromFile = $result["TrisoftLanguage"] 
   $global:TMSLocaleIDFromFile = $result["TMSLocaleId"]

   $global:TemplateIdFromFile = $result["TemplateId"]
   $global:TemplateNameFromFile = $result["TemplateName"] 

   $global:RequestedNameFromFile = $result["RequestedName"]
   $global:RequestedLevelFromFile = $result["RequestedLevel"]
   $global:RequestedIshvaluetypeFromFile = $result["RequestedIshvaluetype"]

   $global:GroupingNameFromFile = $result["GroupingName"]
   $global:GroupingLevelFromFile = $result["GroupingLevel"] 
   $global:GroupingIshvaluetypeFromFile = $result["GroupingIshvaluetype"]

}


Describe "Testing ISHIntegrationTMS"{
    BeforeEach {
        ArtifactCleaner -filePath $filePath -fileName "TranslationOrganizer.exe.config"
        UndoDeploymentBackToVanila $testingDeploymentName $true
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
        DestinationPortNumber =$DestinationPortNumber;
        IsapiFilterLocation = $IsapiFilterLocation;
        UseCompression = $UseCompression;
        UseSsl =$UseSsl;
        UseDefaultProxyCredentials = $UseDefaultProxyCredentials;
        ProxyServer = $ProxyServer;
        ProxyPort = $ProxyPort;
        RequestedMetadata = $Metadata;
        GroupingMetadata = $Metadata 
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
        $DestinationPortNumberFromFile | Should be $DestinationPortNumber
        $IsapiFilterLocationFromFile | Should be $IsapiFilterLocation
        $UseCompressionFromFile | Should be $UseCompression
        $UseSslFromFile | Should be $UseSsl
        $UseDefaultProxyCredentialsFromFile | Should be $UseDefaultProxyCredentials
        $ProxyServerFromFile | Should be $ProxyServer
        $ProxyPortFromFile | Should be $ProxyPort

        $TrisoftLanguageFromFile | Should be $TrisoftLanguage 
        $TMSLocaleIDFromFile | Should be $TMSLocaleId

        $TemplateIdFromFile | Should be $testId
        $TemplateNameFromFile | Should be $testName

        $RequestedNameFromFile | Should be $fieldName
        $RequestedLevelFromFile | Should be $fieldLevel
        $RequestedIshvaluetypeFromFile | Should be $fieldValueType

        $GroupingNameFromFile | Should be $fieldName
        $GroupingLevelFromFile | Should be $fieldLevel
        $GroupingIshvaluetypeFromFile | Should be $fieldValueType
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
            DestinationPortNumber =$DestinationPortNumber;
            IsapiFilterLocation = $IsapiFilterLocation;
            UseCompression = $UseCompression;
            UseSsl =$UseSsl;
            UseDefaultProxyCredentials = $UseDefaultProxyCredentials;
            ProxyServer = $ProxyServer;
            ProxyPort = $ProxyPort;
            RequestedMetadata = $Metadata;
            GroupingMetadata = $Metadata 
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
            DestinationPortNumber =$DestinationPortNumber;
            IsapiFilterLocation = $IsapiFilterLocation;
            UseCompression = $UseCompression;
            UseSsl =$UseSsl;
            UseDefaultProxyCredentials = $UseDefaultProxyCredentials;
            ProxyServer = $ProxyServer;
            ProxyPort = $ProxyPort;
            RequestedMetadata = $Metadata;
            GroupingMetadata = $Metadata 
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
            DestinationPortNumber =$DestinationPortNumber;
            IsapiFilterLocation = $IsapiFilterLocation;
            UseCompression = $UseCompression;
            UseSsl =$UseSsl;
            UseDefaultProxyCredentials = $UseDefaultProxyCredentials;
            ProxyServer = $ProxyServer;
            ProxyPort = $ProxyPort;
            RequestedMetadata = $Metadata;
            GroupingMetadata = $Metadata 
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
            DestinationPortNumber =$DestinationPortNumber;
            IsapiFilterLocation = $IsapiFilterLocation;
            UseCompression = $UseCompression;
            UseSsl =$UseSsl;
            UseDefaultProxyCredentials = $UseDefaultProxyCredentials;
            ProxyServer = $ProxyServer;
            ProxyPort = $ProxyPort;
            RequestedMetadata = $Metadata;
            GroupingMetadata = $Metadata 
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
            DestinationPortNumber =$DestinationPortNumber;
            IsapiFilterLocation = $IsapiFilterLocation;
            UseCompression = $UseCompression;
            UseSsl =$UseSsl;
            UseDefaultProxyCredentials = $UseDefaultProxyCredentials;
            ProxyServer = $ProxyServer;
            ProxyPort = $ProxyPort;
            RequestedMetadata = $Metadata;
            GroupingMetadata = $Metadata 
        }
        
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationTMS -Session $session -ArgumentList $testingDeploymentName, $params
        $history = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetHistory -Session $session -ArgumentList $testingDeploymentName

        #Assert
        $history.Contains('Set-ISHIntegrationTMS -ISHDeployment $deploymentName -MaximumJobSize 250 -Name "testName" -Credential (New-Object System.Management.Automation.PSCredential ("testUserName", (ConvertTo-SecureString "testPassword" -AsPlainText -Force))) -RetriesOnTimeout 2 -Uri "testUri" -Mappings @((New-ISHIntegrationTMSMapping -ISHLanguage en -WSLocaleID 192))') | Should be "True"     
    } 
    #>
     UndoDeploymentBackToVanila $testingDeploymentName $true
}