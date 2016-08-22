param(
    $session = $null,
    $testingDeploymentName = "InfoShare"
)
. "$PSScriptRoot\Common.ps1"

# Script block for getting ISH deployment
$scriptBlockGetDeployment = {
    param (
        [Parameter(Mandatory=$false)]
        $ishDeployName 
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
    Get-ISHDeployment -Name $ishDeployName 
}
#region variables
$testingDeployment = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetDeployment -Session $session -ArgumentList $testingDeploymentName
$suffix = GetProjectSuffix($testingDeployment.Name)
$dnsComputerName = $computerName
$computerName = $computerName.split(".")[0]
$internalFolderPath = "{0}\Web{1}\InfoShareWS\Internal" -f $testingDeployment.Webpath, $suffix
#endregion

#region Script Blocks 

$scriptBlockEnableISHIntegrationSTSInternalAuthentication = {
    param (
        [Parameter(Mandatory=$false)]
        $ishDeployName, 
        [Parameter(Mandatory=$false)]
        $LCHost, 
        [Parameter(Mandatory=$false)]
        $LCWebAppName
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Enable-ISHIntegrationSTSInternalAuthentication  -ISHDeployment $ishDeploy -LCHost $LCHost -LCWebAppName $LCWebAppName
    
}

$scriptBlockDisableISHIntegrationSTSInternalAuthentication = {
    param (
        [Parameter(Mandatory=$false)]
        $ishDeployName
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Disable-ISHIntegrationSTSInternalAuthentication  -ISHDeployment $ishDeploy
    
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


Describe "Testing ISHIntegrationSTSInternalAuthentication"{
    BeforeEach {
        UndoDeploymentBackToVanila $testingDeploymentName $true
    }
    
    It "Enable ISHIntegrationSTSInternalAuthentication"{
        RemotePathCheck $internalFolderPath | Should be $false
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnableISHIntegrationSTSInternalAuthentication -Session $session -ArgumentList $testingDeploymentName, $null, $null
        RemotePathCheck $internalFolderPath | Should be $true

        $indexFile = Invoke-CommandRemoteOrLocal -ScriptBlock {param($internalFolderPath) Get-Content "$internalFolderPath\index.html"} -Session $session -ArgumentList $internalFolderPath
        $indexFile -match "https://$dnsComputerName/ISHWS$suffix/Internal/" | Should be $true
    }

    It "Enable ISHIntegrationSTSInternalAuthentication with LC"{
        RemotePathCheck $internalFolderPath | Should be $false
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnableISHIntegrationSTSInternalAuthentication -Session $session -ArgumentList $testingDeploymentName, "test", "testName"
        RemotePathCheck $internalFolderPath | Should be $true

        $indexFile = Invoke-CommandRemoteOrLocal -ScriptBlock {param($internalFolderPath) Get-Content "$internalFolderPath\index.html"} -Session $session -ArgumentList $internalFolderPath
        $indexFile -match ' var cdUrl = "https://test/testName"' | Should be $true
    }

    It "Enable ISHIntegrationSTSInternalAuthentication sets default value of LCWebAppName"{
        RemotePathCheck $internalFolderPath | Should be $false
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnableISHIntegrationSTSInternalAuthentication -Session $session -ArgumentList $testingDeploymentName, "test", $null
        RemotePathCheck $internalFolderPath | Should be $true

        $indexFile = Invoke-CommandRemoteOrLocal -ScriptBlock {param($internalFolderPath) Get-Content "$internalFolderPath\index.html"} -Session $session -ArgumentList $internalFolderPath
        $indexFile -match ' var cdUrl = "https://test/ContentDelivery"' | Should be $true
    }

    It "Enable ISHIntegrationSTSInternalAuthentication LCWebAppName can be set only after LCHost"{
        RemotePathCheck $internalFolderPath | Should be $false
        $expectedErrorMessage = "Parameter '-LCWebAppName' could not be inserted without '-LCHost'"
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnableISHIntegrationSTSInternalAuthentication -Session $session -ArgumentList $testingDeploymentName, $null, "testName"} | Should throw $expectedErrorMessage
        
    }

    It "Disable enabled ISHIntegrationSTSInternalAuthentication"{
        RemotePathCheck $internalFolderPath | Should be $false
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnableISHIntegrationSTSInternalAuthentication -Session $session -ArgumentList $testingDeploymentName, $null, $null
        RemotePathCheck $internalFolderPath | Should be $true

        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisableISHIntegrationSTSInternalAuthentication -Session $session -ArgumentList $testingDeploymentName
        RemotePathCheck $internalFolderPath | Should be $false
    }

    It "Disable disabled ISHIntegrationSTSInternalAuthentication"{
        RemotePathCheck $internalFolderPath | Should be $false
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisableISHIntegrationSTSInternalAuthentication -Session $session -ArgumentList $testingDeploymentName} | Should not throw

    }

    It "Undo-ISHDeployment removes ISHIntegrationSTSInternalAuthentication folder"{
        RemotePathCheck $internalFolderPath | Should be $false
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnableISHIntegrationSTSInternalAuthentication -Session $session -ArgumentList $testingDeploymentName, $null, $null
        RemotePathCheck $internalFolderPath | Should be $true

        UndoDeploymentBackToVanila $testingDeploymentName $true
        RemotePathCheck $internalFolderPath | Should be $false
    }
}