Import-Module InfoShare.Deployment
. "$PSScriptRoot\Common.ps1"
CLS

#region Variables initoialization
$htmlStyle = Set-Style

$logFile = "D:\Test4.htm"

$WarningPreference = “Continue"

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
$xmlPath = $xmlPath + "\Author\ASP\Editors\Xopus\config"

#LogArray for tests results
$global:logArray = @()

#endregion

function readTargetXML(){
[xml]$XmlConfig = Get-Content "$xmlPath\config.xml" -ErrorAction SilentlyContinue
[xml]$XmlBlueLionConfig = Get-Content "$xmlPath\bluelion-config.xml" -ErrorAction SilentlyContinue

$global:textConfig = $XmlConfig.config.javascript | ? {$_.src -eq "../BlueLion-Plugin/Bootstrap/bootstrap.js"}
$global:textBlueLionConfig = $XmlBlueLionConfig.SelectNodes("*/*[local-name()='import'][@src='../BlueLion-Plugin/create-toolbar.xml']")
}


#region Tests
function enableQualityAssistance_test(){

    readTargetXML

   if ((Test-Path "$xmlPath\config.xml") -and ("$xmlPath\bluelion-config.xml")){ 

        if ($textConfig -and $textBlueLionConfig.Count -eq 1 ) {
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
            logger "1" $MyInvocation.MyCommand.Name "Blocked" " "
    }
}

function disableQualityAssistance_test(){

    readTargetXML

   if ((Test-Path "$xmlPath\config.xml") -and ("$xmlPath\bluelion-config.xml")){ 

        if (!$textConfig -and $textBlueLionConfig.Count -eq 0 ) {
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
            logger "2" $MyInvocation.MyCommand.Name "Blocked" " "
    }
}

function enableEnabledQualityAssistance_test(){

    Enable-ISHUIQualityAssistant -ISHDeployment $deploy
    Enable-ISHUIQualityAssistant -ISHDeployment $deploy

    readTargetXML

   if ((Test-Path "$xmlPath\config.xml") -and ("$xmlPath\bluelion-config.xml")){ 

        if ($textConfig -and $textBlueLionConfig.Count -eq 1 ) {
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
            logger "3" $MyInvocation.MyCommand.Name "Blocked" " "
    }
}

function disableDisabledQualityAssistance_test(){
    Disable-ISHUIQualityAssistant -ISHDeployment $deploy
    Disable-ISHUIQualityAssistant -ISHDeployment $deploy
    readTargetXML

   if ((Test-Path "$xmlPath\config.xml") -and ("$xmlPath\bluelion-config.xml")){ 

        if (!$textConfig -and $textBlueLionConfig.Count -eq 0 ) {
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
            logger "4" $MyInvocation.MyCommand.Name "Blocked" " "
    }
}

function enableQualityAssistanceWithWrongXML_test(){

    Rename-Item "$xmlPath\config.xml" "_config.xml"
    New-Item "$xmlPath\config.xml" -type file |Out-Null
  
        try
        {
            Enable-ISHUIQualityAssistant -WarningVariable Warning -ErrorAction Stop 
        
        }
        catch 
        {
            $ErrorMessage = $_.Exception.Message
        }

   if ((Test-Path "$xmlPath\config.xml") -and ("$xmlPath\bluelion-config.xml")){ 

        if ($ErrorMessage -Match "Root element is missing") {
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
            logger "5" $MyInvocation.MyCommand.Name "Blocked" " "
    }

    Remove-Item "$xmlPath\config.xml"
    Rename-Item "$xmlPath\_config.xml" "config.xml"
}

function disableQualityAssistanceWithWrongXML_test(){

    Rename-Item "$xmlPath\config.xml" "_config.xml"
    New-Item "$xmlPath\config.xml" -type file |Out-Null
  
        try
        {
            Disable-ISHUIQualityAssistant -WarningVariable Warning -ErrorAction Stop 
        
        }
        catch 
        {
            $ErrorMessage = $_.Exception.Message
        }

   if ((Test-Path "$xmlPath\config.xml") -and ("$xmlPath\bluelion-config.xml")){ 

        if ($ErrorMessage -Match "Root element is missing") {
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
            logger "6" $MyInvocation.MyCommand.Name "Blocked" " "
    }

    Remove-Item "$xmlPath\config.xml"
    Rename-Item "$xmlPath\_config.xml" "config.xml"
}

function enableQualityAssistanceWithNoXML_test(){

    Rename-Item "$xmlPath\config.xml" "_config.xml"
    
  
        try
        {
            Enable-ISHUIQualityAssistant -WarningVariable Warning -ErrorAction Stop 
        
        }
        catch 
        {
            $ErrorMessage = $_.Exception.Message
        }

   if (!(Test-Path "$xmlPath\config.xml") -and ("$xmlPath\bluelion-config.xml")){ 

        if ($ErrorMessage -Match "Could not find file") {
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
            logger "7" $MyInvocation.MyCommand.Name "Blocked" " "
    }

    
    Rename-Item "$xmlPath\_config.xml" "config.xml"
}

function disableQualityAssistanceWithNoXML_test(){

    Rename-Item "$xmlPath\config.xml" "_config.xml"
    
  
        try
        {
            Disable-ISHUIQualityAssistant -WarningVariable Warning -ErrorAction Stop 
        
        }
        catch 
        {
            $ErrorMessage = $_.Exception.Message
        }

   if (!(Test-Path "$xmlPath\config.xml") -and ("$xmlPath\bluelion-config.xml")){ 

        if ($ErrorMessage -Match "Could not find file") {
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
            logger "8" $MyInvocation.MyCommand.Name "Blocked" " "
    }

    
    Rename-Item "$xmlPath\_config.xml" "config.xml"
}

#endregion


Enable-ISHUIQualityAssistant -ISHDeployment $deploy

enableQualityAssistance_test

Disable-ISHUIQualityAssistant -ISHDeployment $deploy

disableQualityAssistance_test

enableEnabledQualityAssistance_test
disableDisabledQualityAssistance_test


enableQualityAssistanceWithWrongXML_test
disableQualityAssistanceWithWrongXML_test

enableQualityAssistanceWithNoXML_test
disableQualityAssistanceWithNoXML_test

$global:logArray | ConvertTo-HTML -Head $htmlStyle | Out-File $logFile


Edit-LogHtml -targetHTML $logFile


Invoke-Expression $logFile