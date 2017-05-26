param(
    $session = $null,
    $testingDeploymentName = "InfoShareSQL2014"
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

#$userName = Get-TestDataValue “testDomainUserName”
#$userPassword = Get-TestDataValue "testDomainUserPassword"

#$secpasswd = ConvertTo-SecureString “$userPassword” -AsPlainText -Force
#$testCreds = New-Object System.Management.Automation.PSCredential ($userName, $secpasswd)

$webAppCMName =  $testingDeployment.WebAppNameCM
$webAppWSName =  $testingDeployment.WebAppNameWS
$webAppSTSName =  $testingDeployment.WebAppNameSTS
#endregion

#region Script Blocks

$scriptBlockEnableISHCOMPlus = {
    param (
        $ishDeployName
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Enable-ISHCOMPlus -ISHDeployment $ishDeploy
}

$scriptBlockDisableISHCOMPlus = {
    param (
        $ishDeployName
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Disable-ISHCOMPlus -ISHDeployment $ishDeploy
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

$scriptBlockStartISHDeployment = {
    param (
        $ishDeployName
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Start-ISHDeployment -ISHDeployment $ishDeploy
}

$scriptBlockStopISHDeployment = {
    param (
        $ishDeployName
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Stop-ISHDeployment -ISHDeployment $ishDeploy
}

$scriptBlockRestartISHDeployment = {
    param (
        $ishDeployName
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Restart-ISHDeployment -ISHDeployment $ishDeploy
}

$scriptBlockGetCOMState = {
    param (
        $testingDeployment
    )

    $result = @{}
    $comAdmin = New-Object -com ("COMAdmin.COMAdminCatalog.1")
    $catalog = New-Object -com COMAdmin.COMAdminCatalog 
    $applications = $catalog.getcollection("Applications") 
    $applications.populate()
    $trisoftInfoShareAuthorApplication=$applications|Where-Object -Property Name -EQ "Trisoft-InfoShare-Author"
    $trisoftInfoShareAuthorISOApplication=$applications|Where-Object -Property Name -EQ "Trisoft-InfoShare-AuthorIso"
    $trisoftInfoShareUtilitiesApplication=$applications|Where-Object -Property Name -EQ "Trisoft-Utilities"
    $trisoftInfoShareTriDKApplication=$applications|Where-Object -Property Name -EQ "Trisoft-TriDK"
    
    $result["AuthorApplication"] = $trisoftInfoShareAuthorApplication.Value("IsEnabled")
    $result["ISOApplication"] = $trisoftInfoShareAuthorISOApplication.Value("IsEnabled")
    $result["UtilitiesApplication"] = $trisoftInfoShareUtilitiesApplication.Value("IsEnabled")
    $result["TriDKApplication"] = $trisoftInfoShareTriDKApplication.Value("IsEnabled")

    return $result
}

function GetComObjectState() {

    #read all files that are touched with commandlet
    $result = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetCOMState -Session $session -ArgumentList $filePath
    
    #get variables and nodes from files


   $global:AuthorApplication =$result["AuthorApplication"]  
   $global:ISOApplication = $result["ISOApplication"] 
   $global:UtilitiesApplication = $result["UtilitiesApplication"]
   $global:TriDKApplication = $result["TriDKApplication"]  
   
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



Describe "Testing Start and Stop ISH Deployment"{
    BeforeEach {
        UndoDeploymentBackToVanila $testingDeploymentName $true
    }

    It "Stop ISHDeployment stops deployment"{

        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockStopISHDeployment -Session $session -ArgumentList $testingDeploymentName
        #Assert
        
        GetComObjectState
        $AuthorApplication | Should be False  
        $ISOApplication | Should be False 
        $UtilitiesApplication | Should be False 
        $TriDKApplication | Should be False 

        $pools = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetAppPoolState -Session $session -ArgumentList $testingDeploymentName, $webAppCMName, $webAppWSName, $webAppSTSName
        $pools.Count | Should be 0    
        $checkDeployment = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetDeployment -Session $session -ArgumentList $testingDeploymentName
        $checkDeployment.Status | Should be "Stopped"    
    }
    
    It "Start ISHDeployment stops deployment"{

        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockStopISHDeployment -Session $session -ArgumentList $testingDeploymentName
        #Assert
        
        GetComObjectState
        $AuthorApplication | Should be False  
        $ISOApplication | Should be False 
        $UtilitiesApplication | Should be False 
        $TriDKApplication | Should be False 
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockStartISHDeployment -Session $session -ArgumentList $testingDeploymentName

        GetComObjectState
        $AuthorApplication | Should be True  
        $ISOApplication | Should be True
        $UtilitiesApplication | Should be True 
        $TriDKApplication | Should be True

        $pools = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetAppPoolState -Session $session -ArgumentList $testingDeploymentName, $webAppCMName, $webAppWSName, $webAppSTSName
        $pools.Count | Should be 3
        $checkDeployment = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetDeployment -Session $session -ArgumentList $testingDeploymentName
        $checkDeployment.Status | Should be "Running"

    }

    It "Restart ISHDeployment restarts deployment"{
        #Act
        GetComObjectState
        $AuthorApplication | Should be True  
        $ISOApplication | Should be True
        $UtilitiesApplication | Should be True 
        $TriDKApplication | Should be True

        $pools = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetAppPoolState -Session $session -ArgumentList $testingDeploymentName, $webAppCMName, $webAppWSName, $webAppSTSName
        $pools.Count | Should be 3
        
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRestartISHDeployment -Session $session -ArgumentList $testingDeploymentName
        #Assert
        GetComObjectState
        $AuthorApplication | Should be True  
        $ISOApplication | Should be True
        $UtilitiesApplication | Should be True 
        $TriDKApplication | Should be True

        $pools = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetAppPoolState -Session $session -ArgumentList $testingDeploymentName, $webAppCMName, $webAppWSName, $webAppSTSName
        $pools.Count | Should be 3

    }

    It "Start ISHDeployment on already started deployment"{

        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockStartISHDeployment -Session $session -ArgumentList $testingDeploymentName
        #Assert
        
        GetComObjectState
        $AuthorApplication | Should be True  
        $ISOApplication | Should be True
        $UtilitiesApplication | Should be True 
        $TriDKApplication | Should be True
        $pools = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetAppPoolState -Session $session -ArgumentList $testingDeploymentName, $webAppCMName, $webAppWSName, $webAppSTSName
        $pools.Count | Should be 3
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockStartISHDeployment -Session $session -ArgumentList $testingDeploymentName

        GetComObjectState
        $AuthorApplication | Should be True  
        $ISOApplication | Should be True
        $UtilitiesApplication | Should be True 
        $TriDKApplication | Should be True

        $pools = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetAppPoolState -Session $session -ArgumentList $testingDeploymentName, $webAppCMName, $webAppWSName, $webAppSTSName
        $pools.Count | Should be 3

    }
    #>
     UndoDeploymentBackToVanila $testingDeploymentName $true
}