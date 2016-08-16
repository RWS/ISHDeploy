param(
    $session = $null,
    $testingDeploymentName = "InfoShare"
)
. "$PSScriptRoot\Common.ps1"

#region variables
$xmlPath = $testingDeployment.WebPath
$xmlPath = $xmlPath.ToString().replace(":", "$")
$xmlPath = "\\$computerName\$xmlPath"

$filepath = "$xmlPath\Web{0}\InfoShareWS" -f $suffix
#endregion

#region Script Blocks
$scriptBlockSetISHIntegrationSTSCertificate = {
    param (
        $ishDeployName,
        $thumbprint,
        $issuer,
        $validationMode
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }

    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Set-ISHIntegrationSTSCertificate -ISHDeployment $ishDeploy -Thumbprint $thumbprint -Issuer $issuer -ValidationMode $validationMode

}

$scriptBlockRemoveISHIntegrationSTSCertificate = {
    param (
        $ishDeployName,
        $issuer
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }

    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Remove-ISHIntegrationSTSCertificate -ISHDeployment $ishDeploy -Issuer $issuer

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

$authorWebConfigFilePath


# Function reads target files and their content, searches for specified nodes in xml
$scriptBlockReadTargetXML = {
    param(
        $Thumbprint,
        $ValidationMode,
        $xmlPath,
        $suffix
    )
    #read all files that are touched with commandlet
    
    [System.Xml.XmlDocument]$authorWebConfig = new-object System.Xml.XmlDocument
    $authorWebConfig.load("$xmlPath\Web{0}\Author\ASP\Web.config" -f $suffix)
    [System.Xml.XmlDocument]$wsWebConfig = new-object System.Xml.XmlDocument
    $wsWebConfig.load("$xmlPath\Web{0}\InfoShareWS\Web.config" -f $suffix)
    [System.Xml.XmlDocument]$stsWebConfig = new-object System.Xml.XmlDocument
    $stsWebConfig.load("$xmlPath\Web{0}\InfoShareSTS\Web.config" -f $suffix )
    
    $result =  @{}
    #get variables and nodes from files
    $result["authorWebConfigNodesCount"] = $authorWebConfig.SelectNodes("configuration/system.identityModel/identityConfiguration/issuerNameRegistry/trustedIssuers/add[@thumbprint='$Thumbprint']").Count
    $result["authorWebConfigIssuer"] = $authorWebConfig.SelectNodes("configuration/system.identityModel/identityConfiguration/issuerNameRegistry/trustedIssuers/add[@thumbprint='$Thumbprint']")[0].name
    $result["authorWebConfigValidationModeCount"] = $authorWebConfig.SelectNodes("configuration/system.identityModel/identityConfiguration/certificateValidation[@certificateValidationMode='$ValidationMode']").Count
    $result["authorWebConfigValidationModeCertificateValidationMode"] = $authorWebConfig.SelectNodes("configuration/system.identityModel/identityConfiguration/certificateValidation[@certificateValidationMode='$ValidationMode']")[0].certificateValidationMode
    $result["wsWebConfigNodesCount"] = $wsWebConfig.SelectNodes("configuration/system.identityModel/identityConfiguration/issuerNameRegistry/trustedIssuers/add[@thumbprint='$Thumbprint']").Count
    $result["wsWebConfigIssuer"] = $wsWebConfig.SelectNodes("configuration/system.identityModel/identityConfiguration/issuerNameRegistry/trustedIssuers/add[@thumbprint='$Thumbprint']")[0].name
    $result["wsWebConfigNodesValidationModeCount"] = $wsWebConfig.SelectNodes("configuration/system.identityModel/identityConfiguration/certificateValidation[@certificateValidationMode='$ValidationMode']").Count
    $result["wsWebConfigNodesValidationModeCertificateValidationMode"] = $wsWebConfig.SelectNodes("configuration/system.identityModel/identityConfiguration/certificateValidation[@certificateValidationMode='$ValidationMode']")[0].certificateValidationMode
    $result["stsWebConfigNodesCount"] = $stsWebConfig.SelectNodes("configuration/system.serviceModel/behaviors/serviceBehaviors/behavior/addActAsTrustedIssuer[@thumbprint='$Thumbprint']").Count

    return $result
}
function remoteReadTargetXML() {
    param(
        $Thumbprint,
        $ValidationMode
    )
    #read all files that are touched with commandlet
    $result = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockReadTargetXML -Session $session -ArgumentList $Thumbprint, $ValidationMode, $xmlPath, $suffix
    
    #get variables and nodes from files
    $global:authorWebConfigNodesCount = $result["authorWebConfigNodesCount"]
    $global:authorWebConfigIssuer = $result["authorWebConfigIssuer"]
    $global:authorWebConfigValidationModeCount = $result["authorWebConfigValidationModeCount"]
    $global:authorWebConfigValidationModeCertificateValidationMode = $result["authorWebConfigValidationModeCertificateValidationMode"]
    $global:wsWebConfigNodesCount = $result["wsWebConfigNodesCount"]
    $global:wsWebConfigIssuer = $result["wsWebConfigIssuer"]
    $global:wsWebConfigNodesValidationModeCount = $result["wsWebConfigNodesValidationModeCount"]
    $global:wsWebConfigNodesValidationModeCertificateValidationMode = $result["wsWebConfigNodesValidationModeCertificateValidationMode"]
    $global:stsWebConfigNodesCount = $result["stsWebConfigNodesCount"]
}


Describe "Testing ISHIntegrationSTSCertificate"{
    BeforeEach {
        ArtifactCleaner -filePath $filePath -fileName "web.config"
        UndoDeploymentBackToVanila $testingDeploymentName $true
    }

    It "Set ISHIntegrationSTSCertificate"{       
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationSTSCertificate -Session $session -ArgumentList $testingDeploymentName, "testThumbprint", "testIssuer", "PeerOrChainTrust" -WarningVariable Warning
        
        #Assert
        remoteReadTargetXML -Thumbprint "testThumbprint" -ValidationMode "PeerOrChainTrust"
        
        $authorWebConfigNodesCount | Should be 1
        $authorWebConfigIssuer | Should be "testIssuer"
        $authorWebConfigValidationModeCount | Should be 1
        $wsWebConfigNodesCount | Should be 1
        $wsWebConfigIssuer | Should be "testIssuer"
        $wsWebConfigNodesValidationModeCount | Should be 1
        $stsWebConfigNodesCount | Should be 0
        $Warning | Should be $null 
    }


    It "Set ISHIntegrationSTSCertificate with wrong XML"{
        #Arrange
        # Running valid scenario commandlet to out files into backup folder before they will ba manually modified in test
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationSTSCertificate -Session $session -ArgumentList $testingDeploymentName, "testThumbprint", "testIssuer", "PeerOrChainTrust"
        Rename-Item "$filepath\Web.config"  "_Web.config"
        New-Item "$filepath\Web.config" -type file |Out-Null
        
        #Act/Assert
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationSTSCertificate -Session $session -ArgumentList $testingDeploymentName, "testThumbprint", "testIssuer", "PeerOrChainTrust" -ErrorAction Stop }| Should Throw "Root element is missing"
        #Rollback
        Remove-Item "$filepath\Web.config"
        Rename-Item "$filepath\_Web.config" "Web.config"
    }

    It "Set ISHIntegrationSTSCertificate with no XML"{
        #Arrange
        Rename-Item "$filepath\Web.config"  "_Web.config"

        #Act/Assert
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationSTSCertificate -Session $session -ArgumentList $testingDeploymentName, "testThumbprint", "testIssuer", "PeerOrChainTrust" -ErrorAction Stop }| Should Throw "Could not find file"
        #Rollback
        Rename-Item "$filepath\_Web.config" "Web.config"
    }

    It "Set ISHIntegrationSTSCertificate several times"{        
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationSTSCertificate -Session $session -ArgumentList $testingDeploymentName, "testThumbprint", "testIssuer", "PeerOrChainTrust"
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationSTSCertificate -Session $session -ArgumentList $testingDeploymentName, "testThumbprint", "testIssuer222", "PeerOrChainTrust"} | Should not Throw
        
        #Assert
        remoteReadTargetXML -Thumbprint "testThumbprint" -ValidationMode "PeerOrChainTrust"
        
        $authorWebConfigNodesCount | Should be 1
        $authorWebConfigValidationModeCount | Should be 1
        $authorWebConfigIssuer | Should be "testIssuer222"
        $wsWebConfigNodesCount | Should be 1
        $wsWebConfigNodesValidationModeCount | Should be 1
        $wsWebConfigIssuer | Should be "testIssuer222"
        $stsWebConfigNodesCount | Should be 0
    }

    It "Set ISHIntegrationSTSCertificate normalizes thumbprint"{       
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationSTSCertificate -Session $session -ArgumentList $testingDeploymentName, "test T h u m b p rint  2", "testIssuerNormalized", "PeerOrChainTrust" -WarningVariable Warning
        
        #Assert
        $Warning | Should Match "has been normalized to 'testThumbprint2'"

        remoteReadTargetXML -Thumbprint "testThumbprint2" -ValidationMode "PeerOrChainTrust"
        
        $authorWebConfigNodesCount | Should be 1
        $authorWebConfigValidationModeCount | Should be 1
        $authorWebConfigIssuer | Should be "testIssuerNormalized"
        $wsWebConfigNodesCount | Should be 1
        $wsWebConfigNodesValidationModeCount | Should be 1
        $wsWebConfigIssuer | Should be "testIssuerNormalized"
        $stsWebConfigNodesCount | Should be 0
       
    }

    It "Set ISHIntegrationSTSCertificate normalizes thumbprint with wrong symbols"{       
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationSTSCertificate -Session $session -ArgumentList $testingDeploymentName, "test ,T h u> m<{ b! [p] rint  3", "testIssuerNormalized2", "PeerOrChainTrust" -WarningVariable Warning
        
        #Assert
        ($Warning-join -'') | Should Match "has been normalized to 'testThumbprint3'"

        remoteReadTargetXML -Thumbprint "testThumbprint3" -ValidationMode "PeerOrChainTrust"
        
        $authorWebConfigNodesCount | Should be 1
        $authorWebConfigValidationModeCount | Should be 1
        $authorWebConfigIssuer | Should be "testIssuerNormalized2"
        $wsWebConfigNodesCount | Should be 1
        $wsWebConfigNodesValidationModeCount | Should be 1
        $wsWebConfigIssuer | Should be "testIssuerNormalized2"
        $stsWebConfigNodesCount | Should be 0
    }

    It "Set ISHIntegrationSTSCertificate writes proper history"{        
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationSTSCertificate -Session $session -ArgumentList $testingDeploymentName, "testThumbprint", "testIssuer", "PeerOrChainTrust"
        $history = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetHistory -Session $session -ArgumentList $testingDeploymentName
        
        #Assert
        $history.Contains('Set-ISHIntegrationSTSCertificate -ISHDeployment $deploymentName -Thumbprint "testThumbprint" -Issuer "testIssuer" -ValidationMode PeerOrChainTrust') | Should be "True"     
    }
    
    It "Set ISHIntegrationSTSCertificate change ValidationMode"{       
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationSTSCertificate -Session $session -ArgumentList $testingDeploymentName, "testThumbprint", "testIssuer", "Custom"
        
        #Assert
        remoteReadTargetXML -Thumbprint "testThumbprint" -ValidationMode "Custom" 
        
        $authorWebConfigValidationModeCount | Should be 1
        $authorWebConfigValidationModeCertificateValidationMode | Should be "Custom"
        $wsWebConfigNodesValidationModeCount | Should be 1
        $wsWebConfigNodesValidationModeCertificateValidationMode | Should be "Custom"
    }

    It "Remove ISHIntegrationSTSCertificate"{       
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationSTSCertificate -Session $session -ArgumentList $testingDeploymentName, "testThumbprint", "testIssuer", "PeerOrChainTrust"
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationSTSCertificate -Session $session -ArgumentList $testingDeploymentName, "testThumbprint2", "testIssuer", "PeerOrChainTrust"
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationSTSCertificate -Session $session -ArgumentList $testingDeploymentName, "testThumbprint3", "testIssuer2", "PeerOrChainTrust"
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveISHIntegrationSTSCertificate -Session $session -ArgumentList $testingDeploymentName, "testIssuer"
        
        #Assert
        remoteReadTargetXML -Thumbprint "testThumbprint" -ValidationMode "PeerOrChainTrust"
        $authorWebConfigNodesCount | Should be 0
        $authorWebConfigValidationModeCount | Should be 1
        $wsWebConfigNodesCount | Should be 0
        $wsWebConfigNodesValidationModeCount | Should be 1
        $stsWebConfigNodesCount | Should be 0

        
        remoteReadTargetXML -Thumbprint "testThumbprint3" -ValidationMode "PeerOrChainTrust"
        $authorWebConfigNodesCount | Should be 1
        $authorWebConfigValidationModeCount | Should be 1
        $authorWebConfigIssuer | Should be "testIssuer2"
        $wsWebConfigNodesCount | Should be 1
        $wsWebConfigNodesValidationModeCount | Should be 1
        $wsWebConfigIssuer | Should be "testIssuer2"
        $stsWebConfigNodesCount | Should be 0
    }

    It "Remove ISHIntegrationSTSCertificate with wrong XML"{
        #Arrange
        # Running valid scenario commandlet to out files into backup folder before they will ba manually modified in test
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveISHIntegrationSTSCertificate -Session $session -ArgumentList $testingDeploymentName, "testIssuer"
        Rename-Item "$filepath\Web.config"  "_Web.config"
        New-Item "$filepath\Web.config" -type file |Out-Null
        
        #Act/Assert
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveISHIntegrationSTSCertificate -Session $session -ArgumentList $testingDeploymentName, "testIssuer" -ErrorAction Stop }| Should Throw "Root element is missing"
        #Rollback
        Remove-Item "$filepath\Web.config"
        Rename-Item "$filepath\_Web.config" "Web.config"
    }

    It "Remove ISHIntegrationSTSCertificate with no XML"{
        #Arrange
        Rename-Item "$filepath\Web.config"  "_Web.config"

        #Act/Assert
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveISHIntegrationSTSCertificate -Session $session -ArgumentList $testingDeploymentName, "testIssuer" -ErrorAction Stop }| Should Throw "Could not find file"
        #Rollback
        Rename-Item "$filepath\_Web.config" "Web.config"
    }

    It "Set ISHIntegrationSTSCertificate writes proper history. One more test"{        
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationSTSCertificate -Session $session -ArgumentList $testingDeploymentName, "testThumbprint", "testIssuer", "PeerOrChainTrust"
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveISHIntegrationSTSCertificate -Session $session -ArgumentList $testingDeploymentName, "testIssuer"
        $history = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetHistory -Session $session -ArgumentList $testingDeploymentName
        
        #Assert
        $history.Contains('Remove-ISHIntegrationSTSCertificate -ISHDeployment $deploymentName -Issuer "testIssuer"') | Should be "True"
              
    }

    It "Remove ISHIntegrationSTSCertificate - simple remove"{       
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationSTSCertificate -Session $session -ArgumentList $testingDeploymentName, "testThumbprint", "testIssuer", "PeerOrChainTrust"
        #Assert
        remoteReadTargetXML -Thumbprint "testThumbprint" -ValidationMode "PeerOrChainTrust"
        $authorWebConfigNodesCount | Should be 1
        $authorWebConfigValidationModeCount | Should be 1
        $authorWebConfigIssuer | Should be "testIssuer"
        $wsWebConfigNodesCount | Should be 1
        $wsWebConfigNodesValidationModeCount | Should be 1
        $wsWebConfigIssuer | Should be "testIssuer"
        $stsWebConfigNodesCount | Should be 0

        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveISHIntegrationSTSCertificate -Session $session -ArgumentList $testingDeploymentName, "testIssuer"
        remoteReadTargetXML -Thumbprint "testThumbprint" -ValidationMode "PeerOrChainTrust"
        
        $authorWebConfigNodesCount | Should be 0
        $authorWebConfigValidationModeCount | Should be 1
        $wsWebConfigNodesCount | Should be 0
        $wsWebConfigNodesValidationModeCount | Should be 1
        $stsWebConfigNodesCount | Should be 0
    }

    It "Set-ISHIntegrationSTSCertificate works after last issuer was removed"{       
      #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveISHIntegrationSTSCertificate -Session $session -ArgumentList $testingDeploymentName, "Issuer"
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationSTSCertificate -Session $session -ArgumentList $testingDeploymentName, "Thumbprint", "Issuer", "PeerOrChainTrust"} | Should not Throw
        #Assert
        remoteReadTargetXML -Thumbprint "Thumbprint" -ValidationMode "PeerOrChainTrust"
        
        $authorWebConfigNodesCount | Should be 1
        $authorWebConfigValidationModeCount | Should be 1
        $authorWebConfigIssuer | Should be "Issuer"
        $wsWebConfigNodesCount | Should be 1
        $wsWebConfigNodesValidationModeCount | Should be 1
        $wsWebConfigIssuer | Should be "Issuer"
        $stsWebConfigNodesCount | Should be 0
    }

	It "Set ISHIntegrationSTSCertificate writes inputparameters"{       
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationSTSCertificate -Session $session -ArgumentList $testingDeploymentName, "testThumbprint", "testIssuer", "PeerOrChainTrust" -WarningVariable Warning
        
        #Assert
        $result = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetInputParameters -Session $session -ArgumentList $testingDeploymentName

        $result["issuercertificatethumbprint"] | Should be "testThumbprint"
		$result["issuercertificatevalidationmode"] | Should be "PeerOrChainTrust"

    }
}