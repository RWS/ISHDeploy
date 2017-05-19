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

$webAppCMName =  $testingDeployment.WebAppNameCM
$webAppWSName =  $testingDeployment.WebAppNameWS
$webAppSTSName =  $testingDeployment.WebAppNameSTS
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
    $feedSDLLiveContentConfig.load("$xmlDataPath\PublishingService\Tools\FeedSDLLiveContent.ps1.config")
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

$scriptBlockEnableISHIISAppPool = {
    param (
        $ishDeployName
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Enable-ISHIISAppPool -ISHDeployment $ishDeploy
}

$scriptBlockDisableISHIISAppPool = {
    param (
        $ishDeployName
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Disable-ISHIISAppPool -ISHDeployment $ishDeploy
}

$scriptBlockGetISHComponent = {
    param (
        $ishDeployName

    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }

    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Get-ISHComponent -ISHDeployment $ishDeploy

}

$scriptBlockGetAppPoolState = {
    param (
        $testingDeployment,
        $webAppCMName,
        $webAppWSName,
        $webAppSTSName
    )

    $cmAppPoolName = ("TrisoftAppPool{0}" -f $webAppCMName)
    $wsAppPoolName = ("TrisoftAppPool{0}" -f $webAppWSName)
    $stsAppPoolName = ("TrisoftAppPool{0}" -f $webAppSTSName)
    
    $result = @{}

    [Array]$array = iex 'C:\Windows\system32\inetsrv\appcmd list wps'
    foreach ($line in $array) {
            $splitedLine = $line.split(" ")
            $processIdAsString = $splitedLine[1]
            $processId = $processIdAsString.Substring(1,$processIdAsString.Length-2)
            if ($splitedLine[2] -match $cmAppPoolName)
            {
                $result["CM"] = "$cmAppPoolName started 1"
            } 
            if ($splitedLine[2] -match $wsAppPoolName)
            {
                $result["WS"] = "$wsAppPoolName started 2"
            }
            if ($splitedLine[2] -match $stsAppPoolName)
            {
                $result["STS"] = "$stsAppPoolName started 3"
            }
        }
    return $result
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

    It "Disable ISHIISAppPool disables AppPools"{

        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisableISHIISAppPool -Session $session -ArgumentList $testingDeploymentName
         #Assert
        $comp = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHComponent -Session $session -ArgumentList $testingDeploymentName
        ($comp | Where-Object -Property Name -EQ "CM").IsEnabled | Should be "False"
        ($comp | Where-Object -Property Name -EQ "WS").IsEnabled | Should be "False"
        ($comp | Where-Object -Property Name -EQ "STS").IsEnabled | Should be "False"

        $pools = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetAppPoolState -Session $session -ArgumentList $testingDeploymentName, $webAppCMName, $webAppWSName, $webAppSTSName
        $pools.Count | Should be 0
    }
    
    It "Enable ISHIISAppPool enables AppPools"{

        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisableISHIISAppPool -Session $session -ArgumentList $testingDeploymentName
         #Assert
        $comp = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHComponent -Session $session -ArgumentList $testingDeploymentName
        ($comp | Where-Object -Property Name -EQ "CM").IsEnabled | Should be "False"
        ($comp | Where-Object -Property Name -EQ "WS").IsEnabled | Should be "False"
        ($comp | Where-Object -Property Name -EQ "STS").IsEnabled | Should be "False"

        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnableISHIISAppPool -Session $session -ArgumentList $testingDeploymentName
         #Assert
        $comp = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHComponent -Session $session -ArgumentList $testingDeploymentName
        ($comp | Where-Object -Property Name -EQ "CM").IsEnabled | Should be "True"
        ($comp | Where-Object -Property Name -EQ "WS").IsEnabled | Should be "True"
        ($comp | Where-Object -Property Name -EQ "STS").IsEnabled | Should be "True"
        
        $pools = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetAppPoolState -Session $session -ArgumentList $testingDeploymentName, $webAppCMName, $webAppWSName, $webAppSTSName
        $pools.Count | Should be 3
    }
    
    It "Set ISHServiceUser writes history"{       
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnableISHIISAppPool -Session $session -ArgumentList $testingDeploymentName, $testCreds
        $history = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetHistory -Session $session -ArgumentList $testingDeploymentName
        $history.Contains('Enable-ISHIISAppPool') | Should be "True"

    }
    #>
     UndoDeploymentBackToVanila $testingDeploymentName $true
}