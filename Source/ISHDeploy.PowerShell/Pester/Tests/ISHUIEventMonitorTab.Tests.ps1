param(
    $session = $null,
    $testingDeploymentName = "InfoShare"
)
. "$PSScriptRoot\Common.ps1"

$computerName = If ($session) {$session.ComputerName} Else {$env:COMPUTERNAME}

#region variables
# Script block for getting ISH deployment
$scriptBlockGetDeployment = {
    param (
        [Parameter(Mandatory=$false)]
        $ishDeployName 
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
    Get-ISHDeployment -Name $ishDeployName 
}

# Generating file pathes to remote PC files
$testingDeployment = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetDeployment -Session $session -ArgumentList $testingDeploymentName
$xmlPath = Join-Path $testingDeployment.WebPath ("\Web{0}\Author\ASP\XSL" -f $testingDeployment.OriginalParameters.projectsuffix )
$xmlPath = $xmlPath.ToString().replace(":", "$")
$xmlPath = "\\$computerName\$xmlPath"

$testLabelName = "test"
$testDescription = "Administrator"
$invalidLabel = "Invalid"
#endregion

#region Script Blocks 

# Script block for Enable-ISHUIQualityAssistant
$scriptBlockEnable = {
    param (
        [Parameter(Mandatory=$true)]
        $ishDeployName 
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
        $ishDeploy = Get-ISHDeployment -Name $ishDeployName
        Enable-ISHUIQualityAssistant -ISHDeployment $ishDeploy
  
}

# Script block for Disable-ISHUIQualityAssistant
$scriptBlockDisable = {
    param (
        [Parameter(Mandatory=$true)]
        $ishDeployname 
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Disable-ISHUIQualityAssistant -ISHDeployment $ishDeploy
}


$scriptBlockUndoDeployment = {
    param (
        [Parameter(Mandatory=$false)]
        $ishDeployName 
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Undo-ISHDeployment -ISHDeployment $ishDeploy
}

#endregion



# Function reads target files and their content, searches for specified nodes in xm
function readTargetXML() {
	[xml]$XmlConfig = Get-Content "$xmlPath\EventMonitorMenuBar.xml" -ErrorAction SilentlyContinue
Write-Debug "COnfig path $xmlPath\EventMonitorMenuBar.xml"
    [xml]$XmlBlueLionConfig = Get-Content "$xmlPath\bluelion-EventMonitorMenuBar.xml" -ErrorAction SilentlyContinue
    Write-Debug "COnfig path $xmlPath\bluelion-EventMonitorMenuBar.xml"
    [xml]$XmlEnrichWebConfig = Get-Content "$configPath\BlueLion-Plugin\web.config" -ErrorAction SilentlyContinue

    $global:textConfig = $XmlConfig.config.javascript | ? {$_.src -eq "../BlueLion-Plugin/Bootstrap/bootstrap.js"}
    $global:textBlueLionConfig = $XmlBlueLionConfig.SelectNodes("*/*[local-name()='import'][@src='../BlueLion-Plugin/create-toolbar.xml']")
    $global:textEnrichBluelionWebConfigJsonMimeMapNodes = $XmlEnrichWebConfig.SelectNodes("configuration/system.webServer/staticContent/mimeMap[@fileExtension='.json']")

	if($textConfig -and $textBlueLionConfig.Count -eq 1 -and $textEnrichBluelionWebConfigJsonMimeMapNodes.Count -eq 1){
		Return "Enabled"
	}
	else{
		Return "Disabled"
	}

}
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
    [string]$Description 
     )
        [xml]$XmlEventMonitorBar= Get-Content "$xmlPath\EventMonitorMenuBar.xml"  -ErrorAction SilentlyContinue
        $actionLine = "EventMonitor/Main/Overview?eventTypesFilter=$EventTypesFilter&statusFilter=$StatusFilter&selectedMenuItemTitle=$SelectedMenuItemTitle&modifiedSinceMinutesFilter=$ModifiedSinceMinutesFilter&selectedButtonTitle=$SelectedButtonTitle"
        

        $global:textEventMenuBar = $XmlEventMonitorBar.menubar.menuitem | ? {($_.label -eq $Label) -and ($_.action -eq $actionLine) -and ($_.icon -eq $Icon)}

        $commentCheck = ($global:textEventMenuBar.PreviousSibling.Name -match "#comment") -and ($global:textEventMenuBar.PreviousSibling.Value -match $Description)
        $userCheck = ($global:textEventMenuBar.userrole -eq $UserRole) -and ($global:textEventMenuBar.description -eq $Description)

   
        if ($global:textEventMenuBar -and $commentCheck -and $userCheck){
            Return "Added"
        }
        else  {
            Return "Deleted" 
        }
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
    Set-ISHUIEventMonitorTab -ISHDeployment $ishDeploy @parametersHash
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
        Move-ISHUIEventMonitorTab -ISHDeployment $ishDeploy @parametersHash -First
    }
    elseif($switchState -eq "Last"){
        Move-ISHUIEventMonitorTab -ISHDeployment $ishDeploy @parametersHash -Last
    }
    else{
        Move-ISHUIEventMonitorTab -ISHDeployment $ishDeploy @parametersHash
    }

    
    #Set-ISHUIEventMonitorTab -ISHDeployment $ishDeploy -Label $label -EventTypesFilter $eventTypesFilter -Icon $icon -SelectedStatusFilter $selectedStatusFilter -ModifiedSinceMinutesFilter $modifiedSinceMinutesFilter -UserRole $userRole -Description $description
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
    Remove-ISHUIEventMonitorTab -ISHDeployment $ishDeploy -Label $label
}

Describe "Testing ISHUIEventMonitorTab"{
    BeforeEach {
            Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockUndoDeployment -Session $session -ArgumentList $testingDeploymentName
    }

    It "Set monitor tab"{
        #Arrange
        $params = @{label = $testLabelName; Description = $testDescription}
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetEventMonitor -Session $session -ArgumentList $testingDeploymentName, $params
        #Assert
        checkEventMonitorTabExist -Label $params.label -Icon "~/UIFramework/events.32x32.png" -SelectedMenuItemTitle $params.label -ModifiedSinceMinutesFilter 1440 -Description $params.Description | Should be "Added"
    }

    It "Remove monitor tab"{
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveEventMonitorTab -Session $session -ArgumentList $testingDeploymentName, $testLabelName
        #Assert
        checkEventMonitorTabExist -Label $params.label -Icon "~/UIFramework/events.32x32.png" -SelectedMenuItemTitle $params.label -ModifiedSinceMinutesFilter 1440 -Description $params.Description | Should be "Deleted"
    }
       
    It "Sets monitor tab with no XML"{
        #Arrange
        Rename-Item "$xmlPath\EventMonitorMenuBar.xml" "_EventMonitorMenuBar.xml"
        $params = @{label = $testLabelName; Description = $testDescription}
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
        $params = @{label = $testLabelName; Description = $testDescription}
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
        $params = @{label = $testLabelName; Description = $testDescription}
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetEventMonitor -Session $session -ArgumentList $testingDeploymentName, $params
        #Assert
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetEventMonitor -Session $session -ArgumentList $testingDeploymentName, $params} | Should Not Throw
        checkEventMonitorTabExist -Label $params.label -Icon "~/UIFramework/events.32x32.png" -SelectedMenuItemTitle $params.label -ModifiedSinceMinutesFilter 1440 -Description $params.Description | Should be "Added"
    }

    It "Remove unexisting monitor tab"{
        #Arrange
        $nodeExist = checkEventMonitorTabExist -Label $params.label -Icon "~/UIFramework/events.32x32.png" -SelectedMenuItemTitle $params.label -ModifiedSinceMinutesFilter 1440 -Description $params.Description
        if($nodeExist -eq "Added"){
            Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveEventMonitorTab -Session $session -ArgumentList $testingDeploymentName, $testLabelName
        }
        checkEventMonitorTabExist -Label $params.label -Icon "~/UIFramework/events.32x32.png" -SelectedMenuItemTitle $params.label -ModifiedSinceMinutesFilter 1440 -Description $params.Description | Should be "Deleted"
        $params = @{label = $testLabelName; Description = $testDescription}
        #Act/Assert
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveEventMonitorTab -Session $session -ArgumentList $testingDeploymentName, $testLabelName} | Should Not Throw
        checkEventMonitorTabExist -Label $params.label -Icon "~/UIFramework/events.32x32.png" -SelectedMenuItemTitle $params.label -ModifiedSinceMinutesFilter 1440 -Description $params.Description | Should be "Deleted"
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
        [xml]$XmlEventMonitorBar= Get-Content "$xmlPath\EventMonitorMenuBar.xml"  -ErrorAction SilentlyContinue
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
        [xml]$XmlEventMonitorBar= Get-Content "$xmlPath\EventMonitorMenuBar.xml"  -ErrorAction SilentlyContinue
        $thirdArray =$XmlEventMonitorBar.menubar.menuitem.label 
    
        #Compare 2 arrays
        $compareArrayResult = 0
        for ($i=0; $i -le $arrayLength - 1;$i++){
            if ($labelArray[$i] -ne $thirdArray[$i]){$compareArrayResult++}
        }
        $checkResult = $compareArrayResult -eq 0
		Write-Verbose "Comparing arrays differense: $checkResult"
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
        [xml]$XmlEventMonitorBar= Get-Content "$xmlPath\EventMonitorMenuBar.xml"  -ErrorAction SilentlyContinue
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
        [xml]$XmlEventMonitorBar= Get-Content "$xmlPath\EventMonitorMenuBar.xml"  -ErrorAction SilentlyContinue
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
        [xml]$XmlEventMonitorBar= Get-Content "$xmlPath\EventMonitorMenuBar.xml"  -ErrorAction SilentlyContinue
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
        [xml]$XmlEventMonitorBar= Get-Content "$xmlPath\EventMonitorMenuBar.xml"  -ErrorAction SilentlyContinue
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
        checkEventMonitorTabExist -Label $params.label -Icon "~/UIFramework/events.32x32.png" -SelectedMenuItemTitle $params.label -ModifiedSinceMinutesFilter 2000 -Description $params.Description -EventTypesFilter "NotDef, 2423" -UserRole "User" -SelectedButtonTitle "Show%20All"| Should be "Added"
    }

}