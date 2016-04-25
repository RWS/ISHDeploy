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
$scriptBlockUndoDeployment = {
    param (
        [Parameter(Mandatory=$false)]
        $ishDeployName 
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Undo-ISHDeployment -ISHDeployment $ishDeploy
}

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



# Function reads target files and their content, searches for specified nodes in xm
function readTargetXML() {
    param(
        $endpoint,
        $mex,
        $bindingType,
        $actorUser,
        $actorpass
        
    )
    [xml]$connectionconfiguration = get-content ("$xmlPath\Web{0}\InfoShareWS\connectionconfiguration.xml" -f $testingDeployment.OriginalParameters.projectsuffix )
    [xml]$feedSDLLiveContent = get-content ("$xmlPath\Data{0}\PublishingService\Tools\FeedSDLLiveContent.ps1.config" -f $testingDeployment.OriginalParameters.projectsuffix )       
    [xml]$translationOrganizer = get-content ("$xmlPath\App{0}\TranslationOrganizer\Bin\TranslationOrganizer.exe.config" -f $testingDeployment.OriginalParameters.projectsuffix )   
    [xml]$synchronizeToLiveContent = get-content ("$xmlPath\App{0}\Utilities\SynchronizeToLiveContent\SynchronizeToLiveContent.ps1.config" -f $testingDeployment.OriginalParameters.projectsuffix )  
    [xml]$trisoftInfoShareClientconfig = get-content ("$xmlPath\Web{0}\Author\ASP\Trisoft.InfoShare.Client.config" -f $testingDeployment.OriginalParameters.projectsuffix )  
    [xml]$wsWebConfig = get-content ("$xmlPath\Web{0}\InfoShareWS\Web.config" -f $testingDeployment.OriginalParameters.projectsuffix )  

    $issuerwstrustedendpoint = $connectionconfiguration.connectionconfiguration.issuer.url
    $issuerwstrustbindingtype = $connectionconfiguration.connectionconfiguration.issuer.authenticationtype

    $nodeFeedSDLLiveContent = $feedSDLLiveContent.configuration.'trisoft.utilities.serviceReferences'.serviceUser.issuer | ? {($_.wsTrustBindingType -eq $bindingType) -and ($_.wsTrustEndpoint -eq $endpoint)}

    $nodetranslationOrganizer = $translationOrganizer.configuration.'trisoft.utilities.serviceReferences'.serviceUser.issuer | ? {($_.wsTrustBindingType -eq $bindingType) -and ($_.wsTrustEndpoint -eq $endpoint)}

    $nodeSynchronizeToLiveContent = $synchronizeToLiveContent.configuration.'trisoft.utilities.serviceReferences'.serviceUser.issuer | ? {($_.wsTrustBindingType -eq $bindingType) -and ($_.wsTrustEndpoint -eq $endpoint)}

    $trisoftIssuerUri = $trisoftInfoShareClientconfig.configuration.'trisoft.infoshare.client.settings'.datasources.datasource.issuer.uri
    $trisoftIssuerBindingtype = $trisoftInfoShareClientconfig.configuration.'trisoft.infoshare.client.settings'.datasources.datasource.issuer.bindingtype

    $webConfigMexNodes = $wsWebConfig.configuration.'system.serviceModel'.bindings.customBinding.binding.security.secureConversationBootstrap.issuedTokenParameters.issuerMetadata | ? {$_.address -eq $mex }


    $issueractorusername = $trisoftInfoShareClientconfig.configuration.'trisoft.infoshare.client.settings'.datasources.datasource.actor.credentials.username
    $issueractoruserpass = $trisoftInfoShareClientconfig.configuration.'trisoft.infoshare.client.settings'.datasources.datasource.actor.credentials.password

    $connectionConfigCheck = $false
    if($issuerwstrustedendpoint -eq $endpoint -and $issuerwstrustbindingtype -eq $bindingType){
        $connectionConfigCheck = $true
    }

    $includeCheck = $false
    if($nodeFeedSDLLiveContent -and $nodetranslationOrganizer -and $nodeSynchronizeToLiveContent -and  $trisoftIssuerUri -eq $endpoint -and $trisoftIssuerBindingtype -eq $bindingType){
        $includeCheck = $true
    }

    $actorCheck = $false
    if($issueractorusername -eq $actorUser -and $issueractoruserpass -eq $actorpass){
        $actorCheck = $true
    }

	if($connectionConfigCheck -and $webConfigMexNodes.Count -eq 2 -and $includeCheck -and $actorCheck)
    {
		Return "With Actor And Internal clients"
	}

	if($connectionConfigCheck -and $webConfigMexNodes.Count -eq 2-and !$includeCheck -and !$actorCheck)
    {
		Return $true
	}
    
    Return "endoint $issuerwstrustedendpoint bindrype $issuerwstrustbindingtype node 1 $nodeFeedSDLLiveContent node 2 $nodetranslationOrganizer node 3 $nodeSynchronizeToLiveContent trisofturi $trisoftIssuerUri trisoft bind $trisoftIssuerBindingtype mex $webConfigMexNodes usr $issueractorusernam pass $issueractoruserpass"
}


Describe "Testing ISHIntegrationSTSWSTrust"{
    BeforeEach {
            Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockUndoDeployment -Session $session -ArgumentList $testingDeploymentName
    }

    It "Set ISHIntegrationSTSWSTrust with full parameters"{
        #Arrange
        $params = @{Endpoint = "test"; MexEndpoint = "test"; BindingType  = "UserNameMixed"; ActorUsername = "test"; ActorPassword = "test"}
        
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetWSTrust -Session $session -ArgumentList $testingDeploymentName, $params, $true
        #Assert
        Start-Sleep -Milliseconds 7000
        readTargetXML -endpoint "test" -mex "test" -bindingType "UserNameMixed" -actorUser "test" -actorpass "test" | Should be "With Actor And Internal clients"
       
    }

    It "Set ISHIntegrationSTSWSTrust not with full parameters"{
        #Arrange
        $params = @{Endpoint = "test"; MexEndpoint = "test"; BindingType  = "UserNameMixed"}
        
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetWSTrust -Session $session -ArgumentList $testingDeploymentName, $params
        #Assert
        Start-Sleep -Milliseconds 7000
        readTargetXML -endpoint "test" -mex "test" -bindingType "UserNameMixed" -actorUser "test" -actorpass "test" | Should be $true
       
    }

    It "Set ISHIntegrationSTSWSTrust with wrong XML"{
        #Arrange
        $filepath = "$xmlPath\Web{0}\InfoShareWS" -f $testingDeployment.OriginalParameters.projectsuffix
        $params = @{Endpoint = "test"; MexEndpoint = "test"; BindingType  = "UserNameMixed"}
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetWSTrust -Session $session -ArgumentList $testingDeploymentName, $params
        Rename-Item "$filepath\Web.config"  "_web.config"
        New-Item "$filepath\Web.config" -type file |Out-Null
        
        #Act/Assert
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetWSTrust -Session $session -ArgumentList $testingDeploymentName, $params -ErrorAction Stop }| Should Throw "Root element is missing"
        #Rollback
        Remove-Item "$filepath\Web.config"
        Rename-Item "$filepath\_Web.config" "web.config"
    }

    It "Set ISHIntegrationSTSWSTrust with no XML"{
        #Arrange
        $filepath = "$xmlPath\Web{0}\InfoShareWS" -f $testingDeployment.OriginalParameters.projectsuffix
        $params = @{Endpoint = "test"; MexEndpoint = "test"; BindingType  = "UserNameMixed"}
        Rename-Item "$filepath\Web.config"  "_web.config"

        #Act/Assert
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetWSTrust -Session $session -ArgumentList $testingDeploymentName, $params -ErrorAction Stop }| Should Throw "Could not find file"
        #Rollback
        Rename-Item "$filepath\_Web.config" "web.config"
    }

    It "Set ISHIntegrationSTSWSTrust several times"{
        #Arrange
        $params = @{Endpoint = "test"; MexEndpoint = "test"; BindingType  = "UserNameMixed"}
        
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetWSTrust -Session $session -ArgumentList $testingDeploymentName, $params
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetWSTrust -Session $session -ArgumentList $testingDeploymentName, $params} | Should not Throw
        #Assert
        Start-Sleep -Milliseconds 7000
        readTargetXML -endpoint "test" -mex "test" -bindingType "UserNameMixed" -actorUser "test" -actorpass "test" | Should be $true
       
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