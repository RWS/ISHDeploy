param(
    $session = $null,
    $testingDeploymentName = "InfoShare"
)
. "$PSScriptRoot\Common.ps1"

#region variables

# Generating file pathes to remote PC files
$xmlPath = Join-Path $testingDeployment.WebPath ("\Web{0}\Author\ASP\XSL" -f $suffix )
$xmlPath = $xmlPath.ToString().replace(":", "$")
$xmlPath = "\\$computerName\$xmlPath"

$testLabelName = "test"
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

function getMainMenuButton{
    param([string]$Label)

    #[System.Xml.Linq.XDocument]$menuitemXml = System.Xml.Linq.XDocument.Load("$xmlPath\MainMenuBar.xml")

    [System.Xml.XmlDocument]$xmlMainMenuBar = new-object System.Xml.XmlDocument
    $xmlMainMenuBar.load("$xmlPath\MainMenuBar.xml")
    $menuitemXml = $xmlMainMenuBar.SelectSingleNode("mainmenubar/menuitem[@label='$Label']")
    
    if ($menuitemXml)
    {
        $menuitem = @{Label = $menuitemXml.label; Action = $menuitemXml.action; Id = $menuitemXml.id; Userrole = @{}}
    
        $i = 0
        $menuitemXml.userrole | ForEach-Object { $menuitem.Userrole[$i] = $_; $i = $i + 1; }
    
        return $menuitem
    }
    else
    {
        return $null 
    }
}

function checkAmountOFUserroles{
    param( [string]$Label)

    [xml]$XmlMainMenuBar= Get-Content "$xmlPath\MainMenuBar.xml"  -ErrorAction SilentlyContinue
      
    $textMainMenuBar = $XmlMainMenuBar.menubar.menuitem | ? {($_.label -eq $Label)}
    Return  $textMainMenuBar.userrole.Count
  
}

$scriptBlockSetMainMenuButton = {
    param (
        $ishDeployName,
        $parametersHash
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }

    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Set-ISHUIMainMenuButton -ISHDeployment $ishDeploy @parametersHash
}

$scriptBlockMoveMainMenuButton = {
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
        Move-ISHUIMainMenuButton -ISHDeployment $ishDeploy @parametersHash -First
    }
    elseif($switchState -eq "Last"){
        Move-ISHUIMainMenuButton -ISHDeployment $ishDeploy @parametersHash -Last
    }
    else{
        Move-ISHUIMainMenuButton -ISHDeployment $ishDeploy @parametersHash
    }

    
    #Set-ISHUIMainMenuButton -ISHDeployment $ishDeploy -Label $label -EventTypesFilter $eventTypesFilter -Icon $icon -SelectedStatusFilter $selectedStatusFilter -ModifiedSinceMinutesFilter $modifiedSinceMinutesFilter -UserRole $userRole -Description $description
}

$scriptBlockRemoveMainMenuBar= {
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
    Remove-ISHUIMainMenuButton -ISHDeployment $ishDeploy -Label $label
}

