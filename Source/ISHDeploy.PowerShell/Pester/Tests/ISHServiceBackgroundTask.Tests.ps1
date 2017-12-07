param(
    $session = $null,
    $testingDeploymentName = "InfoShareSQL2014"
)
. "$PSScriptRoot\Common.ps1"

#region variables
$moduleName = Invoke-CommandRemoteOrLocal -ScriptBlock { (Get-Module "ISHDeploy.*").Name } -Session $session

$xmlPath = $webPath.ToString().replace(":", "$")
$xmlPath = "\\$computerName\$xmlPath"
$filePath = $appPath.ToString().replace(":", "$")
$filePath = "\\$computerName\$filePath"
$filePath = Join-Path $filePath "BackgroundTask\Bin"

$MaxObjectsInOnePush = 500
$MaxJobItemsCreatedInOneCall = 250
$CompletedJobLifeSpan = "0.00:01:00.000"
$JobProcessingTimeout = "00:11:00.000"
$JobPollingInterval = "00:11:10.000"
$PendingJobPollingInterval = "00:11:11.000"
#endregion

#region Script Blocks
$scriptBlockSetISHServiceBackgroundTask = {
    param (
        $ishDeployName,
        $parameters

    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }

    Set-ISHServiceBackgroundTask -ISHDeployment $ishDeployName @parameters
}

$scriptBlockGetISHServiceBackgroundTask = {
    param (
        $ishDeployName,
        $parameters

    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }

    if($parameters -eq $null) {
        $parameters = @{}
    }

    Get-ISHServiceBackgroundTask -ISHDeployment $ishDeployName @parameters
}

$scriptBlockEnableISHServiceBackgroundTask = {
    param (
        $ishDeployName,
        $parameters

    )
    if ($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }

    if($parameters -eq $null) {
        $parameters = @{}
    }
    
    Enable-ISHServiceBackgroundTask -ISHDeployment $ishDeployName @parameters   
}

$scriptBlockDisableISHServiceBackgroundTask = {
    param (
        $ishDeployName,
        $parameters

    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }


    if($parameters -eq $null) {
        $parameters = @{}
    }
        
    Disable-ISHServiceBackgroundTask -ISHDeployment $ishDeployName @parameters

}

