param(
    $session = $null,
    $testingDeploymentName = "InfoShareSQL"
)

. "$PSScriptRoot\Common.ps1"

#region variables
$xmlPath = $webPath.ToString().replace(":", "$")
$xmlPath = "\\$computerName\$xmlPath"
$filePath = $appPath.ToString().replace(":", "$")
$filePath = "\\$computerName\$filePath"
$organizerFilePath = Join-Path $filePath "TranslationOrganizer\Bin"
$builderFilePath = Join-Path $filePath "TranslationBuilder\Bin"
$xmlAppPath = $appPath.replace(":", "$")
$xmlAppPath = "\\$computerName\$xmlAppPath"
$xmlDataPath = $dataPath.replace(":", "$")
$xmlDataPath = "\\$computerName\$xmlDataPath"

$userName = Get-TestDataValue “testDomainUserName”
$userPassword = Get-TestDataValue "testDomainUserPassword"

$secpasswd = ConvertTo-SecureString “$userPassword” -AsPlainText -Force
$testCreds = New-Object System.Management.Automation.PSCredential ($userName, $secpasswd)
#endregion

#region Script Blocks

# Function reads target files and their content, searches for specified nodes in xml
$scriptBlockReadTargetXML = {
    param(
        $organizerFilePath,
        $builderFilePath,
        $xmlAppPath,
        $xmlDataPath,
        $xmlWebPath
        
    )
    #read all files that are touched with commandlet
    
    [System.Xml.XmlDocument]$OrganizerConfig = new-object System.Xml.XmlDocument
    $OrganizerConfig.load("$organizerFilePath\TranslationOrganizer.exe.config")
    [System.Xml.XmlDocument]$builderConfig = new-object System.Xml.XmlDocument
    $builderConfig.load("$builderFilePath\TranslationBuilder.exe.config")

    [System.Xml.XmlDocument]$feedSDLLiveContentConfig = new-object System.Xml.XmlDocument
    $feedSDLLiveContentConfig.load("$xmlAppPath\Utilities\PublishingService\Tools\FeedSDLLiveContent.ps1.config")
    [System.Xml.XmlDocument]$synchronizeToLiveContentConfig = new-object System.Xml.XmlDocument
    $synchronizeToLiveContentConfig.load("$xmlAppPath\Utilities\SynchronizeToLiveContent\SynchronizeToLiveContent.ps1.config")
    


    $result =  @{}
    #get variables and nodes from files
    $result["feedSDLLiveContentConfigUsername"] = $feedSDLLiveContentConfig.SelectNodes("configuration/trisoft.utilities.serviceReferences/serviceUser/user").username
    $result["feedSDLLiveContentConfigPassword"] = $feedSDLLiveContentConfig.SelectNodes("configuration/trisoft.utilities.serviceReferences/serviceUser/user").password

    $result["synchronizeToLiveContentUsername"] = $synchronizeToLiveContentConfig.SelectNodes("configuration/trisoft.utilities.serviceReferences/serviceUser/user").username
    $result["synchronizeToLiveContentPassword"] = $synchronizeToLiveContentConfig.SelectNodes("configuration/trisoft.utilities.serviceReferences/serviceUser/user").password

    $result["organizerUsername"] = $OrganizerConfig.SelectNodes("configuration/trisoft.utilities.serviceReferences/serviceUser/user").username
    $result["organizerPassword"] = $OrganizerConfig.SelectNodes("configuration/trisoft.utilities.serviceReferences/serviceUser/user").password

    $result["builderUsername"] = $builderConfig.SelectNodes("configuration/trisoft.infoShare.translationBuilder/settings").userName
    $result["builderPassword"] = $builderConfig.SelectNodes("configuration/trisoft.infoShare.translationBuilder/settings").password
    return $result
}

$scriptBlockSetISHServiceUser = {
    param (
        $ishDeployName,
        $credentials

    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }

    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Set-ISHServiceUser -ISHDeployment $ishDeploy -Credential $credentials

}
#endregion

function readTargetXML() {

    #read all files that are touched with commandlet
    $result = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockReadTargetXML -Session $session -ArgumentList $organizerFilePath, $builderFilePath, $xmlAppPath, $xmlDataPath, $xmlWebPath
    $global:feedSDLLiveContentConfigUsername = $result["feedSDLLiveContentConfigUsername"]
    $global:feedSDLLiveContentConfigPassword = $result["feedSDLLiveContentConfigPassword"]

    $global:synchronizeToLiveContentUsername = $result["synchronizeToLiveContentUsername"]
    $global:synchronizeToLiveContentPassword = $result["synchronizeToLiveContentPassword"]

    $global:organizerUsername = $result["organizerUsername"]
    $global:organizerPassword = $result["organizerPassword"]

    $global:builderUsername = $result["builderUsername"]
    $global:builderPassword = $result["builderPassword"]
    return $result

}


Describe "Testing ISHServiceUser"{
    BeforeEach {
        UndoDeploymentBackToVanila $testingDeploymentName $true
    }

    It "Set ISHServiceUser sets user in all files"{

        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHServiceUser -Session $session -ArgumentList $testingDeploymentName, $testCreds
        
        #Assert
        readTargetXML

        $feedSDLLiveContentConfigUsername | Should be $userName
        $feedSDLLiveContentConfigPassword | Should be $userPassword

        $synchronizeToLiveContentUsername | Should be $userName
        $synchronizeToLiveContentPassword | Should be $userPassword

        $organizerUsername | Should be $userName
        $organizerPassword | Should be $userPassword

        $builderUsername | Should be $userName
        $builderPassword | Should be $userPassword
    }
    
    It "Set ISHServiceUser writes inputparameters"{       
        #Act

        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHServiceUser -Session $session -ArgumentList $testingDeploymentName, $testCreds
        $result = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetInputParameters -Session $session -ArgumentList $testingDeploymentName
        $result["serviceusername"] | Should be $userName
    }
    
    It "Set ISHServiceUser writes history"{       
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHServiceUser -Session $session -ArgumentList $testingDeploymentName, $testCreds
        $history = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetHistory -Session $session -ArgumentList $testingDeploymentName
        $history.Contains('Set-ISHServiceUser') | Should be "True"
        $history.Contains("-Credential (New-Object System.Management.Automation.PSCredential") | Should be "True"
    }

     UndoDeploymentBackToVanila $testingDeploymentName $true
}