Import-Module InfoShare.Deployment
. "$PSScriptRoot\Common.ps1"
CLS

#region Variables initoialization
$htmlStyle = Set-Style

$logFile = "C:\Automated_deployment\Test3.htm"


$WarningPreference = “Continue"
$VerbosePreference = "SilentlyCOntinue"
$global:logArray = @()


$dict = New-Object "System.Collections.Generic.Dictionary``2[System.String,System.String]"
$dict.Add('webpath', 'C:\InfoShare')
$dict.Add('apppath', 'C:\InfoShare')
$dict.Add('projectsuffix', 'SQL2014')
$dict.Add('datapath', 'C:\InfoShare')
$version = New-Object System.Version -ArgumentList '1.0.0.0';

$deploy = New-Object InfoShare.Deployment.Models.ISHDeployment -ArgumentList ($dict, $version)

Set-ISHDeployment $deploy


$LicensePath = $deploy.WebPath
$LicensePath = $LicensePath + "\Web" 
$LicensePath = $LicensePath + $deploy.Suffix 
$LicensePath = $LicensePath + "\Author\ASP"
$xmlPath = $LicensePath + "\XSL"
#endregion

function readTargetXML(){
[xml]$XmlFolderButtonbar = Get-Content "$xmlPath\FolderButtonbar.xml" -ErrorAction SilentlyContinue
[xml]$XmlInboxButtonBar = Get-Content "$xmlPath\InboxButtonBar.xml" -ErrorAction SilentlyContinue
[xml]$XmlLanguageDocumentButtonbar = Get-Content "$xmlPath\LanguageDocumentButtonbar.xml" -ErrorAction SilentlyContinue
$global:textFolderButtonbar = $XmlFolderButtonbar.BUTTONBAR.BUTTON.INPUT | ? {$_.NAME -eq "CheckOutWithXopus"}
$global:textInboxButtonBar = $XmlInboxButtonBar.BUTTONBAR.BUTTON.INPUT | ? {$_.NAME -eq "CheckOutWithXopus"}
$global:textLanguageDocumentButtonbar = $XmlLanguageDocumentButtonbar.BUTTONBAR.BUTTON.INPUT | ? {$_.NAME -eq "CheckOutWithXopus"}

}



#region Tests

function enableContentEditor_test(){

    readTargetXML


    if ($textFolderButtonbar -and $textInboxButtonBar -and $textLanguageDocumentButtonbar) {
        Write-Host $MyInvocation.MyCommand.Name -NoNewline 
        Write-Host " Passed"  -foregroundcolor "green" 
        logger "1" $MyInvocation.MyCommand.Name "Passed" " "
    }

    else {
         Write-Host $MyInvocation.MyCommand.Name -NoNewline 
         Write-Host " Failed"  -foregroundcolor "red"
         logger "1" $MyInvocation.MyCommand.Name "Failed" " "
    }
}

function disableContentEditor_test(){

    readTargetXML

    if (!$textFolderButtonbar -and !$textInboxButtonBar -and !$textLanguageDocumentButtonbar) {
        Write-Host $MyInvocation.MyCommand.Name -NoNewline 
        Write-Host " Passed"  -foregroundcolor "green" 
        logger "2" $MyInvocation.MyCommand.Name "Passed" " "
    }

    else {
         Write-Host $MyInvocation.MyCommand.Name -NoNewline 
         Write-Host " Failed"  -foregroundcolor "red"
         logger "2" $MyInvocation.MyCommand.Name "Failed" " "
    }
}

function enableContentEditorWithNoXML_test(){

    Rename-Item "$xmlPath\FolderButtonbar.xml" "_FolderButtonbar.xml"
        try
        {
            Enable-ISHUIContentEditor -WarningVariable Warning -ErrorAction Stop
        }
        catch 
        {
            $ErrorMessage = $_.Exception.Message
        }

    if ($ErrorMessage -Match "Could not find file") {
        Write-Host $MyInvocation.MyCommand.Name -NoNewline 
        Write-Host " Passed"  -foregroundcolor "green" 
        logger "3" $MyInvocation.MyCommand.Name "Passed" " "
    }

    else {
         Write-Host $MyInvocation.MyCommand.Name -NoNewline 
         Write-Host " Failed"  -foregroundcolor "red"
         logger "3" $MyInvocation.MyCommand.Name "Failed" " "
    }

    Rename-Item "$xmlPath\_FolderButtonbar.xml" "FolderButtonbar.xml"
}

