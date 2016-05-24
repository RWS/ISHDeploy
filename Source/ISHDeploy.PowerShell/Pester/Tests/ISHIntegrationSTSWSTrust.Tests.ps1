param(
    $session = $null,
    $testingDeploymentName = "InfoShare"
)
. "$PSScriptRoot\Common.ps1"

$computerName = If ($session) {$session.ComputerName} Else {$env:COMPUTERNAME}

#region variables
# Script block for getting ISH deployment
$scriptBlockGetDeployment = {
    param (
        [Parameter(Mandatory=$false)]
        $ishDeployName 
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
    Get-ISHDeployment -Name $ishDeployName 
}

# Generating file pathes to remote PC files
$testingDeployment = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetDeployment -Session $session -ArgumentList $testingDeploymentName
$xmlPath = $testingDeployment.WebPath
$xmlPath = $xmlPath.ToString().replace(":", "$")
$xmlPath = "\\$computerName\$xmlPath"
#endregion

#region Script Blocks 
$scriptBlockSetWSTrust = {
    param (
        $ishDeployName,
        $parametersHash,
        $IncludeInternalClients
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }

    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    if($IncludeInternalClients){
        Set-ISHIntegrationSTSWSTrust -ISHDeployment $ishDeploy -IncludeInternalClients @parametersHash
    }
    else{
        Set-ISHIntegrationSTSWSTrust -ISHDeployment $ishDeploy @parametersHash
    }
}

$scriptBlockGetHistory = {
    param (
        [Parameter(Mandatory=$false)]
        $ishDeployName 
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Get-ISHDeploymentHistory -ISHDeployment $ishDeploy
}

#endregion



# Function reads target files and their content, searches for specified nodes in xml
$scriptBlockReadTargetXML = {
    param(
        $xmlPath,
        $testingDeployment
    )
    #read all files that are touched with commandlet
    [System.Xml.XmlDocument]$connectionConfig = new-object System.Xml.XmlDocument
    $connectionConfig.load("$xmlPath\Web{0}\InfoShareWS\connectionconfiguration.xml" -f $testingDeployment.OriginalParameters.projectsuffix)
    [System.Xml.XmlDocument]$feedSDLLiveContentConfig = new-object System.Xml.XmlDocument
    $feedSDLLiveContentConfig.load("$xmlPath\Data{0}\PublishingService\Tools\FeedSDLLiveContent.ps1.config" -f $testingDeployment.OriginalParameters.projectsuffix)
    [System.Xml.XmlDocument]$translationOrganizerConfig = new-object System.Xml.XmlDocument
    $translationOrganizerConfig.load("$xmlPath\App{0}\TranslationOrganizer\Bin\TranslationOrganizer.exe.config" -f $testingDeployment.OriginalParameters.projectsuffix)
    [System.Xml.XmlDocument]$synchronizeToLiveContentConfig = new-object System.Xml.XmlDocument
    $synchronizeToLiveContentConfig.load("$xmlPath\App{0}\Utilities\SynchronizeToLiveContent\SynchronizeToLiveContent.ps1.config" -f $testingDeployment.OriginalParameters.projectsuffix)
    [System.Xml.XmlDocument]$trisoftInfoShareClientConfig = new-object System.Xml.XmlDocument
    $trisoftInfoShareClientConfig.load("$xmlPath\Web{0}\Author\ASP\Trisoft.InfoShare.Client.config" -f $testingDeployment.OriginalParameters.projectsuffix)
    [System.Xml.XmlDocument]$infoShareWSWebConfig = new-object System.Xml.XmlDocument
    $infoShareWSWebConfig.load("$xmlPath\Web{0}\InfoShareWS\Web.config" -f $testingDeployment.OriginalParameters.projectsuffix)

    $result = @{}

    #get variables and nodes from files
    $result["connectionConfigWSTrustBindingType"] = $connectionConfig.SelectNodes("connectionconfiguration/issuer/authenticationtype")[0].InnerText
    $result["connectionConfigWSTrustEndpointUrl"] = $connectionConfig.SelectNodes("connectionconfiguration/issuer/url")[0].InnerText
    $result["feedSDLLiveContentConfigWSTrustIssuerNodeWsTrustEndpoint"] = $feedSDLLiveContentConfig.SelectNodes("configuration/trisoft.utilities.serviceReferences/serviceUser/issuer")[0].wsTrustEndpoint
    $result["feedSDLLiveContentConfigWSTrustIssuerNodeWsTrustBindingType"] = $feedSDLLiveContentConfig.SelectNodes("configuration/trisoft.utilities.serviceReferences/serviceUser/issuer")[0].wsTrustBindingType
    $result["translationOrganizerConfigWSTrustIssuerNodeWsTrustEndpoint"] = $translationOrganizerConfig.SelectNodes("configuration/trisoft.utilities.serviceReferences/serviceUser/issuer")[0].wsTrustEndpoint
    $result["translationOrganizerConfigWSTrustIssuerNodeWsTrustBindingType"] = $translationOrganizerConfig.SelectNodes("configuration/trisoft.utilities.serviceReferences/serviceUser/issuer")[0].wsTrustBindingType
    $result["synchronizeToLiveContentConfigWSTrustIssuerNodeWsTrustEndpoint"] = $synchronizeToLiveContentConfig.SelectNodes("configuration/trisoft.utilities.serviceReferences/serviceUser/issuer")[0].wsTrustEndpoint
    $result["synchronizeToLiveContentConfigWSTrustIssuerNodeWsTrustBindingType"] = $synchronizeToLiveContentConfig.SelectNodes("configuration/trisoft.utilities.serviceReferences/serviceUser/issuer")[0].wsTrustBindingType
    $result["trisoftInfoShareClientConfigWSTrustEndpointUrl"] = $trisoftInfoShareClientConfig.SelectNodes("configuration/trisoft.infoshare.client.settings/datasources/datasource/issuer/uri")[0].InnerText
    $result["trisoftInfoShareClientConfigWSTrustBindingType"] = $trisoftInfoShareClientConfig.SelectNodes("configuration/trisoft.infoshare.client.settings/datasources/datasource/issuer/bindingtype")[0].InnerText
    $result["trisoftInfoShareClientConfigWSTrustActorUserName"] = $trisoftInfoShareClientConfig.SelectNodes("configuration/trisoft.infoshare.client.settings/datasources/datasource/actor/credentials/username")[0].InnerText
    $result["trisoftInfoShareClientConfigWSTrustActorPassword"] = $trisoftInfoShareClientConfig.SelectNodes("configuration/trisoft.infoshare.client.settings/datasources/datasource/actor/credentials/password")[0].InnerText
    $result["infoShareWSWebConfigWSTrustMexEndpointUrlHttp"] = $infoShareWSWebConfig.SelectNodes("configuration/system.serviceModel/bindings/customBinding/binding[@name='InfoShareWS(http)']/security/secureConversationBootstrap/issuedTokenParameters/issuerMetadata")[0].address
    $result["infoShareWSWebConfigWSTrustMexEndpointUrlHttps"] = $infoShareWSWebConfig.SelectNodes("configuration/system.serviceModel/bindings/customBinding/binding[@name='InfoShareWS(https)']/security/secureConversationBootstrap/issuedTokenParameters/issuerMetadata")[0].address
    
    return $result
}
function readTargetXML() {
    
    #read all files that are touched with commandlet
    $result = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockReadTargetXML -Session $session -ArgumentList $xmlPath, $testingDeployment

    #get variables and nodes from files
    $global:connectionConfigWSTrustBindingType = $result["connectionConfigWSTrustBindingType"]
    $global:connectionConfigWSTrustEndpointUrl = $result["connectionConfigWSTrustEndpointUrl"]
    $global:feedSDLLiveContentConfigWSTrustIssuerNodeWsTrustEndpoint = $result["feedSDLLiveContentConfigWSTrustIssuerNodeWsTrustEndpoint"]
    $global:feedSDLLiveContentConfigWSTrustIssuerNodeWsTrustBindingType = $result["feedSDLLiveContentConfigWSTrustIssuerNodeWsTrustBindingType"]
    $global:translationOrganizerConfigWSTrustIssuerNodeWsTrustEndpoint = $result["translationOrganizerConfigWSTrustIssuerNodeWsTrustEndpoint"]
    $global:translationOrganizerConfigWSTrustIssuerNodeWsTrustBindingType = $result["translationOrganizerConfigWSTrustIssuerNodeWsTrustBindingType"]
    $global:synchronizeToLiveContentConfigWSTrustIssuerNodeWsTrustEndpoint = $result["synchronizeToLiveContentConfigWSTrustIssuerNodeWsTrustEndpoint"]
    $global:synchronizeToLiveContentConfigWSTrustIssuerNodeWsTrustBindingType = $result["synchronizeToLiveContentConfigWSTrustIssuerNodeWsTrustBindingType"]
    $global:trisoftInfoShareClientConfigWSTrustEndpointUrl = $result["trisoftInfoShareClientConfigWSTrustEndpointUrl"]
    $global:trisoftInfoShareClientConfigWSTrustBindingType = $result["trisoftInfoShareClientConfigWSTrustBindingType"]
    $global:trisoftInfoShareClientConfigWSTrustActorUserName = $result["trisoftInfoShareClientConfigWSTrustActorUserName"]
    $global:trisoftInfoShareClientConfigWSTrustActorPassword = $result["trisoftInfoShareClientConfigWSTrustActorPassword"]
    $global:infoShareWSWebConfigWSTrustMexEndpointUrlHttp = $result["infoShareWSWebConfigWSTrustMexEndpointUrlHttp"]
    $global:infoShareWSWebConfigWSTrustMexEndpointUrlHttps = $result["infoShareWSWebConfigWSTrustMexEndpointUrlHttps"]
}


Describe "Testing ISHIntegrationSTSWSTrust"{
    BeforeEach {
        if(RemotePathCheck "$filepath\_Web.config")
        {
            if (RemotePathCheck "$filepath\Web.config")
            {
                RemoteRemoveItem "$filepath\Web.config"
            }
            RemoteRenameItem "$filepath\_Web.config" "Web.config"
        }
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockUndoDeploymentWithoutRestartingAppPools -Session $session -ArgumentList $testingDeploymentName
    }

    It "Set ISHIntegrationSTSWSTrust with full parameters"{
        #Arrange
        $params = @{Endpoint = "testEndpoint"; MexEndpoint = "testMexEndpoint"; BindingType  = "UserNameMixed"; ActorUsername = "testActorUsername"; ActorPassword = "testActorPassword"}
        
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetWSTrust -Session $session -ArgumentList $testingDeploymentName, $params, $true
        
        #Assert
        readTargetXML

        $connectionConfigWSTrustBindingType | Should be "UserNameMixed"
        $connectionConfigWSTrustEndpointUrl | Should be "testEndpoint"

        $feedSDLLiveContentConfigWSTrustIssuerNodeWsTrustEndpoint | Should be "testEndpoint"
        $feedSDLLiveContentConfigWSTrustIssuerNodeWsTrustBindingType | Should be "UserNameMixed"
    
        $translationOrganizerConfigWSTrustIssuerNodeWsTrustEndpoint | Should be "testEndpoint"
        $translationOrganizerConfigWSTrustIssuerNodeWsTrustBindingType | Should be "UserNameMixed"
    
        $synchronizeToLiveContentConfigWSTrustIssuerNodeWsTrustEndpoint | Should be "testEndpoint"
        $synchronizeToLiveContentConfigWSTrustIssuerNodeWsTrustBindingType | Should be "UserNameMixed"

        $trisoftInfoShareClientConfigWSTrustEndpointUrl | Should be "testEndpoint"
        $trisoftInfoShareClientConfigWSTrustBindingType | Should be "UserNameMixed"
        $trisoftInfoShareClientConfigWSTrustActorUserName | Should be "testActorUsername"
        $trisoftInfoShareClientConfigWSTrustActorPassword | Should be "testActorPassword"
    
        $infoShareWSWebConfigWSTrustMexEndpointUrlHttp | Should be "testMexEndpoint"
        $infoShareWSWebConfigWSTrustMexEndpointUrlHttps | Should be "testMexEndpoint"
    }

    It "Set ISHIntegrationSTSWSTrust with mandatory parameters"{
        #Arrange
        $params = @{Endpoint = "testEndpoint"; MexEndpoint = "testMexEndpoint"; BindingType  = "UserNameMixed"}
        
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetWSTrust -Session $session -ArgumentList $testingDeploymentName, $params
        
        #Assert
        readTargetXML

        $connectionConfigWSTrustBindingType | Should be "UserNameMixed"
        $connectionConfigWSTrustEndpointUrl | Should be "testEndpoint"

        $infoShareWSWebConfigWSTrustMexEndpointUrlHttp | Should be "testMexEndpoint"
        $infoShareWSWebConfigWSTrustMexEndpointUrlHttps | Should be "testMexEndpoint"
    }

    It "Set ISHIntegrationSTSWSTrust with wrong XML"{
        #Arrange
        $filepath = "$xmlPath\Web{0}\InfoShareWS" -f $testingDeployment.OriginalParameters.projectsuffix
        $params = @{Endpoint = "test"; MexEndpoint = "test"; BindingType  = "UserNameMixed"}
        # Running valid scenario commandlet to out files into backup folder before they will ba manually modified in test
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetWSTrust -Session $session -ArgumentList $testingDeploymentName, $params
        Rename-Item "$filepath\Web.config"  "_Web.config"
        New-Item "$filepath\Web.config" -type file |Out-Null
        
        #Act/Assert
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetWSTrust -Session $session -ArgumentList $testingDeploymentName, $params -ErrorAction Stop }| Should Throw "Root element is missing"
        #Rollback
        Remove-Item "$filepath\Web.config"
        Rename-Item "$filepath\_Web.config" "Web.config"
    }

    It "Set ISHIntegrationSTSWSTrust with no XML"{
        #Arrange
        $filepath = "$xmlPath\Web{0}\InfoShareWS" -f $testingDeployment.OriginalParameters.projectsuffix
        $params = @{Endpoint = "test"; MexEndpoint = "test"; BindingType  = "UserNameMixed"}
        Rename-Item "$filepath\Web.config"  "_Web.config"

        #Act/Assert
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetWSTrust -Session $session -ArgumentList $testingDeploymentName, $params -ErrorAction Stop }| Should Throw "Could not find file"
        #Rollback
        Rename-Item "$filepath\_Web.config" "Web.config"
    }

    It "Set ISHIntegrationSTSWSTrust several times"{
        #Arrange
        $params = @{Endpoint = "testEndpoint"; MexEndpoint = "testMexEndpoint"; BindingType  = "UserNameMixed"}
        
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetWSTrust -Session $session -ArgumentList $testingDeploymentName, $params
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetWSTrust -Session $session -ArgumentList $testingDeploymentName, $params} | Should not Throw
        $params2 = @{Endpoint = "testEndpoint2"; MexEndpoint = "testMexEndpoint2"; BindingType  = "WindowsMixed"}
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetWSTrust -Session $session -ArgumentList $testingDeploymentName, $params2} | Should not Throw
        
        #Assert
        readTargetXML

        $connectionConfigWSTrustBindingType | Should be "WindowsMixed"
        $connectionConfigWSTrustEndpointUrl | Should be "testEndpoint2"
    
        $infoShareWSWebConfigWSTrustMexEndpointUrlHttp | Should be "testMexEndpoint2"
        $infoShareWSWebConfigWSTrustMexEndpointUrlHttps | Should be "testMexEndpoint2"
    }

    It "Set ISHIntegrationSTSWSTrust without -IncludeInternalClients parameter"{
        #Arrange
        $params = @{Endpoint = "testEndpoint"; MexEndpoint = "testMexEndpoint"; BindingType  = "UserNameMixed"; ActorUsername = "testActorUsername"; ActorPassword = "testActorPassword"}
        
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetWSTrust -Session $session -ArgumentList $testingDeploymentName, $params, $true

        $params = @{Endpoint = "testEndpoint2"; MexEndpoint = "testMexEndpoint2"; BindingType  = "WindowsMixed"; ActorUsername = "testActorUsername2"; ActorPassword = "testActorPassword2"}
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetWSTrust -Session $session -ArgumentList $testingDeploymentName, $params, $false
        
        #Assert
        readTargetXML

        $connectionConfigWSTrustBindingType | Should be "WindowsMixed"
        $connectionConfigWSTrustEndpointUrl | Should be "testEndpoint2"

        $feedSDLLiveContentConfigWSTrustIssuerNodeWsTrustEndpoint | Should be "testEndpoint"
        $feedSDLLiveContentConfigWSTrustIssuerNodeWsTrustBindingType | Should be "UserNameMixed"
    
        $translationOrganizerConfigWSTrustIssuerNodeWsTrustEndpoint | Should be "testEndpoint"
        $translationOrganizerConfigWSTrustIssuerNodeWsTrustBindingType | Should be "UserNameMixed"
    
        $synchronizeToLiveContentConfigWSTrustIssuerNodeWsTrustEndpoint | Should be "testEndpoint"
        $synchronizeToLiveContentConfigWSTrustIssuerNodeWsTrustBindingType | Should be "UserNameMixed"

        $trisoftInfoShareClientConfigWSTrustEndpointUrl | Should be "testEndpoint"
        $trisoftInfoShareClientConfigWSTrustBindingType | Should be "UserNameMixed"
        $trisoftInfoShareClientConfigWSTrustActorUserName | Should be "testActorUsername"
        $trisoftInfoShareClientConfigWSTrustActorPassword | Should be "testActorPassword"
    
        $infoShareWSWebConfigWSTrustMexEndpointUrlHttp | Should be "testMexEndpoint2"
        $infoShareWSWebConfigWSTrustMexEndpointUrlHttps | Should be "testMexEndpoint2"
    }

    It "Set ISHIntegrationSTSWSTrust change only ActorUsername"{
        #Arrange
        $params = @{Endpoint = "testEndpoint"; MexEndpoint = "testMexEndpoint"; BindingType  = "UserNameMixed"; ActorUsername = "testActorUsername"; ActorPassword = "testActorPassword"}
        
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetWSTrust -Session $session -ArgumentList $testingDeploymentName, $params, $true

        $params = @{Endpoint = "testEndpoint2"; MexEndpoint = "testMexEndpoint2"; BindingType  = "WindowsMixed"; ActorUsername = "testActorUsername2"}
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetWSTrust -Session $session -ArgumentList $testingDeploymentName, $params, $true
        
        #Assert
        readTargetXML

        $trisoftInfoShareClientConfigWSTrustActorUserName | Should be "testActorUsername2"
        $trisoftInfoShareClientConfigWSTrustActorPassword | Should be "testActorPassword"
    }

    It "Set ISHIntegrationSTSWSTrust change only ActorPassword"{
        #Arrange
        $params = @{Endpoint = "testEndpoint"; MexEndpoint = "testMexEndpoint"; BindingType  = "UserNameMixed"; ActorUsername = "testActorUsername"; ActorPassword = "testActorPassword"}
        
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetWSTrust -Session $session -ArgumentList $testingDeploymentName, $params, $true

        $params = @{Endpoint = "testEndpoint2"; MexEndpoint = "testMexEndpoint2"; BindingType  = "WindowsMixed"; ActorPassword = "testActorPassword2"}
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetWSTrust -Session $session -ArgumentList $testingDeploymentName, $params, $true
        
        #Assert
        readTargetXML

        $trisoftInfoShareClientConfigWSTrustActorUserName | Should be "testActorUsername"
        $trisoftInfoShareClientConfigWSTrustActorPassword | Should be "testActorPassword2"
    }

    It "Set ISHIntegrationSTSWSTrust set ActorUsername as empty string"{
        #Arrange
        $params = @{Endpoint = "testEndpoint"; MexEndpoint = "testMexEndpoint"; BindingType  = "UserNameMixed"; ActorUsername = "testActorUsername"; ActorPassword = "testActorPassword"}
        
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetWSTrust -Session $session -ArgumentList $testingDeploymentName, $params, $true

        $params = @{Endpoint = "testEndpoint2"; MexEndpoint = "testMexEndpoint2"; BindingType  = "WindowsMixed"; ActorUsername = ""; ActorPassword = "testActorPassword2"}
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetWSTrust -Session $session -ArgumentList $testingDeploymentName, $params, $true
        
        #Assert
        readTargetXML

        $trisoftInfoShareClientConfigWSTrustActorUserName | Should be ""
        $trisoftInfoShareClientConfigWSTrustActorPassword | Should be "testActorPassword2"
    }

    It "Set ISHIntegrationSTSWSTrust set ActorPassword as empty string"{
        #Arrange
        $params = @{Endpoint = "testEndpoint"; MexEndpoint = "testMexEndpoint"; BindingType  = "UserNameMixed"; ActorUsername = "testActorUsername"; ActorPassword = "testActorPassword"}
        
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetWSTrust -Session $session -ArgumentList $testingDeploymentName, $params, $true

        $params = @{Endpoint = "testEndpoint2"; MexEndpoint = "testMexEndpoint2"; BindingType  = "WindowsMixed"; ActorUsername = "testActorUsername2"; ActorPassword = ""}
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetWSTrust -Session $session -ArgumentList $testingDeploymentName, $params, $true
        
        #Assert
        readTargetXML

        $trisoftInfoShareClientConfigWSTrustActorUserName | Should be "testActorUsername2"
        $trisoftInfoShareClientConfigWSTrustActorPassword | Should be ""
    }

    It "Set ISHIntegrationSTSWSTrust set ActorUsername as NULL"{
        #Arrange
        $params = @{Endpoint = "testEndpoint"; MexEndpoint = "testMexEndpoint"; BindingType  = "UserNameMixed"; ActorUsername = "testActorUsername"; ActorPassword = "testActorPassword"}
        
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetWSTrust -Session $session -ArgumentList $testingDeploymentName, $params, $true

        $params = @{Endpoint = "testEndpoint2"; MexEndpoint = "testMexEndpoint2"; BindingType  = "WindowsMixed"; ActorUsername = $null; ActorPassword = "testActorPassword2"}
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetWSTrust -Session $session -ArgumentList $testingDeploymentName, $params, $true
        
        #Assert
        readTargetXML

        $trisoftInfoShareClientConfigWSTrustActorUserName | Should be ""
        $trisoftInfoShareClientConfigWSTrustActorPassword | Should be "testActorPassword2"
    }

    It "Set ISHIntegrationSTSWSTrust set ActorPassword as NULL"{
        #Arrange
        $params = @{Endpoint = "testEndpoint"; MexEndpoint = "testMexEndpoint"; BindingType  = "UserNameMixed"; ActorUsername = "testActorUsername"; ActorPassword = "testActorPassword"}
        
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetWSTrust -Session $session -ArgumentList $testingDeploymentName, $params, $true

        $params = @{Endpoint = "testEndpoint2"; MexEndpoint = "testMexEndpoint2"; BindingType  = "WindowsMixed"; ActorUsername = "testActorUsername2"; ActorPassword = $null}
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetWSTrust -Session $session -ArgumentList $testingDeploymentName, $params, $true
        
        #Assert
        readTargetXML

        $trisoftInfoShareClientConfigWSTrustActorUserName | Should be "testActorUsername2"
        $trisoftInfoShareClientConfigWSTrustActorPassword | Should be ""
    }

    It "Set ISHIntegrationSTSWSTrust writes proper history"{
        #Arrange
        $params = @{Endpoint = "test"; MexEndpoint = "test"; BindingType  = "UserNameMixed"}
        
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetWSTrust -Session $session -ArgumentList $testingDeploymentName, $params
        $history = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetHistory -Session $session -ArgumentList $testingDeploymentName

        #Assert
        $history.Contains('Set-ISHIntegrationSTSWSTrust -ISHDeployment $deployment -Endpoint test -MexEndpoint test -BindingType UserNameMixed') | Should be "True"
    }
}