param(
    $session = $null,
    $testingDeploymentName = "InfoShare"
)
. "$PSScriptRoot\Common.ps1"

#region variables
$xmlPath = $testingDeployment.WebPath
$xmlPath = $xmlPath.ToString().replace(":", "$")
$xmlPath = "\\$computerName\$xmlPath"

$filePath = Join-Path $appPath "TranslationBuilder\Bin"

$MaxObjectsInOnePush = 500
$MaxJobItemsCreatedInOneCall = 250
$CompletedJobLifeSpan = "0.00:01:00.000"
$JobProcessingTimeout = "00:11:00.000"
$JobPollingInterval = "00:11:10.000"
$PendingJobPollingInterval = "00:11:11.000"
#endregion

#region Script Blocks
$scriptBlockSetISHServiceTranslationBuilder = {
    param (
        $ishDeployName,
        $parameters

    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }

    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Set-ISHServiceTranslationBuilder -ISHDeployment $ishDeploy @parameters

}



#endregion


# Function reads target files and their content, searches for specified nodes in xml
$scriptBlockReadTargetXML = {
    param(
        $MaxObjectsInOnePush,
        $filePath
        
    )
    #read all files that are touched with commandlet
    
    [System.Xml.XmlDocument]$builderConfig = new-object System.Xml.XmlDocument
    $builderConfig.load("$filePath\TranslationBuilder.exe.config")

    
    $result =  @{}
    #get variables and nodes from files
    $result["MaxObjectsInOnePush"] = $builderConfig.SelectNodes("configuration/trisoft.infoShare.translationBuilder/settings").maxObjectsInOnePushTranslation 
    $result["MaxJobItemsCreatedInOneCall"] = $builderConfig.SelectNodes("configuration/trisoft.infoShare.translationBuilder/settings").maxTranslationJobItemsCreatedInOneCall
    $result["CompletedJobLifeSpan"] = $builderConfig.SelectNodes("configuration/trisoft.infoShare.translationBuilder/settings").completedJobLifeSpan 
    $result["JobProcessingTimeout"] = $builderConfig.SelectNodes("configuration/trisoft.infoShare.translationBuilder/settings").jobProcessingTimeout  
    $result["JobPollingInterval"] = $builderConfig.SelectNodes("configuration/trisoft.infoShare.translationBuilder/settings").jobPollingInterval
    $result["PendingJobPollingInterval"] = $builderConfig.SelectNodes("configuration/trisoft.infoShare.translationBuilder/settings").pendingJobPollingInterval

    return $result
}
function remoteReadTargetXML() {

    #read all files that are touched with commandlet
    $result = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockReadTargetXML -Session $session -ArgumentList $MaxObjectsInOnePush, $filePath
    
    #get variables and nodes from files
    $global:MaxObjectsInOnePushFromFile = $result["MaxObjectsInOnePush"]
    $global:MaxJobItemsCreatedInOneCallFromFile = $result["MaxJobItemsCreatedInOneCall"]
    $global:CompletedJobLifeSpanFromFile = $result["CompletedJobLifeSpan"]
    $global:JobProcessingTimeoutFromFile = $result["JobProcessingTimeout"]
    $global:JobPollingIntervalFromFile = $result["JobPollingInterval"]
    $global:PendingJobPollingIntervalFromFile = $result["PendingJobPollingInterval"]

}


