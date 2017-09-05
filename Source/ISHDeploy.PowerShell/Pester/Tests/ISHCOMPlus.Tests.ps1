param(
    $session = $null,
    $testingDeploymentName = "InfoShareSQL"
)

. "$PSScriptRoot\Common.ps1"

#region variables

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
    
    $result["AuthorApplication"] = $trisoftInfoShareAuthorApplication.Value("IsEnabled")
    $result["ISOApplication"] = $trisoftInfoShareAuthorISOApplication.Value("IsEnabled")
    $result["UtilitiesApplication"] = $trisoftInfoShareUtilitiesApplication.Value("IsEnabled")
    $result["TriDKApplication"] = $trisoftInfoShareTriDKApplication.Value("IsEnabled")

    return $result
}

$scriptBlockGetCOMRunningOrNot = {
    param (
        $testingDeployment
    )
    $result = @{}
    $catalog = [ISHDeploy.Data.Managers.COMAdminCatalogWrapperSingleton]::Instance
    $applications = $catalog.GetApplications()
    
    $trisoftInfoShareAuthorApplication = $applications|Where-Object -Property Name -EQ "Trisoft-InfoShare-Author"
    
    $applicationsInstances = $catalog.GetApplicationInstances()

    foreach ($component in $applicationsInstances)
    {
        if ($component.Value("Application") -EQ $trisoftInfoShareAuthorApplication.Key)
        {
            return $true
        }
    }

    return $false
}

function GetComObjectState() {
    $result = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetCOMState -Session $session -ArgumentList $testingDeployment
    
    $global:AuthorApplication =$result["AuthorApplication"]  
    $global:ISOApplication = $result["ISOApplication"] 
    $global:UtilitiesApplication = $result["UtilitiesApplication"]
    $global:TriDKApplication = $result["TriDKApplication"]  
   
    $global:AuthorApplicationIsRunning = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetCOMRunningOrNot -Session $session -ArgumentList $testingDeployment 
}
#endregion



Describe "Testing ISHCOMPlus"{
    BeforeEach {
        UndoDeploymentBackToVanila $testingDeploymentName $true
    }

    It "Disable ISHCOMPlus disables Com+"{

        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisableISHCOMPlus -Session $session -ArgumentList $testingDeploymentName
         #Assert
        $comp = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHComponent -Session $session -ArgumentList $testingDeploymentName
        ($comp | Where-Object -Property Name -EQ "COMPlus").IsEnabled | Should be "False"

        GetComObjectState
        $AuthorApplication | Should be False  
        $ISOApplication | Should be False 
        $UtilitiesApplication | Should be False 
        $TriDKApplication | Should be False 
        $AuthorApplicationIsRunning | Should be False  
    }
    
    It "Enable ISHCOMPlus enables Com+"{

        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisableISHCOMPlus -Session $session -ArgumentList $testingDeploymentName
         #Assert
        $comp = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHComponent -Session $session -ArgumentList $testingDeploymentName
        ($comp | Where-Object -Property Name -EQ "COMPlus").IsEnabled | Should be "False"
        

        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnableISHCOMPlus -Session $session -ArgumentList $testingDeploymentName
         #Assert
        $comp = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHComponent -Session $session -ArgumentList $testingDeploymentName
        ($comp | Where-Object -Property Name -EQ "COMPlus").IsEnabled | Should be "True"

        GetComObjectState
        $AuthorApplication | Should be True  
        $ISOApplication | Should be True 
        $UtilitiesApplication | Should be True 
        $TriDKApplication | Should be True
        $AuthorApplicationIsRunning | Should be True 
    }
    
    It "Enable ISHCOMPlus writes history"{       
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnableISHCOMPlus -Session $session -ArgumentList $testingDeploymentName, $testCreds
        $history = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetHistory -Session $session -ArgumentList $testingDeploymentName
        $history.Contains('Enable-ISHCOMPlus') | Should be "True"

    }
    UndoDeploymentBackToVanila $testingDeploymentName $true
}