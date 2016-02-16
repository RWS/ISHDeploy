Import-Module InfoShare.Deployment
. "$PSScriptRoot\Common.ps1"
CLS

#region Variables initoialization
$htmlStyle = Set-Style

$logFile = "D:\Test5.htm"

$WarningPreference = “Continue"

$customID = "Default"

$dict = New-Object "System.Collections.Generic.Dictionary``2[System.String,System.String]"
$dict.Add('webpath', 'D:\InfoShare')
$dict.Add('apppath', 'D:\InfoSharer')
$dict.Add('projectsuffix', 'SQL2014')
$dict.Add('datapath', 'D:\InfoShare')
$version = New-Object System.Version -ArgumentList '1.0.0.0';

$deploy = New-Object InfoShare.Deployment.Models.ISHDeployment -ArgumentList ($dict, $version)


$xmlPath = $deploy.WebPath
$xmlPath = $xmlPath + "\Web" 
$xmlPath = $xmlPath + $deploy.Suffix 
$xmlPath = $xmlPath + "\Author\ASP"

#LogArray for tests results
$global:logArray = @()

#endregion


Enable-ISHExternalPreview -ISHDeployment $deploy
function readTargetXML(){
[xml]$XmlWebConfig = Get-Content "$xmlPath\Web.config" #-ErrorAction SilentlyContinue

$global:textConfig = $XmlWebConfig.configuration.'trisoft.infoshare.web.externalpreviewmodule'.identity | ? {$_.externalId -eq "ServiceUser"}
$global:configSection = $XmlWebConfig.configuration.configSections.section | ? {$_.name -eq "trisoft.infoshare.web.externalpreviewmodule"}
$global:module = $XmlWebConfig.configuration.'system.webServer'.modules.add  | ? {$_.name -eq "TrisoftExternalPreviewModule"}
$global:configCustomID = $XmlWebConfig.configuration.'trisoft.infoshare.web.externalpreviewmodule'.identity | ? {$_.externalId -eq $customID}
}


function enableExternalPreview_test(){

    readTargetXML


   if (Test-Path "$xmlPath\Web.config"){ 

        if ($textConfig -and $configSection -and $module) {
            Write-Host $MyInvocation.MyCommand.Name -NoNewline
            Write-Host " Passed" -foregroundcolor "green" 
            logger "1" $MyInvocation.MyCommand.Name "Passed" " "
        }

        else {
             Write-Host $MyInvocation.MyCommand.Name -NoNewline 
             Write-Host " Failed"  -foregroundcolor "red"
             logger "1" $MyInvocation.MyCommand.Name "Failed" " "
        }
    }

    else{
        Write-Host $MyInvocation.MyCommand.Name -NoNewline
            Write-Host " Blocked" -foregroundcolor "yellow" 
            logger "1" $MyInvocation.MyCommand.Name "BLocked" " "
    }
}

function disableExternalPreview_test(){

    readTargetXML
 

   if (Test-Path "$xmlPath\Web.config"){ 

        if (!$textConfig -and !$configSection -and !$module) {
            Write-Host $MyInvocation.MyCommand.Name -NoNewline
            Write-Host " Passed" -foregroundcolor "green" 
            logger "2" $MyInvocation.MyCommand.Name "Passed" " "
        }

        else {
             Write-Host $MyInvocation.MyCommand.Name -NoNewline 
             Write-Host " Failed"  -foregroundcolor "red"
             logger "2" $MyInvocation.MyCommand.Name "Failed" " "
        }
    }

    else{
        Write-Host $MyInvocation.MyCommand.Name -NoNewline
            Write-Host " Blocked" -foregroundcolor "yellow" 
            logger "2" $MyInvocation.MyCommand.Name "BLocked" " "
    }
}

function enableEnabledExternalPreview_test(){

    
    Enable-ISHExternalPreview -ISHDeployment $deploy
    Enable-ISHExternalPreview -ISHDeployment $deploy
    readTargetXML

   if (Test-Path "$xmlPath\Web.config"){ 

        if ($textConfig -and $configSection -and $module) {
            Write-Host $MyInvocation.MyCommand.Name -NoNewline
            Write-Host " Passed" -foregroundcolor "green" 
            logger "3" $MyInvocation.MyCommand.Name "Passed" " "
        }

        else {
             Write-Host $MyInvocation.MyCommand.Name -NoNewline 
             Write-Host " Failed"  -foregroundcolor "red"
             logger "3" $MyInvocation.MyCommand.Name "Failed" " "
        }
    }

    else{
        Write-Host $MyInvocation.MyCommand.Name -NoNewline
            Write-Host " Blocked" -foregroundcolor "yellow" 
            logger "3" $MyInvocation.MyCommand.Name "BLocked" " "
    }
}

