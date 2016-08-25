param(
    $session = $null,
    $testingDeploymentName = "InfoShare"
)
. "$PSScriptRoot\Common.ps1"

$testCertName = "TestISHAPIWCFServiceCertificate"
$testCertificate = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockCreateCertificate -Session $session
$testEncriptedCertificate = [System.Convert]::ToBase64String($testCertificate.RawData)
$dbPath = ("\\$computerName\{0}\Web{1}\InfoShareSTS\App_Data\IdentityServerConfiguration-2.2.sdf" -f $testingDeployment.Webpath, $suffix).replace(":", "$")
$testThumbprint = $testCertificate.Thumbprint
#region variables

# Generating file pathes to remote PC files
$xmlPath = $testingDeployment.WebPath
$xmlPath = $xmlPath.ToString().replace(":", "$")
$xmlPath = "\\$computerName\$xmlPath"

$filepath = "$xmlPath\Web{0}\Author\ASP" -f $suffix
$absolutePath = $testingDeployment.WebPath
#endregion

#region Script Blocks
$scriptBlockSetISHSTSConfiguration = {
    param (
        $ishDeployName,
        $thumbprint,
        $authenticationMode
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }

    $ishDeploy = Get-ISHDeployment -Name $ishDeployName

    Set-ISHSTSConfiguration -ISHDeployment $ishDeploy -TokenSigningCertificateThumbprint $thumbprint -AuthenticationType $authenticationMode
 
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

$scriptBlockGetWebConfigurationProperty = {
    param (
        [Parameter(Mandatory=$true)]
        $ishDeployName
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    $siteName = $ishDeploy.WebAppNameSTS 
    $result = Get-WebConfigurationProperty -filter /system.WebServer/security/authentication/windowsAuthentication -name enabled -PSPath "IIS:\Sites\Default Web Site\$siteName"

    Return $result.Value

}
#endregion

# Function reads target files and their content, searches for specified nodes in xml
$scriptBlockReadTargetXML = {
    param(
        $thumbprint,
       $authenticationMode,
        $suffix,
        $xmlPath,
        $absolutePath,
        $infosharewswebappname
    )
    #read all files that are touched with commandlet
    [System.Xml.XmlDocument]$ConnectionConfig = new-object System.Xml.XmlDocument
    $ConnectionConfig.load("$xmlPath\Web{0}\InfoShareWS\connectionconfiguration.xml" -f $suffix)
    [System.Xml.XmlDocument]$FeedSDLLCConfig = new-object System.Xml.XmlDocument
    $FeedSDLLCConfig.Load("$xmlPath\Data{0}\PublishingService\Tools\FeedSDLLiveContent.ps1.config" -f $suffix)
    [System.Xml.XmlDocument]$TranslationOrganizerConfig = new-object System.Xml.XmlDocument
    $TranslationOrganizerConfig.Load("$xmlPath\App{0}\TranslationOrganizer\Bin\TranslationOrganizer.exe.config" -f $suffix)
    [System.Xml.XmlDocument]$SynchronizeToLCConfig = new-object System.Xml.XmlDocument
    $SynchronizeToLCConfig.Load("$xmlPath\App{0}\Utilities\SynchronizeToLiveContent\SynchronizeToLiveContent.ps1.config" -f $suffix)
    [System.Xml.XmlDocument]$TrisoftInfoShareClientConfig = new-object System.Xml.XmlDocument
    $TrisoftInfoShareClientConfig.Load("$xmlPath\Web{0}\Author\ASP\Trisoft.InfoShare.Client.config" -f $suffix)
    [System.Xml.XmlDocument]$infoShareSTSConfigThumbprint = new-object System.Xml.XmlDocument
    $infoShareSTSConfigThumbprint.Load("$xmlPath\Web{0}\InfoShareSTS\Configuration\infoShareSTS.config" -f $suffix)

    $result =  @{}
    #get variables and nodes from files
    $result["connectionConfigNode"] = $ConnectionConfig.SelectNodes("connectionconfiguration/issuer/authenticationtype")[0].InnerText
    $result["trisoftInfoShareClientConfigNode"] = $TrisoftInfoShareClientConfig.SelectNodes("configuration/trisoft.infoshare.client.settings/datasources/datasource/issuer/bindingtype").InnerText 
    $result["feedSDLLCConfigNode"] = $FeedSDLLCConfig.SelectNodes("configuration/trisoft.utilities.serviceReferences/serviceUser/issuer").wsTrustBindingType
    $result["translationOrganizerConfigNode"] = $TranslationOrganizerConfig.SelectNodes("configuration/trisoft.utilities.serviceReferences/serviceUser/issuer").wsTrustBindingType
    $result["synchronizeToLCConfigNode"] = $SynchronizeToLCConfig.SelectNodes("configuration/trisoft.utilities.serviceReferences/serviceUser/issuer").wsTrustBindingType  
    $result["infoShareSTSConfigThumbprint"] = $infoShareSTSConfigThumbprint.SelectNodes("infoShareSTS/initialize").certificateThumbprint
   
    return $result
}

function remoteReadTargetXML() {
    param(
        $thumbprint,
       $authenticationMode
    )
    
    $inputParameters = Get-InputParameters $testingDeploymentName
    $infosharewswebappname = $inputParameters["infosharewswebappname"]
    #read all files that are touched with commandlet
    $scriptBlockSetISHIntegrationSTSCertificate
    $result = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockReadTargetXML -Session $session -ArgumentList $thumbprint,$authenticationMode, $suffix, $xmlPath, $absolutePath, $infosharewswebappname

    #get variables and nodes from files

    $global:feedSDLLCConfigNode = $result["feedSDLLCConfigNode"]
    $global:translationOrganizerConfigNode = $result["translationOrganizerConfigNode"]
    $global:synchronizeToLCConfigNode = $result["synchronizeToLCConfigNode"]
    $global:trisoftInfoShareClientConfigNode = $result["trisoftInfoShareClientConfigNode"]
    $global:connectionConfigNode = $result["connectionConfigNode"]
    $global:infoShareSTSConfigThumbprintNode = $result["infoShareSTSConfigThumbprint"]


}



Describe "Testing Set-ISHSTSConfiguration"{
    BeforeEach {
        UndoDeploymentBackToVanila $testingDeploymentName
    }
    
    It "Set-ISHSTSConfiguration enables UserNameMixed "{       
	
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHSTSConfiguration -Session $session -ArgumentList $testingDeploymentName, $testThumbprint, "UsernamePassword"
        #Assert
        remoteReadTargetXML -thumbprint $testThumbprint
        $feedSDLLCConfigNode -eq "UserNameMixed" | Should be $true
        $translationOrganizerConfigNode -eq "UserNameMixed" | Should be $true
        $synchronizeToLCConfigNode -eq "UserNameMixed" | Should be $true
        $trisoftInfoShareClientConfigNode -eq "UserNameMixed" | Should be $true
        $connectionConfigNode -eq "UserNameMixed" | Should be $true
        $infoShareSTSConfigThumbprintNode -eq $testThumbprint | Should be $true
    }

    It "Set-ISHSTSConfiguration enables WindowsMixed "{       
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHSTSConfiguration -Session $session -ArgumentList $testingDeploymentName, $testThumbprint, "Windows"
        #Assert
        remoteReadTargetXML -thumbprint $testThumbprint
        $feedSDLLCConfigNode -eq "WindowsMixed" | Should be $true
        $translationOrganizerConfigNode -eq "WindowsMixed" | Should be $true
        $synchronizeToLCConfigNode -eq "WindowsMixed" | Should be $true
        $trisoftInfoShareClientConfigNode -eq "WindowsMixed" | Should be $true
        $connectionConfigNode -eq "WindowsMixed" | Should be $true 
        $infoShareSTSConfigThumbprintNode -eq $testThumbprint | Should be $true
    }    

     It "Set-ISHSTSConfiguration works if no db "{  
        if (Test-Path $dbPath){ 
            Remove-Item $dbPath
        }     
        #Act
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHSTSConfiguration -Session $session -ArgumentList $testingDeploymentName, $testThumbprint, "Windows"} | Should not Throw 
    }
    
	It "Set-ISHSTSConfiguration shows error when no windows feature "{  
          try {
		  $expectedErrorMessage = "IIS-WindowsAuthentication feature has not been turned on. You can run command: 'Enable-WindowsOptionalFeature -Online -FeatureName IIS-WindowsAuthentication' to enable it"
			Invoke-CommandRemoteOrLocal -ScriptBlock {Disable-WindowsOptionalFeature -Online -FeatureName IIS-WindowsAuthentication} -Session $session 
        #Act
			{Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHSTSConfiguration -Session $session -ArgumentList $testingDeploymentName, $testThumbprint, "Windows"} | Should Throw $expectedErrorMessage
			}
		finally{
			Invoke-CommandRemoteOrLocal -ScriptBlock {Enable-WindowsOptionalFeature -Online -FeatureName IIS-WindowsAuthentication} -Session $session 
		}
    }
	

	It "Undo-ISHDeployment switches windows authentication in iis "{      
        #Act
		Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHSTSConfiguration -Session $session -ArgumentList $testingDeploymentName, $testThumbprint, "Windows"
        $property = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetWebConfigurationProperty -Session $session -ArgumentList $testingDeploymentName
        $property| Should be True

        UndoDeploymentBackToVanila $testingDeploymentName
		$property = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetWebConfigurationProperty -Session $session -ArgumentList $testingDeploymentName
		
		$property| Should be False
        
    }
}

Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveCertificate -Session $session -ArgumentList $testThumbprint