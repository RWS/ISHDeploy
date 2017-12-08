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
    $comAdminCatalog = New-Object -com ("COMAdmin.COMAdminCatalog.1")
    $applications = $comAdminCatalog.getcollection("Applications") 
    $applications.populate()
    $trisoftInfoShareAuthorApplication=$applications|Where-Object -Property Name -EQ "Trisoft-InfoShare-Author"
    $trisoftInfoShareAuthorISOApplication=$applications|Where-Object -Property Name -EQ "Trisoft-InfoShare-AuthorIso"
    $trisoftInfoShareUtilitiesApplication=$applications|Where-Object -Property Name -EQ "Trisoft-Utilities"
    $trisoftInfoShareTriDKApplication=$applications|Where-Object -Property Name -EQ "Trisoft-TriDK"
    
    $result["AuthorApplicationIsEnabled"] = $trisoftInfoShareAuthorApplication.Value("IsEnabled")
    $result["ISOApplicationIsEnabled"] = $trisoftInfoShareAuthorISOApplication.Value("IsEnabled")
    $result["UtilitiesApplicationIsEnabled"] = $trisoftInfoShareUtilitiesApplication.Value("IsEnabled")
    $result["TriDKApplicationIsEnabled"] = $trisoftInfoShareTriDKApplication.Value("IsEnabled")
         
    $skeyappli = $trisoftInfoShareAuthorApplication.key 
    $appliInstances = $applications.getcollection("ApplicationInstances", $skeyappli) 
    $appliInstances.populate() 
    If ($appliInstances.count -eq 0) { 
        $result["AuthorApplicationIsStarted"] = $false
    } Else{ 
        $result["AuthorApplicationIsStarted"] = $true
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

$scriptBlockGetISHServiceTranslationBuilder = {
    param (
        $ishDeployName

    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }

    Get-ISHServiceTranslationBuilder -ISHDeployment $ishDeployName
}

$scriptBlockEnableISHServiceTranslationBuilder = {
    param (
        $ishDeployName

    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }

    Enable-ISHServiceTranslationBuilder -ISHDeployment $ishDeployName
}

$scriptBlockDisableISHServiceTranslationBuilder = {
    param (
        $ishDeployName

    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }

    Disable-ISHServiceTranslationBuilder -ISHDeployment $ishDeployName
}

$scriptBlockGetTranslationBuilderServiceState = {
    param (
        $testingDeployment
    )

    $result = @{}
    $service = Get-WmiObject Win32_Service -filter "name like 'Trisoft $testingDeployment TranslationBuilder One'"
    $result["StartupType"] = $service.StartMode
    $result["Status"] = $service.State
    return $result
}

#endregion

#region Assist Functions

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

function CheckStoppedVanillaDeployment() {
    $checkDeployment = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetDeployment -Session $session -ArgumentList $testingDeploymentName
    $checkDeployment.Status | Should be "Stopped" 

    GetComObjectState
    $AuthorApplicationIsEnabled | Should be False  
    $ISOApplicationIsEnabled | Should be False 
    $UtilitiesApplicationIsEnabled | Should be False 
    $TriDKApplicationIsEnabled | Should be False
    $AuthorApplicationIsStarted | Should be False     

    $pools = GetAppPoolState
    $pools.Count | Should be 0   
     
    $components = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHComponent -Session $session -ArgumentList $testingDeploymentName
    ($components | Where-Object -Property Name -EQ "CM").IsEnabled | Should be "True"
    ($components | Where-Object -Property Name -EQ "CM").IsRunning | Should be "False"
    ($components | Where-Object -Property Name -EQ "WS").IsEnabled | Should be "True"
    ($components | Where-Object -Property Name -EQ "WS").IsRunning | Should be "False"
    ($components | Where-Object -Property Name -EQ "STS").IsEnabled | Should be "True"
    ($components | Where-Object -Property Name -EQ "STS").IsRunning | Should be "False"
    ($components | Where-Object -Property Name -EQ "COMPlus").IsEnabled | Should be "True"
    ($components | Where-Object -Property Name -EQ "COMPlus").IsRunning | Should be "False"
    ($components | Where-Object -Property Name -EQ "BackgroundTask").IsEnabled | Should be "False"
    ($components | Where-Object -Property Name -EQ "BackgroundTask").IsRunning | Should be "False"
    ($components | Where-Object -Property Name -EQ "Crawler").IsEnabled | Should be "False"
    ($components | Where-Object -Property Name -EQ "Crawler").IsRunning | Should be "False"
    ($components | Where-Object -Property Name -EQ "SolrLucene").IsEnabled | Should be "False"
    ($components | Where-Object -Property Name -EQ "SolrLucene").IsRunning | Should be "False"
    ($components | Where-Object -Property Name -EQ "TranslationBuilder").IsEnabled | Should be "False"
    ($components | Where-Object -Property Name -EQ "TranslationBuilder").IsRunning | Should be "False"
    ($components | Where-Object -Property Name -EQ "TranslationOrganizer").IsEnabled | Should be "False"  
    ($components | Where-Object -Property Name -EQ "TranslationOrganizer").IsRunning | Should be "False"
}

function CheckStartedVanillaDeployment() {
    $checkDeployment = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetDeployment -Session $session -ArgumentList $testingDeploymentName
    $checkDeployment.Status | Should be "Started" 

    GetComObjectState
    $AuthorApplicationIsEnabled | Should be True  
    $ISOApplicationIsEnabled | Should be True 
    $UtilitiesApplicationIsEnabled | Should be True 
    $TriDKApplicationIsEnabled | Should be True
    $AuthorApplicationIsStarted | Should be True     

    $pools = GetAppPoolState
    $pools.Count | Should be 3   
     
    $components = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHComponent -Session $session -ArgumentList $testingDeploymentName
    ($components | Where-Object -Property Name -EQ "CM").IsEnabled | Should be "True"
    ($components | Where-Object -Property Name -EQ "CM").IsRunning | Should be "True"
    ($components | Where-Object -Property Name -EQ "WS").IsEnabled | Should be "True"
    ($components | Where-Object -Property Name -EQ "WS").IsRunning | Should be "True"
    ($components | Where-Object -Property Name -EQ "STS").IsEnabled | Should be "True"
    ($components | Where-Object -Property Name -EQ "STS").IsRunning | Should be "True"
    ($components | Where-Object -Property Name -EQ "COMPlus").IsEnabled | Should be "True"
    ($components | Where-Object -Property Name -EQ "COMPlus").IsRunning | Should be "True"
    ($components | Where-Object -Property Name -EQ "BackgroundTask").IsEnabled | Should be "False"
    ($components | Where-Object -Property Name -EQ "BackgroundTask").IsRunning | Should be "False"
    ($components | Where-Object -Property Name -EQ "Crawler").IsEnabled | Should be "False"
    ($components | Where-Object -Property Name -EQ "Crawler").IsRunning | Should be "False"
    ($components | Where-Object -Property Name -EQ "SolrLucene").IsEnabled | Should be "False"
    ($components | Where-Object -Property Name -EQ "SolrLucene").IsRunning | Should be "False"
    ($components | Where-Object -Property Name -EQ "TranslationBuilder").IsEnabled | Should be "False"
    ($components | Where-Object -Property Name -EQ "TranslationBuilder").IsRunning | Should be "False"
    ($components | Where-Object -Property Name -EQ "TranslationOrganizer").IsEnabled | Should be "False"  
    ($components | Where-Object -Property Name -EQ "TranslationOrganizer").IsRunning | Should be "False"
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
        CheckStoppedVanillaDeployment
    }
    
    It "Start of stopped ISHDeployment should start deployment"{
    
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockStopISHDeployment -Session $session -ArgumentList $testingDeploymentName
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockStartISHDeployment -Session $session -ArgumentList $testingDeploymentName        
    
        #Assert
        CheckStartedVanillaDeployment
    }
    
    It "Restart of stopped ISHDeployment should start deployment"{
         #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockStopISHDeployment -Session $session -ArgumentList $testingDeploymentName
    
        #Assert
        CheckStoppedVanillaDeployment
        
         #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRestartISHDeployment -Session $session -ArgumentList $testingDeploymentName
    
        #Assert
        CheckStartedVanillaDeployment
    }
    
    It "Start ISHDeployment on already started deployment"{
    
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockStartISHDeployment -Session $session -ArgumentList $testingDeploymentName
    
        #Assert
        CheckStartedVanillaDeployment
    
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockStartISHDeployment -Session $session -ArgumentList $testingDeploymentName
    
        #Assert
        CheckStartedVanillaDeployment
    }
    
	It "Start ISHDeployment should not start disabled components"{
        #Act
		Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisableISHCOMPlus -Session $session -ArgumentList $testingDeploymentName
		Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisableISHIISAppPool -Session $session -ArgumentList $testingDeploymentName
    
        #Assert
        $checkDeployment = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetDeployment -Session $session -ArgumentList $testingDeploymentName
        $checkDeployment.Status | Should be "Started" 
    
        GetComObjectState
        $AuthorApplicationIsEnabled | Should be False  
        $ISOApplicationIsEnabled | Should be False 
        $UtilitiesApplicationIsEnabled | Should be False 
        $TriDKApplicationIsEnabled | Should be False
        $AuthorApplicationIsStarted | Should be False     
    
        $pools = GetAppPoolState
        $pools.Count | Should be 0   
     
        $components = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHComponent -Session $session -ArgumentList $testingDeploymentName
        ($components | Where-Object -Property Name -EQ "CM").IsEnabled | Should be "False"
        ($components | Where-Object -Property Name -EQ "CM").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "WS").IsEnabled | Should be "False"
        ($components | Where-Object -Property Name -EQ "WS").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "STS").IsEnabled | Should be "False"
        ($components | Where-Object -Property Name -EQ "STS").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "COMPlus").IsEnabled | Should be "False"
        ($components | Where-Object -Property Name -EQ "COMPlus").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "BackgroundTask").IsEnabled | Should be "False"
        ($components | Where-Object -Property Name -EQ "BackgroundTask").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "Crawler").IsEnabled | Should be "False"
        ($components | Where-Object -Property Name -EQ "Crawler").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "SolrLucene").IsEnabled | Should be "False"
        ($components | Where-Object -Property Name -EQ "SolrLucene").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "TranslationBuilder").IsEnabled | Should be "False"
        ($components | Where-Object -Property Name -EQ "TranslationBuilder").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "TranslationOrganizer").IsEnabled | Should be "False"  
        ($components | Where-Object -Property Name -EQ "TranslationOrganizer").IsRunning | Should be "False"
        
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockStartISHDeployment -Session $session -ArgumentList $testingDeploymentName
    
        #Assert
        $checkDeployment = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetDeployment -Session $session -ArgumentList $testingDeploymentName
        $checkDeployment.Status | Should be "Started" 
    
        GetComObjectState
        $AuthorApplicationIsEnabled | Should be False  
        $ISOApplicationIsEnabled | Should be False 
        $UtilitiesApplicationIsEnabled | Should be False 
        $TriDKApplicationIsEnabled | Should be False
        $AuthorApplicationIsStarted | Should be False     
    
        $pools = GetAppPoolState
        $pools.Count | Should be 0   
     
        $components = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHComponent -Session $session -ArgumentList $testingDeploymentName
        ($components | Where-Object -Property Name -EQ "CM").IsEnabled | Should be "False"
        ($components | Where-Object -Property Name -EQ "CM").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "WS").IsEnabled | Should be "False"
        ($components | Where-Object -Property Name -EQ "WS").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "STS").IsEnabled | Should be "False"
        ($components | Where-Object -Property Name -EQ "STS").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "COMPlus").IsEnabled | Should be "False"
        ($components | Where-Object -Property Name -EQ "COMPlus").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "BackgroundTask").IsEnabled | Should be "False"
        ($components | Where-Object -Property Name -EQ "BackgroundTask").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "Crawler").IsEnabled | Should be "False"
        ($components | Where-Object -Property Name -EQ "Crawler").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "SolrLucene").IsEnabled | Should be "False"
        ($components | Where-Object -Property Name -EQ "SolrLucene").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "TranslationBuilder").IsEnabled | Should be "False"
        ($components | Where-Object -Property Name -EQ "TranslationBuilder").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "TranslationOrganizer").IsEnabled | Should be "False"  
        ($components | Where-Object -Property Name -EQ "TranslationOrganizer").IsRunning | Should be "False"
    }

	It "Enabling components on stopped deployment should not start components"{
        #Act
		Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisableISHCOMPlus -Session $session -ArgumentList $testingDeploymentName
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisableISHIISAppPool -Session $session -ArgumentList $testingDeploymentName
    
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockStopISHDeployment -Session $session -ArgumentList $testingDeploymentName
    
		#Assert
		$checkDeployment = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetDeployment -Session $session -ArgumentList $testingDeploymentName
        $checkDeployment.Status | Should be "Stopped"
    
        GetComObjectState
        $AuthorApplicationIsEnabled | Should be False  
        $ISOApplicationIsEnabled | Should be False 
        $UtilitiesApplicationIsEnabled | Should be False 
        $TriDKApplicationIsEnabled | Should be False
        $AuthorApplicationIsStarted | Should be False     
    
        $pools = GetAppPoolState
        $pools.Count | Should be 0   
     
        $components = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHComponent -Session $session -ArgumentList $testingDeploymentName
        ($components | Where-Object -Property Name -EQ "CM").IsEnabled | Should be "False"
        ($components | Where-Object -Property Name -EQ "CM").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "WS").IsEnabled | Should be "False"
        ($components | Where-Object -Property Name -EQ "WS").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "STS").IsEnabled | Should be "False"
        ($components | Where-Object -Property Name -EQ "STS").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "COMPlus").IsEnabled | Should be "False"
        ($components | Where-Object -Property Name -EQ "COMPlus").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "BackgroundTask").IsEnabled | Should be "False"
        ($components | Where-Object -Property Name -EQ "BackgroundTask").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "Crawler").IsEnabled | Should be "False"
        ($components | Where-Object -Property Name -EQ "Crawler").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "SolrLucene").IsEnabled | Should be "False"
        ($components | Where-Object -Property Name -EQ "SolrLucene").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "TranslationBuilder").IsEnabled | Should be "False"
        ($components | Where-Object -Property Name -EQ "TranslationBuilder").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "TranslationOrganizer").IsEnabled | Should be "False"  
        ($components | Where-Object -Property Name -EQ "TranslationOrganizer").IsRunning | Should be "False"
    
        #Act
		Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnableISHCOMPlus -Session $session -ArgumentList $testingDeploymentName
    
        #Assert
        GetComObjectState
        $AuthorApplicationIsEnabled | Should be False  
        $ISOApplicationIsEnabled | Should be False 
        $UtilitiesApplicationIsEnabled | Should be False 
        $TriDKApplicationIsEnabled | Should be False
        $AuthorApplicationIsStarted | Should be False     
    
        $pools = GetAppPoolState
        $pools.Count | Should be 0   
     
        $components = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHComponent -Session $session -ArgumentList $testingDeploymentName
        ($components | Where-Object -Property Name -EQ "CM").IsEnabled | Should be "False"
        ($components | Where-Object -Property Name -EQ "CM").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "WS").IsEnabled | Should be "False"
        ($components | Where-Object -Property Name -EQ "WS").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "STS").IsEnabled | Should be "False"
        ($components | Where-Object -Property Name -EQ "STS").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "COMPlus").IsEnabled | Should be "True"
        ($components | Where-Object -Property Name -EQ "COMPlus").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "BackgroundTask").IsEnabled | Should be "False"
        ($components | Where-Object -Property Name -EQ "BackgroundTask").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "Crawler").IsEnabled | Should be "False"
        ($components | Where-Object -Property Name -EQ "Crawler").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "SolrLucene").IsEnabled | Should be "False"
        ($components | Where-Object -Property Name -EQ "SolrLucene").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "TranslationBuilder").IsEnabled | Should be "False"
        ($components | Where-Object -Property Name -EQ "TranslationBuilder").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "TranslationOrganizer").IsEnabled | Should be "False"  
        ($components | Where-Object -Property Name -EQ "TranslationOrganizer").IsRunning | Should be "False" 
    }

    It "Enable TranslationBuilder on started deployment"{
        #Act
		Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnableISHServiceTranslationBuilder -Session $session -ArgumentList $testingDeploymentName

		#Assert
        $checkDeployment = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetDeployment -Session $session -ArgumentList $testingDeploymentName
        $checkDeployment.Status | Should be "Started" 
     
        $components = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHComponent -Session $session -ArgumentList $testingDeploymentName
        ($components | Where-Object -Property Name -EQ "CM").IsEnabled | Should be "True"
        ($components | Where-Object -Property Name -EQ "CM").IsRunning | Should be "True"
        ($components | Where-Object -Property Name -EQ "WS").IsEnabled | Should be "True"
        ($components | Where-Object -Property Name -EQ "WS").IsRunning | Should be "True"
        ($components | Where-Object -Property Name -EQ "STS").IsEnabled | Should be "True"
        ($components | Where-Object -Property Name -EQ "STS").IsRunning | Should be "True"
        ($components | Where-Object -Property Name -EQ "COMPlus").IsEnabled | Should be "True"
        ($components | Where-Object -Property Name -EQ "COMPlus").IsRunning | Should be "True"
        ($components | Where-Object -Property Name -EQ "BackgroundTask").IsEnabled | Should be "False"
        ($components | Where-Object -Property Name -EQ "BackgroundTask").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "Crawler").IsEnabled | Should be "False"
        ($components | Where-Object -Property Name -EQ "Crawler").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "SolrLucene").IsEnabled | Should be "False"
        ($components | Where-Object -Property Name -EQ "SolrLucene").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "TranslationBuilder").IsEnabled | Should be "True"
        ($components | Where-Object -Property Name -EQ "TranslationBuilder").IsRunning | Should be "True"
        ($components | Where-Object -Property Name -EQ "TranslationOrganizer").IsEnabled | Should be "False"  
        ($components | Where-Object -Property Name -EQ "TranslationOrganizer").IsRunning | Should be "False"

        $services = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceTranslationBuilder -Session $session -ArgumentList $testingDeploymentName
        if ($services.GetType().BaseType.Name -ne "Object"){
            $services.Count | Should be 1
        }
        $services[0].Status | Should be "Running"

        $serviceState = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetTranslationBuilderServiceState -Session $session -ArgumentList $testingDeploymentName
        $serviceState["StartupType"] | Should be "Auto"
        $serviceState["Status"] | Should be "Running"
    }

    It "Enable TranslationBuilder on stopped deployment"{
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockStopISHDeployment -Session $session -ArgumentList $testingDeploymentName
		Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnableISHServiceTranslationBuilder -Session $session -ArgumentList $testingDeploymentName
    
		#Assert
        $checkDeployment = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetDeployment -Session $session -ArgumentList $testingDeploymentName
        $checkDeployment.Status | Should be "Stopped" 
     
        $components = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHComponent -Session $session -ArgumentList $testingDeploymentName
        ($components | Where-Object -Property Name -EQ "CM").IsEnabled | Should be "True"
        ($components | Where-Object -Property Name -EQ "CM").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "WS").IsEnabled | Should be "True"
        ($components | Where-Object -Property Name -EQ "WS").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "STS").IsEnabled | Should be "True"
        ($components | Where-Object -Property Name -EQ "STS").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "COMPlus").IsEnabled | Should be "True"
        ($components | Where-Object -Property Name -EQ "COMPlus").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "BackgroundTask").IsEnabled | Should be "False"
        ($components | Where-Object -Property Name -EQ "BackgroundTask").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "Crawler").IsEnabled | Should be "False"
        ($components | Where-Object -Property Name -EQ "Crawler").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "SolrLucene").IsEnabled | Should be "False"
        ($components | Where-Object -Property Name -EQ "SolrLucene").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "TranslationBuilder").IsEnabled | Should be "True"
        ($components | Where-Object -Property Name -EQ "TranslationBuilder").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "TranslationOrganizer").IsEnabled | Should be "False"  
        ($components | Where-Object -Property Name -EQ "TranslationOrganizer").IsRunning | Should be "False"
    
        $services = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceTranslationBuilder -Session $session -ArgumentList $testingDeploymentName
        if ($services.GetType().BaseType.Name -ne "Object"){
            $services.Count | Should be 1
        }
        $services[0].Status | Should be "Stopped"
        
        $serviceState = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetTranslationBuilderServiceState -Session $session -ArgumentList $testingDeploymentName
        $serviceState["StartupType"] | Should be "Manual"
        $serviceState["Status"] | Should be "Stopped"
    }
    
    It "Disable TranslationBuilder after enabling on started deployment"{
        #Act
		Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnableISHServiceTranslationBuilder -Session $session -ArgumentList $testingDeploymentName
    
		#Assert
        $checkDeployment = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetDeployment -Session $session -ArgumentList $testingDeploymentName
        $checkDeployment.Status | Should be "Started" 
     
        $components = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHComponent -Session $session -ArgumentList $testingDeploymentName
        ($components | Where-Object -Property Name -EQ "CM").IsEnabled | Should be "True"
        ($components | Where-Object -Property Name -EQ "CM").IsRunning | Should be "True"
        ($components | Where-Object -Property Name -EQ "WS").IsEnabled | Should be "True"
        ($components | Where-Object -Property Name -EQ "WS").IsRunning | Should be "True"
        ($components | Where-Object -Property Name -EQ "STS").IsEnabled | Should be "True"
        ($components | Where-Object -Property Name -EQ "STS").IsRunning | Should be "True"
        ($components | Where-Object -Property Name -EQ "COMPlus").IsEnabled | Should be "True"
        ($components | Where-Object -Property Name -EQ "COMPlus").IsRunning | Should be "True"
        ($components | Where-Object -Property Name -EQ "BackgroundTask").IsEnabled | Should be "False"
        ($components | Where-Object -Property Name -EQ "BackgroundTask").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "Crawler").IsEnabled | Should be "False"
        ($components | Where-Object -Property Name -EQ "Crawler").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "SolrLucene").IsEnabled | Should be "False"
        ($components | Where-Object -Property Name -EQ "SolrLucene").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "TranslationBuilder").IsEnabled | Should be "True"
        ($components | Where-Object -Property Name -EQ "TranslationBuilder").IsRunning | Should be "True"
        ($components | Where-Object -Property Name -EQ "TranslationOrganizer").IsEnabled | Should be "False"  
        ($components | Where-Object -Property Name -EQ "TranslationOrganizer").IsRunning | Should be "False"
    
        $services = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceTranslationBuilder -Session $session -ArgumentList $testingDeploymentName
        if ($services.GetType().BaseType.Name -ne "Object"){
            $services.Count | Should be 1
        }
        $services[0].Status | Should be "Running"
    
        $serviceState = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetTranslationBuilderServiceState -Session $session -ArgumentList $testingDeploymentName
        $serviceState["StartupType"] | Should be "Auto"
        $serviceState["Status"] | Should be "Running"
    
        #Act
		Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisableISHServiceTranslationBuilder -Session $session -ArgumentList $testingDeploymentName
    
		#Assert
        $checkDeployment = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetDeployment -Session $session -ArgumentList $testingDeploymentName
        $checkDeployment.Status | Should be "Started" 
     
        $components = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHComponent -Session $session -ArgumentList $testingDeploymentName
        ($components | Where-Object -Property Name -EQ "CM").IsEnabled | Should be "True"
        ($components | Where-Object -Property Name -EQ "CM").IsRunning | Should be "True"
        ($components | Where-Object -Property Name -EQ "WS").IsEnabled | Should be "True"
        ($components | Where-Object -Property Name -EQ "WS").IsRunning | Should be "True"
        ($components | Where-Object -Property Name -EQ "STS").IsEnabled | Should be "True"
        ($components | Where-Object -Property Name -EQ "STS").IsRunning | Should be "True"
        ($components | Where-Object -Property Name -EQ "COMPlus").IsEnabled | Should be "True"
        ($components | Where-Object -Property Name -EQ "COMPlus").IsRunning | Should be "True"
        ($components | Where-Object -Property Name -EQ "BackgroundTask").IsEnabled | Should be "False"
        ($components | Where-Object -Property Name -EQ "BackgroundTask").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "Crawler").IsEnabled | Should be "False"
        ($components | Where-Object -Property Name -EQ "Crawler").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "SolrLucene").IsEnabled | Should be "False"
        ($components | Where-Object -Property Name -EQ "SolrLucene").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "TranslationBuilder").IsEnabled | Should be "False"
        ($components | Where-Object -Property Name -EQ "TranslationBuilder").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "TranslationOrganizer").IsEnabled | Should be "False"  
        ($components | Where-Object -Property Name -EQ "TranslationOrganizer").IsRunning | Should be "False"
    
        $services = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceTranslationBuilder -Session $session -ArgumentList $testingDeploymentName
        if ($services.GetType().BaseType.Name -ne "Object"){
            $services.Count | Should be 1
        }
        $services[0].Status | Should be "Stopped"
    
        $serviceState = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetTranslationBuilderServiceState -Session $session -ArgumentList $testingDeploymentName
        $serviceState["StartupType"] | Should be "Manual"
        $serviceState["Status"] | Should be "Stopped"
    }
    
    It "Disable TranslationBuilder after enabling on stopped deployment"{
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockStopISHDeployment -Session $session -ArgumentList $testingDeploymentName
		Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnableISHServiceTranslationBuilder -Session $session -ArgumentList $testingDeploymentName
    
		#Assert
        $checkDeployment = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetDeployment -Session $session -ArgumentList $testingDeploymentName
        $checkDeployment.Status | Should be "Stopped" 
     
        $components = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHComponent -Session $session -ArgumentList $testingDeploymentName
        ($components | Where-Object -Property Name -EQ "CM").IsEnabled | Should be "True"
        ($components | Where-Object -Property Name -EQ "CM").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "WS").IsEnabled | Should be "True"
        ($components | Where-Object -Property Name -EQ "WS").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "STS").IsEnabled | Should be "True"
        ($components | Where-Object -Property Name -EQ "STS").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "COMPlus").IsEnabled | Should be "True"
        ($components | Where-Object -Property Name -EQ "COMPlus").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "BackgroundTask").IsEnabled | Should be "False"
        ($components | Where-Object -Property Name -EQ "BackgroundTask").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "Crawler").IsEnabled | Should be "False"
        ($components | Where-Object -Property Name -EQ "Crawler").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "SolrLucene").IsEnabled | Should be "False"
        ($components | Where-Object -Property Name -EQ "SolrLucene").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "TranslationBuilder").IsEnabled | Should be "True"
        ($components | Where-Object -Property Name -EQ "TranslationBuilder").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "TranslationOrganizer").IsEnabled | Should be "False"  
        ($components | Where-Object -Property Name -EQ "TranslationOrganizer").IsRunning | Should be "False"
    
        $services = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceTranslationBuilder -Session $session -ArgumentList $testingDeploymentName
        if ($services.GetType().BaseType.Name -ne "Object"){
            $services.Count | Should be 1
        }
        $services[0].Status | Should be "Stopped"
        
        $serviceState = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetTranslationBuilderServiceState -Session $session -ArgumentList $testingDeploymentName
        $serviceState["StartupType"] | Should be "Manual"
        $serviceState["Status"] | Should be "Stopped"
                
        #Act
		Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisableISHServiceTranslationBuilder -Session $session -ArgumentList $testingDeploymentName
    
		#Assert
        $checkDeployment = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetDeployment -Session $session -ArgumentList $testingDeploymentName
        $checkDeployment.Status | Should be "Stopped" 
     
        $components = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHComponent -Session $session -ArgumentList $testingDeploymentName
        ($components | Where-Object -Property Name -EQ "CM").IsEnabled | Should be "True"
        ($components | Where-Object -Property Name -EQ "CM").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "WS").IsEnabled | Should be "True"
        ($components | Where-Object -Property Name -EQ "WS").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "STS").IsEnabled | Should be "True"
        ($components | Where-Object -Property Name -EQ "STS").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "COMPlus").IsEnabled | Should be "True"
        ($components | Where-Object -Property Name -EQ "COMPlus").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "BackgroundTask").IsEnabled | Should be "False"
        ($components | Where-Object -Property Name -EQ "BackgroundTask").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "Crawler").IsEnabled | Should be "False"
        ($components | Where-Object -Property Name -EQ "Crawler").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "SolrLucene").IsEnabled | Should be "False"
        ($components | Where-Object -Property Name -EQ "SolrLucene").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "TranslationBuilder").IsEnabled | Should be "False"
        ($components | Where-Object -Property Name -EQ "TranslationBuilder").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "TranslationOrganizer").IsEnabled | Should be "False"  
        ($components | Where-Object -Property Name -EQ "TranslationOrganizer").IsRunning | Should be "False"
    
        $services = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceTranslationBuilder -Session $session -ArgumentList $testingDeploymentName
        if ($services.GetType().BaseType.Name -ne "Object"){
            $services.Count | Should be 1
        }
        $services[0].Status | Should be "Stopped"
        
        $serviceState = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetTranslationBuilderServiceState -Session $session -ArgumentList $testingDeploymentName
        $serviceState["StartupType"] | Should be "Manual"
        $serviceState["Status"] | Should be "Stopped"
    }
    
    It "Stop deployment after enabling TranslationBuilder on started deployment"{
        #Act
		Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnableISHServiceTranslationBuilder -Session $session -ArgumentList $testingDeploymentName
    
		#Assert
        $checkDeployment = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetDeployment -Session $session -ArgumentList $testingDeploymentName
        $checkDeployment.Status | Should be "Started" 
     
        $components = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHComponent -Session $session -ArgumentList $testingDeploymentName
        ($components | Where-Object -Property Name -EQ "CM").IsEnabled | Should be "True"
        ($components | Where-Object -Property Name -EQ "CM").IsRunning | Should be "True"
        ($components | Where-Object -Property Name -EQ "WS").IsEnabled | Should be "True"
        ($components | Where-Object -Property Name -EQ "WS").IsRunning | Should be "True"
        ($components | Where-Object -Property Name -EQ "STS").IsEnabled | Should be "True"
        ($components | Where-Object -Property Name -EQ "STS").IsRunning | Should be "True"
        ($components | Where-Object -Property Name -EQ "COMPlus").IsEnabled | Should be "True"
        ($components | Where-Object -Property Name -EQ "COMPlus").IsRunning | Should be "True"
        ($components | Where-Object -Property Name -EQ "BackgroundTask").IsEnabled | Should be "False"
        ($components | Where-Object -Property Name -EQ "BackgroundTask").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "Crawler").IsEnabled | Should be "False"
        ($components | Where-Object -Property Name -EQ "Crawler").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "SolrLucene").IsEnabled | Should be "False"
        ($components | Where-Object -Property Name -EQ "SolrLucene").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "TranslationBuilder").IsEnabled | Should be "True"
        ($components | Where-Object -Property Name -EQ "TranslationBuilder").IsRunning | Should be "True"
        ($components | Where-Object -Property Name -EQ "TranslationOrganizer").IsEnabled | Should be "False"  
        ($components | Where-Object -Property Name -EQ "TranslationOrganizer").IsRunning | Should be "False"
    
        $services = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceTranslationBuilder -Session $session -ArgumentList $testingDeploymentName
        if ($services.GetType().BaseType.Name -ne "Object"){
            $services.Count | Should be 1
        }
        $services[0].Status | Should be "Running"
    
        $serviceState = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetTranslationBuilderServiceState -Session $session -ArgumentList $testingDeploymentName
        $serviceState["StartupType"] | Should be "Auto"
        $serviceState["Status"] | Should be "Running"
    
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockStopISHDeployment -Session $session -ArgumentList $testingDeploymentName
    
		#Assert
        $checkDeployment = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetDeployment -Session $session -ArgumentList $testingDeploymentName
        $checkDeployment.Status | Should be "Stopped" 
     
        $components = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHComponent -Session $session -ArgumentList $testingDeploymentName
        ($components | Where-Object -Property Name -EQ "CM").IsEnabled | Should be "True"
        ($components | Where-Object -Property Name -EQ "CM").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "WS").IsEnabled | Should be "True"
        ($components | Where-Object -Property Name -EQ "WS").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "STS").IsEnabled | Should be "True"
        ($components | Where-Object -Property Name -EQ "STS").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "COMPlus").IsEnabled | Should be "True"
        ($components | Where-Object -Property Name -EQ "COMPlus").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "BackgroundTask").IsEnabled | Should be "False"
        ($components | Where-Object -Property Name -EQ "BackgroundTask").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "Crawler").IsEnabled | Should be "False"
        ($components | Where-Object -Property Name -EQ "Crawler").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "SolrLucene").IsEnabled | Should be "False"
        ($components | Where-Object -Property Name -EQ "SolrLucene").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "TranslationBuilder").IsEnabled | Should be "True"
        ($components | Where-Object -Property Name -EQ "TranslationBuilder").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "TranslationOrganizer").IsEnabled | Should be "False"  
        ($components | Where-Object -Property Name -EQ "TranslationOrganizer").IsRunning | Should be "False"
    
        $services = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceTranslationBuilder -Session $session -ArgumentList $testingDeploymentName
        if ($services.GetType().BaseType.Name -ne "Object"){
            $services.Count | Should be 1
        }
        $services[0].Status | Should be "Stopped"
    
        $serviceState = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetTranslationBuilderServiceState -Session $session -ArgumentList $testingDeploymentName
        $serviceState["StartupType"] | Should be "Manual"
        $serviceState["Status"] | Should be "Stopped"
    }
    
    It "Start deployment after enabling TranslationBuilder on stopped deployment"{
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockStopISHDeployment -Session $session -ArgumentList $testingDeploymentName
		Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnableISHServiceTranslationBuilder -Session $session -ArgumentList $testingDeploymentName
    
		#Assert
        $checkDeployment = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetDeployment -Session $session -ArgumentList $testingDeploymentName
        $checkDeployment.Status | Should be "Stopped" 
     
        $components = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHComponent -Session $session -ArgumentList $testingDeploymentName
        ($components | Where-Object -Property Name -EQ "CM").IsEnabled | Should be "True"
        ($components | Where-Object -Property Name -EQ "CM").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "WS").IsEnabled | Should be "True"
        ($components | Where-Object -Property Name -EQ "WS").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "STS").IsEnabled | Should be "True"
        ($components | Where-Object -Property Name -EQ "STS").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "COMPlus").IsEnabled | Should be "True"
        ($components | Where-Object -Property Name -EQ "COMPlus").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "BackgroundTask").IsEnabled | Should be "False"
        ($components | Where-Object -Property Name -EQ "BackgroundTask").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "Crawler").IsEnabled | Should be "False"
        ($components | Where-Object -Property Name -EQ "Crawler").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "SolrLucene").IsEnabled | Should be "False"
        ($components | Where-Object -Property Name -EQ "SolrLucene").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "TranslationBuilder").IsEnabled | Should be "True"
        ($components | Where-Object -Property Name -EQ "TranslationBuilder").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "TranslationOrganizer").IsEnabled | Should be "False"  
        ($components | Where-Object -Property Name -EQ "TranslationOrganizer").IsRunning | Should be "False"
    
        $services = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceTranslationBuilder -Session $session -ArgumentList $testingDeploymentName
        if ($services.GetType().BaseType.Name -ne "Object"){
            $services.Count | Should be 1
        }
        $services[0].Status | Should be "Stopped"
        
        $serviceState = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetTranslationBuilderServiceState -Session $session -ArgumentList $testingDeploymentName
        $serviceState["StartupType"] | Should be "Manual"
        $serviceState["Status"] | Should be "Stopped"
    
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockStartISHDeployment -Session $session -ArgumentList $testingDeploymentName
    
		#Assert
        $checkDeployment = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetDeployment -Session $session -ArgumentList $testingDeploymentName
        $checkDeployment.Status | Should be "Started" 
     
        $components = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHComponent -Session $session -ArgumentList $testingDeploymentName
        ($components | Where-Object -Property Name -EQ "CM").IsEnabled | Should be "True"
        ($components | Where-Object -Property Name -EQ "CM").IsRunning | Should be "True"
        ($components | Where-Object -Property Name -EQ "WS").IsEnabled | Should be "True"
        ($components | Where-Object -Property Name -EQ "WS").IsRunning | Should be "True"
        ($components | Where-Object -Property Name -EQ "STS").IsEnabled | Should be "True"
        ($components | Where-Object -Property Name -EQ "STS").IsRunning | Should be "True"
        ($components | Where-Object -Property Name -EQ "COMPlus").IsEnabled | Should be "True"
        ($components | Where-Object -Property Name -EQ "COMPlus").IsRunning | Should be "True"
        ($components | Where-Object -Property Name -EQ "BackgroundTask").IsEnabled | Should be "False"
        ($components | Where-Object -Property Name -EQ "BackgroundTask").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "Crawler").IsEnabled | Should be "False"
        ($components | Where-Object -Property Name -EQ "Crawler").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "SolrLucene").IsEnabled | Should be "False"
        ($components | Where-Object -Property Name -EQ "SolrLucene").IsRunning | Should be "False"
        ($components | Where-Object -Property Name -EQ "TranslationBuilder").IsEnabled | Should be "True"
        ($components | Where-Object -Property Name -EQ "TranslationBuilder").IsRunning | Should be "True"
        ($components | Where-Object -Property Name -EQ "TranslationOrganizer").IsEnabled | Should be "False"  
        ($components | Where-Object -Property Name -EQ "TranslationOrganizer").IsRunning | Should be "False"
    
        $services = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceTranslationBuilder -Session $session -ArgumentList $testingDeploymentName
        if ($services.GetType().BaseType.Name -ne "Object"){
            $services.Count | Should be 1
        }
        $services[0].Status | Should be "Running"
        
        $serviceState = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetTranslationBuilderServiceState -Session $session -ArgumentList $testingDeploymentName
        $serviceState["StartupType"] | Should be "Auto"
        $serviceState["Status"] | Should be "Running"
    
    }

    UndoDeploymentBackToVanila $testingDeploymentName
}