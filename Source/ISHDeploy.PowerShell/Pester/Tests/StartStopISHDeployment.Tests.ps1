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
    Enable-ISHCOMPlus -ISHDeployment $ishDeployName
}

$scriptBlockDisableISHCOMPlus = {
    param (
        $ishDeployName
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
    Disable-ISHCOMPlus -ISHDeployment $ishDeployName
}

$scriptBlockGetISHComponent = {
    param (
        $ishDeployName

    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }

    Get-ISHComponent -ISHDeployment $ishDeployName

}

$scriptBlockStartISHDeployment = {
    param (
        $ishDeployName
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
    Start-ISHDeployment -ISHDeployment $ishDeployName
}

$scriptBlockStopISHDeployment = {
    param (
        $ishDeployName
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
    Stop-ISHDeployment -ISHDeployment $ishDeployName
}

$scriptBlockRestartISHDeployment = {
    param (
        $ishDeployName
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
    Restart-ISHDeployment -ISHDeployment $ishDeployName
}

$scriptBlockGetCOMState = {
    param (
        $testingDeployment
    )

    $result = @{}
    $catalog = [ISHDeploy.Data.Managers.COMAdminCatalogWrapperSingleton]::Instance
    $applications = $catalog.GetApplications()
    $trisoftInfoShareAuthorApplication=$applications|Where-Object -Property Name -EQ "Trisoft-InfoShare-Author"
    $trisoftInfoShareAuthorISOApplication=$applications|Where-Object -Property Name -EQ "Trisoft-InfoShare-AuthorIso"
    $trisoftInfoShareUtilitiesApplication=$applications|Where-Object -Property Name -EQ "Trisoft-Utilities"
    $trisoftInfoShareTriDKApplication=$applications|Where-Object -Property Name -EQ "Trisoft-TriDK"
    
    $result["AuthorApplicationIsEnabled"] = $trisoftInfoShareAuthorApplication.Value("IsEnabled")
    $result["ISOApplicationIsEnabled"] = $trisoftInfoShareAuthorISOApplication.Value("IsEnabled")
    $result["UtilitiesApplicationIsEnabled"] = $trisoftInfoShareUtilitiesApplication.Value("IsEnabled")
    $result["TriDKApplicationIsEnabled"] = $trisoftInfoShareTriDKApplication.Value("IsEnabled")

    Start-Sleep -Milliseconds 3000

    $comAdminCatalog = New-Object -com ("COMAdmin.COMAdminCatalog.1")
    $oapplications = $comAdminCatalog.getcollection("Applications") 
    $oapplications.populate()
    foreach ($oapplication in $oapplications){ 
        if ($oapplication.Name.ToString() -Match "Trisoft-InfoShare-Author")
        {        
            $skeyappli = $oapplication.key 
            $oappliInstances = $oapplications.getcollection("ApplicationInstances", $skeyappli) 
            $oappliInstances.populate() 
            If ($oappliInstances.count -eq 0) { 
                $result["AuthorApplicationIsStarted"] = $false
            } Else{ 
                $result["AuthorApplicationIsStarted"] = $true
            }
        }
    }
    return $result
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
function GetComObjectState() {

    #read all files that are touched with commandlet
    $result = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetCOMState -Session $session -ArgumentList $filePath
    
    #get variables and nodes from files


   $global:AuthorApplicationIsEnabled =$result["AuthorApplicationIsEnabled"]  
   $global:ISOApplicationIsEnabled = $result["ISOApplicationIsEnabled"] 
   $global:UtilitiesApplicationIsEnabled = $result["UtilitiesApplicationIsEnabled"]
   $global:TriDKApplicationIsEnabled = $result["TriDKApplicationIsEnabled"]  
   $global:AuthorApplicationIsStarted = $result["AuthorApplicationIsStarted"] 
   
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
        UndoDeploymentBackToVanila $testingDeploymentName
    }

    It "Stop ISHDeployment should stop deployment"{

        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockStopISHDeployment -Session $session -ArgumentList $testingDeploymentName
        #Assert
        
        GetComObjectState
        $AuthorApplicationIsEnabled | Should be True  
        $ISOApplicationIsEnabled | Should be True 
        $UtilitiesApplicationIsEnabled | Should be True 
        $TriDKApplicationIsEnabled | Should be True
        $AuthorApplicationIsStarted | Should be False

        $pools = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetAppPoolState -Session $session -ArgumentList $testingDeploymentName, $webAppCMName, $webAppWSName, $webAppSTSName
        $pools.Count | Should be 0    
        $checkDeployment = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetDeployment -Session $session -ArgumentList $testingDeploymentName
        $checkDeployment.Status | Should be "Stopped"    
    }
    
    It "Start of stopped ISHDeployment should start deployment"{

        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockStopISHDeployment -Session $session -ArgumentList $testingDeploymentName
        #Assert
        
        GetComObjectState
        $AuthorApplicationIsEnabled | Should be True  
        $ISOApplicationIsEnabled | Should be True 
        $UtilitiesApplicationIsEnabled | Should be True 
        $TriDKApplicationIsEnabled | Should be True
        $AuthorApplicationIsStarted | Should be False

        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockStartISHDeployment -Session $session -ArgumentList $testingDeploymentName

        GetComObjectState
        $AuthorApplicationIsEnabled | Should be True  
        $ISOApplicationIsEnabled | Should be True 
        $UtilitiesApplicationIsEnabled | Should be True 
        $TriDKApplicationIsEnabled | Should be True
        $AuthorApplicationIsStarted | Should be True

        $pools = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetAppPoolState -Session $session -ArgumentList $testingDeploymentName, $webAppCMName, $webAppWSName, $webAppSTSName
        $pools.Count | Should be 3
        $checkDeployment = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetDeployment -Session $session -ArgumentList $testingDeploymentName
        $checkDeployment.Status | Should be "Started"
    }

    It "Restart of stopped ISHDeployment should start deployment"{
         #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockStopISHDeployment -Session $session -ArgumentList $testingDeploymentName
        #Assert
        
        GetComObjectState
        $AuthorApplicationIsEnabled | Should be True  
        $ISOApplicationIsEnabled | Should be True 
        $UtilitiesApplicationIsEnabled | Should be True 
        $TriDKApplicationIsEnabled | Should be True
        $AuthorApplicationIsStarted | Should be False
        
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRestartISHDeployment -Session $session -ArgumentList $testingDeploymentName
        #Assert
        GetComObjectState
        $AuthorApplicationIsEnabled | Should be True  
        $ISOApplicationIsEnabled | Should be True 
        $UtilitiesApplicationIsEnabled | Should be True 
        $TriDKApplicationIsEnabled | Should be True
        $AuthorApplicationIsStarted | Should be True

        $pools = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetAppPoolState -Session $session -ArgumentList $testingDeploymentName, $webAppCMName, $webAppWSName, $webAppSTSName
        $pools.Count | Should be 3
    }

    It "Start ISHDeployment on already started deployment"{

        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockStartISHDeployment -Session $session -ArgumentList $testingDeploymentName
        #Assert
        
        GetComObjectState
        $AuthorApplicationIsEnabled | Should be True  
        $ISOApplicationIsEnabled | Should be True 
        $UtilitiesApplicationIsEnabled | Should be True 
        $TriDKApplicationIsEnabled | Should be True
        $AuthorApplicationIsStarted | Should be True
        $pools = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetAppPoolState -Session $session -ArgumentList $testingDeploymentName, $webAppCMName, $webAppWSName, $webAppSTSName
        $pools.Count | Should be 3
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockStartISHDeployment -Session $session -ArgumentList $testingDeploymentName

        GetComObjectState
        $AuthorApplicationIsEnabled | Should be True  
        $ISOApplicationIsEnabled | Should be True 
        $UtilitiesApplicationIsEnabled | Should be True 
        $TriDKApplicationIsEnabled | Should be True
        $AuthorApplicationIsStarted | Should be True

        $pools = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetAppPoolState -Session $session -ArgumentList $testingDeploymentName, $webAppCMName, $webAppWSName, $webAppSTSName
        $pools.Count | Should be 3

    }

	It "Start ISHDeployment should not start disabled components"{
        #Act
		Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisableISHCOMPlus -Session $session -ArgumentList $testingDeploymentName
		Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisableISHIISAppPool -Session $session -ArgumentList $testingDeploymentName

        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockStartISHDeployment -Session $session -ArgumentList $testingDeploymentName
        #Assert
        GetComObjectState
        $AuthorApplicationIsEnabled | Should be False  
        $ISOApplicationIsEnabled | Should be False 
        $UtilitiesApplicationIsEnabled | Should be False 
        $TriDKApplicationIsEnabled | Should be False
        $AuthorApplicationIsStarted | Should be False

        $pools = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetAppPoolState -Session $session -ArgumentList $testingDeploymentName, $webAppCMName, $webAppWSName, $webAppSTSName
        $pools.Count | Should be 0  
    }

	It "Enabling components on stopped deployment should not start components"{
         #Act
		Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisableISHCOMPlus -Session $session -ArgumentList $testingDeploymentName
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisableISHIISAppPool -Session $session -ArgumentList $testingDeploymentName

        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockStopISHDeployment -Session $session -ArgumentList $testingDeploymentName

        GetComObjectState
        $AuthorApplicationIsEnabled | Should be False  
        $ISOApplicationIsEnabled | Should be False 
        $UtilitiesApplicationIsEnabled | Should be False 
        $TriDKApplicationIsEnabled | Should be False 
        $AuthorApplicationIsStarted | Should be False

        $pools = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetAppPoolState -Session $session -ArgumentList $testingDeploymentName, $webAppCMName, $webAppWSName, $webAppSTSName
        $pools.Count | Should be 0      
		$checkDeployment = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetDeployment -Session $session -ArgumentList $testingDeploymentName
        $checkDeployment.Status | Should be "Stopped"
		#Assert
		Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnableISHCOMPlus -Session $session -ArgumentList $testingDeploymentName
		GetComObjectState
        $AuthorApplicationIsEnabled | Should be True  
        $ISOApplicationIsEnabled | Should be True 
        $UtilitiesApplicationIsEnabled | Should be True 
        $TriDKApplicationIsEnabled | Should be True
        $AuthorApplicationIsStarted | Should be False
    }

    UndoDeploymentBackToVanila $testingDeploymentName
}