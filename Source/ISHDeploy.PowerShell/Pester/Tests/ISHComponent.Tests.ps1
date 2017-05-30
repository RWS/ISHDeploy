param(
    $session = $null,
    $testingDeploymentName = "InfoShare"
)

. "$PSScriptRoot\Common.ps1"

#region variables
#region variables
$xmlPath = $webPath.ToString().replace(":", "$")
$xmlPath = "\\$computerName\$xmlPath"
$filePath = $appPath.ToString().replace(":", "$")
$filePath = "\\$computerName\$filePath"
$filePath = Join-Path $filePath "TranslationOrganizer\Bin"

$DumpFolder = "C:\DubpFolder" 
$MaxTranslationJobItemsUpdatedInOneCall = 250 
$SystemTaskInterval = "11:11:00.000" 
$AttemptsBeforeFailOnRetrieval = 2
$UpdateLeasedByPerNumberOfItems = 20
$RetriesOnTimeout = 2
$JobPollingInterval = "00:11:10.000"
$PendingJobPollingInterval = "00:11:11.000"

$ISHWS = "http://jira.com/"
$ISHWSCertificateValidationMode = "ChainTrust" 
$ISHWSDnsIdentity = "test" 
$IssuerBindingType = "UserNameMixed" 
$IssuerEndpoint = "http://test/"

$userName = “GLOBAL\InfoshareServiceUser”
$userPassword = "!nf0Shar3"
$secpasswd = ConvertTo-SecureString “$userPassword” -AsPlainText -Force
$testCreds = New-Object System.Management.Automation.PSCredential ($userName, $secpasswd)
#endregion

#region Script Blocks
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


Describe "Testing ISHComponent"{
    BeforeEach {
        UndoDeploymentBackToVanila $testingDeploymentName $true
    }

    It "Get ISHComponent returns default value"{

        #Act
        $comp = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHComponent -Session $session -ArgumentList $testingDeploymentName
         #Assert
        ($comp | Where-Object -Property Name -EQ "CM").IsEnabled | Should be "True"
        ($comp | Where-Object -Property Name -EQ "WS").IsEnabled | Should be "True"
        ($comp | Where-Object -Property Name -EQ "STS").IsEnabled | Should be "True"
        ($comp | Where-Object -Property Name -EQ "COMPlus").IsEnabled | Should be "True"
        ($comp | Where-Object -Property Name -EQ "BackgroundTask").IsEnabled | Should be "False"
        ($comp | Where-Object -Property Name -EQ "Crawler").IsEnabled | Should be "False"
        ($comp | Where-Object -Property Name -EQ "SolrLucene").IsEnabled | Should be "False"
        ($comp | Where-Object -Property Name -EQ "TranslationBuilder").IsEnabled | Should be "False"
        ($comp | Where-Object -Property Name -EQ "TranslationOrganizer").IsEnabled | Should be "False"
    }
    
    It "Enable components commandlets change state of component"{       
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnableISHServiceTranslationBuilder -Session $session -ArgumentList $testingDeploymentName, $params

        $comp = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHComponent -Session $session -ArgumentList $testingDeploymentName
        ($comp | Where-Object -Property Name -EQ "TranslationBuilder").IsEnabled | Should be "True"
    }
    
     UndoDeploymentBackToVanila $testingDeploymentName $true
}