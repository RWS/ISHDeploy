﻿param(
    $session = $null,
    $testingDeploymentName = "InfoShare"
)
. "$PSScriptRoot\Common.ps1"

#region variables

# Generating file pathes to remote PC files
$xmlPath = Join-Path $webPath "Author\ASP\XSL"
$xmlPath = $xmlPath.ToString().replace(":", "$")
$xmlPath = "\\$computerName\$xmlPath"

$testLabelName = "test"
$testDescription = "Administrator"
$invalidLabel = "Invalid"
#endregion

#region Script Blocks 

# Function reads target files and their content, searches for specified nodes in xm

function compareArray([string[]]$firstArray, [string[]]$secondArray){
    if ($firstArray.Length -eq $secondArray.Length){
        
        $compareArrayResult = 0
        for ($i=0; $i -le $firstArray.Length - 1;$i++){
            if ($firstArray[$i] -ne $secondArray[$i]) {$compareArrayResult++}
        } 

        return $compareArrayResult 
    }
    else{
        return $false
    }
}

function checkEventMonitorTabExist{
    param( [string]$Label, 
    [string]$Icon,
    [string]$EventTypesFilter, 
    [string]$StatusFilter = "All", 
    [string]$SelectedMenuItemTitle, 
    [int]$ModifiedSinceMinutesFilter,
    [string]$SelectedButtonTitle = "Show%20Recent", 
    [string]$UserRole = "Administrator", 
    [string]$Description)

    [xml]$XmlEventMonitorBar= Get-Content "$xmlPath\EventMonitorMenuBar.xml"  -ErrorAction SilentlyContinue
    $actionLine = "EventMonitor/Main/Overview?eventTypesFilter=$EventTypesFilter&statusFilter=$StatusFilter&selectedMenuItemTitle=$SelectedMenuItemTitle&modifiedSinceMinutesFilter=$ModifiedSinceMinutesFilter&selectedButtonTitle=$SelectedButtonTitle"
        
    $textEventMenuBar = $XmlEventMonitorBar.menubar.menuitem | ? {($_.label -eq $Label)}
    $commentCheck = ($textEventMenuBar.PreviousSibling.Name -match "#comment") -and ($textEventMenuBar.PreviousSibling.Value -match $Description)    

    if (!$textEventMenuBar -and !$commentCheck){
        Return "Deleted"
    }
    elseif ($textEventMenuBar -and $commentCheck){
        $userCheck = $textEventMenuBar.userrole -eq $UserRole
        $descriptionCheck = $textEventMenuBar.description -eq $Description 
        $actionCheck = $textEventMenuBar.action -eq $actionLine
        $iconCheck = $textEventMenuBar.icon -eq $Icon

        if(!$userCheck -or !$descriptionCheck -or !$actionCheck -or !$iconCheck ){
            Throw "Xml structure is wrong. Label found, but it has invalid elements. User: = $userCheck, Description:= $descriptionCheck, Action:= $actionCheck, Icon:= $iconCheck"
        }
        Return "Added"    
    }

    Throw "Found label without comment or comment without label. Label = $textEventMenuBar, comment = $commentCheck"    

}

function checkAmountOFUserroles{
    param( [string]$Label)

    [xml]$XmlEventMonitorBar= Get-Content "$xmlPath\EventMonitorMenuBar.xml"  -ErrorAction SilentlyContinue
      
    $textEventMenuBar = $XmlEventMonitorBar.menubar.menuitem | ? {($_.label -eq $Label)}
    Return  $textEventMenuBar.userrole.Count
  
}

$scriptBlockSetEventMonitor = {
    param (
        $ishDeployName,
        $parametersHash
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }

    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Set-ISHUIEventMonitorMenuBarItem -ISHDeployment $ishDeploy @parametersHash
}

