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

function getSearchMenuButton{
    param([string]$Label)

    [System.Xml.XmlDocument]$xmlSearchMenuBar = new-object System.Xml.XmlDocument
    $xmlSearchMenuBar.load("$xmlPath\SearchMenuBar.xml")
    $itemXml = $xmlSearchMenuBar.SelectSingleNode("searchmenubar/menuitem[@label='$Label']")
    
    if ($itemXml)
    {
        $menuitem = @{Label = $itemXml.label; Action = $itemXml.action; Id = $itemXml.id; Icon = $itemXml.icon;  Userrole = @{}}
    
        $i = 0
        $itemXml.userrole | ForEach-Object { $menuitem.Userrole[$i] = $_; $i = $i + 1; }
    
        return $menuitem
    }
    else
    {
        return $null 
    }
}

function getCountSearchMenuButton{
    param([string]$Label)

    [System.Xml.XmlDocument]$xmlSearchMenuBar = new-object System.Xml.XmlDocument
    $xmlSearchMenuBar.load("$xmlPath\SearchMenuBar.xml")
    $itemXml = $xmlSearchMenuBar.SelectNodes("searchmenubar/menuitem[@label='$Label']")
    
    return $itemXml.Count
}

function checkAmountOFUserroles{
    param( [string]$Label)

    [xml]$XmlSearchMenuBar= Get-Content "$xmlPath\SearchMenuBar.xml"  -ErrorAction SilentlyContinue
      
    $textSearchMenuBar = $XmlSearchMenuBar.menubar.menuitem | ? {($_.label -eq $Label)}
    Return  $textSearchMenuBar.userrole.Count
}

$scriptBlockSetSearchMenuButton = {
    param (
        $ishDeployName,
        $parametersHash
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }

    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Set-ISHUISearchMenuBarItem -ISHDeployment $ishDeploy @parametersHash
}

$scriptBlockMoveSearchMenuButton = {
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
        Move-ISHUISearchMenuBarItem -ISHDeployment $ishDeploy @parametersHash -First
    }
    elseif($switchState -eq "Last"){
        Move-ISHUISearchMenuBarItem -ISHDeployment $ishDeploy @parametersHash -Last
    }
    else{
        Move-ISHUISearchMenuBarItem -ISHDeployment $ishDeploy @parametersHash
    }
}

$scriptBlockRemoveSearchMenuBar= {
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
    Remove-ISHUISearchMenuBarItem -ISHDeployment $ishDeploy -Label $label
}

