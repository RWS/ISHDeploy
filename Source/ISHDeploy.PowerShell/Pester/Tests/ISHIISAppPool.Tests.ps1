param(
    $session = $null,
    $testingDeploymentName = "InfoShareSQL"
)

. "$PSScriptRoot\Common.ps1"

#region variables
$xmlPath = $webPath.ToString().replace(":", "$")
$xmlPath = "\\$computerName\$xmlPath"
$filePath = $appPath.ToString().replace(":", "$")
$filePath = "\\$computerName\$filePath"
$organizerFilePath = Join-Path $filePath "TranslationOrganizer\Bin"
$builderFilePath = Join-Path $filePath "TranslationBuilder\Bin"
$xmlAppPath = $appPath.replace(":", "$")
$xmlAppPath = "\\$computerName\$xmlAppPath"
$xmlDataPath = $dataPath.replace(":", "$")
$xmlDataPath = "\\$computerName\$xmlDataPath"

$userName = Get-TestDataValue “testDomainUserName”
$userPassword = Get-TestDataValue "testDomainUserPassword"

$secpasswd = ConvertTo-SecureString “$userPassword” -AsPlainText -Force
$testCreds = New-Object System.Management.Automation.PSCredential ($userName, $secpasswd)

$webAppCMName =  $testingDeployment.WebAppNameCM
$webAppWSName =  $testingDeployment.WebAppNameWS
$webAppSTSName =  $testingDeployment.WebAppNameSTS
#endregion

#region Script Blocks

# Function reads target files and their content, searches for specified nodes in xml


$scriptBlockSetISHServiceUser = {
    param (
        $ishDeployName,
        $credentials

    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }

    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Set-ISHServiceUser -ISHDeployment $ishDeploy -Credential $credentials

}

$scriptBlockEnableISHIISAppPool = {
    param (
        $ishDeployName
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Enable-ISHIISAppPool -ISHDeployment $ishDeploy
}

$scriptBlockDisableISHIISAppPool = {
    param (
        $ishDeployName
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Disable-ISHIISAppPool -ISHDeployment $ishDeploy
}

$scriptBlockGetISHComponent = {
    param (
        $ishDeployName

    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }

    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Get-ISHComponent -ISHDeployment $ishDeploy

}

$scriptBlockGetAppPoolState = {
    param (
        $testingDeployment,
        $webAppCMName,
        $webAppWSName,
        $webAppSTSName
    )

    $cmAppPoolName = ("TrisoftAppPool{0}" -f $webAppCMName)
    $wsAppPoolName = ("TrisoftAppPool{0}" -f $webAppWSName)
    $stsAppPoolName = ("TrisoftAppPool{0}" -f $webAppSTSName)
    
    $result = @{}

    [Array]$array = iex 'C:\Windows\system32\inetsrv\appcmd list wps'
    foreach ($line in $array) {
            $splitedLine = $line.split(" ")
            $processIdAsString = $splitedLine[1]
            $processId = $processIdAsString.Substring(1,$processIdAsString.Length-2)
            if ($splitedLine[2] -match $cmAppPoolName)
            {
                $result["CM"] = "$cmAppPoolName started 1"
            } 
            if ($splitedLine[2] -match $wsAppPoolName)
            {
                $result["WS"] = "$wsAppPoolName started 2"
            }
            if ($splitedLine[2] -match $stsAppPoolName)
            {
                $result["STS"] = "$stsAppPoolName started 3"
            }
        }
    return $result
}

#endregion



Describe "Testing ISHIISAppPool"{
    BeforeEach {
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnableISHIISAppPool -Session $session -ArgumentList $testingDeploymentName
    }

    It "Disable ISHIISAppPool disables AppPools"{

        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisableISHIISAppPool -Session $session -ArgumentList $testingDeploymentName
         #Assert
        $comp = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHComponent -Session $session -ArgumentList $testingDeploymentName
        ($comp | Where-Object -Property Name -EQ "CM").IsEnabled | Should be "False"
        ($comp | Where-Object -Property Name -EQ "CM").IsRunning | Should be "False"
        ($comp | Where-Object -Property Name -EQ "WS").IsEnabled | Should be "False"
        ($comp | Where-Object -Property Name -EQ "WS").IsRunning | Should be "False"
        ($comp | Where-Object -Property Name -EQ "STS").IsEnabled | Should be "False"
        ($comp | Where-Object -Property Name -EQ "STS").IsRunning | Should be "False"
        
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisableISHIISAppPool -Session $session -ArgumentList $testingDeploymentName
         #Assert
        $comp = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHComponent -Session $session -ArgumentList $testingDeploymentName
        ($comp | Where-Object -Property Name -EQ "CM").IsEnabled | Should be "False"
        ($comp | Where-Object -Property Name -EQ "CM").IsRunning | Should be "False"
        ($comp | Where-Object -Property Name -EQ "WS").IsEnabled | Should be "False"
        ($comp | Where-Object -Property Name -EQ "WS").IsRunning | Should be "False"
        ($comp | Where-Object -Property Name -EQ "STS").IsEnabled | Should be "False"
        ($comp | Where-Object -Property Name -EQ "STS").IsRunning | Should be "False"

        $pools = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetAppPoolState -Session $session -ArgumentList $testingDeploymentName, $webAppCMName, $webAppWSName, $webAppSTSName
        $pools.Count | Should be 0
    }
    
    It "Enable ISHIISAppPool enables AppPools"{

        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnableISHIISAppPool -Session $session -ArgumentList $testingDeploymentName
         #Assert
        $comp = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHComponent -Session $session -ArgumentList $testingDeploymentName
        ($comp | Where-Object -Property Name -EQ "CM").IsEnabled | Should be "True"
        ($comp | Where-Object -Property Name -EQ "CM").IsRunning | Should be "True"
        ($comp | Where-Object -Property Name -EQ "WS").IsEnabled | Should be "True"
        ($comp | Where-Object -Property Name -EQ "WS").IsRunning | Should be "True"
        ($comp | Where-Object -Property Name -EQ "STS").IsEnabled | Should be "True"
        ($comp | Where-Object -Property Name -EQ "STS").IsRunning | Should be "True"
        
        $pools = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetAppPoolState -Session $session -ArgumentList $testingDeploymentName, $webAppCMName, $webAppWSName, $webAppSTSName
        $pools.Count | Should be 3
    }
    
    It "Set ISHIISAppPool writes history"{       
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnableISHIISAppPool -Session $session -ArgumentList $testingDeploymentName, $testCreds
        $history = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetHistory -Session $session -ArgumentList $testingDeploymentName
        $history.Contains('Enable-ISHIISAppPool') | Should be "True"

    }
    
    Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnableISHIISAppPool -Session $session -ArgumentList $testingDeploymentName
}