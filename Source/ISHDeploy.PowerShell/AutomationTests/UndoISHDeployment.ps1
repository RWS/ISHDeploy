Import-Module ISHDeploy
. "$PSScriptRoot\Common.ps1"

#region Variables initoialization
$htmlStyle = Set-Style

$logFile = "C:\Automated_deployment\Test8.htm"


$WarningPreference = “Continue"
$VerbosePreference = "SilentlyCOntinue"
$global:logArray = @()

$deploy = Get-ISHDeployment -Name "InfoShareSQL2014"


function readTargetXML(){
#Content Editor XML
$CExmlPath = $deploy.WebPath+"\Web" + $deploy.OriginalParameters.projectsuffix   + "\Author\ASP\XSL"
[xml]$XmlFolderButtonbar = Get-Content "$CExmlPath\FolderButtonbar.xml" -ErrorAction SilentlyContinue
[xml]$XmlInboxButtonBar = Get-Content "$CExmlPath\InboxButtonBar.xml" -ErrorAction SilentlyContinue
[xml]$XmlLanguageDocumentButtonbar = Get-Content "$CExmlPath\LanguageDocumentButtonbar.xml" -ErrorAction SilentlyContinue
$global:textFolderButtonbar = $XmlFolderButtonbar.BUTTONBAR.BUTTON.INPUT | ? {$_.NAME -eq "CheckOutWithXopus"}
$global:textInboxButtonBar = $XmlInboxButtonBar.BUTTONBAR.BUTTON.INPUT | ? {$_.NAME -eq "CheckOutWithXopus"}
$global:textLanguageDocumentButtonbar = $XmlLanguageDocumentButtonbar.BUTTONBAR.BUTTON.INPUT | ? {$_.NAME -eq "CheckOutWithXopus"}

#External preview XML
$EPxmlPath = $deploy.WebPath+"\Web" + $deploy.OriginalParameters.projectsuffix   + "\Author\ASP"
[xml]$XmlWebConfig = Get-Content "$EPxmlPath\Web.config" #-ErrorAction SilentlyContinue

$global:textConfig = $XmlWebConfig.configuration.'trisoft.infoshare.web.externalpreviewmodule'.identity | ? {$_.externalId -eq "ServiceUser"}
$global:configSection = $XmlWebConfig.configuration.configSections.section | ? {$_.name -eq "trisoft.infoshare.web.externalpreviewmodule"}
$global:module = $XmlWebConfig.configuration.'system.webServer'.modules.add  | ? {$_.name -eq "TrisoftExternalPreviewModule"}
$global:configCustomID = $XmlWebConfig.configuration.'trisoft.infoshare.web.externalpreviewmodule'.identity | ? {$_.externalId -eq $customID}

#Quality Assistant XML
$QAxmlPath =$deploy.WebPath+"\Web" + $deploy.OriginalParameters.projectsuffix  + '\Author\ASP\Editors\Xopus\config'
[xml]$XmlConfig = Get-Content "$QAxmlPath\config.xml" -ErrorAction SilentlyContinue
[xml]$XmlBlueLionConfig = Get-Content "$QAxmlPath\bluelion-config.xml" -ErrorAction SilentlyContinue

$global:textConfig = $XmlConfig.config.javascript | ? {$_.src -eq "../BlueLion-Plugin/Bootstrap/bootstrap.js"}
$global:textBlueLionConfig = $XmlBlueLionConfig.SelectNodes("*/*[local-name()='import'][@src='../BlueLion-Plugin/create-toolbar.xml']")

}