Describe "Testing ISHServiceTranslationBuilder"{
    BeforeEach {
        ArtifactCleaner -filePath $filePath -fileName "TranslationBuilder.exe.config"
        UndoDeploymentBackToVanila $testingDeploymentName $true
    }

    It "Set ISHServiceTranslationBuilder with all parameters"{       
        #Act
        $params = @{MaxObjectsInOnePush = $MaxObjectsInOnePush; MaxJobItemsCreatedInOneCall = $MaxJobItemsCreatedInOneCall; CompletedJobLifeSpan = $CompletedJobLifeSpan; JobProcessingTimeout = $JobProcessingTimeout; JobPollingInterval = $JobPollingInterval; PendingJobPollingInterval = $PendingJobPollingInterval}
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHServiceTranslationBuilder -Session $session -ArgumentList $testingDeploymentName, $params
        
        #Assert
        remoteReadTargetXML
        
        $MaxObjectsInOnePushFromFile | Should be $MaxObjectsInOnePush
        $MaxJobItemsCreatedInOneCallFromFile  | Should be $MaxJobItemsCreatedInOneCall
        $CompletedJobLifeSpanFromFile | Should be $CompletedJobLifeSpan
        $JobProcessingTimeoutFromFile | Should be $JobProcessingTimeout
        $JobPollingIntervalFromFile | Should be $JobPollingInterval
        $PendingJobPollingIntervalFromFile | Should be $PendingJobPollingInterval
    }

    It "Set ISHServiceTranslationBuilder with few parameters"{       
        #Act
        $params = @{MaxJobItemsCreatedInOneCall = $MaxJobItemsCreatedInOneCall; JobProcessingTimeout = $JobProcessingTimeout; JobPollingInterval = $JobPollingInterval; PendingJobPollingInterval = $PendingJobPollingInterval}
       
        remoteReadTargetXML
        $tempMaxObjectsInOnePush = $MaxObjectsInOnePushFromFile 
        $tempCompletedJobLifeSpan = $CompletedJobLifeSpanFromFile
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHServiceTranslationBuilder -Session $session -ArgumentList $testingDeploymentName, $params
        
        #Assert
        remoteReadTargetXML
        
        $MaxObjectsInOnePushFromFile | Should be $tempMaxObjectsInOnePush 
        $MaxJobItemsCreatedInOneCallFromFile  | Should be $MaxJobItemsCreatedInOneCall
        $CompletedJobLifeSpanFromFile | Should be $tempCompletedJobLifeSpan
        $JobProcessingTimeoutFromFile | Should be $JobProcessingTimeout
        $JobPollingIntervalFromFile | Should be $JobPollingInterval
        $PendingJobPollingIntervalFromFile | Should be $PendingJobPollingInterval
    }
   
   It "Set ISHServiceTranslationBuilder with wrong XML"{
        #Arrange
        # Running valid scenario commandlet to out files into backup folder before they will ba manually modified in test
        $params = @{MaxObjectsInOnePush = $MaxObjectsInOnePush; MaxJobItemsCreatedInOneCall = $MaxJobItemsCreatedInOneCall; CompletedJobLifeSpan = $CompletedJobLifeSpan; JobProcessingTimeout = $JobProcessingTimeout; JobPollingInterval = $JobPollingInterval; PendingJobPollingInterval = $PendingJobPollingInterval}
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHServiceTranslationBuilder -Session $session -ArgumentList $testingDeploymentName, $params

        Rename-Item "$filepath\TranslationBuilder.exe.config"  "_TranslationBuilder.exe.config"
        New-Item "$filepath\TranslationBuilder.exe.config" -type file |Out-Null
        
        #Act/Assert
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHServiceTranslationBuilder -Session $session -ArgumentList $testingDeploymentName, $params -ErrorAction Stop }| Should Throw "Root element is missing"
        #Rollback
        Remove-Item "$filepath\TranslationBuilder.exe.config"
        Rename-Item "$filepath\_TranslationBuilder.exe.config" "TranslationBuilder.exe.config"
    }

    It "Set ISHServiceTranslationBuilder with no XML"{
        #Arrange
        Rename-Item "$filepath\TranslationBuilder.exe.config"  "_TranslationBuilder.exe.config"

        #Act/Assert
        $params = @{MaxObjectsInOnePush = $MaxObjectsInOnePush; MaxJobItemsCreatedInOneCall = $MaxJobItemsCreatedInOneCall; CompletedJobLifeSpan = $CompletedJobLifeSpan; JobProcessingTimeout = $JobProcessingTimeout; JobPollingInterval = $JobPollingInterval; PendingJobPollingInterval = $PendingJobPollingInterval}
        
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHServiceTranslationBuilder -Session $session -ArgumentList $testingDeploymentName, $params -ErrorAction Stop }| Should Throw "Could not find file"
        #Rollback
        Rename-Item "$filepath\_TranslationBuilder.exe.config" "TranslationBuilder.exe.config"
    }

    It "Set ISHServiceTranslationBuilder writes proper history"{        
       #Act
        $params = @{MaxObjectsInOnePush = $MaxObjectsInOnePush; MaxJobItemsCreatedInOneCall = $MaxJobItemsCreatedInOneCall; CompletedJobLifeSpan = $CompletedJobLifeSpan; JobProcessingTimeout = $JobProcessingTimeout; JobPollingInterval = $JobPollingInterval; PendingJobPollingInterval = $PendingJobPollingInterval}
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHServiceTranslationBuilder -Session $session -ArgumentList $testingDeploymentName, $params
        $history = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetHistory -Session $session -ArgumentList $testingDeploymentName
        
        #Assert
        $history.Contains('Set-ISHServiceTranslationBuilder -ISHDeployment $deploymentName -PendingJobPollingInterval 00:11:11 -JobPollingInterval 00:11:10 -MaxObjectsInOnePush 500 -JobProcessingTimeout 00:11:00 -CompletedJobLifeSpan 00:01:00 -MaxJobItemsCreatedInOneCall 250') | Should be "True"     
    }
}