Import-Module InfoShare.Deployment
. "$PSScriptRoot\Common.ps1"


#region Variables initoialization
$htmlStyle = Set-Style

$logFile = "C:\Automated_deployment\Test3.htm"


$WarningPreference = “Continue"
$VerbosePreference = "SilentlyCOntinue"
$global:logArray = @()


$deploy = Get-ISHDeployment -Deployment "SQL2014"


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

    $checkResult = $textFolderButtonbar -and $textInboxButtonBar -and $textLanguageDocumentButtonbar
    # Assert
    Assert_IsTrue $checkResult $MyInvocation.MyCommand.Name "1"
}

function disableContentEditor_test(){

    readTargetXML

    $checkResult = !$textFolderButtonbar -and !$textInboxButtonBar -and !$textLanguageDocumentButtonbar
    # Assert
    Assert_IsTrue $checkResult $MyInvocation.MyCommand.Name "2"
   
}

function enableContentEditorWithNoXML_test(){
    #Arange
    Rename-Item "$xmlPath\FolderButtonbar.xml" "_FolderButtonbar.xml"

    #Action       
    try
    {
        Enable-ISHUIContentEditor -WarningVariable Warning -ErrorAction Stop
    }
    catch 
    {
        $ErrorMessage = $_.Exception.Message
    }

    $checkResult = $ErrorMessage -Match "Could not find file"
    # Assert
    Assert_IsTrue $checkResult $MyInvocation.MyCommand.Name "3"
    
    #Rollback
    Rename-Item "$xmlPath\_FolderButtonbar.xml" "FolderButtonbar.xml"
}

function disableContentEditorWithNoXML_test(){
    #Arange
    Rename-Item "$xmlPath\FolderButtonbar.xml" "_FolderButtonbar.xml"

    #Action  
    try
    {
        Disable-ISHUIContentEditor -WarningVariable Warning -ErrorAction Stop
    }
    catch 
    {
        $ErrorMessage = $_.Exception.Message
    }
    $checkResult = $ErrorMessage -Match "Could not find file"
    # Assert
    Assert_IsTrue $checkResult $MyInvocation.MyCommand.Name "4"

    #Rollback
    Rename-Item "$xmlPath\_FolderButtonbar.xml" "FolderButtonbar.xml"
}

function enableContentEditorWithWrongXML_test(){
    #Arange
    Rename-Item "$xmlPath\FolderButtonbar.xml" "_FolderButtonbar.xml"
    New-Item "$xmlPath\FolderButtonbar.xml" -type file |Out-Null

    #Action 
    try
    {
        Enable-ISHUIContentEditor -WarningVariable Warning -ErrorAction Stop 
        
    }
    catch 
    {
        $ErrorMessage = $_.Exception.Message
    }

     $checkResult = $ErrorMessage -Match "Root element is missing"

     # Assert
    Assert_IsTrue $checkResult $MyInvocation.MyCommand.Name "5"

    #Rollback
    Remove-Item "$xmlPath\FolderButtonbar.xml"
    Rename-Item "$xmlPath\_FolderButtonbar.xml" "FolderButtonbar.xml"
}

function disableContentEditorWithWrongXML_test(){
    #Arange
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

    $checkResult = $ErrorMessage -Match "Root element is missing"

    #Assert
    Assert_IsTrue $checkResult $MyInvocation.MyCommand.Name "6"

    #Rollback
    Remove-Item "$xmlPath\FolderButtonbar.xml"
    Rename-Item "$xmlPath\_FolderButtonbar.xml" "FolderButtonbar.xml"
}

function enableEnabledContentEditor_test(){
    #Action
    Enable-ISHUIContentEditor
    Enable-ISHUIContentEditor
    readTargetXML
    
    $checkResult = $textFolderButtonbar -and $textInboxButtonBar -and $textLanguageDocumentButtonbar
    
    #Assert
    Assert_IsTrue $checkResult $MyInvocation.MyCommand.Name "7"
}

