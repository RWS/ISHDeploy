param(
    $session = $null,
    $testingDeploymentName = "InfoShare"
)

. "$PSScriptRoot\Common.ps1"
$invalidSuffix = "Invalid"


$scriptBlock = {
    param (
        [Parameter(Mandatory=$false)]
        $ishDeployName 
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
    if ($ishDeployName){
        Get-ISHDeployment -Name $ishDeployName 
    }
    else {
        Get-ISHDeployment
    }
}

Describe "Testing Get-ISHDeployment"{
    It "doesnot match 'InfoShare' pattern"{
		#Act/Assert
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlock -Session $session -ArgumentList $invalidSuffix} | Should Throw 'The argument '+$invalidSuffix+' does not match the "^(InfoShare)" pattern. Supply an argument that matches "^(InfoShare)" and try the command again.'
    }

    It "returns correct deployment"{
		#Act
        $deploy = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlock -Session $session -ArgumentList $testingDeploymentName
        #Assert
		$deploy.Name | Should Be $testingDeploymentName 
    }

    It "returns correct ammount of deployments"{
        #Act
        $deployments = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlock -Session $session
        #Assert
		$deployments.Count | Should Be 2
    }

    It "returns message when deployment is not found"{
        #Act/Assert
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlock -Session $session -ArgumentList "InfoShare$invalidSuffix"}  | Should Throw "Deployment with suffix $invalidSuffix is not found on the system"
    }
}

