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
$WorldServerLocaleId = 192
$TrisoftLanguage = "en"
$mappingParameters = @{ISHLanguage = $TrisoftLanguage; WSLocaleID = $WorldServerLocaleId}
$Mapping = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockNewISHIntegrationWorldServerMapping -Session $session -ArgumentList $mappingParameters
#endregion

#region Script Blocks
$scriptBlockSetISHIntegrationWorldServer = {
    param (
        $ishDeployName,
        $parameters

    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }

    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Set-ISHIntegrationWorldServer -ISHDeployment $ishDeploy @parameters

}

$scriptBlockRemoveISHIntegrationWorldServer = {
    param (
        $ishDeployName

    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }

    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Remove-ISHIntegrationWorldServer -ISHDeployment $ishDeploy 

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
    $result["Name"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/worldServer/instances/add").alias
    $result["Uri"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/worldServer/instances/add").uri
    $result["Username"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/worldServer/instances/add").userName
    $result["Password"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/worldServer/instances/add").password
    $result["RetriesOnTimeout"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/worldServer/instances/add").retriesOnTimeout 
    $result["MaxJobSize"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/worldServer/instances/add").externalJobMaxTotalUncompressedSizeBytes
    $result["TrisoftLanguage"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/worldServer/instances/add/mappings/add").trisoftLanguage
    $result["WorldServerLocaleId"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/worldServer/instances/add/mappings/add").worldServerLocaleId

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
   $global:WorldServerLocaleIDFromFile = $result["WorldServerLocaleId"]

}

Describe "Testing ISHIntegrationWorldServer"{
    BeforeEach {
        ArtifactCleaner -filePath $filePath -fileName "TranslationOrganizer.exe.config"
        UndoDeploymentBackToVanila $testingDeploymentName $true
    }

    It "Set ISHIntegrationWorldServer with all parameters"{       
        #Act

        $params = @{
            Name=$Name;
            Uri=$Uri;
            Credential=$Credential;
            MaximumJobSize=$MaxJobSize;
            RetriesOnTimeout=$RetriesOnTimeout;
            Mapping=$Mapping
        }
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationWorldServer -Session $session -ArgumentList $testingDeploymentName, $params
        
        #Assert
        remoteReadTargetXML
                
        getRemoteComputerName

        $NameFromFile | Should be $Name
        $UriFromFile | Should be $Uri
        $UserNameFromFile | Should be "$RemoteComputerName\$testUsername"
        $PasswordFromFile | Should be $testPassword 
        $RetriesOnTimeutFromFile | Should be $RetriesOnTimeout 
        $MaxJobSizeFromFile | Should be $MaxJobSize
        $TrisoftLanguageFromFile | Should be $TrisoftLanguage 
        $WorldServerLocaleIDFromFile | Should be $WorldServerLocaleId
    }

    It "Set ISHIntegrationWorldServer with default MaximumJobSize"{       
        #Act

        $params = @{
            Name=$Name;
            Uri=$Uri;
            Credential=$Credential;
            RetriesOnTimeout=$RetriesOnTimeout;
            Mapping=$Mapping
        }
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationWorldServer -Session $session -ArgumentList $testingDeploymentName, $params
        
        #Assert
        remoteReadTargetXML

        $MaxJobSizeFromFile | Should be 5242880
    }

    It "Set ISHIntegrationWorldServer with do not update MaximumJobSize if it is not specified"{       
        #Act
        $params = @{
            Name=$Name;
            Uri=$Uri;
            Credential=$Credential;
            MaximumJobSize=$MaxJobSize;
            RetriesOnTimeout=$RetriesOnTimeout;
            Mapping=$Mapping
        }
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationWorldServer -Session $session -ArgumentList $testingDeploymentName, $params
        
        $params = @{
            Name=$Name;
            Uri=$Uri;
            Credential=$Credential;
            RetriesOnTimeout=$RetriesOnTimeout;
            Mapping=$Mapping
        }
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationWorldServer -Session $session -ArgumentList $testingDeploymentName, $params
        
        #Assert
        remoteReadTargetXML

        $MaxJobSizeFromFile | Should be $MaxJobSize
    }

    It "Set ISHIntegrationWorldServer with default RetriesOnTimeout"{       
        #Act

        $params = @{
            Name=$Name;
            Uri=$Uri;
            Credential=$Credential;
            MaximumJobSize=$MaxJobSize;
            Mapping=$Mapping
        }
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationWorldServer -Session $session -ArgumentList $testingDeploymentName, $params
        
        #Assert
        remoteReadTargetXML
        
        $RetriesOnTimeutFromFile | Should be 3 
    }

    It "Set ISHIntegrationWorldServer with do not update RetriesOnTimeout if it is not specified"{       
        #Act
        $params = @{
            Name=$Name;
            Uri=$Uri;
            Credential=$Credential;
            MaximumJobSize=$MaxJobSize;
            RetriesOnTimeout=$RetriesOnTimeout;
            Mapping=$Mapping
        }
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationWorldServer -Session $session -ArgumentList $testingDeploymentName, $params
        
        $params = @{
            Name=$Name;
            Uri=$Uri;
            Credential=$Credential;
            MaximumJobSize=$MaxJobSize;
            Mapping=$Mapping
        }
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationWorldServer -Session $session -ArgumentList $testingDeploymentName, $params
        
        #Assert
        remoteReadTargetXML
        
        $RetriesOnTimeutFromFile | Should be $RetriesOnTimeout 
    }
   
    It "Set ISHIntegrationWorldServer with wrong XML"{
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
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationWorldServer -Session $session -ArgumentList $testingDeploymentName, $params

        Rename-Item "$filepath\TranslationOrganizer.exe.config"  "_TranslationOrganizer.exe.config"
        New-Item "$filepath\TranslationOrganizer.exe.config" -type file |Out-Null
        
        #Act/Assert
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationWorldServer -Session $session -ArgumentList $testingDeploymentName, $params -ErrorAction Stop }| Should Throw "Root element is missing"
        #Rollback
        Remove-Item "$filepath\TranslationOrganizer.exe.config"
        Rename-Item "$filepath\_TranslationOrganizer.exe.config" "TranslationOrganizer.exe.config"
    }

    It "Set ISHIntegrationWorldServer with no XML"{
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

        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationWorldServer -Session $session -ArgumentList $testingDeploymentName, $params -ErrorAction Stop }| Should Throw "Could not find file"
        #Rollback
        Rename-Item "$filepath\_TranslationOrganizer.exe.config" "TranslationOrganizer.exe.config"
    }

	It "Remove ISHIntegrationWorldServer"{       
        #Act

        $params = @{
        Name=$Name;
        Uri=$Uri;
        Credential=$Credential;
        MaximumJobSize=$MaxJobSize;
        RetriesOnTimeout=$RetriesOnTimeout
        Mapping=$Mapping
        }
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationWorldServer -Session $session -ArgumentList $testingDeploymentName, $params
        
        remoteReadTargetXML
        getRemoteComputerName
        $NameFromFile | Should be $Name
        $UriFromFile | Should be $Uri
        $UserNameFromFile | Should be "$RemoteComputerName\$testUsername"
        $PasswordFromFile | Should be $testPassword 
        $RetriesOnTimeutFromFile | Should be $RetriesOnTimeout 
        $MaxJobSizeFromFile | Should be $MaxJobSize
        $TrisoftLanguageFromFile | Should be $TrisoftLanguage 
        $WorldServerLocaleIDFromFile | Should be $WorldServerLocaleId

		Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveISHIntegrationWorldServer -Session $session -ArgumentList $testingDeploymentName
		remoteReadTargetXML
		#Assert
		$NameFromFile | Should be $null
        $UriFromFile | Should be $null
        $UserNameFromFile | Should be $null
        $PasswordFromFile | Should be $null 
        $RetriesOnTimeutFromFile | Should be $null 
        $MaxJobSizeFromFile | Should be $null
        $TrisoftLanguageFromFile | Should be $null 
        $WorldServerLocaleIDFromFile | Should be $null
    }

    It "Remove ISHIntegrationWorldServer when no WS integration"{       
        #Act


		{Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveISHIntegrationWorldServer -Session $session -ArgumentList $testingDeploymentName} | Should not Throw
	
    }

    It "Remove ISHIntegrationWorldServer with wrong XML"{
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
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationWorldServer -Session $session -ArgumentList $testingDeploymentName, $params

        Rename-Item "$filepath\TranslationOrganizer.exe.config"  "_TranslationOrganizer.exe.config"
        New-Item "$filepath\TranslationOrganizer.exe.config" -type file |Out-Null
        
        #Act/Assert
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveISHIntegrationWorldServer -Session $session -ArgumentList $testingDeploymentName -ErrorAction Stop }| Should Throw "Root element is missing"
        #Rollback
        Remove-Item "$filepath\TranslationOrganizer.exe.config"
        Rename-Item "$filepath\_TranslationOrganizer.exe.config" "TranslationOrganizer.exe.config"
    }

    It "Remove ISHIntegrationWorldServer with no XML"{
        #Arrange
        Rename-Item "$filepath\TranslationOrganizer.exe.config"  "_TranslationOrganizer.exe.config"

        #Act/Assert
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveISHIntegrationWorldServer -Session $session -ArgumentList $testingDeploymentName -ErrorAction Stop }| Should Throw "Could not find file"
        #Rollback
        Rename-Item "$filepath\_TranslationOrganizer.exe.config" "TranslationOrganizer.exe.config"
    }
    <#
    It "Set ISHIntegrationWorldServer writes proper history"{        
       #Act
        $params = @{
        Name=$Name;
        Uri=$Uri;
        Credential=$Credential;
        MaximumJobSize=$MaxJobSize;
        RetriesOnTimeout=$RetriesOnTimeout
        Mapping=$Mapping
        }
        
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationWorldServer -Session $session -ArgumentList $testingDeploymentName, $params
        $history = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetHistory -Session $session -ArgumentList $testingDeploymentName

        #Assert
        $history.Contains('Set-ISHIntegrationWorldServer -ISHDeployment $deploymentName -MaximumJobSize 250 -Name "testName" -Credential (New-Object System.Management.Automation.PSCredential ("testUserName", (ConvertTo-SecureString "testPassword" -AsPlainText -Force))) -RetriesOnTimeout 2 -Uri "testUri" -Mappings @((New-ISHIntegrationWorldServerMapping -ISHLanguage en -WSLocaleID 192))') | Should be "True"     
    } 
	#>
     UndoDeploymentBackToVanila $testingDeploymentName $true
}