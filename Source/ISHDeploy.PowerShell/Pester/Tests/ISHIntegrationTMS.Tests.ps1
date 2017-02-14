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
$Template = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockNewISHIntegrationTMSTemplate -Session $session -ArgumentList $templateParameters
$fieldName = "testMetadataName"
$fieldLevel = "testMetadatdLevel"
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
    $result["Name"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/TMS/instances/add").alias
    $result["Uri"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/TMS/instances/add").uri
    $result["Username"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/TMS/instances/add").userName
    $result["Password"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/TMS/instances/add").password
    $result["RetriesOnTimeout"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/TMS/instances/add").retriesOnTimeout 
    $result["MaxJobSize"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/TMS/instances/add").externalJobMaxTotalUncompressedSizeBytes
    $result["TrisoftLanguage"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/TMS/instances/add/mappings/add").trisoftLanguage
    $result["TMSLocaleId"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/TMS/instances/add/mappings/add").TMSLocaleId

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
        RetriesOnTimeout=$RetriesOnTimeout
        Mapping=$Mapping
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
    }
   
   It "Set ISHIntegrationTMS with wrong XML"{
        #Arrange
        # Running valid scenario commandlet to out files into backup folder before they will ba manually modified in test
        $params = @{
        Name=$Name;
        Uri=$Uri;
        Credential=$Credential;
        MaximumJobSize=$MaxJobSize;
        RetriesOnTimeout=$RetriesOnTimeout
        Mapping=$Mapping
        }
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationTMS -Session $session -ArgumentList $testingDeploymentName, $params

        Rename-Item "$filepath\TranslationOrganizer.exe.config"  "_TranslationOrganizer.exe.config"
        New-Item "$filepath\TranslationOrganizer.exe.config" -type file |Out-Null
        
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
        RetriesOnTimeout=$RetriesOnTimeout
        Mapping=$Mapping
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
        RetriesOnTimeout=$RetriesOnTimeout
        Mapping=$Mapping
        }
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationTMS -Session $session -ArgumentList $testingDeploymentName, $params
        
        remoteReadTargetXML
        $NameFromFile | Should be $Name
        $UriFromFile | Should be $Uri
        $UserNameFromFile | Should be $testUsername
        $PasswordFromFile | Should be $testPassword 
        $RetriesOnTimeutFromFile | Should be $RetriesOnTimeout 
        $MaxJobSizeFromFile | Should be $MaxJobSize
        $TrisoftLanguageFromFile | Should be $TrisoftLanguage 
        $TMSLocaleIDFromFile | Should be $TMSLocaleId

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
        RetriesOnTimeout=$RetriesOnTimeout
        Mapping=$Mapping
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
    
    It "Set ISHIntegrationTMS writes proper history"{        
       #Act
        $params = @{
        Name=$Name;
        Uri=$Uri;
        Credential=$Credential;
        MaximumJobSize=$MaxJobSize;
        RetriesOnTimeout=$RetriesOnTimeout
        Mapping=$Mapping
        }
        
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationTMS -Session $session -ArgumentList $testingDeploymentName, $params
        $history = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetHistory -Session $session -ArgumentList $testingDeploymentName

        #Assert
        $history.Contains('Set-ISHIntegrationTMS -ISHDeployment $deploymentName -MaximumJobSize 250 -Name "testName" -Credential (New-Object System.Management.Automation.PSCredential ("testUserName", (ConvertTo-SecureString "testPassword" -AsPlainText -Force))) -RetriesOnTimeout 2 -Uri "testUri" -Mappings @((New-ISHIntegrationTMSMapping -ISHLanguage en -WSLocaleID 192))') | Should be "True"     
    } 

     UndoDeploymentBackToVanila $testingDeploymentName $true
}