function disableContentEditorWithNoXML_test(){

    Rename-Item "$xmlPath\FolderButtonbar.xml" "_FolderButtonbar.xml"
        try
        {
            Disable-ISHUIContentEditor -WarningVariable Warning -ErrorAction Stop
        }
        catch 
        {
            $ErrorMessage = $_.Exception.Message
        }

    if ($ErrorMessage -Match "Could not find file") {
        Write-Host $MyInvocation.MyCommand.Name -NoNewline 
        Write-Host " Passed"  -foregroundcolor "green" 
        logger "4" $MyInvocation.MyCommand.Name "Passed" " "
    }

    else {
         Write-Host $MyInvocation.MyCommand.Name -NoNewline 
         Write-Host " Failed"  -foregroundcolor "red"
         logger "4" $MyInvocation.MyCommand.Name "Failed" " "
    }

    Rename-Item "$xmlPath\_FolderButtonbar.xml" "FolderButtonbar.xml"
}

function enableContentEditorWithWrongXML_test(){

    Rename-Item "$xmlPath\FolderButtonbar.xml" "_FolderButtonbar.xml"
    New-Item "$xmlPath\FolderButtonbar.xml" -type file |Out-Null
  
        try
        {
            Enable-ISHUIContentEditor -WarningVariable Warning -ErrorAction Stop 
        
        }
        catch 
        {
            $ErrorMessage = $_.Exception.Message
        }
    if ($ErrorMessage -Match "Root element is missing") {
        Write-Host $MyInvocation.MyCommand.Name -NoNewline 
        Write-Host " Passed"  -foregroundcolor "green" 
        logger "5" $MyInvocation.MyCommand.Name "Passed" " "
    }

    else {
         Write-Host $MyInvocation.MyCommand.Name -NoNewline 
         Write-Host " Failed"  -foregroundcolor "red"
         logger "5" $MyInvocation.MyCommand.Name "Failed" " "
    }

    Remove-Item "$xmlPath\FolderButtonbar.xml"
    Rename-Item "$xmlPath\_FolderButtonbar.xml" "FolderButtonbar.xml"
}

function disableContentEditorWithWrongXML_test(){

    Rename-Item "$xmlPath\FolderButtonbar.xml" "_FolderButtonbar.xml"
    New-Item "$xmlPath\FolderButtonbar.xml" -type file |Out-Null
  
        try
        {
            disable-ISHUIContentEditor -WarningVariable Warning -ErrorAction Stop 
        
        }
        catch 
        {
            $ErrorMessage = $_.Exception.Message
        }
    if ($ErrorMessage -Match "Root element is missing") {
        Write-Host $MyInvocation.MyCommand.Name -NoNewline 
        Write-Host " Passed"  -foregroundcolor "green" 
        logger "6" $MyInvocation.MyCommand.Name "Passed" " "
    }

    else {
         Write-Host $MyInvocation.MyCommand.Name -NoNewline 
         Write-Host " Failed"  -foregroundcolor "red"
         logger "6" $MyInvocation.MyCommand.Name "Failed" " "
    }

    Remove-Item "$xmlPath\FolderButtonbar.xml"
    Rename-Item "$xmlPath\_FolderButtonbar.xml" "FolderButtonbar.xml"
}