$scriptBlockRemoveISHServiceBackgroundTask = {
    param (
        $ishDeployName,
        $parameters

    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }

    Remove-ISHServiceBackgroundTask -ISHDeployment $ishDeployName @parameters
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

#endregion


# Function reads target files and their content, searches for specified nodes in xml
$scriptBlockReadTargetXML = {
    param(
        $MaxObjectsInOnePush,
        $filePath
        
    )
    #read all files that are touched with commandlet
    
    [System.Xml.XmlDocument]$builderConfig = new-object System.Xml.XmlDocument
    $builderConfig.load("$filePath\BackgroundTask.exe.config")

    
    $result =  @{}
    #get variables and nodes from files
    $result["MaxObjectsInOnePush"] = $builderConfig.SelectNodes("configuration/trisoft.infoShare.BackgroundTask/settings").maxObjectsInOnePushTranslation 
    $result["MaxJobItemsCreatedInOneCall"] = $builderConfig.SelectNodes("configuration/trisoft.infoShare.BackgroundTask/settings").maxTranslationJobItemsCreatedInOneCall
    $result["CompletedJobLifeSpan"] = $builderConfig.SelectNodes("configuration/trisoft.infoShare.BackgroundTask/settings").completedJobLifeSpan 
    $result["JobProcessingTimeout"] = $builderConfig.SelectNodes("configuration/trisoft.infoShare.BackgroundTask/settings").jobProcessingTimeout  
    $result["JobPollingInterval"] = $builderConfig.SelectNodes("configuration/trisoft.infoShare.BackgroundTask/settings").jobPollingInterval
    $result["PendingJobPollingInterval"] = $builderConfig.SelectNodes("configuration/trisoft.infoShare.BackgroundTask/settings").pendingJobPollingInterval

    return $result
}
function remoteReadTargetXML() {

    #read all files that are touched with commandlet
    $result = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockReadTargetXML -Session $session -ArgumentList $MaxObjectsInOnePush, $filePath
    
    #get variables and nodes from files
    $global:MaxObjectsInOnePushFromFile = $result["MaxObjectsInOnePush"]
    $global:MaxJobItemsCreatedInOneCallFromFile = $result["MaxJobItemsCreatedInOneCall"]
    $global:CompletedJobLifeSpanFromFile = $result["CompletedJobLifeSpan"]
    $global:JobProcessingTimeoutFromFile = $result["JobProcessingTimeout"]
    $global:JobPollingIntervalFromFile = $result["JobPollingInterval"]
    $global:PendingJobPollingIntervalFromFile = $result["PendingJobPollingInterval"]
}


Describe "Testing ISHServiceBackgroundTask"{
    BeforeEach {
        UndoDeploymentBackToVanila $testingDeploymentName $true
    }
    
     It "Set ISHServiceBackgroundTask sets amount of services"{
        #Arrange
        $params = @{Count = 3}
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHServiceBackgroundTask -Session $session -ArgumentList $testingDeploymentName, $params
        $Services = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceBackgroundTask -Session $session -ArgumentList $testingDeploymentName
        $Services.Count | Should be 3
     }

     It "Set ISHServiceBackgroundTask sets default role by default"{
        #Arrange
        $params = @{Count = 3}
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHServiceBackgroundTask -Session $session -ArgumentList $testingDeploymentName, $params
        $Services = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceBackgroundTask -Session $session -ArgumentList $testingDeploymentName
        foreach($Service in $Services){
            $Service.Role | Should be "Default"
        }
        $Services.Count | Should be 3
     }

     It "Enable-ISHServiceBackgroundTask Disabled BackgroundTask Service"{
		#Arrange
        # Make sure, that BackgroundTask Service is disabled, otherwise - make it disabled 
        $precondition = (Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceBackgroundTask -Session $session -ArgumentList $testingDeploymentName)[0].Status
        if ($precondition -eq "Running"){Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisableISHServiceBackgroundTask -Session $session -ArgumentList $testingDeploymentName }
        # Now BackgroundTask Service should be fo sure disabled. Otherwise test fails
        RetryCommand -numberOfRetries 10 -command {(Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceBackgroundTask -Session $session -ArgumentList $testingDeploymentName)[0].Status} -expectedResult "Stopped"| Should be "Stopped"
		#Act
        # Try enabling BackgroundTask Service
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnableISHServiceBackgroundTask -Session $session -ArgumentList $testingDeploymentName
        RetryCommand -numberOfRetries 10 -command {(Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceBackgroundTask -Session $session -ArgumentList $testingDeploymentName)[0].Status} -expectedResult "Running"| Should be "Running"

    }
    
     It "Disable-ISHServiceBackgroundTask enabled BackgroundTask Service" {
		#Arrange
        # Make sure, that BackgroundTask Service is enabled, otherwise - make it enabled
        $precondition = (Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceBackgroundTask -Session $session -ArgumentList $testingDeploymentName)[0].Status
        if ($precondition -eq "Stopped"){Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnableISHServiceBackgroundTask -Session $session -ArgumentList $testingDeploymentName }
        # Now BackgroundTask Service should be fo sure enabled. Otherwise test fails
        RetryCommand -numberOfRetries 10 -command {(Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceBackgroundTask -Session $session -ArgumentList $testingDeploymentName)[0].Status} -expectedResult "Running"| Should be "Running"
		#Act
        # Try enabling BackgroundTask Service
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisableISHServiceBackgroundTask -Session $session -ArgumentList $testingDeploymentName
        RetryCommand -numberOfRetries 10 -command {(Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceBackgroundTask -Session $session -ArgumentList $testingDeploymentName)[0].Status} -expectedResult "Stopped"| Should be "Stopped"
    }
      
     It "Enable-ISHServiceBackgroundTask enabled BackgroundTask Service"{
		#Arrange
        # Make sure, that BackgroundTask Service is disabled, otherwise - make it disabled 
        $precondition = (Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceBackgroundTask -Session $session -ArgumentList $testingDeploymentName)[0].Status
        if ($precondition -eq "Running"){Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisableISHServiceBackgroundTask -Session $session -ArgumentList $testingDeploymentName }

        # Now BackgroundTask Service should be fo sure disabled. Otherwise test fails
        RetryCommand -numberOfRetries 10 -command {(Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceBackgroundTask -Session $session -ArgumentList $testingDeploymentName)[0].Status} -expectedResult "Stopped"| Should be "Stopped"
		#Act
        # Try enabling BackgroundTask Service
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnableISHServiceBackgroundTask -Session $session -ArgumentList $testingDeploymentName
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnableISHServiceBackgroundTask -Session $session -ArgumentList $testingDeploymentName} | Should not Throw
        #Assert
        RetryCommand -numberOfRetries 10 -command {(Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceBackgroundTask -Session $session -ArgumentList $testingDeploymentName)[0].Status} -expectedResult "Running"| Should be "Running"

    }
    
     It "Disable-ISHServiceBackgroundTask disabled BackgroundTask Service"{
		#Arrange
        # Make sure, that BackgroundTask Service is disabled, otherwise - make it disabled 
        $precondition = (Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceBackgroundTask -Session $session -ArgumentList $testingDeploymentName)[0].Status
        if ($precondition -eq "Stopped"){Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnableISHServiceBackgroundTask -Session $session -ArgumentList $testingDeploymentName }

        # Now BackgroundTask Service should be fo sure enabled. Otherwise test fails
        RetryCommand -numberOfRetries 10 -command {(Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceBackgroundTask -Session $session -ArgumentList $testingDeploymentName)[0].Status} -expectedResult "Running"| Should be "Running"
		#Act
        # Try enabling BackgroundTask Service
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisableISHServiceBackgroundTask -Session $session -ArgumentList $testingDeploymentName
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisableISHServiceBackgroundTask -Session $session -ArgumentList $testingDeploymentName} | Should not Throw
        RetryCommand -numberOfRetries 10 -command {(Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceBackgroundTask -Session $session -ArgumentList $testingDeploymentName)[0].Status} -expectedResult "Stopped"| Should be "Stopped"
    }
    
     It "Enable-ISHServiceBackgroundTask changes component state"{
		#Arrange
        # Make sure, that BackgroundTask Service is disabled, otherwise - make it disabled 
        $precondition = (Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceBackgroundTask -Session $session -ArgumentList $testingDeploymentName)[0].Status
        if ($precondition -eq "Running"){Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisableISHServiceBackgroundTask -Session $session -ArgumentList $testingDeploymentName }
        # Now BackgroundTask Service should be fo sure disabled. Otherwise test fails
        RetryCommand -numberOfRetries 10 -command {(Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceBackgroundTask -Session $session -ArgumentList $testingDeploymentName)[0].Status} -expectedResult "Stopped"| Should be "Stopped"
		#Act
        # Try enabling BackgroundTask Service
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnableISHServiceBackgroundTask -Session $session -ArgumentList $testingDeploymentName
        RetryCommand -numberOfRetries 10 -command {(Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceBackgroundTask -Session $session -ArgumentList $testingDeploymentName)[0].Status} -expectedResult "Running"| Should be "Running"
        $comp = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHComponent -Session $session -ArgumentList $testingDeploymentName
        ($comp | Where-Object -Property Name -EQ "BackgroundTask").IsEnabled | Should be "True"
    }
     
     It "Enable-ISHServiceBackgroundTask writes history"{       
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnableISHServiceBackgroundTask -Session $session -ArgumentList $testingDeploymentName
        $history = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetHistory -Session $session -ArgumentList $testingDeploymentName
        $history.Contains('Enable-ISHServiceBackgroundTask -ISHDeployment') | Should be "True"
    }

     It "Enable ISHServiceBackgroundTask enables only selected role"{
        #Arrange
        $params = @{Count = 3}
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHServiceBackgroundTask -Session $session -ArgumentList $testingDeploymentName, $params
        $Services = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceBackgroundTask -Session $session -ArgumentList $testingDeploymentName
        $Services.Count | Should be 3

     }
    
     It "Set ISHServiceBackgroundTask does not allow 0 as count value"{
        #Arrange
        $params = @{Count = 0}
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHServiceBackgroundTask -Session $session -ArgumentList $testingDeploymentName, $params} | Should Throw "Cannot validate argument on parameter 'Count'"
     }

     It "Remove ISHServiceBackgroundTask removes services"{
        #Arrange
        $params = @{}
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveISHServiceBackgroundTask -Session $session -ArgumentList $testingDeploymentName, $params
        $Services = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceBackgroundTask -Session $session -ArgumentList $testingDeploymentName
        $Services.Count | Should be 0
     }
     
     It "Enabling and disabling BackgroundTask Service with role Console" {
        #Enable-ISHServiceBackgroundTask with role "Default"
        $params = @{Role = "Default"}
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnableISHServiceBackgroundTask -Session $session -ArgumentList $testingDeploymentName, $params
        $services = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceBackgroundTask -Session $session -ArgumentList $testingDeploymentName
        if ($services.GetType().BaseType.Name -ne "Object"){
            $services.Count | Should be 1
        }
        $services[0].Status | Should be "Running"
        $Services[0].Role | Should be "Default"

        #Add background task with role "Console"
        $params = @{Role = "Console"; Count= "2"}
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHServiceBackgroundTask -Session $session -ArgumentList $testingDeploymentName, $params

        $params = @{Role = "Console"}
        $services = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceBackgroundTask -Session $session -ArgumentList $testingDeploymentName, $params
        $services.Count | Should be 2
        foreach ($Service in $services)
        {
            $service.Status | Should be "Stopped"
        }

        #Start deployment
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockStartISHDeployment -Session $session -ArgumentList $testingDeploymentName
        $allServices = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceBackgroundTask -Session $session -ArgumentList $testingDeploymentName
        foreach ($service in $allServices)
        {
            if ($service.Role -eq "Console")
            {
                $service.Status | Should be "Stopped"
            } 
            else
            {
                $service.Status | Should be "Running"
            }
        }

        #Enable background services with role Console
        $params = @{Role = "Console"}
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnableISHServiceBackgroundTask -Session $session -ArgumentList $testingDeploymentName, $params
        $allServices = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceBackgroundTask -Session $session -ArgumentList $testingDeploymentName
        foreach ($Service in $allServices)
        {
            if ($service.Role -eq "Console")
            {
                $service.Status | Should be "Running"
            }
            else
            {
                $service.Status | Should be "Running"
            }
        }

        #Disable background services with role Console
        $params = @{Role = "Console"}
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisableISHServiceBackgroundTask -Session $session -ArgumentList $testingDeploymentName, $params
        $allServices = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceBackgroundTask -Session $session -ArgumentList $testingDeploymentName
        foreach ($Service in $allServices)
        {
            if ($service.Role -eq "Console")
            {
                $service.Status | Should be "Stopped"
            }
            else
            {
                $service.Status | Should be "Running"
            }
        }

     }
     
     It "Removing BackgroundTask Service with role Console" {
        #Add background task with role "Console"
        $params = @{Role = "Console"; Count= "2"}
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHServiceBackgroundTask -Session $session -ArgumentList $testingDeploymentName, $params

        $allServices = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceBackgroundTask -Session $session -ArgumentList $testingDeploymentName
        $allServices.Count | Should be 3

        $params = @{Role = "Console"}
        $services = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceBackgroundTask -Session $session -ArgumentList $testingDeploymentName, $params
        $services.Count | Should be 2

        #Remove background services with role Console
        $params = @{Role = "Console"}
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveISHServiceBackgroundTask -Session $session -ArgumentList $testingDeploymentName, $params
        $allServices = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceBackgroundTask -Session $session -ArgumentList $testingDeploymentName
        if ($allServices.GetType().BaseType.Name -ne "Object"){
            $allServices.Count | Should be 1
        }
     }
     
	 It "[SCTCM-310] Background tasks are created with proper names" {
        #Creating list of Background tasks with custom roles
			foreach($role in @("Default","Single","Multi","Custom1","Custom2")){
				if($role -ne "Default")
				{
				    $params = @{Role = $role; Count= "1"}
					Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHServiceBackgroundTask -Session $session -ArgumentList $testingDeploymentName, $params
				}
				else
				{
					$params = @{Role = $role}
					 Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveISHServiceBackgroundTask -Session $session -ArgumentList $testingDeploymentName, $params
					Write-Verbose "Removed BackgroundTask with $role role service"
				}
			}
			$allServices = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHServiceBackgroundTask -Session $session -ArgumentList $testingDeploymentName
			#Check if all services are created without word SINGLE in name
			foreach ($service in $allServices){
				$service.Name -like "*Single*" | Should be $false
			}
     }

     Start-Sleep -Seconds 20
     UndoDeploymentBackToVanila $testingDeploymentName $true
}