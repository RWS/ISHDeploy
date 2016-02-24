﻿Import-Module InfoShare.Deployment
. "$PSScriptRoot\Common.ps1"
CLS

#region Variables initoialization
$htmlStyle = Set-Style

$logFile = "C:\Automated_deployment\Test4.htm"

$WarningPreference = “Continue"

$dict = New-Object "System.Collections.Generic.Dictionary``2[System.String,System.String]"
$dict.Add('webpath', 'C:\InfoShare')
$dict.Add('apppath', 'C:\InfoShare')
$dict.Add('projectsuffix', 'SQL2014')
$dict.Add('datapath', 'C:\InfoShare')
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

    $checkResult = $textConfig -and $textBlueLionConfig.Count -eq 1
   if ((Test-Path "$xmlPath\config.xml") -and ("$xmlPath\bluelion-config.xml")){ 

        #Assert
        Assert_IsTrue $checkResult $MyInvocation.MyAction.Name "1"
    }

    else{
        TestIsBlocked $MyInvocation.MyAction.Name "1" "No target xml files" 
    }
}

function disableQualityAssistance_test(){

    readTargetXML

      $checkResult = !$textConfig -and $textBlueLionConfig.Count -eq 0
   if ((Test-Path "$xmlPath\config.xml") -and ("$xmlPath\bluelion-config.xml")){ 

        #Assert
        Assert_IsTrue $checkResult $MyInvocation.MyAction.Name "2"
    }

    else{
        TestIsBlocked $MyInvocation.MyAction.Name "2" "No target xml files" 
    }
}

function enableEnabledQualityAssistance_test(){

    Enable-ISHUIQualityAssistant -ISHDeployment $deploy
    Enable-ISHUIQualityAssistant -ISHDeployment $deploy

    readTargetXML
    $checkResult = $textConfig -and $textBlueLionConfig.Count -eq 1

   if ((Test-Path "$xmlPath\config.xml") -and ("$xmlPath\bluelion-config.xml")){ 

        #Assert
        Assert_IsTrue $checkResult $MyInvocation.MyAction.Name "3"
    }

    else{
      TestIsBlocked $MyInvocation.MyAction.Name "3" "No target xml files" 
    }
}

function disableDisabledQualityAssistance_test(){
    Disable-ISHUIQualityAssistant -ISHDeployment $deploy
    Disable-ISHUIQualityAssistant -ISHDeployment $deploy
    readTargetXML

    $checkResult = !$textConfig -and $textBlueLionConfig.Count -eq 0
   if ((Test-Path "$xmlPath\config.xml") -and ("$xmlPath\bluelion-config.xml")){ 

        #Assert
        Assert_IsTrue $checkResult $MyInvocation.MyAction.Name "4"
    }

    else{
        TestIsBlocked $MyInvocation.MyAction.Name "4" "No target xml files" 
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
        $checkResult = $ErrorMessage -Match "Root element is missing"
   if ((Test-Path "$xmlPath\config.xml") -and ("$xmlPath\bluelion-config.xml")){ 

         #Assert
        Assert_IsTrue $checkResult $MyInvocation.MyAction.Name "5"
    }

    else{
       TestIsBlocked $MyInvocation.MyAction.Name "5" "No target xml files" 
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

    $checkResult = $ErrorMessage -Match "Root element is missing"
   if ((Test-Path "$xmlPath\config.xml") -and ("$xmlPath\bluelion-config.xml")){ 

        
         #Assert
        Assert_IsTrue $checkResult $MyInvocation.MyAction.Name "6"
    }

    else{
        TestIsBlocked $MyInvocation.MyAction.Name "6" "No target xml files" 
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

    $checkResult = $ErrorMessage -Match "Could not find file"
   if (!(Test-Path "$xmlPath\config.xml") -and ("$xmlPath\bluelion-config.xml")){ 

         #Assert
        Assert_IsTrue $checkResult $MyInvocation.MyAction.Name "7"
    }

    else{
        TestIsBlocked $MyInvocation.MyAction.Name "7" "No target xml files"
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

        $checkResult = $ErrorMessage -Match "Could not find file"

   if (!(Test-Path "$xmlPath\config.xml") -and ("$xmlPath\bluelion-config.xml")){ 

         #Assert
        Assert_IsTrue $checkResult $MyInvocation.MyAction.Name "8"
    }

    else{
        TestIsBlocked $MyInvocation.MyAction.Name "8" "No target xml files"
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