function fileExist(){
$exist =     
    (Test-Path ("C:\ProgramData\ISHDeploy\v" + $deploy.SoftwareVersion +"\"+ $deploy.Name  + "\Backup\Web\Author\ASP\Editors\Xopus\config\config.xml")) -and
    (Test-Path ("C:\ProgramData\ISHDeploy\v" + $deploy.SoftwareVersion +"\"+ $deploy.Name  + "\Backup\Web\Author\ASP\Editors\Xopus\config\bluelion-config.xml")) -and
    (Test-Path ("C:\ProgramData\ISHDeploy\v" + $deploy.SoftwareVersion +"\"+ $deploy.Name  + "\Backup\Web\Author\ASP\XSL\FolderButtonbar.xml")) -and
    (Test-Path ("C:\ProgramData\ISHDeploy\v" + $deploy.SoftwareVersion +"\"+ $deploy.Name  + "\Backup\Web\Author\ASP\XSL\InboxButtonBar.xml")) -and
    (Test-Path ("C:\ProgramData\ISHDeploy\v" + $deploy.SoftwareVersion +"\"+ $deploy.Name  + "\Backup\Web\Author\ASP\XSL\LanguageDocumentButtonbar.xml")) -and
    (Test-Path ("C:\ProgramData\ISHDeploy\v" + $deploy.SoftwareVersion +"\"+ $deploy.Name  + "\Backup\Web\Author\ASP\Web.config"))

 return $exist   
}

 Remove-Item ("C:\ProgramData\ISHDeploy\ISH"+$deploy.OriginalParameters.projectsuffix ) -Recurse -Force -ErrorAction SilentlyContinue

function UndoISHDeployment_test(){

    Enable-ISHExternalPreview -ISHDeployment $deploy 
    Enable-ISHUIContentEditor -ISHDeployment $deploy
    Enable-ISHUIQualityAssistant -ISHDeployment $deploy
    #Action
    readTargetXML

    $CECheck = $textFolderButtonbar -and $textInboxButtonBar -and $textLanguageDocumentButtonbar
    $QACheck = $textConfig -and $textBlueLionConfig.Count -eq 1
    $EPCheck = $textConfig -and $configSection -and $module

    $backupFilesExists = fileExist

    $initialCheckResult = $CECheck -and $QACheck -and $EPCheck -and $backupFilesExists

    
    Undo-ISHDeployment -ISHDeployment $deploy

    readTargetXML

    $CECheck = !$textFolderButtonbar -and !$textInboxButtonBar -and !$textLanguageDocumentButtonbar
    $QACheck = !$textConfig -and $textBlueLionConfig.Count -eq 0
    $EPCheck = !$textConfig -and !$configSection -and !$module

    $backupFilesExists = fileExist

    $secondaryCheckResult = !$CECheck -and !$QACheck -and !$EPCheck -and !$backupFilesExists

    $checkResult = $initialCheckResult -and $secondaryCheckResult

    # Assert
    Assert_IsTrue $checkResult $MyInvocation.MyCommand.Name "1"
 }

function VanilaBackupWhenCommandletNotSuccesful_test{
 Remove-Item ("C:\ProgramData\ISHDeploy\v" + $deploy.SoftwareVersion +"\"+ $deploy.Name ) -Recurse -Force -ErrorAction SilentlyContinue

 $targetFilePath = $deploy.WebPath+"\Web" + $deploy.OriginalParameters.projectsuffix  + '\Author\ASP\Editors\Xopus\config\'

 Rename-Item $targetFilePath"bluelion-config.xml" "_bluelion-config.xml"

 try{
  Enable-ISHUIQualityAssistant -ISHDeployment $deploy  -ErrorAction Stop
  }
 catch{
 
 }

    $checkResult = Test-Path ("C:\ProgramData\ISHDeploy\v" + $deploy.SoftwareVersion +"\"+ $deploy.Name  + "\Backup\Web\Author\ASP\Editors\Xopus\config\config.xml")
     Rename-Item $targetFilePath"_bluelion-config.xml" "bluelion-config.xml"
 # Assert
    Assert_IsTrue $checkResult $MyInvocation.MyCommand.Name "2"
 }




# Remove-Item ("C:\ProgramData\ISHDeploy\v" + $deploy.SoftwareVersion + "\"+ $deploy.Name ) -Recurse -Force -ErrorAction SilentlyContinue

UndoISHDeployment_test
VanilaBackupWhenCommandletNotSuccesful_test

$global:logArray | ConvertTo-HTML -Head $htmlStyle | Out-File $logFile


Edit-LogHtml -targetHTML $logFile


#Invoke-Expression $logFile