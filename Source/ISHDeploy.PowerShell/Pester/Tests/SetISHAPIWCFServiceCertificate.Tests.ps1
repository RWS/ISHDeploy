param(
    $session = $null,
    $testingDeploymentName = "InfoShare"
)
. "$PSScriptRoot\Common.ps1"

$testCertName = "TestISHAPIWCFServiceCertificate"
$testCertificate = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockCreateCertificate -Session $session
$testEncriptedCertificate = [System.Convert]::ToBase64String($testCertificate.RawData)
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
$scriptBlockSetISHIntegrationSTSCertificate = {
    param (
        $ishDeployName,
        $thumbprint,
        $validationMode
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }

    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Set-ISHAPIWCFServiceCertificate -ISHDeployment $ishDeploy -Thumbprint $thumbprint -ValidationMode $validationMode
 
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
        $thumbprint,
        $ValidationMode,
        $suffix,
        $xmlPath,
        $absolutePath,
        $infosharewswebappname
    )
    #read all files that are touched with commandlet
    
    [System.Xml.XmlDocument]$wsWebConfig = new-object System.Xml.XmlDocument
    $wsWebConfig.load("$xmlPath\Web{0}\InfoShareWS\Web.config" -f $suffix)
    [System.Xml.XmlDocument]$authorWebConfig = new-object System.Xml.XmlDocument
    $authorWebConfig.load("$xmlPath\Web{0}\Author\ASP\Web.config" -f $suffix)
    [System.Xml.XmlDocument]$STSConfig = new-object System.Xml.XmlDocument
    $STSConfig.Load("$xmlPath\Web{0}\InfoShareSTS\Configuration\infoShareSTS.config" -f $suffix)
    [System.Xml.XmlDocument]$FeedSDLLCConfig = new-object System.Xml.XmlDocument
    $FeedSDLLCConfig.Load("$xmlPath\Data{0}\PublishingService\Tools\FeedSDLLiveContent.ps1.config" -f $suffix)
    [System.Xml.XmlDocument]$TranslationOrganizerConfig = new-object System.Xml.XmlDocument
    $TranslationOrganizerConfig.Load("$xmlPath\App{0}\TranslationOrganizer\Bin\TranslationOrganizer.exe.config" -f $suffix)
    [System.Xml.XmlDocument]$SynchronizeToLCConfig = new-object System.Xml.XmlDocument
    $SynchronizeToLCConfig.Load("$xmlPath\App{0}\Utilities\SynchronizeToLiveContent\SynchronizeToLiveContent.ps1.config" -f $suffix)
    [System.Xml.XmlDocument]$TrisoftInfoShareClientConfig = new-object System.Xml.XmlDocument
    $TrisoftInfoShareClientConfig.Load("$xmlPath\Web{0}\Author\ASP\Trisoft.InfoShare.Client.config" -f $suffix)
    [System.Xml.XmlDocument]$ConnectionConfig = new-object System.Xml.XmlDocument
    $ConnectionConfig.Load("$xmlPath\Web{0}\InfoShareWS\connectionconfiguration.xml" -f $suffix)
    
    $result =  @{}
    #get variables and nodes from files
    $result["wsWebConfigNodesCount"] = $wsWebConfig.SelectNodes("configuration/system.serviceModel/behaviors/serviceBehaviors/behavior[@name='']/serviceCredentials[@useIdentityConfiguration='true']/serviceCertificate[@findValue='$thumbprint']").Count
    $result["authorWebConfigNodesCount"] = $authorWebConfig.SelectNodes("configuration/system.identityModel.services/federationConfiguration/serviceCertificate/certificateReference[@findValue='$thumbprint']").Count
    $result["stsConfigNodesCount"] = $STSConfig.SelectNodes("infoShareSTS/initialize[@certificateThumbprint='$thumbprint']").Count
    $result["feedSDLLCConfigNodesCount"] = $FeedSDLLCConfig.SelectNodes("configuration/trisoft.utilities.serviceReferences/serviceUser/uri[@infoShareWSServiceCertificateValidationMode='$ValidationMode']").Count
    $result["translationOrganizerConfigNodesCount"] = $TranslationOrganizerConfig.SelectNodes("configuration/trisoft.utilities.serviceReferences/serviceUser/uri[@infoShareWSServiceCertificateValidationMode='$ValidationMode']").Count 
    $result["synchronizeToLCConfigNodesCount"] = $SynchronizeToLCConfig.SelectNodes("configuration/trisoft.utilities.serviceReferences/serviceUser/uri[@infoShareWSServiceCertificateValidationMode='$ValidationMode']").Count   
    $result["trisoftInfoShareClientConfigNode"] = $TrisoftInfoShareClientConfig.SelectNodes("configuration/trisoft.infoshare.client.settings/datasources/datasource/certificatevalidationmode")[0].InnerText 
    $result["connectionConfigNode"] = $ConnectionConfig.SelectNodes("connectionconfiguration/infosharewscertificatevalidationmode")[0].InnerText

    #Create System.Data.SqlServerCe.dll path
    $sqlCEAssemblyPath=[System.IO.Path]::Combine("$absolutePath\Web$suffix\InfoShareSTS\bin","System.Data.SqlServerCe.dll")
    
    #Add SQL Server CE Engine
    $var = [Reflection.Assembly]::LoadFile($sqlCEAssemblyPath)

    #Create Connection String
    [System.String] $dbName="IdentityServerConfiguration-2.2.sdf"
    [System.String] $dbPath="$xmlpath\Web$suffix\InfoShareSTS\App_Data"
    $infoShareSTSDBPath=[System.IO.Path]::Combine($dbPath,$dbName)
    $connectionString="Data Source=$infoShareSTSDBPath;"

    #Prepare Database Connection and Command
	$connection = New-Object "System.Data.SqlServerCe.SqlCeConnection" $connectionString
        
    $existCommand = New-Object "System.Data.SqlServerCe.SqlCeCommand"
	$existCommand.CommandType = [System.Data.CommandType]::Text
	$existCommand.Connection = $connection
    $myServer = $env:COMPUTERNAME + "." + $env:USERDNSDOMAIN
	$parameter=$existCommand.Parameters.Add("@realm","https://$myServer/$infosharewswebappname/Wcf/API20/Folder.svc")
	$existCommand.CommandText = "SELECT EncryptingCertificate FROM RelyingParties WHERE Realm=@realm"

	#Execute Command
	try
	{
		$connection.Open()
		$result["encryptingCertificate"] =$existCommand.ExecuteScalar().ToString()
	}
    catch{
        Write-Host $Error
    }
	finally
	{
		$connection.Close()
		$connection.Dispose()
	}
    return $result
}