Describe "Testing ISHUIMainMenuButton"{
    BeforeEach {
		ArtifactCleaner -filePath $xmlPath -fileName "MainMenuBar.xml"
		UndoDeploymentBackToVanila $testingDeploymentName $true
    }

    It "Set main menu button"{
        #Arrange
        $params = @{Label = $testLabelName; UserRole = @("Administrator", "Translator"); Action = "TestPage.asp" }
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetMainMenuButton -Session $session -ArgumentList $testingDeploymentName, $params
        #Assert
        $item = getMainMenuButton -Label $params.Label
        $item.Label | Should be $params.Label
        $item.Action | Should be $params.Action
        $item.UserRole[0] | Should Match $params.UserRole[0]
        $item.UserRole[1] | Should Match $params.UserRole[1]
    }

    It "Remove main menu button"{
        #Arrange
        $params = @{Label = $testLabelName; UserRole = @("Administrator", "Translator"); Action = "TestPage.asp" }
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetMainMenuButton -Session $session -ArgumentList $testingDeploymentName, $params
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveMainMenuBar -Session $session -ArgumentList $testingDeploymentName, $testLabelName
        #Assert
        $item = getMainMenuButton -Label $params.Label
        $item | Should be $null
    }
       
 #   It "Sets main menu button with no XML"{
 #       #Arrange
 #       Rename-Item "$xmlPath\MainMenuBar.xml" "_MainMenuBar.xml"
 #       $params = @{label = $testLabelName; Description = $testDescription}
 #       #Act/Assert
 #       {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetMainMenuButton -Session $session -ArgumentList $testingDeploymentName, $params} |Should Throw "Could not find file" 
 #       #Rollback
 #       Rename-Item "$xmlPath\_MainMenuBar.xml" "MainMenuBar.xml"
 #   }    

 #   It "Sets main menu button with no XML"{
 #       #Arrange
 #       Rename-Item "$xmlPath\MainMenuBar.xml" "_MainMenuBar.xml"
 #       #Act/Assert
 #       {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveMainMenuBar -Session $session -ArgumentList $testingDeploymentName, $testLabelName} |Should Throw "Could not find file" 
 #       #Rollback
 #       Rename-Item "$xmlPath\_MainMenuBar.xml" "MainMenuBar.xml"
 #   }  
      
 #   It "Sets main menu button with wrong XML"{
 #       #Arrange
 #       # Running valid scenario commandlet to out files into backup folder before they will ba manually modified in test
 #       Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveMainMenuBar -Session $session -ArgumentList $testingDeploymentName, $testLabelName
 #       Rename-Item "$xmlPath\MainMenuBar.xml" "_MainMenuBar.xml"
 #       New-Item "$xmlPath\MainMenuBar.xml" -type file |Out-Null
 #       $params = @{label = $testLabelName; Description = $testDescription}
 #       #Act/Assert
 #       {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetMainMenuButton -Session $session -ArgumentList $testingDeploymentName, $params} |Should Throw "Root element is missing" 
 #       #Rollback
 #       Remove-Item "$xmlPath\MainMenuBar.xml"
 #       Rename-Item "$xmlPath\_MainMenuBar.xml" "MainMenuBar.xml"
 #   }
  
 #   It "Remove main menu button with no XML"{
 #       #Arrange
 #       # Running valid scenario commandlet to out files into backup folder before they will ba manually modified in test
 #       Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveMainMenuBar -Session $session -ArgumentList $testingDeploymentName, $testLabelName
 #       Rename-Item "$xmlPath\MainMenuBar.xml" "_MainMenuBar.xml"
 #       New-Item "$xmlPath\MainMenuBar.xml" -type file |Out-Null
 #       #Act/Assert
 #       {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveMainMenuBar -Session $session -ArgumentList $testingDeploymentName, $testLabelName} |Should Throw "Root element is missing" 
 #       #Rollback
 #       Remove-Item "$xmlPath\MainMenuBar.xml"
 #       Rename-Item "$xmlPath\_MainMenuBar.xml" "MainMenuBar.xml"
 #   }

 #   It "Set existing main menu button"{
 #       #Arrange
 #       $params = @{label = $testLabelName; Description = $testDescription}
 #       #Act
 #       Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetMainMenuButton -Session $session -ArgumentList $testingDeploymentName, $params
 #       #Assert
 #       {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetMainMenuButton -Session $session -ArgumentList $testingDeploymentName, $params} | Should Not Throw
 #       RetryCommand -numberOfRetries 10 -command {checkMainMenuBarExist -Label $params.label -Icon "~/UIFramework/events.32x32.png" -SelectedMenuItemTitle $params.label -ModifiedSinceMinutesFilter 1440 -Description $params.Description} -expectedResult "Added" | Should be "Added"
 #   }

 #   It "Remove unexisting main menu button"{
 #       #Arrange
 #       $nodeExist = checkMainMenuBarExist -Label $params.label -Icon "~/UIFramework/events.32x32.png" -SelectedMenuItemTitle $params.label -ModifiedSinceMinutesFilter 1440 -Description $params.Description
 #       if($nodeExist -eq "Added"){
 #           Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveMainMenuBar -Session $session -ArgumentList $testingDeploymentName, $testLabelName
 #       }
 #       RetryCommand -numberOfRetries 10 -command {checkMainMenuBarExist -Label $params.label -Icon "~/UIFramework/events.32x32.png" -SelectedMenuItemTitle $params.label -ModifiedSinceMinutesFilter 1440 -Description $params.Description} -expectedResult "Deleted"| Should be "Deleted"
 #       $params = @{label = $testLabelName; Description = $testDescription}
 #       #Act/Assert
 #       {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveMainMenuBar -Session $session -ArgumentList $testingDeploymentName, $testLabelName} | Should Not Throw
 #       RetryCommand -numberOfRetries 10 -command {checkMainMenuBarExist -Label $params.label -Icon "~/UIFramework/events.32x32.png" -SelectedMenuItemTitle $params.label -ModifiedSinceMinutesFilter 1440 -Description $params.Description} -expectedResult "Deleted" | Should be "Deleted"
 #   }

 #   It "Move After"{
 #       #Arrange
 #       #getting 2 arrays with labels of nodes of Event Monitor Menu bar
 #       [xml]$XmlMainMenuBar= Get-Content "$xmlPath\MainMenuBar.xml"  -ErrorAction SilentlyContinue
 #       $labelArray = $XmlMainMenuBar.menubar.menuitem.label
 #       #don't rewrite this to $tempArray = $labelArray - in powershell it will be link to an object and if one changes - another will change too and test fails
 #       $tempArray = $XmlMainMenuBar.menubar.menuitem.label
 #       $arrayLength = $labelArray.Length
 #       #get label that will be moved
 #       $moveblelabel = $labelArray[$arrayLength -1]

 #       #Move array object in same way, as Move commandklet will move nodes
 #       $labelArray[1] = $moveblelabel
 #       for ($i=2; $i -le $arrayLength - 1;$i++){
 #           $labelArray[$i] = $tempArray[$i-1]
 #       }
 #       $params = @{label = $moveblelabel; After = $tempArray[0]}
 #       #Act
 #       Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockMoveMainMenuButton -Session $session -ArgumentList $testingDeploymentName, $params

 #       #read the updated xml file
 #       [xml]$XmlMainMenuBar = RemoteReadXML "$xmlPath\MainMenuBar.xml"
 #       $thirdArray =$XmlMainMenuBar.menubar.menuitem.label 
    
 #       #Compare 2 arrays
 #       $compareArrayResult = 0
 #       for ($i=0; $i -le $arrayLength - 1;$i++){
 #           if ($labelArray[$i] -ne $thirdArray[$i]){$compareArrayResult++}
 #       }
 #       $checkResult = $compareArrayResult -eq 0
    
 #       #Assert
 #       $CheckResult | Should Be "True"
 #   }

 #   It "Move Last"{
 #       #Arrange
 #       #getting 2 arrays with labels of nodes of Event Monitor Menu bar
 #       [xml]$XmlMainMenuBar= Get-Content "$xmlPath\MainMenuBar.xml"  -ErrorAction SilentlyContinue
 #       $labelArray = $XmlMainMenuBar.menubar.menuitem.label
 #       #don't rewrite this to $tempArray = $labelArray - in powershell it will be link to an object and if one changes - another will change too and test fails
 #       $tempArray = $XmlMainMenuBar.menubar.menuitem.label
 #       $arrayLength = $labelArray.Length
 #       #get label that will be moved
 #       $moveblelabel = $labelArray[0]

 #       #Move array object in same way, as Move commandklet will move nodes
 #       $labelArray[$arrayLength-1] = $moveblelabel
 #       for ($i=0; $i -le $arrayLength - 2;$i++){
 #           $labelArray[$i] = $tempArray[$i+1]
 #       }
 #       $params = @{label = $moveblelabel;}
 #       #Act
 #       Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockMoveMainMenuButton -Session $session -ArgumentList $testingDeploymentName, $params, "Last"

 #       #read the updated xml file
 #       [xml]$XmlMainMenuBar = RemoteReadXML "$xmlPath\MainMenuBar.xml"
 #       $thirdArray =$XmlMainMenuBar.menubar.menuitem.label 
    
 #       #Compare 2 arrays
 #       $compareArrayResult = 0
 #       for ($i=0; $i -le $arrayLength - 1;$i++){
 #           if ($labelArray[$i] -ne $thirdArray[$i]){$compareArrayResult++}
 #       }
 #       $checkResult = $compareArrayResult -eq 0 
 #       #Assert
 #       $checkResult | Should Be "True"
 #   }

 #   It "Move After itsels"{
 #       #Arrange
 #       #get array with labels on nodes in Event Monitor Menu Bar
 #       [xml]$XmlMainMenuBar= Get-Content "$xmlPath\MainMenuBar.xml"  -ErrorAction SilentlyContinue
 #       $labelArray = $XmlMainMenuBar.menubar.menuitem.label
 #       $arrayLength = $labelArray.Length
 #       #select label that will be moved
 #       $moveblelabel = $labelArray[$arrayLength -1]
 #       $params = @{label = $moveblelabel; After = $moveblelabel }
 #       #Act
 #       {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockMoveMainMenuButton -Session $session -ArgumentList $testingDeploymentName, $params} | Should Not Throw

 #       #Get updated array with labels
 #       [xml]$XmlMainMenuBar = RemoteReadXML "$xmlPath\MainMenuBar.xml"
 #       $compareArray =$XmlMainMenuBar.menubar.menuitem.label
 #       #Compare 2 arrays - before move and after - they should be same
 #       $compareArrayResult = compareArray -firstArray $labelArray -secondArray $compareArray
    
 #       #Assert
 #       ($compareArrayResult -eq 0) | Should Be "True"
 #   }

 #   It "Move After non-existing label"{
 #       #Arrange
 #       #get array with labels on nodes in Event Monitor Menu Bar
 #       [xml]$XmlMainMenuBar= Get-Content "$xmlPath\MainMenuBar.xml"  -ErrorAction SilentlyContinue
 #       $labelArray = $XmlMainMenuBar.menubar.menuitem.label
 #       $arrayLength = $labelArray.Length
 #       #select label that will be moved
 #       $moveblelabel = $labelArray[$arrayLength -1]
 #       $params = @{label = $moveblelabel; After = $invalidLabel }
 #       #Act
 #       {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockMoveMainMenuButton -Session $session -ArgumentList $testingDeploymentName, $params} | Should Not Throw

 #       #Get updated array with labels
 #       [xml]$XmlMainMenuBar = RemoteReadXML "$xmlPath\MainMenuBar.xml"
 #       $compareArray =$XmlMainMenuBar.menubar.menuitem.label
 #       #Compare 2 arrays - before move and after - they should be same
 #       $compareArrayResult = compareArray -firstArray $labelArray -secondArray $compareArray
    
 #       #Assert
 #       ($compareArrayResult -eq 0) | Should Be "True"
 #   }

 #   It "Move After non-existing label"{
 #       #Arrange
 #       #get array with labels on nodes in Event Monitor Menu Bar
 #       [xml]$XmlMainMenuBar= Get-Content "$xmlPath\MainMenuBar.xml"  -ErrorAction SilentlyContinue
 #       $labelArray = $XmlMainMenuBar.menubar.menuitem.label
 #       $arrayLength = $labelArray.Length
 #       #select label that will be moved
 #       $moveblelabel = $labelArray[$arrayLength -1]
 #       $params = @{label = $invalidLabel ; After = $moveblelabel }
 #       #Act
 #       {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockMoveMainMenuButton -Session $session -ArgumentList $testingDeploymentName, $params} | Should Not Throw

 #       #Get updated array with labels
 #       [xml]$XmlMainMenuBar = RemoteReadXML "$xmlPath\MainMenuBar.xml"
 #       $compareArray =$XmlMainMenuBar.menubar.menuitem.label
 #       #Compare 2 arrays - before move and after - they should be same
 #       $compareArrayResult = compareArray -firstArray $labelArray -secondArray $compareArray
    
 #       #Assert
 #       ($compareArrayResult -eq 0)| Should Be "True"
 #   }

 #   It "Move First"{
 #       #Arrange
 #       #getting 2 arrays with labels of nodes of Event Monitor Menu bar
 #       [xml]$XmlMainMenuBar= Get-Content "$xmlPath\MainMenuBar.xml"  -ErrorAction SilentlyContinue
 #       $labelArray = $XmlMainMenuBar.menubar.menuitem.label
 #       #don't rewrite this to $tempArray = $labelArray - in powershell it will be link to an object and if one changes - another will change too and test fails
 #       $tempArray = $XmlMainMenuBar.menubar.menuitem.label
 #       $arrayLength = $labelArray.Length
 #       #get label that will be moved
 #       $moveblelabel = $labelArray[$arrayLength -1]

 #       #Move array object in same way, as Move commandklet will move nodes
 #       $labelArray[0] = $moveblelabel
 #       for ($i=1; $i -le $arrayLength - 1;$i++){
 #           $labelArray[$i] = $tempArray[$i-1]
 #       }
 #       $params = @{label = $moveblelabel}
 #       #Act
 #       Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockMoveMainMenuButton -Session $session -ArgumentList $testingDeploymentName, $params, "First"

 #       #read the updated xml file
 #       [xml]$XmlMainMenuBar = RemoteReadXML "$xmlPath\MainMenuBar.xml"
 #       $thirdArray =$XmlMainMenuBar.menubar.menuitem.label 
    
 #       #Compare 2 arrays
 #       $compareArrayResult = 0
 #       for ($i=0; $i -le $arrayLength - 1;$i++){
 #           if ($labelArray[$i] -ne $thirdArray[$i]){$compareArrayResult++}
 #       }
 #       $checkResult = $compareArrayResult -eq 0 
 #       $CheckResult | Should Be "True"
 #    }

 #   It "Set all optional fields"{
 #       #Arrange
 #       $params = @{label = $testLabelName; Description = $testDescription;EventTypesFilter = @("NotDef", "2423");SelectedStatusFilter = "All";ModifiedSinceMinutesFilter = "2000";UserRole = "User"  }
 #       #Act
 #       Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetMainMenuButton -Session $session -ArgumentList $testingDeploymentName, $params
 #       #Assert
 #       RetryCommand -numberOfRetries 10 -command {checkMainMenuBarExist -Label $params.label -Icon "~/UIFramework/events.32x32.png" -SelectedMenuItemTitle $params.label -ModifiedSinceMinutesFilter 2000 -Description $params.Description -EventTypesFilter "NotDef, 2423" -UserRole "User" -SelectedButtonTitle "Show%20All"} -expectedResult "Added" | Should be "Added"
 #   }

	#It "Set accepts multiple roles"{
 #       #Arrange
 #       $params =  $params = @{label = $testLabelName; Description = $testDescription;EventTypesFilter = @("NotDef", "2423");SelectedStatusFilter = "All";ModifiedSinceMinutesFilter = "2000";UserRole = "User", "Administrator"  }
 #       #Act
 #       Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetMainMenuButton -Session $session -ArgumentList $testingDeploymentName, $params
 #       #Assert
 #       RetryCommand -numberOfRetries 10 -command {checkAmountOFUserroles -Label $params.label } -expectedResult 2 | Should be 2
 #   }
}