function disableDisabledExternalPreview_test(){

    Disable-ISHExternalPreview -ISHDeployment $deploy
    Disable-ISHExternalPreview -ISHDeployment $deploy

    readTargetXML
 

   if (Test-Path "$xmlPath\Web.config"){ 

        if (!$textConfig -and !$configSection -and !$module) {
            Write-Host $MyInvocation.MyCommand.Name -NoNewline
            Write-Host " Passed" -foregroundcolor "green" 
            logger "4" $MyInvocation.MyCommand.Name "Passed" " "
        }

        else {
             Write-Host $MyInvocation.MyCommand.Name -NoNewline 
             Write-Host " Failed"  -foregroundcolor "red"
             logger "4" $MyInvocation.MyCommand.Name "Failed" " "
        }
    }

    else{
        Write-Host $MyInvocation.MyCommand.Name -NoNewline
            Write-Host " Blocked" -foregroundcolor "yellow" 
            logger "4" $MyInvocation.MyCommand.Name "BLocked" " "
    }
}

function enableExternalPreviewWithCustomExternalID_test(){

    Enable-ISHExternalPreview -ISHDeployment $deploy -ExternalId $customID
    readTargetXML


   if (Test-Path "$xmlPath\Web.config"){ 

        if (!$textConfig -and $configSection -and $module -and $configCustomID) {
            Write-Host $MyInvocation.MyCommand.Name -NoNewline
            Write-Host " Passed" -foregroundcolor "green" 
            logger "5" $MyInvocation.MyCommand.Name "Passed" " "
        }

        else {
             Write-Host $MyInvocation.MyCommand.Name -NoNewline 
             Write-Host " Failed"  -foregroundcolor "red"
             logger "5" $MyInvocation.MyCommand.Name "Failed" " "
        }
    }

    else{
        Write-Host $MyInvocation.MyCommand.Name -NoNewline
            Write-Host " Blocked" -foregroundcolor "yellow" 
            logger "5" $MyInvocation.MyCommand.Name "BLocked" " "
    }
}

function disableExternalPreviewWithCustomExternalID_test(){

    Disable-ISHExternalPreview -ISHDeployment $deploy
    readTargetXML


   if (Test-Path "$xmlPath\Web.config"){ 

        if (!$textConfig -and !$configSection -and !$module -and !$configCustomID) {
            Write-Host $MyInvocation.MyCommand.Name -NoNewline
            Write-Host " Passed" -foregroundcolor "green" 
            logger "6" $MyInvocation.MyCommand.Name "Passed" " "
        }

        else {
             Write-Host $MyInvocation.MyCommand.Name -NoNewline 
             Write-Host " Failed"  -foregroundcolor "red"
             logger "6" $MyInvocation.MyCommand.Name "Failed" " "
        }
    }

    else{
        Write-Host $MyInvocation.MyCommand.Name -NoNewline
            Write-Host " Blocked" -foregroundcolor "yellow" 
            logger "6" $MyInvocation.MyCommand.Name "BLocked" " "
    }
}

function enableExternalPreviewWithWrongXML_test(){

    Rename-Item "$xmlPath\Web.config" "_Web.config"
    New-Item "$xmlPath\Web.config" -type file |Out-Null
  
        try
        {
              Enable-ISHExternalPreview -WarningVariable Warning -ErrorAction Stop 
        
        }
        catch 
        {
            $ErrorMessage = $_.Exception.Message
        }

   if (Test-Path "$xmlPath\Web.config"){ 

        if ($ErrorMessage -Match "Root element is missing") {
            Write-Host $MyInvocation.MyCommand.Name -NoNewline
            Write-Host " Passed" -foregroundcolor "green" 
            logger "7" $MyInvocation.MyCommand.Name "Passed" " "
        }

        else {
             Write-Host $MyInvocation.MyCommand.Name -NoNewline 
             Write-Host " Failed"  -foregroundcolor "red"
             logger "7" $MyInvocation.MyCommand.Name "Failed" " "
        }
    }

    else{
        Write-Host $MyInvocation.MyCommand.Name -NoNewline
            Write-Host " Blocked" -foregroundcolor "yellow" 
            logger "7" $MyInvocation.MyCommand.Name "BLocked" " "
    }

    Remove-Item "$xmlPath\Web.config"
    Rename-Item "$xmlPath\_Web.config" "Web.config"
}

