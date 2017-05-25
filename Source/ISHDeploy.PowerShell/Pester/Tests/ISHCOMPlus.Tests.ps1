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
    }
    
    It "Enable ISHCOMPlus writes history"{       
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnableISHCOMPlus -Session $session -ArgumentList $testingDeploymentName, $testCreds
        $history = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetHistory -Session $session -ArgumentList $testingDeploymentName
        $history.Contains('Enable-ISHCOMPlus') | Should be "True"

    }
    UndoDeploymentBackToVanila $testingDeploymentName $true
}