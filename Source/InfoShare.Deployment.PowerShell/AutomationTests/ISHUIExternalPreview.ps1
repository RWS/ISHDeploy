Import-Module InfoShare.Deployment
. "$PSScriptRoot\Common.ps1"
CLS

#region Variables initoialization
$htmlStyle = Set-Style

$logFile = "C:\Automated_deployment\Test4.htm"

$WarningPreference = “Continue"

$customID = "Default"

$dict = New-Object "System.Collections.Generic.Dictionary``2[System.String,System.String]"
$dict.Add('webpath', 'C:\InfoShare')
$dict.Add('apppath', 'C:\InfoShare')
$dict.Add('projectsuffix', 'SQL2014')
$dict.Add('datapath', 'C:\InfoShare')
$version = New-Object System.Version -ArgumentList '1.0.0.0';

#$deploy = New-Object InfoShare.Deployment.Models.ISHDeployment -ArgumentList ($dict, $version)
$deploy = Get-ISHDeployment -Deployment "SQL2014"


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
    $checkResult = $textConfig -and $configSection -and $module

    

   if (Test-Path "$xmlPath\Web.config"){ 
        #Assert
        Assert_IsTrue $checkResult $MyInvocation.MyCommand.Name "1"
    }

    else{
        TestIsBlocked $MyInvocation.MyCommand.Name "1" "No web.config file"
    }
}

function disableExternalPreview_test(){

    readTargetXML
 
    $checkResult = !$textConfig -and !$configSection -and !$module
   if (Test-Path "$xmlPath\Web.config"){ 
        #Assert
        Assert_IsTrue $checkResult $MyInvocation.MyCommand.Name "2"
    }

    else{
        TestIsBlocked $MyInvocation.MyCommand.Name "2" "No web.config file"
    }
}

function enableEnabledExternalPreview_test(){

    
    Enable-ISHExternalPreview -ISHDeployment $deploy
    Enable-ISHExternalPreview -ISHDeployment $deploy
    readTargetXML

    $checkResult = $textConfig -and $configSection -and $module

   if (Test-Path "$xmlPath\Web.config"){ 

        #Assert
        Assert_IsTrue $checkResult $MyInvocation.MyCommand.Name "3"
    }

    else{
        TestIsBlocked $MyInvocation.MyCommand.Name "3" "No web.config file"
    }
}

function disableDisabledExternalPreview_test(){

    Disable-ISHExternalPreview -ISHDeployment $deploy
    Disable-ISHExternalPreview -ISHDeployment $deploy

    readTargetXML
 
    $checkResult = !$textConfig -and !$configSection -and !$module
   if (Test-Path "$xmlPath\Web.config"){ 
        #Assert
        Assert_IsTrue $checkResult $MyInvocation.MyCommand.Name "4"
        }
    

    else{
       TestIsBlocked $MyInvocation.MyCommand.Name "4" "No web.config file"
    }
}

function enableExternalPreviewWithCustomExternalID_test(){

    Enable-ISHExternalPreview -ISHDeployment $deploy -ExternalId $customID
    readTargetXML

    
    $checkResult = !$textConfig -and $configSection -and $module -and $configCustomID
   if (Test-Path "$xmlPath\Web.config"){ 
    

        #Assert
        Assert_IsTrue $checkResult $MyInvocation.MyCommand.Name "5"
        }
    

    else{
        TestIsBlocked $MyInvocation.MyCommand.Name "5" "No web.config file"
    }
}

function disableExternalPreviewWithCustomExternalID_test(){

    Disable-ISHExternalPreview -ISHDeployment $deploy
    readTargetXML
    $checkResult = !$textConfig -and !$configSection -and !$module -and !$configCustomID

   if (Test-Path "$xmlPath\Web.config"){ 

        #Assert
        Assert_IsTrue $checkResult $MyInvocation.MyCommand.Name "6"
    }

    else{
        TestIsBlocked $MyInvocation.MyCommand.Name "6" "No web.config file"
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

         $checkResult = $ErrorMessage -Match "Root element is missing"
   if (Test-Path "$xmlPath\Web.config"){ 

         #Assert
        Assert_IsTrue $checkResult $MyInvocation.MyCommand.Name "7"
    }

    else{
        TestIsBlocked $MyInvocation.MyCommand.Name "7" "No web.config file"
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

        $checkResult = $ErrorMessage -Match "Root element is missing"

   if (Test-Path "$xmlPath\Web.config"){ 

        #Assert
        Assert_IsTrue $checkResult $MyInvocation.MyCommand.Name "8"
    }

    else{
        TestIsBlocked $MyInvocation.MyCommand.Name "8" "No web.config file"
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

        $checkResult = $ErrorMessage -Match "Could not find file"

   if (!(Test-Path "$xmlPath\Web.config")){ 

        #Assert
        Assert_IsTrue $checkResult $MyInvocation.MyCommand.Name "9"
    }

    else{
       TestIsBlocked $MyInvocation.MyCommand.Name "9" "Could not rename web.config file"
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

        $checkResult = $ErrorMessage -Match "Could not find file"
   if (!(Test-Path "$xmlPath\Web.config")){ 
        #Assert
        Assert_IsTrue $checkResult $MyInvocation.MyCommand.Name "10"
    }

    else{
        TestIsBlocked $MyInvocation.MyCommand.Name "10" "Could not rename web.config file"
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

   