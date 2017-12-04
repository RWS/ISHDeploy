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
        $debugMessage -like "*Invalid commandline arguments: use*" | Should be $true
        $DebugPreference = $oldDebugPreference
    }
    
    

    UndoDeploymentBackToVanila $testingDeploymentName $true
}