param(
    $session = $null,
    $testingDeploymentName = "InfoShare"
)

. "$PSScriptRoot\Common.ps1"
$invalidSuffix = "Invalid"


$scriptBlockGetISHDeployment = {
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

$testingDeployment = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHDeployment -Session $session -ArgumentList $testingDeploymentName


$scriptBlockGetVersionValue = {
    param (
        $ishDeployName
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
    $currentInstall = (Get-ItemProperty -Path HKLM:\Software\Wow6432Node\Trisoft\InstallTool\InfoShare\$ishDeployName).Current
    $currentVersion = (Get-ItemProperty -Path HKLM:\Software\Wow6432Node\Trisoft\InstallTool\InfoShare\$ishDeployName\History\$currentInstall).Version
    Return $currentVersion 
    
}

$scriptBlockSetVersionValue = {
    param (
        $ishDeployName,
        $value
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
    $currentInstall = (Get-ItemProperty -Path HKLM:\Software\Wow6432Node\Trisoft\InstallTool\InfoShare\$ishDeployName).Current
    Set-ItemProperty -Path HKLM:\Software\Wow6432Node\Trisoft\InstallTool\InfoShare\$ishDeployName\History\$currentInstall -Name Version -Value $value
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

Describe "Testing Get-ISHDeployment"{
    It "doesnot match 'InfoShare' pattern"{
		#Act/Assert
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHDeployment -Session $session -ArgumentList $invalidSuffix} | Should Throw 'The argument '+$invalidSuffix+' does not match the "^(InfoShare)" pattern. Supply an argument that matches "^(InfoShare)" and try the command again.'
    }

    It "returns correct deployment"{
		#Act
        $deploy = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHDeployment -Session $session -ArgumentList $testingDeploymentName
        #Assert
		$deploy.Name | Should Be $testingDeploymentName 
    }

    It "returns correct ammount of deployments"{
        #Act
        $deployments = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHDeployment -Session $session
        #Assert
		$deployments.Count | Should Be 2
    }

    It "returns message when deployment is not found"{
        #Act/Assert
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHDeployment -Session $session -ArgumentList "InfoShare$invalidSuffix"}  | Should Throw "Deployment with suffix $invalidSuffix is not found on the system"
    }

    It "returns warning when CM version doesnot match"{
        #Arrange
        $current = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetVersionValue -Session $session -ArgumentList $testingDeploymentName
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlocksetVersionValue -Session $session -ArgumentList $testingDeploymentName, "9.0.2417.0"
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHDeployment -Session $session -ArgumentList $testingDeploymentName  -WarningVariable Warning
        #Asssert
        $Warning | Should Match "does not correspond to deployment version"
        #Rollback
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlocksetVersionValue -Session $session -ArgumentList $testingDeploymentName, $current 
    }

    It "Commandlets that accept ISHDeployment throw exception when CM version doesnot match"{
        #Arrange
        $current = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetVersionValue -Session $session -ArgumentList $testingDeploymentName
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlocksetVersionValue -Session $session -ArgumentList $testingDeploymentName, "9.0.2417.0"
        #Act
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetHistory -Session $session -ArgumentList $testingDeploymentName} | Should Throw "does not correspond to deployment version "
		#Rollback
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlocksetVersionValue -Session $session -ArgumentList $testingDeploymentName, $current 
    }

}

