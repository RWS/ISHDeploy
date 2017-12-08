param(
    $session = $null,
    $testingDeploymentName = "InfoShareSQL2014"
)

. "$PSScriptRoot\Common.ps1"

$scriptBlockEnableISHServiceCrawler = {
    param (
        $ishDeployName

    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }

    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Enable-ISHServiceCrawler -ISHDeployment $ishDeploy

}

$scriptBlockDisableISHServiceCrawler = {
    param (
        $ishDeployName

    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }

    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Disable-ISHServiceCrawler -ISHDeployment $ishDeploy

}

$scriptBlockGetISHServiceCrawler = {
    param (
        $ishDeployName

    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }

    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Get-ISHServiceCrawler -ISHDeployment $ishDeploy

}

$scriptBlockSetISHIntegrationFullTextIndex = {
    param (
        $ishDeployName,
        $uri

    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }

    Set-ISHIntegrationFullTextIndex -ISHDeployment $ishDeployName -Uri $uri
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

Describe "Testing ISHServiceCrawler"{
    BeforeEach {
		UndoDeploymentBackToVanila $testingDeploymentName $true
    }
    
    It "Enable-ISHServiceCrawler Disabled Crawler Service"{
		#Arrange
        # Make sure, that Crawler Service is disabled, otherwise - make it disabled 
        $precondition = (Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceCrawler -Session $session -ArgumentList $testingDeploymentName)[0].Status
        if ($precondition -eq "Running"){Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisableISHServiceCrawler -Session $session -ArgumentList $testingDeploymentName }
        # Now Crawler Service should be fo sure disabled. Otherwise test fails
        RetryCommand -numberOfRetries 10 -command {(Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceCrawler -Session $session -ArgumentList $testingDeploymentName)[0].Status} -expectedResult "Stopped"| Should be "Stopped"
		#Act
        # Try enabling Crawler Service
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnableISHServiceCrawler -Session $session -ArgumentList $testingDeploymentName
        RetryCommand -numberOfRetries 10 -command {(Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceCrawler -Session $session -ArgumentList $testingDeploymentName)[0].Status} -expectedResult "Running"| Should be "Running"

    }
    
    It "Disable-ISHServiceCrawler enabled Crawler Service" {
		#Arrange
        # Make sure, that Crawler Service is enabled, otherwise - make it enabled
        $precondition = (Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceCrawler -Session $session -ArgumentList $testingDeploymentName)[0].Status
        if ($precondition -eq "Stopped"){Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnableISHServiceCrawler -Session $session -ArgumentList $testingDeploymentName }
        # Now Crawler Service should be fo sure enabled. Otherwise test fails
        RetryCommand -numberOfRetries 10 -command {(Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceCrawler -Session $session -ArgumentList $testingDeploymentName)[0].Status} -expectedResult "Running"| Should be "Running"
		#Act
        # Try enabling Crawler Service
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisableISHServiceCrawler -Session $session -ArgumentList $testingDeploymentName
        RetryCommand -numberOfRetries 10 -command {(Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceCrawler -Session $session -ArgumentList $testingDeploymentName)[0].Status} -expectedResult "Stopped"| Should be "Stopped"
    }
    
    
     It "Enable-ISHServiceCrawler enabled Crawler Service"{
		#Arrange
        # Make sure, that Crawler Service is disabled, otherwise - make it disabled 
        $precondition = (Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceCrawler -Session $session -ArgumentList $testingDeploymentName)[0].Status
        if ($precondition -eq "Running"){Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisableISHServiceCrawler -Session $session -ArgumentList $testingDeploymentName }

        # Now Crawler Service should be fo sure disabled. Otherwise test fails
        RetryCommand -numberOfRetries 10 -command {(Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceCrawler -Session $session -ArgumentList $testingDeploymentName)[0].Status} -expectedResult "Stopped"| Should be "Stopped"
		#Act
        # Try enabling Crawler Service
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnableISHServiceCrawler -Session $session -ArgumentList $testingDeploymentName
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnableISHServiceCrawler -Session $session -ArgumentList $testingDeploymentName} | Should not Throw
        #Assert
        RetryCommand -numberOfRetries 10 -command {(Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceCrawler -Session $session -ArgumentList $testingDeploymentName)[0].Status} -expectedResult "Running"| Should be "Running"

    }
    
    It "Disable-ISHServiceCrawler disabled Crawler Service"{
		#Arrange
        # Make sure, that Crawler Service is disabled, otherwise - make it disabled 
        $precondition = (Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceCrawler -Session $session -ArgumentList $testingDeploymentName)[0].Status
        if ($precondition -eq "Stopped"){Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnableISHServiceCrawler -Session $session -ArgumentList $testingDeploymentName }

        # Now Crawler Service should be fo sure enabled. Otherwise test fails
        RetryCommand -numberOfRetries 10 -command {(Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceCrawler -Session $session -ArgumentList $testingDeploymentName)[0].Status} -expectedResult "Running"| Should be "Running"
		#Act
        # Try enabling Crawler Service
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisableISHServiceCrawler -Session $session -ArgumentList $testingDeploymentName
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisableISHServiceCrawler -Session $session -ArgumentList $testingDeploymentName} | Should not Throw
        RetryCommand -numberOfRetries 10 -command {(Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceCrawler -Session $session -ArgumentList $testingDeploymentName)[0].Status} -expectedResult "Stopped"| Should be "Stopped"
    }
    
    It "Enable-ISHServiceCrawler changes component state"{
		#Arrange
        # Make sure, that Crawler Service is disabled, otherwise - make it disabled 
        $precondition = (Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceCrawler -Session $session -ArgumentList $testingDeploymentName)[0].Status
        if ($precondition -eq "Running"){Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisableISHServiceCrawler -Session $session -ArgumentList $testingDeploymentName }
        # Now Crawler Service should be fo sure disabled. Otherwise test fails
        RetryCommand -numberOfRetries 10 -command {(Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceCrawler -Session $session -ArgumentList $testingDeploymentName)[0].Status} -expectedResult "Stopped"| Should be "Stopped"
		#Act
        # Try enabling Crawler Service
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnableISHServiceCrawler -Session $session -ArgumentList $testingDeploymentName
        RetryCommand -numberOfRetries 10 -command {(Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceCrawler -Session $session -ArgumentList $testingDeploymentName)[0].Status} -expectedResult "Running"| Should be "Running"
        $comp = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHComponent -Session $session -ArgumentList $testingDeploymentName
        ($comp | Where-Object -Property Name -EQ "Crawler").IsEnabled | Should be "True"
    }
     
    It "Enable-ISHServiceCrawler writes history"{       
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnableISHServiceCrawler -Session $session -ArgumentList $testingDeploymentName
        $history = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetHistory -Session $session -ArgumentList $testingDeploymentName
        $history.Contains('Enable-ISHServiceCrawler -ISHDeployment') | Should be "True"
    }

    It "Enable-ISHServiceCrawler should also start Full Text Index service"{       
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnableISHServiceCrawler -Session $session -ArgumentList $testingDeploymentName

		$crawlerService = (Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceCrawler -Session $session -ArgumentList $testingDeploymentName)[0]
		$crawlerService.Status | Should be "Running"
		
		$fullTextIndexService = (Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceFullTextIndex -Session $session -ArgumentList $testingDeploymentName)[0]
		$fullTextIndexService.Status | Should be "Running"
    }

    It "Disable-ISHServiceFullTextIndex should also stop Crawler service"{       
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnableISHServiceCrawler -Session $session -ArgumentList $testingDeploymentName

		$crawlerService = (Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceCrawler -Session $session -ArgumentList $testingDeploymentName)[0]
		$crawlerService.Status | Should be "Running"
		
		$fullTextIndexService = (Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceFullTextIndex -Session $session -ArgumentList $testingDeploymentName)[0]
		$fullTextIndexService.Status | Should be "Running"

        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisableISHServiceFullTextIndex -Session $session -ArgumentList $testingDeploymentName
        
		$crawlerService = (Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceCrawler -Session $session -ArgumentList $testingDeploymentName)[0]
		$crawlerService.Status | Should be "Stopped"
		
		$fullTextIndexService = (Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceFullTextIndex -Session $session -ArgumentList $testingDeploymentName)[0]
		$fullTextIndexService.Status | Should be "Stopped"
    }

    It "Enable-ISHServiceCrawler linked with external should not start Full Text Index service"{       
        #Act

        $uri = "http://" + $testingDeploymentName + ":8080/solr/"
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationFullTextIndex -Session $session -ArgumentList $testingDeploymentName, $uri        

        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnableISHServiceCrawler -Session $session -ArgumentList $testingDeploymentName

		$crawlerService = (Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceCrawler -Session $session -ArgumentList $testingDeploymentName)[0]
		$crawlerService.Status | Should be "Running"
		
		$fullTextIndexService = (Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceFullTextIndex -Session $session -ArgumentList $testingDeploymentName)[0]
		$fullTextIndexService.Status | Should be "Stopped"
    }

    UndoDeploymentBackToVanila $testingDeploymentName $true
}