function disableDisabledContentEditor_test(){
    #Action
    Disable-ISHUIContentEditor
    Disable-ISHUIContentEditor

    readTargetXML

    $checkResult = !$textFolderButtonbar -and !$textInboxButtonBar -and !$textLanguageDocumentButtonbar

    #Assert
    Assert_IsTrue $checkResult $MyInvocation.MyCommand.Name "8"
}

function SetContentEditorLicense_test(){
            
    #Action
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

        
    if((Test-Path "$LicensePath\Editors\Xopus\license\global.sdl.corp.txt")) {
        TestIsBlocked $MyInvocation.MyCommand.Name "9" "Could not delete license file in specified location"
    }
    else {

        Set-ISHContentEditor -LicensePath C:\Automated_deployment\global.sdl.corp.txt

        $checkResult = Test-Path "$LicensePath\Editors\Xopus\license\global.sdl.corp.txt"

        #Assert
        Assert_IsTrue $checkResult $MyInvocation.MyCommand.Name "9"
    }
}

function SetContentEditorLicenseWhenLicenseExists_test(){
    #Arrange
    if(!(Test-Path "$LicensePath\Editors\Xopus\license\global.sdl.corp.txt")){
        Copy-Item C:\Automated_deployment\global.sdl.corp.txt "$LicensePath\Editors\Xopus\license\global.sdl.corp.txt"
    }       

    if (!(Test-Path "$LicensePath\Editors\Xopus\license\global.sdl.corp.txt")) {
        TestIsBlocked $MyInvocation.MyCommand.Name "10" "Could not find license file in specified location"
    }

    else
        {
         
        #Action       
        try
            {
                 Set-ISHContentEditor -LicensePath C:\Automated_deployment\global.sdl.corp.txt -WarningVariable Warning -ErrorAction Stop
        
            }
        catch 
            {
                $ErrorMessage = $_.Exception.Message
                      
            }

            $checkResult = $ErrorMessage -Match "already exists"
            #Assert
            Assert_IsTrue $checkResult $MyInvocation.MyCommand.Name "10"
        }

}

function SetContentEditorLicenseWhenLicenseNotExists_test(){
    #Action         
    try
        {
            Set-ISHContentEditor -LicensePath C:\Automated_deployment\_global.sdl.corp.txt -WarningVariable Warning -ErrorAction Stop
        
        }
    catch 
        {
            $ErrorMessage = $_.Exception.Message
        }

     
        $checkResult = $ErrorMessage -Match "Could not find file"
        #Assert
        Assert_IsTrue $checkResult $MyInvocation.MyCommand.Name "11"
}

function TestContentEditorLicense_test(){
    
    #Arrange    
    if(!(Test-Path "$LicensePath\Editors\Xopus\license\global.sdl.corp.txt")){
        Copy-Item C:\Automated_deployment\global.sdl.corp.txt "$LicensePath\Editors\Xopus\license\global.sdl.corp.txt"
     }    
     
     #Action      
    try
        {
           $testResponse =  Test-ISHContentEditor -Hostname global.sdl.corp -WarningVariable Warning -ErrorAction Stop
        
        }
    catch 
        {
            $ErrorMessage = $_.Exception.Message
        }
        
    $checkResult = $testResponse -Match "True"
    #Assert
    Assert_IsTrue $checkResult $MyInvocation.MyCommand.Name "12"
}

function TestContentEditorLicenseWithWrongHostName_test(){
    
    #Arrange    
    if(!(Test-Path "$LicensePath\Editors\Xopus\license\global.sdl.corp.txt")){
        Copy-Item C:\Automated_deployment\global.sdl.corp.txt "$LicensePath\Editors\Xopus\license\global.sdl.corp.txt"
     }   
     
     #Action       
    try
        {
            $testResponse =  Test-ISHContentEditor -Hostname global.sdl.cor -WarningVariable Warning -ErrorAction Stop
        
        }
    catch 
        {
            $ErrorMessage = $_.Exception.Message
        }

        $checkResult = $testResponse -match "False"
 #Assert
  Assert_IsTrue $checkResult $MyInvocation.MyCommand.Name "13"
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

