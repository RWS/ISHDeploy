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

$DumpFolder = "C:\DubpFolder" 
$MaxTranslationJobItemsUpdatedInOneCall = 250 
$SystemTaskInterval = "11:11:00.000" 
$AttemptsBeforeFailOnRetrieval = 2
$UpdateLeasedByPerNumberOfItems = 20
$RetriesOnTimeout = 2
$JobPollingInterval = "00:11:10.000"
$PendingJobPollingInterval = "00:11:11.000"
#endregion

#region Script Blocks
$scriptBlockSetISHServiceTranslationOrganizer = {
    param (
        $ishDeployName,
        $parameters

    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }

    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Set-ISHServiceTranslationOrganizer -ISHDeployment $ishDeploy @parameters

}

$scriptBlockGetISHServiceTranslationOrganizer = {
    param (
        $ishDeployName

    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }

    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Get-ISHServiceTranslationOrganizer -ISHDeployment $ishDeploy

}

$scriptBlockEnableISHServiceTranslationOrganizer = {
    param (
        $ishDeployName

    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }

    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Enable-ISHServiceTranslationOrganizer -ISHDeployment $ishDeploy

}
#endregion


# Function reads target files and their content, searches for specified nodes in xml
$scriptBlockReadTargetXML = {
    param(
        $MaxObjectsInOnePush,
        $filePath
        
    )
    #read all files that are touched with commandlet
    
    [System.Xml.XmlDocument]$OrganizerConfig = new-object System.Xml.XmlDocument
    $OrganizerConfig.load("$filePath\TranslationOrganizer.exe.config")

    
    $result =  @{}
    #get variables and nodes from files
    $result["DumpFolder"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/settings").dumpFolder
    $result["MaxTranslationJobItemsUpdatedInOneCall"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/settings").maxTranslationJobItemsUpdatedInOneCall
    $result["SystemTaskInterval"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/settings").systemTaskInterval
    $result["AttemptsBeforeFailOnRetrieval"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/settings").attemptsBeforeFailOnRetrieval 
    $result["UpdateLeasedByPerNumberOfItems"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/settings").updateLeasedByPerNumberOfItems
    $result["RetriesOnTimeout"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/settings").retriesOnTimeout 
    $result["JobPollingInterval"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/settings").jobPollingInterval
    $result["PendingJobPollingInterval"] = $OrganizerConfig.SelectNodes("configuration/trisoft.infoShare.translationOrganizer/settings").pendingJobPollingInterval

    return $result
}
function remoteReadTargetXML() {

    #read all files that are touched with commandlet
    $result = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockReadTargetXML -Session $session -ArgumentList $MaxObjectsInOnePush, $filePath
    
    #get variables and nodes from files
    $global:DumpFolderFromFile = $result["DumpFolder"]
    $global:MaxTranslationJobItemsUpdatedInOneCallFromFile = $result["MaxTranslationJobItemsUpdatedInOneCall"]
    $global:SystemTaskIntervalFromFile = $result["SystemTaskInterval"]
    $global:AttemptsBeforeFailOnRetrievalFromFile = $result["AttemptsBeforeFailOnRetrieval"]
    
    $global:UpdateLeasedByPerNumberOfItemsFromFile = $result["UpdateLeasedByPerNumberOfItems"]
    $global:RetriesOnTimeoutFromFile = $result["RetriesOnTimeout"]
    $global:JobPollingIntervalFromFile = $result["JobPollingInterval"]
    $global:PendingJobPollingIntervalFromFile = $result["PendingJobPollingInterval"]

}


Describe "Testing ISHServiceTranslationOrganizer"{
    BeforeEach {
        ArtifactCleaner -filePath $filePath -fileName "TranslationOrganizer.exe.config"
        UndoDeploymentBackToVanila $testingDeploymentName $true
    }

    It "Set ISHServiceTranslationOrganizer with all parameters"{       
        #Act

        $params = @{DumpFolder = $DumpFolder;MaxTranslationJobItemsUpdatedInOneCall = $MaxTranslationJobItemsUpdatedInOneCall;SystemTaskInterval = $SystemTaskInterval;AttemptsBeforeFailOnRetrieval = $AttemptsBeforeFailOnRetrieval;UpdateLeasedByPerNumberOfItems = $UpdateLeasedByPerNumberOfItems;RetriesOnTimeout = $RetriesOnTimeout;JobPollingInterval= $JobPollingInterval;PendingJobPollingInterval = $PendingJobPollingInterval}
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHServiceTranslationOrganizer -Session $session -ArgumentList $testingDeploymentName, $params
        
        #Assert
        remoteReadTargetXML
        
        $DumpFolderFromFile | Should be $DumpFolder
        $MaxTranslationJobItemsUpdatedInOneCallFromFile | Should be $MaxTranslationJobItemsUpdatedInOneCall
        $SystemTaskIntervalFromFile | Should be $SystemTaskInterval
        $AttemptsBeforeFailOnRetrievalFromFile | Should be $AttemptsBeforeFailOnRetrieval
        $UpdateLeasedByPerNumberOfItemsFromFile | Should be $UpdateLeasedByPerNumberOfItems
        $RetriesOnTimeoutFromFile | Should be $RetriesOnTimeout
        $JobPollingIntervalFromFile | Should be $JobPollingInterval
        $PendingJobPollingIntervalFromFile | Should be $PendingJobPollingInterval
    }

    It "Set ISHServiceTranslationOrganizer with few parameters"{       
        #Act
        $params = @{MaxTranslationJobItemsUpdatedInOneCall = $MaxTranslationJobItemsUpdatedInOneCall;SystemTaskInterval = $SystemTaskInterval;UpdateLeasedByPerNumberOfItems = $UpdateLeasedByPerNumberOfItems;RetriesOnTimeout = $RetriesOnTimeout;JobPollingInterval= $JobPollingInterval;PendingJobPollingInterval = $PendingJobPollingInterval}
        
        remoteReadTargetXML
        $tempDumpFolder = $DumpFolderFromFile
        $tempAttemptsBeforeFailOnRetrieval = $AttemptsBeforeFailOnRetrievalFromFile
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHServiceTranslationOrganizer -Session $session -ArgumentList $testingDeploymentName, $params
        
        #Assert
        remoteReadTargetXML
        
        $DumpFolderFromFile | Should be $tempDumpFolder
        $MaxTranslationJobItemsUpdatedInOneCallFromFile | Should be $MaxTranslationJobItemsUpdatedInOneCall
        $SystemTaskIntervalFromFile | Should be $SystemTaskInterval
        $AttemptsBeforeFailOnRetrievalFromFile | Should be $tempAttemptsBeforeFailOnRetrieval
        $UpdateLeasedByPerNumberOfItemsFromFile | Should be $UpdateLeasedByPerNumberOfItems
        $RetriesOnTimeoutFromFile | Should be $RetriesOnTimeout
        $JobPollingIntervalFromFile | Should be $JobPollingInterval
        $PendingJobPollingIntervalFromFile | Should be $PendingJobPollingInterval
    }
   
   It "Set ISHServiceTranslationOrganizer with wrong XML"{
        #Arrange
        # Running valid scenario commandlet to out files into backup folder before they will ba manually modified in test
        $params = @{DumpFolder = $DumpFolder;MaxTranslationJobItemsUpdatedInOneCall = $MaxTranslationJobItemsUpdatedInOneCall;SystemTaskInterval = $SystemTaskInterval;AttemptsBeforeFailOnRetrieval = $AttemptsBeforeFailOnRetrieval;UpdateLeasedByPerNumberOfItems = $UpdateLeasedByPerNumberOfItems;RetriesOnTimeout = $RetriesOnTimeout;JobPollingInterval= $JobPollingInterval;PendingJobPollingInterval = $PendingJobPollingInterval}
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHServiceTranslationOrganizer -Session $session -ArgumentList $testingDeploymentName, $params

        Rename-Item "$filepath\TranslationOrganizer.exe.config"  "_TranslationOrganizer.exe.config"
        New-Item "$filepath\TranslationOrganizer.exe.config" -type file |Out-Null
        
        #Act/Assert
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHServiceTranslationOrganizer -Session $session -ArgumentList $testingDeploymentName, $params -ErrorAction Stop }| Should Throw "Root element is missing"
        #Rollback
        Remove-Item "$filepath\TranslationOrganizer.exe.config"
        Rename-Item "$filepath\_TranslationOrganizer.exe.config" "TranslationOrganizer.exe.config"
    }

    It "Set ISHServiceTranslationOrganizer with no XML"{
        #Arrange
        Rename-Item "$filepath\TranslationOrganizer.exe.config"  "_TranslationOrganizer.exe.config"

        #Act/Assert
        $params = @{DumpFolder = $DumpFolder;MaxTranslationJobItemsUpdatedInOneCall = $MaxTranslationJobItemsUpdatedInOneCall;SystemTaskInterval = $SystemTaskInterval;AttemptsBeforeFailOnRetrieval = $AttemptsBeforeFailOnRetrieval;UpdateLeasedByPerNumberOfItems = $UpdateLeasedByPerNumberOfItems;RetriesOnTimeout = $RetriesOnTimeout;JobPollingInterval= $JobPollingInterval;PendingJobPollingInterval = $PendingJobPollingInterval}
        
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHServiceTranslationOrganizer -Session $session -ArgumentList $testingDeploymentName, $params -ErrorAction Stop }| Should Throw "Could not find file"
        #Rollback
        Rename-Item "$filepath\_TranslationOrganizer.exe.config" "TranslationOrganizer.exe.config"
    }

    It "Set ISHServiceTranslationOrganizer writes proper history"{        
       #Act
        $params = @{DumpFolder = $DumpFolder;MaxTranslationJobItemsUpdatedInOneCall = $MaxTranslationJobItemsUpdatedInOneCall;SystemTaskInterval = $SystemTaskInterval;AttemptsBeforeFailOnRetrieval = $AttemptsBeforeFailOnRetrieval;UpdateLeasedByPerNumberOfItems = $UpdateLeasedByPerNumberOfItems;RetriesOnTimeout = $RetriesOnTimeout;JobPollingInterval= $JobPollingInterval;PendingJobPollingInterval = $PendingJobPollingInterval}
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHServiceTranslationOrganizer -Session $session -ArgumentList $testingDeploymentName, $params
        $history = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetHistory -Session $session -ArgumentList $testingDeploymentName

        #Assert
        $history.Contains('Set-ISHServiceTranslationOrganizer -ISHDeployment $deploymentName -PendingJobPollingInterval 00:11:11 -AttemptsBeforeFailOnRetrieval 2 -RetriesOnTimeout 2 -UpdateLeasedByPerNumberOfItems 20 -DumpFolder "C:\DubpFolder" -SystemTaskInterval 11:11:00 -JobPollingInterval 00:11:10 -MaxTranslationJobItemsUpdatedInOneCall 250') | Should be "True"     
    }

    It "Set ISHServiceTranslationOrganizer sets amount of services"{
        #Arrange
        # Running valid scenario commandlet to out files into backup folder before they will ba manually modified in test
        $params = @{Count = 3}
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHServiceTranslationOrganizer -Session $session -ArgumentList $testingDeploymentName, $params
        $TranslationServices = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceTranslationOrganizer -Session $session -ArgumentList $testingDeploymentName
        $TranslationServices.Count | Should be 3

     }
	 It "Set ISHServiceTranslationOrganizer downscales amount of services"{
        #Arrange
        $params = @{Count = 3}
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHServiceTranslationOrganizer -Session $session -ArgumentList $testingDeploymentName, $params
        $TranslationServices = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceTranslationOrganizer -Session $session -ArgumentList $testingDeploymentName
        $TranslationServices.Count | Should be 3
		Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnableISHServiceTranslationOrganizer -Session $session -ArgumentList $testingDeploymentName
		$params2 = @{Count = 2}
		Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHServiceTranslationOrganizer -Session $session -ArgumentList $testingDeploymentName, $params2
        #Timeout added because of Windows procedure of stopping and removing services
        Start-Sleep -Seconds 60
        $TranslationServices = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceTranslationOrganizer -Session $session -ArgumentList $testingDeploymentName
        $TranslationServices.Count | Should be 2
     }
     UndoDeploymentBackToVanila $testingDeploymentName $true
}