Describe "Testing ISHUISearchMenuBarItem"{
    BeforeEach {
		ArtifactCleaner -filePath $xmlPath -fileName "SearchMenuBar.xml"
		UndoDeploymentBackToVanila $testingDeploymentName $true
    }

    It "Set search menu button"{
        #Arrange
        $params = @{Label = $testLabelName; UserRole = @("Administrator", "Translator"); Type = "Default"; SearchXML = "SearchNewGeneralNotExist" }
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetSearchMenuButton -Session $session -ArgumentList $testingDeploymentName, $params -WarningVariable Warning
        #Assert
        $item = getSearchMenuButton -Label $params.Label
        $item.Label | Should be $params.Label
        $item.Action | Should be "SearchFrame.asp?SearchXml=SearchNewGeneral&amp;Title=$testLabelName"
        $item.Icon | Should be "./UIFramework/search.32x32.png"
        $item.UserRole[0] | Should Match $params.UserRole[0]
        $item.UserRole[1] | Should Match $params.UserRole[1]
        $Warning | Should be "File SearchNewGeneralNotExist.xml does not exist" 
    }

    It "Remove search menu button"{
        #Arrange
        $params = @{Label = $testLabelName; UserRole = @("Administrator", "Translator"); Icon = "icon.png"; Type = "Default"; SearchXML = "SearchNewGeneral" }
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetSearchMenuButton -Session $session -ArgumentList $testingDeploymentName, $params
        getCountSearchMenuButton -Label $params.Label | Should be 1
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveSearchMenuBar -Session $session -ArgumentList $testingDeploymentName, $testLabelName
        #Assert
        $item = getSearchMenuButton -Label $params.Label
        $item | Should be $null
        getCountSearchMenuButton -Label $params.Label | Should be 0
    }
       
    It "Sets search menu button with no XML"{
        #Arrange
        Rename-Item "$xmlPath\SearchMenuBar.xml" "_SearchMenuBar.xml"
        $params = @{Label = $testLabelName; UserRole = @("Administrator", "Translator"); Icon = "icon.png"; Type = "Default"; SearchXML = "SearchNewGeneral" }
        #Act/Assert
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetSearchMenuButton -Session $session -ArgumentList $testingDeploymentName, $params} |Should Throw "Could not find file" 
        #Rollback
        Rename-Item "$xmlPath\_SearchMenuBar.xml" "SearchMenuBar.xml"
    }    
      
    It "Sets search menu button with wrong XML"{
        #Arrange
        # Running valid scenario commandlet to out files into backup folder before they will ba manually modified in test
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveSearchMenuBar -Session $session -ArgumentList $testingDeploymentName, $testLabelName
        Rename-Item "$xmlPath\SearchMenuBar.xml" "_SearchMenuBar.xml"
        New-Item "$xmlPath\SearchMenuBar.xml" -type file |Out-Null
        $params = @{Label = $testLabelName; UserRole = @("Administrator", "Translator"); Icon = "icon.png"; Type = "Default"; SearchXML = "SearchNewGeneral" }
        #Act/Assert
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetSearchMenuButton -Session $session -ArgumentList $testingDeploymentName, $params} |Should Throw "Root element is missing" 
        #Rollback
        Remove-Item "$xmlPath\SearchMenuBar.xml"
        Rename-Item "$xmlPath\_SearchMenuBar.xml" "SearchMenuBar.xml"
    }
  
    It "Remove search menu button with no XML"{
        #Arrange
        # Running valid scenario commandlet to out files into backup folder before they will ba manually modified in test
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveSearchMenuBar -Session $session -ArgumentList $testingDeploymentName, $testLabelName
        Rename-Item "$xmlPath\SearchMenuBar.xml" "_SearchMenuBar.xml"
        New-Item "$xmlPath\SearchMenuBar.xml" -type file |Out-Null
        #Act/Assert
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveSearchMenuBar -Session $session -ArgumentList $testingDeploymentName, $testLabelName} |Should Throw "Root element is missing" 
        #Rollback
        Remove-Item "$xmlPath\SearchMenuBar.xml"
        Rename-Item "$xmlPath\_SearchMenuBar.xml" "SearchMenuBar.xml"
    }

    It "Set existing search menu button"{
        #Arrange
        $params = @{Label = $testLabelName; UserRole = @("Administrator", "Translator"); Icon = "icon.png"; Type = "Default"; SearchXML = "SearchNewGeneral" }
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetSearchMenuButton -Session $session -ArgumentList $testingDeploymentName, $params
        #Assert
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetSearchMenuButton -Session $session -ArgumentList $testingDeploymentName, $params} | Should Not Throw
        getCountSearchMenuButton -Label $params.Label | Should be 1
    }

    It "Remove unexisting search menu button"{
        #Arrange
        if((getCountSearchMenuButton -Label $params.Label) -gt 0){
        Write-Host "Remove"
            Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveSearchMenuBar -Session $session -ArgumentList $testingDeploymentName, $testLabelName
        }
        #Act/Assert
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveSearchMenuBar -Session $session -ArgumentList $testingDeploymentName, $testLabelName} | Should Not Throw
    }

    It "Move After"{
        #Arrange
        #getting 2 arrays with labels of nodes of Event Monitor Menu bar
        [xml]$XmlSearchMenuBar= Get-Content "$xmlPath\SearchMenuBar.xml"  -ErrorAction SilentlyContinue
        $labelArray = $XmlSearchMenuBar.searchmenubar.menuitem.label
        #don't rewrite this to $tempArray = $labelArray - in powershell it will be link to an object and if one changes - another will change too and test fails
        $tempArray = $XmlSearchMenuBar.searchmenubar.menuitem.label
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
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockMoveSearchMenuButton -Session $session -ArgumentList $testingDeploymentName, $params

        #read the updated xml file
        [xml]$XmlSearchMenuBar = RemoteReadXML "$xmlPath\SearchMenuBar.xml"
        $thirdArray =$XmlSearchMenuBar.searchmenubar.menuitem.label 
    
        #Compare 2 arrays
        $compareArrayResult = 0
        for ($i=0; $i -le $arrayLength - 1;$i++){
            if ($labelArray[$i] -ne $thirdArray[$i]){$compareArrayResult++}
        }
        $checkResult = $compareArrayResult -eq 0
    
        #Assert
        $checkResult | Should Be "True"
    }

    It "Move Last"{
        #Arrange
        #getting 2 arrays with labels of nodes of Event Monitor Menu bar
        [xml]$XmlSearchMenuBar= Get-Content "$xmlPath\SearchMenuBar.xml"  -ErrorAction SilentlyContinue
        $labelArray = $XmlSearchMenuBar.searchmenubar.menuitem.label
        #don't rewrite this to $tempArray = $labelArray - in powershell it will be link to an object and if one changes - another will change too and test fails
        $tempArray = $XmlSearchMenuBar.searchmenubar.menuitem.label
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
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockMoveSearchMenuButton -Session $session -ArgumentList $testingDeploymentName, $params, "Last"

        #read the updated xml file
        [xml]$XmlSearchMenuBar = RemoteReadXML "$xmlPath\SearchMenuBar.xml"
        $thirdArray =$XmlSearchMenuBar.searchmenubar.menuitem.label 
    
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
        [xml]$XmlSearchMenuBar= Get-Content "$xmlPath\SearchMenuBar.xml"  -ErrorAction SilentlyContinue
        $labelArray = $XmlSearchMenuBar.searchmenubar.menuitem.label
        $arrayLength = $labelArray.Length
        #select label that will be moved
        $moveblelabel = $labelArray[$arrayLength -1]
        $params = @{label = $moveblelabel; After = $moveblelabel }
        #Act
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockMoveSearchMenuButton -Session $session -ArgumentList $testingDeploymentName, $params} | Should Not Throw

        #Get updated array with labels
        [xml]$XmlSearchMenuBar = RemoteReadXML "$xmlPath\SearchMenuBar.xml"
        $compareArray =$XmlSearchMenuBar.searchmenubar.menuitem.label
        #Compare 2 arrays - before move and after - they should be same
        $compareArrayResult = compareArray -firstArray $labelArray -secondArray $compareArray
    
        #Assert
        ($compareArrayResult -eq 0) | Should Be "True"
    }

    It "Move After non-existing label"{
        #Arrange
        #get array with labels on nodes in Event Monitor Menu Bar
        [xml]$XmlSearchMenuBar= Get-Content "$xmlPath\SearchMenuBar.xml"  -ErrorAction SilentlyContinue
        $labelArray = $XmlSearchMenuBar.searchmenubar.menuitem.label
        $arrayLength = $labelArray.Length
        #select label that will be moved
        $moveblelabel = $labelArray[$arrayLength -1]
        $params = @{label = $moveblelabel; After = $invalidLabel }
        #Act
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockMoveSearchMenuButton -Session $session -ArgumentList $testingDeploymentName, $params} | Should Not Throw

        #Get updated array with labels
        [xml]$XmlSearchMenuBar = RemoteReadXML "$xmlPath\SearchMenuBar.xml"
        $compareArray =$XmlSearchMenuBar.searchmenubar.menuitem.label
        #Compare 2 arrays - before move and after - they should be same
        $compareArrayResult = compareArray -firstArray $labelArray -secondArray $compareArray
    
        #Assert
        ($compareArrayResult -eq 0) | Should Be "True"
    }

    It "Move After non-existing label"{
        #Arrange
        #get array with labels on nodes in Event Monitor Menu Bar
        [xml]$XmlSearchMenuBar= Get-Content "$xmlPath\SearchMenuBar.xml"  -ErrorAction SilentlyContinue
        $labelArray = $XmlSearchMenuBar.searchmenubar.menuitem.label
        $arrayLength = $labelArray.Length
        #select label that will be moved
        $moveblelabel = $labelArray[$arrayLength -1]
        $params = @{label = $invalidLabel ; After = $moveblelabel }
        #Act
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockMoveSearchMenuButton -Session $session -ArgumentList $testingDeploymentName, $params} | Should Not Throw

        #Get updated array with labels
        [xml]$XmlSearchMenuBar = RemoteReadXML "$xmlPath\SearchMenuBar.xml"
        $compareArray =$XmlSearchMenuBar.searchmenubar.menuitem.label
        #Compare 2 arrays - before move and after - they should be same
        $compareArrayResult = compareArray -firstArray $labelArray -secondArray $compareArray
    
        #Assert
        ($compareArrayResult -eq 0)| Should Be "True"
    }

    It "Move First"{
        #Arrange
        #getting 2 arrays with labels of nodes of Event Monitor Menu bar
        [xml]$XmlSearchMenuBar= Get-Content "$xmlPath\SearchMenuBar.xml"  -ErrorAction SilentlyContinue
        $labelArray = $XmlSearchMenuBar.searchmenubar.menuitem.label
        #don't rewrite this to $tempArray = $labelArray - in powershell it will be link to an object and if one changes - another will change too and test fails
        $tempArray = $XmlSearchMenuBar.searchmenubar.menuitem.label
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
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockMoveSearchMenuButton -Session $session -ArgumentList $testingDeploymentName, $params, "First"

        #read the updated xml file
        [xml]$XmlSearchMenuBar = RemoteReadXML "$xmlPath\SearchMenuBar.xml"
        $thirdArray =$XmlSearchMenuBar.searchmenubar.menuitem.label 
    
        #Compare 2 arrays
        $compareArrayResult = 0
        for ($i=0; $i -le $arrayLength - 1;$i++){
            if ($labelArray[$i] -ne $thirdArray[$i]){$compareArrayResult++}
        }
        $checkResult = $compareArrayResult -eq 0 
        $CheckResult | Should Be "True"
     }

	It "Update existing item"{
        #Arrange
        $params = @{Label = $testLabelName; UserRole = @("Administrator", "Translator"); Icon = "icon.png"; Type = "Default"; SearchXML = "SearchNewGeneral" }
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetSearchMenuButton -Session $session -ArgumentList $testingDeploymentName, $params
        $item = getSearchMenuButton -Label $params.Label
        $item.UserRole.Count | Should Be 2
        $params = @{Label = $testLabelName; UserRole = @("Administrator", "Translator", "Reviewer"); Icon = "icon.png"; Type = "Default"; SearchXML = "SearchNewGeneral" }
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetSearchMenuButton -Session $session -ArgumentList $testingDeploymentName, $params
        #Assert
        $item = getSearchMenuButton -Label $params.Label
        $item.UserRole.Count | Should Be 3
    }
}