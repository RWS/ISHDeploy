param(
    $session = $null,
    $testingDeploymentName = "InfoShare"
)
. "$PSScriptRoot\Common.ps1"

#region variables
$moduleName = Invoke-CommandRemoteOrLocal -ScriptBlock { (Get-Module "ISHDeploy.*").Name } -Session $session

$xmlPath = $webPath.ToString().replace(":", "$")
$xmlPath = "\\$computerName\$xmlPath"
$filePath = $appPath.ToString().replace(":", "$")
$filePath = "\\$computerName\$filePath"
$filePath = Join-Path $filePath "TranslationBuilder\Bin"

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

$scriptBlockGetISHServiceTranslationBuilder = {
    param (
        $ishDeployName

    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }

    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Get-ISHServiceTranslationBuilder -ISHDeployment $ishDeploy

}


$scriptBlockEnableISHServiceTranslationBuilder = {
    param (
        $ishDeployName

    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }

    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Enable-ISHServiceTranslationBuilder -ISHDeployment $ishDeploy

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
        $history.Contains('Set-ISHServiceTranslationBuilder -ISHDeployment $deploymentName -PendingJobPollingInterval 00:11:11 -CompletedJobLifeSpan 00:01:00 -JobProcessingTimeout 00:11:00 -MaxObjectsInOnePush 500 -MaxJobItemsCreatedInOneCall 250 -JobPollingInterval 00:11:10') | Should be "True"     
    }

    It "Set ISHServiceTranslationBuilder sets amount of services"{
        #Arrange
        $params = @{Count = 3}
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHServiceTranslationBuilder -Session $session -ArgumentList $testingDeploymentName, $params
        $TranslationServices = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceTranslationBuilder -Session $session -ArgumentList $testingDeploymentName
        $TranslationServices.Count | Should be 3

     }

	 It "Set ISHServiceTranslationBuilde downscales amount of services"{
        #Arrange
        $params = @{Count = 3}
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHServiceTranslationBuilder -Session $session -ArgumentList $testingDeploymentName, $params
        $TranslationServices = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceTranslationBuilder -Session $session -ArgumentList $testingDeploymentName
        $TranslationServices.Count | Should be 3
		Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnableISHServiceTranslationBuilder -Session $session -ArgumentList $testingDeploymentName
		$params2 = @{Count = 1}
		Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHServiceTranslationBuilder -Session $session -ArgumentList $testingDeploymentName, $params2
        #Timeout added because of Windows procedure of stopping and removing services
        Start-Sleep -Seconds 20
        $TranslationServices = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceTranslationBuilder -Session $session -ArgumentList $testingDeploymentName
        $TranslationServices.Count | Should be 1
     }
     #For ISH version 12.*
     if($moduleName -like "*12*"){
        It "Set ISHServiceTranslationBuilder changes registry on ISH version 12.*"{
        #Arrange
        $params = @{Count = 2}
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHServiceTranslationBuilder -Session $session -ArgumentList $testingDeploymentName, $params
        $RegistryTranslationBuilderServicePath = "SYSTEM\\CurrentControlSet\\Services\\Trisoft InfoShare$suffix TranslationBuilder Two"
        $localMachineregKey = [Microsoft.Win32.RegistryKey]::OpenRemoteBaseKey('LocalMachine',$computerName)
        [Microsoft.Win32.RegistryKey]$translationBuilderRegKey = $localMachineregKey.OpenSubKey($RegistryTranslationBuilderServicePath);
        $isCOMAplication = $translationBuilderRegKey.GetValue("IS_COM_APPLICATION")
        $isCOMAplication | Should be "InfoShareAuthor$suffix"
        }
     }
     else{
        It "Set ISHServiceTranslationBuilder not changes registry on ISH version 13.*"{
        #Arrange
        $params = @{Count = 2}
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHServiceTranslationBuilder -Session $session -ArgumentList $testingDeploymentName, $params
        $RegistryTranslationBuilderServicePath = "SYSTEM\\CurrentControlSet\\Services\\Trisoft InfoShare$suffix TranslationBuilder Two"
        $localMachineregKey = [Microsoft.Win32.RegistryKey]::OpenRemoteBaseKey('LocalMachine',$computerName)
        [Microsoft.Win32.RegistryKey]$translationBuilderRegKey = $localMachineregKey.OpenSubKey($RegistryTranslationBuilderServicePath);
        $regKeyValues= $translationBuilderRegKey.GetValueNames()
        $regKeyValues -contains "IS_COM_APPLICATION" | Should be false
        }
    }
     UndoDeploymentBackToVanila $testingDeploymentName $true
}