function disableExternalPreviewWithWrongXML_test(){

    Rename-Item "$xmlPath\Web.config" "_Web.config"
    New-Item "$xmlPath\Web.config" -type file |Out-Null
  
        try
        {
              Disable-ISHExternalPreview -WarningVariable Warning -ErrorAction Stop 
        
        }
        catch 
        {
            $ErrorMessage = $_.Exception.Message
        }

   if (Test-Path "$xmlPath\Web.config"){ 

        if ($ErrorMessage -Match "Root element is missing") {
            Write-Host $MyInvocation.MyCommand.Name -NoNewline
            Write-Host " Passed" -foregroundcolor "green" 
            logger "8" $MyInvocation.MyCommand.Name "Passed" " "
        }

        else {
             Write-Host $MyInvocation.MyCommand.Name -NoNewline 
             Write-Host " Failed"  -foregroundcolor "red"
             logger "8" $MyInvocation.MyCommand.Name "Failed" " "
        }
    }

    else{
        Write-Host $MyInvocation.MyCommand.Name -NoNewline
            Write-Host " Blocked" -foregroundcolor "yellow" 
            logger "8" $MyInvocation.MyCommand.Name "BLocked" " "
    }

    Remove-Item "$xmlPath\Web.config"
    Rename-Item "$xmlPath\_Web.config" "Web.config"
}

function enableExternalPreviewWithNoXML_test(){

    Rename-Item "$xmlPath\Web.config" "_Web.config"
   
  
        try
        {
              Enable-ISHExternalPreview -WarningVariable Warning -ErrorAction Stop 
        
        }
        catch 
        {
            $ErrorMessage = $_.Exception.Message
        }

   if (!(Test-Path "$xmlPath\Web.config")){ 

        if ($ErrorMessage -Match "Could not find file") {
            Write-Host $MyInvocation.MyCommand.Name -NoNewline
            Write-Host " Passed" -foregroundcolor "green" 
            logger "9" $MyInvocation.MyCommand.Name "Passed" " "
        }

        else {
             Write-Host $MyInvocation.MyCommand.Name -NoNewline 
             Write-Host " Failed"  -foregroundcolor "red"
             logger "9" $MyInvocation.MyCommand.Name "Failed" " "
        }
    }

    else{
        Write-Host $MyInvocation.MyCommand.Name -NoNewline
            Write-Host " Blocked" -foregroundcolor "yellow" 
            logger "9" $MyInvocation.MyCommand.Name "BLocked" " "
    }

   
    Rename-Item "$xmlPath\_Web.config" "Web.config"
}

function disableExternalPreviewWithNoXML_test(){

    Rename-Item "$xmlPath\Web.config" "_Web.config"
   
  
        try
        {
              Disable-ISHExternalPreview -WarningVariable Warning -ErrorAction Stop 
        
        }
        catch 
        {
            $ErrorMessage = $_.Exception.Message
        }

   if (!(Test-Path "$xmlPath\Web.config")){ 

        if ($ErrorMessage -Match "Could not find file") {
            Write-Host $MyInvocation.MyCommand.Name -NoNewline
            Write-Host " Passed" -foregroundcolor "green" 
            logger "10" $MyInvocation.MyCommand.Name "Passed" " "
        }

        else {
             Write-Host $MyInvocation.MyCommand.Name -NoNewline 
             Write-Host " Failed"  -foregroundcolor "red"
             logger "10" $MyInvocation.MyCommand.Name "Failed" " "
        }
    }

    else{
        Write-Host $MyInvocation.MyCommand.Name -NoNewline
            Write-Host " Blocked" -foregroundcolor "yellow" 
            logger "10" $MyInvocation.MyCommand.Name "Blocked" " "
    }

   
    Rename-Item "$xmlPath\_Web.config" "Web.config"
}
Disable-ISHExternalPreview -ISHDeployment $deploy


Enable-ISHExternalPreview -ISHDeployment $deploy
enableExternalPreview_test

Disable-ISHExternalPreview -ISHDeployment $deploy
disableExternalPreview_test

enableEnabledExternalPreview_test
disableDisabledExternalPreview_test

enableExternalPreviewWithCustomExternalID_test
disableExternalPreviewWithCustomExternalID_test

enableExternalPreviewWithWrongXML_test
disableExternalPreviewWithWrongXML_test

enableExternalPreviewWithNoXML_test
disableExternalPreviewWithNoXML_test

$global:logArray | ConvertTo-HTML -Head $htmlStyle | Out-File $logFile


Edit-LogHtml -targetHTML $logFile


Invoke-Expression $logFile

   #$scriptname = split-path $MyInvocation.ScriptName -Leaf
    #Write-Host " this " $scriptname

   