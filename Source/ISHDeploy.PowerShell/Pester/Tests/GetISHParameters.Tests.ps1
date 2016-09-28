param(
    $session = $null,
    $testingDeploymentName = "InfoShare"
)

. "$PSScriptRoot\Common.ps1"


$moduleName = Invoke-CommandRemoteOrLocal -ScriptBlock { (Get-Module "ISHDeploy.*").Name } -Session $session
$backupPath = "\\$computerName\C$\ProgramData\$moduleName\$($testingDeployment.Name)\Backup"

$scriptBlockGetParameters = {
    param (
        [Parameter(Mandatory=$false)]
        $ishDeployName,
        $switshes
         
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Get-ISHDeploymentParameters -ISHDeployment $ishDeploy @switshes
}

$scriptBlockGetBackUpedParameters = {
    param (
        [Parameter(Mandatory=$true)]
        $projectName,
        $backupPath 
    )

    [System.Xml.XmlDocument]$inputParameters = new-object System.Xml.XmlDocument
    $inputParameters.load("$backupPath\inputparameters.xml")

    $result = @{}
    $result["inputparametersFilePath"] = "$inputParametersPath\inputparameters.xml"
    $result["osuser"] = $inputParameters.SelectNodes("inputconfig/param[@name='osuser']/currentvalue")[0].InnerText
    $result["connectstring"] = $inputParameters.SelectNodes("inputconfig/param[@name='connectstring']/currentvalue")[0].InnerText
    $result["infoshareauthorwebappname"] = $inputParameters.SelectNodes("inputconfig/param[@name='infoshareauthorwebappname']/currentvalue")[0].InnerText
    $result["infosharewswebappname"] = $inputParameters.SelectNodes("inputconfig/param[@name='infosharewswebappname']/currentvalue")[0].InnerText
    $result["infosharestswebappname"] = $inputParameters.SelectNodes("inputconfig/param[@name='infosharestswebappname']/currentvalue")[0].InnerText
    $result["baseurl"] = $inputParameters.SelectNodes("inputconfig/param[@name='baseurl']/currentvalue")[0].InnerText
    $result["issueractorusername"] = $inputParameters.SelectNodes("inputconfig/param[@name='issueractorusername']/currentvalue")[0].InnerText
	$result["issueractorpassword"] = $inputParameters.SelectNodes("inputconfig/param[@name='issueractorpassword']/currentvalue")[0].InnerText
	$result["websitename"] = $inputParameters.SelectNodes("inputconfig/param[@name='websitename']/currentvalue")[0].InnerText
	$result["issuerwstrustbindingtype"] = $inputParameters.SelectNodes("inputconfig/param[@name='issuerwstrustbindingtype']/currentvalue")[0].InnerText
	$result["issuerwstrustendpointurl"] = $inputParameters.SelectNodes("inputconfig/param[@name='issuerwstrustendpointurl']/currentvalue")[0].InnerText
	$result["issuerwstrustmexurl"] = $inputParameters.SelectNodes("inputconfig/param[@name='issuerwstrustmexurl']/currentvalue")[0].InnerText
	$result["issuercertificatethumbprint"] = $inputParameters.SelectNodes("inputconfig/param[@name='issuercertificatethumbprint']/currentvalue")[0].InnerText
	$result["issuercertificatevalidationmode"] = $inputParameters.SelectNodes("inputconfig/param[@name='issuercertificatevalidationmode']/currentvalue")[0].InnerText
	$result["issuerwsfederationendpointurl"] = $inputParameters.SelectNodes("inputconfig/param[@name='issuerwsfederationendpointurl']/currentvalue")[0].InnerText
	$result["servicecertificatethumbprint"] = $inputParameters.SelectNodes("inputconfig/param[@name='servicecertificatethumbprint']/currentvalue")[0].InnerText
	$result["servicecertificatevalidationmode"] = $inputParameters.SelectNodes("inputconfig/param[@name='servicecertificatevalidationmode']/currentvalue")[0].InnerText
	$result["servicecertificatesubjectname"] = $inputParameters.SelectNodes("inputconfig/param[@name='servicecertificatesubjectname']/currentvalue")[0].InnerText
	$result["issuerwstrustendpointurl_normalized"] = $inputParameters.SelectNodes("inputconfig/param[@name='issuerwstrustendpointurl_normalized']/currentvalue")[0].InnerText

    return $result
    
}

$scriptBlockSetWSFederation = {
    param (
        $ishDeployName,
        $endpoint
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }

    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    
    Set-ISHIntegrationSTSWSFederation -ISHDeployment $ishDeploy  -Endpoint $endpoint
}

$scriptBlockGetParametersPipeline = {
    param (
        [Parameter(Mandatory=$false)]
        $ishDeployName,
        $switshes
         
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
    $count = 0
    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Get-ISHDeploymentParameters -ISHDeployment $ishDeploy @switshes | % {$count = $count + 1}
    return $count
}

Describe "Testing Get-ISHDeploymentParameters"{
    BeforeEach {
		 UndoDeploymentBackToVanila $testingDeploymentName $true
    }

    It "Get-ISHDeploymentParameters returns changed parameters"{
        #Arrange
        $params = @{Original = $false; Changed = $true; Showpassword  = $false}
        
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetWSFederation -Session $session -ArgumentList $testingDeploymentName, "testEndpoint"
        $inputparameters = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetParameters -Session $session -ArgumentList $testingDeploymentName, $params
        $changedParameters = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetInputParameters -Session $session -ArgumentList $testingDeploymentName
        
        foreach ($parameter in $inputparameters){
             if($parameter.Name -eq "issuerwsfederationendpointurl"){$result =  $parameter.Value}
        }
        #Assert
        $result -eq $changedParameters["issuerwsfederationendpointurl"] | Should be $true
    }

    
    It "Get-ISHDeploymentParameters returns original parameters"{
        #Arrange
        $params = @{Original = $true; Changed = $false; Showpassword  = $false}
        
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetWSFederation -Session $session -ArgumentList $testingDeploymentName, "testEndpoint"
        $inputparameters = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetParameters -Session $session -ArgumentList $testingDeploymentName, $params
        $originalParameters = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetBackUpedParameters -Session $session -ArgumentList $testingDeploymentName, $backupPath 
        
        foreach ($parameter in $inputparameters){
             if($parameter.Name -eq "issuerwsfederationendpointurl"){$result =  $parameter.Value}
        }
        #Assert
        $result -eq $originalParameters["issuerwsfederationendpointurl"] | Should be $true
    }

    It "Get-ISHDeploymentParameters dont throw when no changed parameters"{
        #Arrange
        $params = @{Original = $false; Changed = $true; Showpassword  = $false}
        
         #Assert
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetParameters -Session $session -ArgumentList $testingDeploymentName, $params} | Should not Throw
    }

    It "Get-ISHDeploymentParameters don't show password by default"{
        #Arrange
        $params = @{Original = $true; Changed = $false; Showpassword  = $false}
        
        #Assert
        $inputparameters = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetParameters -Session $session -ArgumentList $testingDeploymentName, $params
        foreach ($parameter in $inputparameters){
            if($parameter.Name -eq "ospassword"){$ospassword =  $parameter.Value}
            if($parameter.Name -eq "issueractorpassword"){$issueractorpassword =  $parameter.Value}
            if($parameter.Name -eq "servicepassword"){$servicepassword =  $parameter.Value}
            if($parameter.Name -eq "databasepassword"){$databasepassword =  $parameter.Value}
        }
        $ospassword | Should be "*******"
        $issueractorpassword | Should be "*******"
        $servicepassword | Should be "*******"
        $databasepassword | Should be "*******"
    }

    It "Get-ISHDeploymentParameters shows password when switch is provided"{
        #Arrange
        $params = @{Original = $true; Changed = $false; Showpassword  = $true}
        
        #Assert
        $inputparameters = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetParameters -Session $session -ArgumentList $testingDeploymentName, $params
        foreach ($parameter in $inputparameters){
            if($parameter.Name -eq "ospassword"){$ospassword =  $parameter.Value}
            if($parameter.Name -eq "issueractorpassword"){$issueractorpassword =  $parameter.Value}
            if($parameter.Name -eq "servicepassword"){$servicepassword =  $parameter.Value}
            if($parameter.Name -eq "databasepassword"){$databasepassword =  $parameter.Value}
        }
        $ospassword | Should not be "*******"
        $issueractorpassword | Should not be "*******"
        $servicepassword | Should not be "*******"
        $databasepassword | Should not be "*******"
    }

    It "Get-ISHDeploymentParameters returns collection"{
        #Arrange
        $params = @{Original = $true; Changed = $false; Showpassword  = $true}
        
        #Assert
        $count = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetParametersPipeline -Session $session -ArgumentList $testingDeploymentName, $params
        $count -gt 1 | should be $true
       
    }
}

