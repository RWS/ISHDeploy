param(
    $session = $null,
    $testingDeploymentName = "InfoShareSQL2014"
)
. "$PSScriptRoot\Common.ps1"

#region variables
$userName = Get-TestDataValue “testDomainUserName”
$userPassword = Get-TestDataValue "testDomainUserPassword"

$secpasswd = ConvertTo-SecureString “$userPassword” -AsPlainText -Force
$testCreds = New-Object System.Management.Automation.PSCredential ($userName, $secpasswd)
#endregion

#region Script Blocks


$scriptBlockGetUsers = {
    param (
        $ishDeployName

    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }

    $ishDeploy = Get-ISHDeployment -Name $ishDeployName

    Import-Module WebAdministration
    
    $result = @{}

    $ishAppPools=Get-ISHDeploymentParameters -ISHDeployment $ishDeploy| Where-Object -Property Name -Like "infoshare*webappname"|ForEach-Object {
        Get-Item "IIS:\AppPools\TrisoftAppPool$($_.Value)"}

    $ishWebSites=Get-ISHDeploymentParameters -ISHDeployment $ishDeploy| Where-Object -Property Name -Like "infoshare*webappname"|ForEach-Object {
        Get-Item "IIS:\Sites\Default Web Site\$($_.Value)"
        }
    $ishWebSites|ForEach-Object {
        $result["Anonymous Authentication $($_.Name)"]=(Get-WebConfigurationProperty -Filter "/system.webServer/security/authentication/anonymousAuthentication" -Name Username -PSPath IIS:\ -Location "Default Web Site/$($_.Name)").Value 
    }
    $ishServices=Get-Service -Name "Trisoft $ishDeploy*"

    # COMPlus
    $comAdmin = New-Object -com ("COMAdmin.COMAdminCatalog.1")
    $catalog = New-Object -com COMAdmin.COMAdminCatalog 
    $applications = $catalog.getcollection("Applications") 
    $applications.populate()
    $trisoftInfoShareAuthorApplication=$applications|Where-Object -Property Name -EQ "Trisoft-InfoShare-Author"
    
    foreach ($pool in $ishAppPools){
        $result[$pool.Name] =  $pool.ProcessModel.UserName
    }

    $ishServices|ForEach-Object {
        # https://gallery.technet.microsoft.com/Powershell-How-to-change-be88ce7e
        $svcD=gwmi win32_service -filter "name like '%$($_.Name)%'" 
        Write-Host "$($_.Name)"
        $result["Service $($_.Name)"]=($svcD.Properties | Where-Object -Property Name -EQ "StartName").Value
    }

    $result["Com+"] = $trisoftInfoShareAuthorApplication.Value("Identity")
    return $result
}

$scriptBlockSetISHOSUser = {
    param (
        $ishDeployName,
        $credentials

    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }

    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Set-ISHOSUser -ISHDeployment $ishDeploy -Credential $credentials

}
#endregion

function remoteGetUsers() {

    #read all files that are touched with commandlet
    $result = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetUsers -Session $session -ArgumentList $testingDeploymentName
    return $result

}


Describe "Testing ISHOSUser"{
    BeforeEach {
        UndoDeploymentBackToVanila $testingDeploymentName $true
    }

    It "Set ISHOSUser set's user correctly"{       
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHOSUser -Session $session -ArgumentList $testingDeploymentName, $testCreds
        $users = remoteGetUsers
        foreach ($key in $users.Keys){
                $tempstring = "Value:"+$users[$key]+ " Key: $key  "
                $shoudBeString = "Value:"+$userName+ " Key: $key  "
                $tempstring | Should be $shoudBeString
        }      
    }
    It "Set ISHOSUser Raises warning for COM+"{       
        #Act

        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHOSUser -Session $session -ArgumentList $testingDeploymentName, $testCreds -WarningVariable Warning
        #Assert
        $message = "The setting of credentials for COM+ components has implications across all deployments."
        Compare-Object $Warning $message | should be $null
    }

    It "Set ISHOSUser writes inputparameters"{       
        #Act

        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHOSUser -Session $session -ArgumentList $testingDeploymentName, $testCreds
        $result = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetInputParameters -Session $session -ArgumentList $testingDeploymentName
        $result["osuser"] | Should be $userName
    }

    It "Set ISHOSUser writes history"{       
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHOSUser -Session $session -ArgumentList $testingDeploymentName, $testCreds
        $history = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetHistory -Session $session -ArgumentList $testingDeploymentName
        $history.Contains('Set-ISHOSUser') | Should be "True"
        $history.Contains("-Credential (New-Object System.Management.Automation.PSCredential") | Should be "True"
    }

    It "Undo ISHDeployment returns all users"{       
        #Act
        $initialUsers =  remoteGetUsers
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHOSUser -Session $session -ArgumentList $testingDeploymentName, $testCreds
        $users = remoteGetUsers
        foreach ($key in $users.Keys){
                $tempstring = "Value:"+$users[$key]+ " Key: $key  "
                $shoudBeString = "Value:"+$userName+ " Key: $key  "
                $tempstring | Should be $shoudBeString
        }      
        UndoDeploymentBackToVanila $testingDeploymentName $true
        $users = remoteGetUsers
        Compare-Object $initialUsers $users | Should be $null
    }
    
    It "Set ISHOSUser raises error for COM+"{       
        #Act
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHOSUser -Session $session -ArgumentList $testingDeploymentName, (New-Object System.Management.Automation.PSCredential ($userName, (ConvertTo-SecureString "TestPassword`"" -AsPlainText -Force))) -ErrorAction Stop }| Should Throw "The identity or password set on the application is not valid"
    }
     UndoDeploymentBackToVanila $testingDeploymentName $true
}