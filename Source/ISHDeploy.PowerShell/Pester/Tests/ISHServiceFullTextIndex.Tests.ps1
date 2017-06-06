param(
    $session = $null,
    $testingDeploymentName = "InfoShareSQL2014"
)

. "$PSScriptRoot\Common.ps1"

$scriptBlockEnableISHServiceFullTextIndex = {
    param (
        $ishDeployName

    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }

    Enable-ISHServiceFullTextIndex -ISHDeployment $ishDeployName
}

$scriptBlockDisableISHServiceFullTextIndex = {
    param (
        $ishDeployName

    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }

    Disable-ISHServiceFullTextIndex -ISHDeployment $ishDeployName
}

$scriptBlockGetISHServiceFullTextIndex = {
    param (
        $ishDeployName

    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }

    Get-ISHServiceFullTextIndex -ISHDeployment $ishDeployName
}

Describe "Testing ISHServiceFullTextIndex"{
    BeforeEach {
		UndoDeploymentBackToVanila $testingDeploymentName $true
    }
    
    It "Enable-ISHServiceFullTextIndex Disabled SolrLucene Service"{
		#Arrange
        # Make sure, that SolrLucene Service is disabled
        RetryCommand -numberOfRetries 10 -command {(Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceFullTextIndex -Session $session -ArgumentList $testingDeploymentName)[0].Status} -expectedResult "Stopped"| Should be "Stopped"
		#Act
        # Try enabling SolrLucene Service
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnableISHServiceFullTextIndex -Session $session -ArgumentList $testingDeploymentName
        RetryCommand -numberOfRetries 10 -command {(Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceFullTextIndex -Session $session -ArgumentList $testingDeploymentName)[0].Status} -expectedResult "Running"| Should be "Running"

    }
    
    It "Disable-ISHServiceFullTextIndex enabled SolrLucene Service" {
		#Arrange
        # Make sure, that SolrLucene Service is enabled, otherwise - make it enabled
        $precondition = (Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceFullTextIndex -Session $session -ArgumentList $testingDeploymentName)[0].Status
        if ($precondition -eq "Stopped"){Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnableISHServiceFullTextIndex -Session $session -ArgumentList $testingDeploymentName }
        # Now SolrLucene Service should be fo sure enabled. Otherwise test fails
        RetryCommand -numberOfRetries 10 -command {(Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceFullTextIndex -Session $session -ArgumentList $testingDeploymentName)[0].Status} -expectedResult "Running"| Should be "Running"
		#Act
        # Try enabling SolrLucene Service
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisableISHServiceFullTextIndex -Session $session -ArgumentList $testingDeploymentName
        RetryCommand -numberOfRetries 10 -command {(Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceFullTextIndex -Session $session -ArgumentList $testingDeploymentName)[0].Status} -expectedResult "Stopped"| Should be "Stopped"
    }
    
    
     It "Enable-ISHServiceFullTextIndex enabled SolrLucene Service"{
		#Arrange
        # Make sure, that SolrLucene Service is disabled, otherwise - make it disabled 
        $precondition = (Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceFullTextIndex -Session $session -ArgumentList $testingDeploymentName)[0].Status
        if ($precondition -eq "Running"){Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisableISHServiceFullTextIndex -Session $session -ArgumentList $testingDeploymentName }

        # Now SolrLucene Service should be fo sure disabled. Otherwise test fails
        RetryCommand -numberOfRetries 10 -command {(Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceFullTextIndex -Session $session -ArgumentList $testingDeploymentName)[0].Status} -expectedResult "Stopped"| Should be "Stopped"
		#Act
        # Try enabling SolrLucene Service
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnableISHServiceFullTextIndex -Session $session -ArgumentList $testingDeploymentName
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnableISHServiceFullTextIndex -Session $session -ArgumentList $testingDeploymentName} | Should not Throw
        #Assert
        RetryCommand -numberOfRetries 10 -command {(Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceFullTextIndex -Session $session -ArgumentList $testingDeploymentName)[0].Status} -expectedResult "Running"| Should be "Running"

    }
    
    It "Disable-ISHServiceFullTextIndex disabled SolrLucene Service"{
		#Arrange
        # Make sure, that SolrLucene Service is disabled, otherwise - make it disabled 
        $precondition = (Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceFullTextIndex -Session $session -ArgumentList $testingDeploymentName)[0].Status
        if ($precondition -eq "Stopped"){Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnableISHServiceFullTextIndex -Session $session -ArgumentList $testingDeploymentName }

        # Now SolrLucene Service should be fo sure enabled. Otherwise test fails
        RetryCommand -numberOfRetries 10 -command {(Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceFullTextIndex -Session $session -ArgumentList $testingDeploymentName)[0].Status} -expectedResult "Running"| Should be "Running"
		#Act
        # Try enabling SolrLucene Service
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisableISHServiceFullTextIndex -Session $session -ArgumentList $testingDeploymentName
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisableISHServiceFullTextIndex -Session $session -ArgumentList $testingDeploymentName} | Should not Throw
        RetryCommand -numberOfRetries 10 -command {(Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceFullTextIndex -Session $session -ArgumentList $testingDeploymentName)[0].Status} -expectedResult "Stopped"| Should be "Stopped"
    }
    
    It "Enable-ISHServiceFullTextIndex changes component state"{
		#Arrange
        # Make sure, that SolrLucene Service is disabled, otherwise - make it disabled 
        $precondition = (Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceFullTextIndex -Session $session -ArgumentList $testingDeploymentName)[0].Status
        if ($precondition -eq "Running"){Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisableISHServiceFullTextIndex -Session $session -ArgumentList $testingDeploymentName }
        # Now SolrLucene Service should be fo sure disabled. Otherwise test fails
        RetryCommand -numberOfRetries 10 -command {(Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceFullTextIndex -Session $session -ArgumentList $testingDeploymentName)[0].Status} -expectedResult "Stopped"| Should be "Stopped"
		#Act
        # Try enabling SolrLucene Service
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnableISHServiceFullTextIndex -Session $session -ArgumentList $testingDeploymentName
        RetryCommand -numberOfRetries 10 -command {(Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceFullTextIndex -Session $session -ArgumentList $testingDeploymentName)[0].Status} -expectedResult "Running"| Should be "Running"
        $comp = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHComponent -Session $session -ArgumentList $testingDeploymentName
        ($comp | Where-Object -Property Name -EQ "SolrLucene").IsEnabled | Should be "True"
    }
     
    It "Enable-ISHServiceFullTextIndex writes history"{       
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnableISHServiceFullTextIndex -Session $session -ArgumentList $testingDeploymentName
        $history = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetHistory -Session $session -ArgumentList $testingDeploymentName
        $history.Contains('Enable-ISHServiceFullTextIndex -ISHDeployment') | Should be "True"
    }
    UndoDeploymentBackToVanila $testingDeploymentName $true
}