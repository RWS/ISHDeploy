param(
    $session = $null,
    $testingDeploymentName = "InfoShare"
)

. "$PSScriptRoot\Common.ps1"
$computerName = If ($session) {$session.ComputerName} Else {$env:COMPUTERNAME}

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

$testingDeployment = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetDeployment -Session $session -ArgumentList $testingDeploymentName

$moduleName = Invoke-CommandRemoteOrLocal -ScriptBlock { (Get-Module "ISHDeploy.*").Name } -Session $session
$backup = "\\$computerName\C$\ProgramData\$moduleName\$($testingDeployment.Name)"

$suffix = GetProjectSuffix($testingDeployment.Name)
$xmlPath = Join-Path ($testingDeployment.WebPath.replace(":", "$")) ("Web{0}\Author" -f $suffix )
$xmlPath = "\\$computerName\$xmlPath"

$CEXmlPath = Join-Path $xmlPath "ASP\XSL"
$EPxmlPath = Join-Path $xmlPath "ASP"
$QAxmlPath = Join-Path $xmlPath "ASP\Editors\Xopus\config"

$scriptBlockGet = {
    param (
        [Parameter(Mandatory=$false)]
        $ishDeployName 
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Get-ISHDeploymentHistory -ISHDeployment $ishDeploy
}

$scriptBlockClean = {
    param (
        [Parameter(Mandatory=$false)]
        $ishDeployName 
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Clear-ISHDeploymentHistory -ISHDeployment $ishDeploy
}

# Script block for Enable-ISHUIQualityAssistant. Added here for generating backup files
$scriptBlockEnableQA = {
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

$scriptBlockEnableContentEditor = {
    param (
        [Parameter(Mandatory=$true)]
        $ishDeployName 
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
        $ishDeploy = Get-ISHDeployment -Name $ishDeployName
        Enable-ISHUIContentEditor -ISHDeployment $ishDeploy
  
}

$scriptBlockBackupFilesExist = {
    param (
        [Parameter(Mandatory=$true)]
        $backup 
    )
	$exist =
		(Test-Path ("$backup\Backup\Web\Author\ASP\Editors\Xopus\config\config.xml")) -and
		(Test-Path ("$backup\Backup\Web\Author\ASP\Editors\Xopus\config\bluelion-config.xml")) -and
		(Test-Path ("$backup\Backup\Web\Author\ASP\XSL\FolderButtonbar.xml")) -and
		(Test-Path ("$backup\Backup\Web\Author\ASP\XSL\InboxButtonBar.xml")) -and
		(Test-Path ("$backup\Backup\Web\Author\ASP\XSL\LanguageDocumentButtonbar.xml")) -and
		(Test-Path ("$backup\Backup\Web\Author\ASP\Web.config"))

	return $exist   
}

$scriptBlockGetCountOfItemsInFolder = {
    param (
        [Parameter(Mandatory=$true)]
        $path
    )
	$items = Get-ChildItem -Path $path

	return $items.Count   
}

function readTargetXML(){
    #Content Editor XML
    [xml]$XmlFolderButtonbar = Get-Content "$CExmlPath\FolderButtonbar.xml" -ErrorAction SilentlyContinue
    [xml]$XmlInboxButtonBar = Get-Content "$CExmlPath\InboxButtonBar.xml" -ErrorAction SilentlyContinue
    [xml]$XmlLanguageDocumentButtonbar = Get-Content "$CExmlPath\LanguageDocumentButtonbar.xml" -ErrorAction SilentlyContinue
    $global:textFolderButtonbar = $XmlFolderButtonbar.BUTTONBAR.BUTTON.INPUT | ? {$_.NAME -eq "CheckOutWithXopus"}
    $global:textInboxButtonBar = $XmlInboxButtonBar.BUTTONBAR.BUTTON.INPUT | ? {$_.NAME -eq "CheckOutWithXopus"}
    $global:textLanguageDocumentButtonbar = $XmlLanguageDocumentButtonbar.BUTTONBAR.BUTTON.INPUT | ? {$_.NAME -eq "CheckOutWithXopus"}

    #External preview XML
    [xml]$XmlWebConfig = Get-Content "$EPxmlPath\Web.config" #-ErrorAction SilentlyContinue

    $global:textWebConfig = $XmlWebConfig.configuration.'trisoft.infoshare.web.externalpreviewmodule'.identity | ? {$_.externalId -eq "ServiceUser"}
    $global:configSection = $XmlWebConfig.configuration.configSections.section | ? {$_.name -eq "trisoft.infoshare.web.externalpreviewmodule"}
    $global:module = $XmlWebConfig.configuration.'system.webServer'.modules.add  | ? {$_.name -eq "TrisoftExternalPreviewModule"}
    $global:configCustomID = $XmlWebConfig.configuration.'trisoft.infoshare.web.externalpreviewmodule'.identity | ? {$_.externalId -eq $customID}

    #Quality Assistant XML
    [xml]$XmlConfig = Get-Content "$QAxmlPath\config.xml" -ErrorAction SilentlyContinue
    [xml]$XmlBlueLionConfig = Get-Content "$QAxmlPath\bluelion-config.xml" -ErrorAction SilentlyContinue

    $global:textConfig = $XmlConfig.config.javascript | ? {$_.src -eq "../BlueLion-Plugin/Bootstrap/bootstrap.js"}
    $global:textBlueLionConfig = $XmlBlueLionConfig.SelectNodes("*/*[local-name()='import'][@src='../BlueLion-Plugin/create-toolbar.xml']")

    $CECheck = $textFolderButtonbar -and $textInboxButtonBar -and $textLanguageDocumentButtonbar
    $QACheck = $textConfig -and $textBlueLionConfig.Count -eq 1
    $EPCheck = $textWebConfig -and $configSection -and $module

    $initialCheckResult = !$CECheck -and !$QACheck -and !$EPCheck
    
	if($initialCheckResult){
        return "VanilaState"
    }
    else{
        return "changedState"
    }
}

$scriptBlockEnableExternalPreview = {
    param (
        [Parameter(Mandatory=$true)]
        $ishDeployName,
        [Parameter(Mandatory=$false)]
        $externalId
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
    if(!$externalId){
        $ishDeploy = Get-ISHDeployment -Name $ishDeployName
        Enable-ISHExternalPreview -ISHDeployment $ishDeploy
    }
    else{
        $ishDeploy = Get-ISHDeployment -Name $ishDeployName
        Enable-ISHExternalPreview -ISHDeployment $ishDeploy -ExternalId $externalId
    }
  
}

#Import-Module WebAdministration
$scriptBlockGetAppPoolStartTime = {
    param (
        $testingDeployment
    )

    $cmAppPoolName = ("TrisoftAppPool{0}" -f $testingDeployment.OriginalParameters.infoshareauthorwebappname)
    $wsAppPoolName = ("TrisoftAppPool{0}" -f $testingDeployment.OriginalParameters.infosharewswebappname)
    $stsAppPoolName = ("TrisoftAppPool{0}" -f $testingDeployment.OriginalParameters.infosharestswebappname)
    
    $result = @{}
    [Array]$array = iex 'C:\Windows\system32\inetsrv\appcmd list wps'
    foreach ($line in $array) {
        $splitedLine = $line.split(" ")
        $processIdAsString = $splitedLine[1]
        $processId = $processIdAsString.Substring(1,$processIdAsString.Length-2)
        if ($splitedLine[2] -match $cmAppPoolName)
        {
            $result["cm"] = (Get-Process -Id $processId).StartTime
        }
        if ($splitedLine[2] -match $wsAppPoolName)
        {
            $result["ws"] = (Get-Process -Id $processId).StartTime
        }
        if ($splitedLine[2] -match $stsAppPoolName)
        {
            $result["sts"] = (Get-Process -Id $processId).StartTime
        }
    }
    
    return $result
}

Describe "Testing Undo-ISHDeploymentHistory"{
    BeforeEach {
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockUndoDeployment -Session $session -ArgumentList $testingDeploymentName
        WebRequestToSTS $testingDeploymentName
    }

    It "Undo ish deploy history"{
		# Try enabling Quality Assistant for generating backup files
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnableContentEditor -Session $session -ArgumentList $testingDeploymentName
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnableQA -Session $session -ArgumentList $testingDeploymentName
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnableExternalPreview -Session $session -ArgumentList $testingDeploymentName
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockBackupFilesExist -Session $session -ArgumentList $backup  | Should Be "True"
        readTargetXML | Should Be "changedState"
        # Get web application pool start times
        $appPoolStartTimes1 = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetAppPoolStartTime -Session $session -ArgumentList $testingDeployment

        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockUndoDeployment -Session $session -ArgumentList $testingDeploymentName
        
        # Get web application pool start times
        $appPoolStartTimes2 = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetAppPoolStartTime -Session $session -ArgumentList $testingDeployment

        (get-date $appPoolStartTimes1["cm"]) -gt (get-date $appPoolStartTimes2["cm"])  | Should Be $false
        (get-date $appPoolStartTimes1["ws"]) -gt (get-date $appPoolStartTimes2["ws"])  | Should Be $false
        (get-date $appPoolStartTimes1["sts"]) -gt (get-date $appPoolStartTimes2["sts"])  | Should Be $false



        RetryCommand -numberOfRetries 20 -command {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockBackupFilesExist -Session $session -ArgumentList $backup} -expectedResult $false | Should Be "False"
        
        readTargetXML | Should Be "VanilaState"
        $path =  Join-Path $testingDeployment.WebPath ("Web{0}\InfoShareSTS\App_Data\" -f $suffix )
        $countOfItemsInDataBaseFolder = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetCountOfItemsInFolder -Session $session -ArgumentList $path 
        $countOfItemsInDataBaseFolder | Should Be 1
    }

    It "Undo-IshHistory works when there is no backup"{
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockUndoDeployment -Session $session -ArgumentList $testingDeploymentName -WarningVariable Warning -ErrorAction Stop }| Should Not Throw
    }
}

