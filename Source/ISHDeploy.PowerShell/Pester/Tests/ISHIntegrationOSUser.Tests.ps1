param(
    $session = $null,
    $testingDeploymentName = "InfoShareSQL2014"
)
. "$PSScriptRoot\Common.ps1"

#region variables
#$licenseKey=Get-TestDataValue "xopusLicenseKey"
#$domain=Get-TestDataValue "xopusLicenseDomain"
$userName = “GLOBAL\infoshareserviceuser”
$userPassword = "!nf0shar3"
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
       # ArtifactCleaner -filePath $filePath -fileName "TranslationOrganizer.exe.config"
        UndoDeploymentBackToVanila $testingDeploymentName $true
    }

    It "Set ISHOSUser"{       
        #Act

        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHOSUser -Session $session -ArgumentList $testingDeploymentName, $testCreds

        $users = remoteGetUsers
        foreach ($key in $users.Keys){
                Write-Host "Value:"$users[$key] "Key: $key  " 
                $tempstring = "Value:"+$users[$key]+ " Key: $key  "
                $shoudBeString = "Value:"+$userName+ " Key: $key  "
                $tempstring | Should be $shoudBeString
        }
        #Assert
        
    }

    
     UndoDeploymentBackToVanila $testingDeploymentName $true
}