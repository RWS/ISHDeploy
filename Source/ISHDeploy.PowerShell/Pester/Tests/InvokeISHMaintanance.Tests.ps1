param(
    $session = $null,
    $testingDeploymentName = "InfoShareSQL2014"
)

. "$PSScriptRoot\Common.ps1"

$scriptBlockInvokeISHMaintanance= {
    param (
        $ishDeployName,
        $parametersHash

    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }

    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Invoke-ISHMaintenance -ISHDeployment $ishDeploy @parametersHash 5>&1

}


Describe "Testing ISHMaintanance"{
    BeforeEach {
		UndoDeploymentBackToVanila $testingDeploymentName $true
    }
    
    It "Reindexing Crawler does not give Invalide argument message"{

		#Arrange
        $oldDebugPreference = $DebugPreference
        $DebugPreference = "Continue"
        $params = @{Crawler = $true; ReIndex = $true }
        #Act
        $debugMessage = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockInvokeISHMaintanance -Session $session -ArgumentList $testingDeploymentName, $params
        #Arrange
        $debugMessage.Contains("Invalid commandline arguments: use") | Should be $false
        $debugMessage -like "*Reindexing was completed for the current catalog*" | Should be $true
        $DebugPreference = $oldDebugPreference
    }
    
    It "Register Crawler gives successful result in crawler output"{

		#Arrange
        $oldDebugPreference = $DebugPreference
        $DebugPreference = "Continue"
        $params = @{Crawler = $true; Register = $true }
        #Act
        $debugMessage = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockInvokeISHMaintanance -Session $session -ArgumentList $testingDeploymentName, $params
        #Arrange
        $debugMessage -like "*Registering the Crawler for * was successful*" | Should be $true
        $DebugPreference = $oldDebugPreference
    }

    It "Unregister Crawler gives successful result in crawler output"{

		#Arrange
        $oldDebugPreference = $DebugPreference
        $DebugPreference = "Continue"
        $params = @{Crawler = $true; UnRegisterAll = $true }
        #Act
        $debugMessage = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockInvokeISHMaintanance -Session $session -ArgumentList $testingDeploymentName, $params
        #Arrange
        $debugMessage -like "*All registered crawlers for * are successful unregistered.*" | Should be $true
        $DebugPreference = $oldDebugPreference
    }

    UndoDeploymentBackToVanila $testingDeploymentName $true
}