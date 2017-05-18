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
    
    [System.Xml.XmlDocument]$TrisoftInfoShareClientConfig = new-object System.Xml.XmlDocument
    $TrisoftInfoShareClientConfig.Load("$xmlWebPath\Author\ASP\Trisoft.InfoShare.Client.config" -f $suffix)
     
    [System.Xml.XmlDocument]$STSConfig = new-object System.Xml.XmlDocument
    $STSConfig.Load("$xmlWebPath\InfoShareSTS\Configuration\infoShareSTS.config")

    $result =  @{}
    #get variables and nodes from files
    $result["ClientConfigUsername"] = $TrisoftInfoShareClientConfig.SelectNodes("configuration/trisoft.infoshare.client.settings/datasources/datasource/actor/credentials").username
    $result["ClientConfigPassword"] = $TrisoftInfoShareClientConfig.SelectNodes("configuration/trisoft.infoshare.client.settings/datasources/datasource/actor/credentials").password

    $result["infoShareSTSUsername"] = $STSConfig.SelectNodes("infoShareSTS/initialize").actorUsername

    return $result
}

$scriptBlockSetISHActorUser = {
    param (
        $ishDeployName,
        $credentials

    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }

    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Set-ISHActor -ISHDeployment $ishDeploy -Credential $credentials

}
#endregion

function readTargetXML() {

    #read all files that are touched with commandlet
    $result = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockReadTargetXML -Session $session -ArgumentList $organizerFilePath, $builderFilePath, $xmlAppPath, $xmlDataPath, $xmlPath
    $global:ClientConfigUsername = $result["ClientConfigUsername"]
    $global:ClientConfigPassword = $result["ClientConfigPassword"]

    $global:infoShareSTSUsername = $result["infoShareSTSUsername"]
    

    return $result
}


Describe "Testing ISHActorUser"{
    BeforeEach {
        UndoDeploymentBackToVanila $testingDeploymentName $true
    }

    It "Set ISHActorUser sets user in all files"{

        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHActorUser -Session $session -ArgumentList $testingDeploymentName, $testCreds
        
        #Assert
        readTargetXML

        $ClientConfigUsername | Should be $userName
        $ClientConfigPassword | Should be $userPassword

        $infoShareSTSUsername | Should be $userName
        

    }
    
    It "Set ISHActorUser writes inputparameters"{       
        #Act

        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHActorUser -Session $session -ArgumentList $testingDeploymentName, $testCreds
        $result = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetInputParameters -Session $session -ArgumentList $testingDeploymentName
        $result["serviceusername"] | Should be $userName
    }
    
    It "Set ISHActorUser writes history"{       
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHActorUser -Session $session -ArgumentList $testingDeploymentName, $testCreds
        $history = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetHistory -Session $session -ArgumentList $testingDeploymentName
        $history.Contains('Set-ISHActorUser') | Should be "True"
        $history.Contains("-Credential (New-Object System.Management.Automation.PSCredential") | Should be "True"
    }

     UndoDeploymentBackToVanila $testingDeploymentName $true
}