function remoteReadTargetXML() {
    param(
        $thumbprint,
        $ValidationMode
    )
    
    $inputParameters = Get-InputParameters $testingDeploymentName
    $infosharewswebappname = $inputParameters["infosharewswebappname"]
    #read all files that are touched with commandlet
    $result = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockReadTargetXML -Session $session -ArgumentList $thumbprint, $ValidationMode, $suffix, $xmlPath, $absolutePath, $infosharewswebappname


    #get variables and nodes from files
    $global:wsWebConfigNodesCount = $result["wsWebConfigNodesCount"]
    $global:authorWebConfigNodesCount = $result["authorWebConfigNodesCount"]
    $global:stsConfigNodesCount = $result["stsConfigNodesCount"]
    $global:feedSDLLCConfigNodesCount = $result["feedSDLLCConfigNodesCount"]
    $global:translationOrganizerConfigNodesCount = $result["translationOrganizerConfigNodesCount"]
    $global:synchronizeToLCConfigNodesCount = $result["synchronizeToLCConfigNodesCount"]
    $global:trisoftInfoShareClientConfigNode = $result["trisoftInfoShareClientConfigNode"]
    $global:connectionConfigNode = $result["connectionConfigNode"]
    $global:encryptingCertificate = $result["encryptingCertificate"]
}



Describe "Testing Set-ISHAPIWCFServiceCertificate"{
    BeforeEach {
        StopPool -projectName $testingDeploymentName
        ArtifactCleaner -filePath $filePath -fileName "web.config"

        UndoDeploymentBackToVanila $testingDeploymentName
    }

    It "Set-ISHAPIWCFServiceCertificate"{       
        #Act
        Start-Sleep -Milliseconds 7000
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationSTSCertificate -Session $session -ArgumentList $testingDeploymentName, $testThumbprint, "PeerOrChainTrust" -WarningVariable Warning
        #Assert
        remoteReadTargetXML -thumbprint $testThumbprint -ValidationMode "PeerOrChainTrust"

        $wsWebConfigNodesCount | Should be 1
        $authorWebConfigNodesCount | Should be 1
        $stsConfigNodesCount | Should be 1
        $feedSDLLCConfigNodesCount | Should be 1
        $translationOrganizerConfigNodesCount | Should be 1
        $synchronizeToLCConfigNodesCount | Should be 1
        $trisoftInfoShareClientConfigNode | Should be "PeerOrChainTrust"
        $connectionConfigNode | Should be "PeerOrChainTrust"
        $Warning | Should be "This cmdlet modified the cookie encryption. All existing browser and client sessions must be recreated." 
        $encryptingCertificate | Should be $testEncriptedCertificate
    }

    It "Set-ISHAPIWCFServiceCertificate with no XML"{
        #Arrange
        Rename-Item "$filepath\Web.config"  "_Web.config"

        #Act/Assert
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationSTSCertificate -Session $session -ArgumentList $testingDeploymentName, $testThumbprint, "PeerOrChainTrust" -ErrorAction Stop }| Should Throw "Could not find file"
        #Rollback
        Rename-Item "$filepath\_Web.config" "Web.config"
    }

    It "Set-ISHAPIWCFServiceCertificate with wrong XML"{
        #Arrange
        # Running valid scenario commandlet to out files into backup folder before they will ba manually modified in test
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationSTSCertificate -Session $session -ArgumentList $testingDeploymentName, $testThumbprint, "PeerOrChainTrust" -WarningVariable Warning
        Rename-Item "$filepath\Web.config"  "_Web.config"
        New-Item "$filepath\Web.config" -type file |Out-Null
        
        #Act/Assert
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationSTSCertificate -Session $session -ArgumentList $testingDeploymentName, $testThumbprint, "PeerOrChainTrust" -ErrorAction Stop}| Should Throw "Root element is missing"
        #Rollback
        Remove-Item "$filepath\Web.config"
        Rename-Item "$filepath\_Web.config" "Web.config"
    }

     It "Set-ISHAPIWCFServiceCertificate writes proper history"{
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationSTSCertificate -Session $session -ArgumentList $testingDeploymentName, $testThumbprint, "PeerOrChainTrust" -WarningVariable Warning
        $history = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetHistory -Session $session -ArgumentList $testingDeploymentName
        
        #Assert
        $history.Contains('Set-ISHAPIWCFServiceCertificate -ISHDeployment $deployment -Thumbprint') | Should be "True"
        $history.Contains('-ValidationMode PeerOrChainTrust') | Should be "True"
        $history.Contains($testThumbprint) | Should be "True"
    }
}

Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveCertificate -Session $session -ArgumentList $testThumbprint