function enableEnabledContentEditor_test(){

Enable-ISHUIContentEditor
Enable-ISHUIContentEditor
readTargetXML


if ($textFolderButtonbar -and $textInboxButtonBar -and $textLanguageDocumentButtonbar) {
    Write-Host $MyInvocation.MyCommand.Name -NoNewline 
    Write-Host " Passed"  -foregroundcolor "green"
    logger "7" $MyInvocation.MyCommand.Name "Passed" " " 
}

else {
     Write-Host $MyInvocation.MyCommand.Name -NoNewline 
     Write-Host " Failed"  -foregroundcolor "red"
     logger "7" $MyInvocation.MyCommand.Name "Failed" " "
}
}

function disableDisabledContentEditor_test(){

    Disable-ISHUIContentEditor
    Disable-ISHUIContentEditor

    readTargetXML

    if (!$textFolderButtonbar -and !$textInboxButtonBar -and !$textLanguageDocumentButtonbar) {
        Write-Host $MyInvocation.MyCommand.Name -NoNewline 
        Write-Host " Passed"  -foregroundcolor "green" 
        logger "8" $MyInvocation.MyCommand.Name "Passed" " "
    }

    else {
         Write-Host $MyInvocation.MyCommand.Name -NoNewline 
         Write-Host " Failed"  -foregroundcolor "red"
         logger "8" $MyInvocation.MyCommand.Name "Failed" " "
    }
}

function SetContentEditorLicense_test(){
            

         if (Test-Path "$LicensePath\Editors\Xopus\license\global.sdl.corp.txt") {
                            try
                {
                      Remove-Item "$LicensePath\Editors\Xopus\license\global.sdl.corp.txt" -Force -WarningVariable Warning -ErrorAction Stop 
        
                }
                catch 
                {
                    $ErrorMessage = $_.Exception.Message
                      
                    }
               }

        if(!(Test-Path "$LicensePath\Editors\Xopus\license\global.sdl.corp.txt")){

            Set-ISHContentEditor -LicensePath C:\Automated_deployment\global.sdl.corp.txt

            if (Test-Path "$LicensePath\Editors\Xopus\license\global.sdl.corp.txt") {
                Write-Host $MyInvocation.MyCommand.Name -NoNewline
                Write-Host " Passed"  -foregroundcolor "green"
                logger "9" $MyInvocation.MyCommand.Name "Passed" " "

            }

            else {
                 Write-Host $MyInvocation.MyCommand.Name -NoNewline
                 Write-Host  " Failed"  -foregroundcolor "red"
                 logger "9" $MyInvocation.MyCommand.Name "Failed" " "
            }
        }

        else {
            Write-Host $MyInvocation.MyCommand.Name -NoNewline
            Write-Host  " blocked"  -foregroundcolor "yellow"
            logger "9" $MyInvocation.MyCommand.Name "Blocked" "Could not delete license file in specified location"
        }
}

function SetContentEditorLicenseWhenLicenseExists_test(){
     if(!(Test-Path "$LicensePath\Editors\Xopus\license\global.sdl.corp.txt")){
        Copy-Item C:\Automated_deployment\global.sdl.corp.txt "$LicensePath\Editors\Xopus\license\global.sdl.corp.txt"
     }       

    if (Test-Path "$LicensePath\Editors\Xopus\license\global.sdl.corp.txt") {
                
        try
            {
                    Set-ISHContentEditor -LicensePath C:\Automated_deployment\global.sdl.corp.txt -WarningVariable Warning -ErrorAction Stop
        
            }
        catch 
            {
                $ErrorMessage = $_.Exception.Message
                      
            }

        if($ErrorMessage -Match "already exists") {
            Write-Host $MyInvocation.MyCommand.Name -NoNewline 
            Write-Host " Passed"  -foregroundcolor "green" 
            logger "10" $MyInvocation.MyCommand.Name "Passed" " "
        }

        else {
                Write-Host $MyInvocation.MyCommand.Name -NoNewline 
                Write-Host " Failed"  -foregroundcolor "red"
                logger "10" $MyInvocation.MyCommand.Name "Failed" " "
               }
    }

    else {
        Write-Host $MyInvocation.MyCommand.Name -NoNewline 
        Write-Host " blocked"  -foregroundcolor "yellow"
        logger "10" $MyInvocation.MyCommand.Name "Blocked" " "
    }
}