$scriptBlockMoveEventMonitor = {
    param (
        $ishDeployName,
        $parametersHash,
        $switchState
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    if ($switchState -eq "First"){
        Move-ISHUIEventMonitorMenuBarItem -ISHDeployment $ishDeploy @parametersHash -First
    }
    elseif($switchState -eq "Last"){
        Move-ISHUIEventMonitorMenuBarItem -ISHDeployment $ishDeploy @parametersHash -Last
    }
    else{
        Move-ISHUIEventMonitorMenuBarItem -ISHDeployment $ishDeploy @parametersHash
    }

    
    #Set-ISHUIEventMonitorMenuBarItem -ISHDeployment $ishDeploy -Label $label -EventTypesFilter $eventTypesFilter -Icon $icon -SelectedStatusFilter $selectedStatusFilter -ModifiedSinceMinutesFilter $modifiedSinceMinutesFilter -UserRole $userRole -Description $description
}

$scriptBlockRemoveEventMonitorTab= {
    param (
        [Parameter(Mandatory=$false)]
        $ishDeployName,
        [Parameter(Mandatory=$false)]
        $label
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Remove-ISHUIEventMonitorMenuBarItem -ISHDeployment $ishDeploy -Label $label
}

Describe "Testing ISHUIEventMonitorMenuBarItem"{
    BeforeEach {
		ArtifactCleaner -filePath $xmlPath -fileName "EventMonitorMenuBar.xml"
		UndoDeploymentBackToVanila $testingDeploymentName $true
    }

    It "Set monitor tab"{
        #Arrange
        $params = @{label = $testLabelName; Description = $testDescription; UserRole = "Administrator"}
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetEventMonitor -Session $session -ArgumentList $testingDeploymentName, $params
        #Assert
        RetryCommand -numberOfRetries 10 -command {checkEventMonitorTabExist -Label $params.label -Icon "~/UIFramework/events.32x32.png" -SelectedMenuItemTitle $params.label -ModifiedSinceMinutesFilter 1440 -Description $params.Description} -expectedResult "Added"| Should be "Added"
    }

    It "Remove monitor tab"{
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveEventMonitorTab -Session $session -ArgumentList $testingDeploymentName, $testLabelName
        #Assert
        RetryCommand -numberOfRetries 10 -command {checkEventMonitorTabExist -Label $params.label -Icon "~/UIFramework/events.32x32.png" -SelectedMenuItemTitle $params.label -ModifiedSinceMinutesFilter 1440 -Description $params.Description} -expectedResult "Deleted" | Should be "Deleted"
    }
       
    It "Sets monitor tab with no XML"{
        #Arrange
        Rename-Item "$xmlPath\EventMonitorMenuBar.xml" "_EventMonitorMenuBar.xml"
        $params = @{label = $testLabelName; Description = $testDescription; UserRole = "Administrator"}
        #Act/Assert
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetEventMonitor -Session $session -ArgumentList $testingDeploymentName, $params} |Should Throw "Could not find file" 
        #Rollback
        Rename-Item "$xmlPath\_EventMonitorMenuBar.xml" "EventMonitorMenuBar.xml"
    }    

    It "Sets monitor tab with no XML"{
        #Arrange
        Rename-Item "$xmlPath\EventMonitorMenuBar.xml" "_EventMonitorMenuBar.xml"
        #Act/Assert
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveEventMonitorTab -Session $session -ArgumentList $testingDeploymentName, $testLabelName} |Should Throw "Could not find file" 
        #Rollback
        Rename-Item "$xmlPath\_EventMonitorMenuBar.xml" "EventMonitorMenuBar.xml"
    }  
      
    It "Sets monitor tab with wrong XML"{
        #Arrange
        # Running valid scenario commandlet to out files into backup folder before they will ba manually modified in test
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveEventMonitorTab -Session $session -ArgumentList $testingDeploymentName, $testLabelName
        Rename-Item "$xmlPath\EventMonitorMenuBar.xml" "_EventMonitorMenuBar.xml"
        New-Item "$xmlPath\EventMonitorMenuBar.xml" -type file |Out-Null
        $params = @{label = $testLabelName; Description = $testDescription; UserRole = "Administrator"}
        #Act/Assert
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetEventMonitor -Session $session -ArgumentList $testingDeploymentName, $params} |Should Throw "Root element is missing" 
        #Rollback
        Remove-Item "$xmlPath\EventMonitorMenuBar.xml"
        Rename-Item "$xmlPath\_EventMonitorMenuBar.xml" "EventMonitorMenuBar.xml"
    }
  
    It "Remove monitor tab with no XML"{
        #Arrange
        # Running valid scenario commandlet to out files into backup folder before they will ba manually modified in test
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveEventMonitorTab -Session $session -ArgumentList $testingDeploymentName, $testLabelName
        Rename-Item "$xmlPath\EventMonitorMenuBar.xml" "_EventMonitorMenuBar.xml"
        New-Item "$xmlPath\EventMonitorMenuBar.xml" -type file |Out-Null
        #Act/Assert
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveEventMonitorTab -Session $session -ArgumentList $testingDeploymentName, $testLabelName} |Should Throw "Root element is missing" 
        #Rollback
        Remove-Item "$xmlPath\EventMonitorMenuBar.xml"
        Rename-Item "$xmlPath\_EventMonitorMenuBar.xml" "EventMonitorMenuBar.xml"
    }

    It "Set existing monitor tab"{
        #Arrange
        $params = @{label = $testLabelName; Description = $testDescription; UserRole = "Administrator"}
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetEventMonitor -Session $session -ArgumentList $testingDeploymentName, $params
        #Assert
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetEventMonitor -Session $session -ArgumentList $testingDeploymentName, $params} | Should Not Throw
        RetryCommand -numberOfRetries 10 -command {checkEventMonitorTabExist -Label $params.label -Icon "~/UIFramework/events.32x32.png" -SelectedMenuItemTitle $params.label -ModifiedSinceMinutesFilter 1440 -Description $params.Description} -expectedResult "Added" | Should be "Added"
    }

    It "Remove unexisting monitor tab"{
        #Arrange
        $nodeExist = checkEventMonitorTabExist -Label $params.label -Icon "~/UIFramework/events.32x32.png" -SelectedMenuItemTitle $params.label -ModifiedSinceMinutesFilter 1440 -Description $params.Description
        if($nodeExist -eq "Added"){
            Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveEventMonitorTab -Session $session -ArgumentList $testingDeploymentName, $testLabelName
        }
        RetryCommand -numberOfRetries 10 -command {checkEventMonitorTabExist -Label $params.label -Icon "~/UIFramework/events.32x32.png" -SelectedMenuItemTitle $params.label -ModifiedSinceMinutesFilter 1440 -Description $params.Description} -expectedResult "Deleted"| Should be "Deleted"
        $params = @{label = $testLabelName; Description = $testDescription; UserRole = "Administrator"}
        #Act/Assert
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveEventMonitorTab -Session $session -ArgumentList $testingDeploymentName, $testLabelName} | Should Not Throw
        RetryCommand -numberOfRetries 10 -command {checkEventMonitorTabExist -Label $params.label -Icon "~/UIFramework/events.32x32.png" -SelectedMenuItemTitle $params.label -ModifiedSinceMinutesFilter 1440 -Description $params.Description} -expectedResult "Deleted" | Should be "Deleted"
    }

    It "Move After"{
        #Arrange
        #getting 2 arrays with labels of nodes of Event Monitor Menu bar
        [xml]$XmlEventMonitorBar= Get-Content "$xmlPath\EventMonitorMenuBar.xml"  -ErrorAction SilentlyContinue
        $labelArray = $XmlEventMonitorBar.menubar.menuitem.label
        #don't rewrite this to $tempArray = $labelArray - in powershell it will be link to an object and if one changes - another will change too and test fails
        $tempArray = $XmlEventMonitorBar.menubar.menuitem.label
        $arrayLength = $labelArray.Length
        #get label that will be moved
        $moveblelabel = $labelArray[$arrayLength -1]

        #Move array object in same way, as Move commandklet will move nodes
        $labelArray[1] = $moveblelabel
        for ($i=2; $i -le $arrayLength - 1;$i++){
            $labelArray[$i] = $tempArray[$i-1]
        }
        $params = @{label = $moveblelabel; After = $tempArray[0]}
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockMoveEventMonitor -Session $session -ArgumentList $testingDeploymentName, $params

        #read the updated xml file
        [xml]$XmlEventMonitorBar = RemoteReadXML "$xmlPath\EventMonitorMenuBar.xml"
        $thirdArray =$XmlEventMonitorBar.menubar.menuitem.label 
    
        #Compare 2 arrays
        $compareArrayResult = 0
        for ($i=0; $i -le $arrayLength - 1;$i++){
            if ($labelArray[$i] -ne $thirdArray[$i]){$compareArrayResult++}
        }
        $checkResult = $compareArrayResult -eq 0
    
        #Assert
        $CheckResult | Should Be "True"
    }

    It "Move Last"{
        #Arrange
        #getting 2 arrays with labels of nodes of Event Monitor Menu bar
        [xml]$XmlEventMonitorBar= Get-Content "$xmlPath\EventMonitorMenuBar.xml"  -ErrorAction SilentlyContinue
        $labelArray = $XmlEventMonitorBar.menubar.menuitem.label
        #don't rewrite this to $tempArray = $labelArray - in powershell it will be link to an object and if one changes - another will change too and test fails
        $tempArray = $XmlEventMonitorBar.menubar.menuitem.label
        $arrayLength = $labelArray.Length
        #get label that will be moved
        $moveblelabel = $labelArray[0]

        #Move array object in same way, as Move commandklet will move nodes
        $labelArray[$arrayLength-1] = $moveblelabel
        for ($i=0; $i -le $arrayLength - 2;$i++){
            $labelArray[$i] = $tempArray[$i+1]
        }
        $params = @{label = $moveblelabel;}
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockMoveEventMonitor -Session $session -ArgumentList $testingDeploymentName, $params, "Last"

        #read the updated xml file
        [xml]$XmlEventMonitorBar = RemoteReadXML "$xmlPath\EventMonitorMenuBar.xml"
        $thirdArray =$XmlEventMonitorBar.menubar.menuitem.label 
    
        #Compare 2 arrays
        $compareArrayResult = 0
        for ($i=0; $i -le $arrayLength - 1;$i++){
            if ($labelArray[$i] -ne $thirdArray[$i]){$compareArrayResult++}
        }
        $checkResult = $compareArrayResult -eq 0 
        #Assert
        $checkResult | Should Be "True"
    }

    It "Move After itsels"{
        #Arrange
        #get array with labels on nodes in Event Monitor Menu Bar
        [xml]$XmlEventMonitorBar= Get-Content "$xmlPath\EventMonitorMenuBar.xml"  -ErrorAction SilentlyContinue
        $labelArray = $XmlEventMonitorBar.menubar.menuitem.label
        $arrayLength = $labelArray.Length
        #select label that will be moved
        $moveblelabel = $labelArray[$arrayLength -1]
        $params = @{label = $moveblelabel; After = $moveblelabel }
        #Act
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockMoveEventMonitor -Session $session -ArgumentList $testingDeploymentName, $params} | Should Not Throw

        #Get updated array with labels
        [xml]$XmlEventMonitorBar = RemoteReadXML "$xmlPath\EventMonitorMenuBar.xml"
        $compareArray =$XmlEventMonitorBar.menubar.menuitem.label
        #Compare 2 arrays - before move and after - they should be same
        $compareArrayResult = compareArray -firstArray $labelArray -secondArray $compareArray
    
        #Assert
        ($compareArrayResult -eq 0) | Should Be "True"
    }

    It "Move After non-existing label"{
        #Arrange
        #get array with labels on nodes in Event Monitor Menu Bar
        [xml]$XmlEventMonitorBar= Get-Content "$xmlPath\EventMonitorMenuBar.xml"  -ErrorAction SilentlyContinue
        $labelArray = $XmlEventMonitorBar.menubar.menuitem.label
        $arrayLength = $labelArray.Length
        #select label that will be moved
        $moveblelabel = $labelArray[$arrayLength -1]
        $params = @{label = $moveblelabel; After = $invalidLabel }
        #Act
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockMoveEventMonitor -Session $session -ArgumentList $testingDeploymentName, $params} | Should Not Throw

        #Get updated array with labels
        [xml]$XmlEventMonitorBar = RemoteReadXML "$xmlPath\EventMonitorMenuBar.xml"
        $compareArray =$XmlEventMonitorBar.menubar.menuitem.label
        #Compare 2 arrays - before move and after - they should be same
        $compareArrayResult = compareArray -firstArray $labelArray -secondArray $compareArray
    
        #Assert
        ($compareArrayResult -eq 0) | Should Be "True"
    }

    It "Move After non-existing label"{
        #Arrange
        #get array with labels on nodes in Event Monitor Menu Bar
        [xml]$XmlEventMonitorBar= Get-Content "$xmlPath\EventMonitorMenuBar.xml"  -ErrorAction SilentlyContinue
        $labelArray = $XmlEventMonitorBar.menubar.menuitem.label
        $arrayLength = $labelArray.Length
        #select label that will be moved
        $moveblelabel = $labelArray[$arrayLength -1]
        $params = @{label = $invalidLabel ; After = $moveblelabel }
        #Act
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockMoveEventMonitor -Session $session -ArgumentList $testingDeploymentName, $params} | Should Not Throw

        #Get updated array with labels
        [xml]$XmlEventMonitorBar = RemoteReadXML "$xmlPath\EventMonitorMenuBar.xml"
        $compareArray =$XmlEventMonitorBar.menubar.menuitem.label
        #Compare 2 arrays - before move and after - they should be same
        $compareArrayResult = compareArray -firstArray $labelArray -secondArray $compareArray
    
        #Assert
        ($compareArrayResult -eq 0)| Should Be "True"
    }

    It "Move First"{
        #Arrange
        #getting 2 arrays with labels of nodes of Event Monitor Menu bar
        [xml]$XmlEventMonitorBar= Get-Content "$xmlPath\EventMonitorMenuBar.xml"  -ErrorAction SilentlyContinue
        $labelArray = $XmlEventMonitorBar.menubar.menuitem.label
        #don't rewrite this to $tempArray = $labelArray - in powershell it will be link to an object and if one changes - another will change too and test fails
        $tempArray = $XmlEventMonitorBar.menubar.menuitem.label
        $arrayLength = $labelArray.Length
        #get label that will be moved
        $moveblelabel = $labelArray[$arrayLength -1]

        #Move array object in same way, as Move commandklet will move nodes
        $labelArray[0] = $moveblelabel
        for ($i=1; $i -le $arrayLength - 1;$i++){
            $labelArray[$i] = $tempArray[$i-1]
        }
        $params = @{label = $moveblelabel}
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockMoveEventMonitor -Session $session -ArgumentList $testingDeploymentName, $params, "First"

        #read the updated xml file
        [xml]$XmlEventMonitorBar = RemoteReadXML "$xmlPath\EventMonitorMenuBar.xml"
        $thirdArray =$XmlEventMonitorBar.menubar.menuitem.label 
    
        #Compare 2 arrays
        $compareArrayResult = 0
        for ($i=0; $i -le $arrayLength - 1;$i++){
            if ($labelArray[$i] -ne $thirdArray[$i]){$compareArrayResult++}
        }
        $checkResult = $compareArrayResult -eq 0 
        $CheckResult | Should Be "True"
     }

    It "Set all optional fields"{
        #Arrange
        $params = @{label = $testLabelName; Description = $testDescription;EventTypesFilter = @("NotDef", "2423");SelectedStatusFilter = "All";ModifiedSinceMinutesFilter = "2000";UserRole = "User"  }
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetEventMonitor -Session $session -ArgumentList $testingDeploymentName, $params
        #Assert
        RetryCommand -numberOfRetries 10 -command {checkEventMonitorTabExist -Label $params.label -Icon "~/UIFramework/events.32x32.png" -SelectedMenuItemTitle $params.label -ModifiedSinceMinutesFilter 2000 -Description $params.Description -EventTypesFilter "NotDef, 2423" -UserRole "User" -SelectedButtonTitle "Show%20All"} -expectedResult "Added" | Should be "Added"
    }

	It "Set accepts multiple roles"{
        #Arrange
        $params =  $params = @{label = $testLabelName; Description = $testDescription;EventTypesFilter = @("NotDef", "2423");SelectedStatusFilter = "All";ModifiedSinceMinutesFilter = "2000";UserRole = "User", "Administrator"  }
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetEventMonitor -Session $session -ArgumentList $testingDeploymentName, $params
        #Assert
        RetryCommand -numberOfRetries 10 -command {checkAmountOFUserroles -Label $params.label } -expectedResult 2 | Should be 2
    }

	It "Set creates tab before All Events"{
        #Arrange
        $params = @{label = $testLabelName; Description = $testDescription; UserRole = "Administrator"}
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetEventMonitor -Session $session -ArgumentList $testingDeploymentName, $params
        #Assert
        RetryCommand -numberOfRetries 10 -command {checkEventMonitorTabExist -Label $params.label -Icon "~/UIFramework/events.32x32.png" -SelectedMenuItemTitle $params.label -ModifiedSinceMinutesFilter 1440 -Description $params.Description} -expectedResult "Added"| Should be "Added"
		#Get array with labels
        [xml]$XmlEventMonitorBar = RemoteReadXML "$xmlPath\EventMonitorMenuBar.xml"
        $tabArray =$XmlEventMonitorBar.menubar.menuitem.label
		#Find created tab position
		 for ($i=1; $i -le $tabArray.Legth - 1;$i++){
			if($tabArray[$i] -eq $testLabelName){
				$tabArray[$i+1] -eq "All Events" | Should be $true
			}
        }
    }

	It "Set creates tab if All Events does not exist"{
        #Arrange
        $params = @{label = $testLabelName; Description = $testDescription; UserRole = "Administrator"}
        #Act
		Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveEventMonitorTab -Session $session -ArgumentList $testingDeploymentName, "All Events"
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetEventMonitor -Session $session -ArgumentList $testingDeploymentName, $params
        #Assert
        RetryCommand -numberOfRetries 10 -command {checkEventMonitorTabExist -Label $params.label -Icon "~/UIFramework/events.32x32.png" -SelectedMenuItemTitle $params.label -ModifiedSinceMinutesFilter 1440 -Description $params.Description} -expectedResult "Added"| Should be "Added"
   }
}