function SetContentEditorLicenseWhenLicenseNotExists_test(){
                
    try
        {
            Set-ISHContentEditor -LicensePath C:\Automated_deployment\_global.sdl.corp.txt -WarningVariable Warning -ErrorAction Stop
        
        }
    catch 
        {
            $ErrorMessage = $_.Exception.Message
        }

     
        # -and $Warning -Match $defaultWarning
    if($ErrorMessage -Match "Could not find file") {
        Write-Host $MyInvocation.MyCommand.Name -NoNewline 
        Write-Host " Passed"  -foregroundcolor "green" 
        logger "11" $MyInvocation.MyCommand.Name "Passed" " "
    }

    else {
            Write-Host $MyInvocation.MyCommand.Name -NoNewline 
            Write-Host " Failed"  -foregroundcolor "red"
            logger "11" $MyInvocation.MyCommand.Name "Failed" $ErrorMessage
            }
}

function TestContentEditorLicense_test(){
        
      if(!(Test-Path "$LicensePath\Editors\Xopus\license\global.sdl.corp.txt")){
        Copy-Item C:\Automated_deployment\global.sdl.corp.txt "$LicensePath\Editors\Xopus\license\global.sdl.corp.txt"
     }          
    try
        {
           $testResponse =  Test-ISHContentEditor -Hostname global.sdl.corp -WarningVariable Warning -ErrorAction Stop
        
        }
    catch 
        {
            $ErrorMessage = $_.Exception.Message
        }
        
        
    if($testResponse -Match "True") {
        Write-Host $MyInvocation.MyCommand.Name -NoNewline 
        Write-Host " Passed"  -foregroundcolor "green" 
        logger "12" $MyInvocation.MyCommand.Name "Passed" " "
    }

    else {
            Write-Host $MyInvocation.MyCommand.Name -NoNewline 
            Write-Host " Failed"  -foregroundcolor "red"
            logger "12" $MyInvocation.MyCommand.Name "Failed" " "
            }
}

function TestContentEditorLicenseWithWrongHostName_test(){
        
      if(!(Test-Path "$LicensePath\Editors\Xopus\license\global.sdl.corp.txt")){
        Copy-Item C:\Automated_deployment\global.sdl.corp.txt "$LicensePath\Editors\Xopus\license\global.sdl.corp.txt"
     }          
    try
        {
           $testResponse =  Test-ISHContentEditor -Hostname global.sdl.cor -WarningVariable Warning -ErrorAction Stop
        
        }
    catch 
        {
            $ErrorMessage = $_.Exception.Message
        }

 
    if($testResponse -match "False") {
        Write-Host $MyInvocation.MyCommand.Name -NoNewline 
        Write-Host " Passed"  -foregroundcolor "green" 
        logger "13" $MyInvocation.MyCommand.Name "Passed" " "
    }

    else {
            Write-Host $MyInvocation.MyCommand.Name -NoNewline 
            Write-Host " Failed"  -foregroundcolor "red"
            logger "13" $MyInvocation.MyCommand.Name "Failed" " "
            }
}


#endregion

#region Test Calls
Enable-ISHUIContentEditor
enableContentEditor_test


Disable-ISHUIContentEditor
disableContentEditor_test


enableContentEditorWithNoXML_test
disableContentEditorWithNoXML_test

enableContentEditorWithWrongXML_test
disableContentEditorWithWrongXML_test

enableEnabledContentEditor_test
disableDisabledContentEditor_test

SetContentEditorLicense_test
SetContentEditorLicenseWhenLicenseExists_test
SetContentEditorLicenseWhenLicenseNotExists_test

TestContentEditorLicense_test
TestContentEditorLicenseWithWrongHostName_test

#endregion


$global:logArray | ConvertTo-HTML -Head $htmlStyle | Out-File $logFile


Edit-LogHtml -targetHTML $logFile


Invoke-Expression $logFile

