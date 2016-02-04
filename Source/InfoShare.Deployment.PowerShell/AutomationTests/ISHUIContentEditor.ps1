Import-Module InfoShare.Deployment
CLS
Set-ISHProject -InstallPath "D:\InfoShare" -Suffix "SQL2014"

$xmlPath = "D:\InfoShare\WebSQL2014\Author\ASP\XSL"
$LicensePath = "D:\InfoShare\WebSQL2014\Author\ASP"
$WarningPreference = “SilentlyContinue"
$defaultWarning = "Operation fails. Rolling back all changes..."
#Core function

function readTargetXML(){
[xml]$XmlFolderButtonbar = Get-Content "$xmlPath\FolderButtonbar.xml" -ErrorAction SilentlyContinue
[xml]$XmlInboxButtonBar = Get-Content "$xmlPath\InboxButtonBar.xml" -ErrorAction SilentlyContinue
[xml]$XmlLanguageDocumentButtonbar = Get-Content "$xmlPath\LanguageDocumentButtonbar.xml" -ErrorAction SilentlyContinue
$global:textFolderButtonbar = $XmlFolderButtonbar.BUTTONBAR.BUTTON.INPUT | ? {$_.NAME -eq "CheckOutWithXopus"}
$global:textInboxButtonBar = $XmlInboxButtonBar.BUTTONBAR.BUTTON.INPUT | ? {$_.NAME -eq "CheckOutWithXopus"}
$global:textLanguageDocumentButtonbar = $XmlLanguageDocumentButtonbar.BUTTONBAR.BUTTON.INPUT | ? {$_.NAME -eq "CheckOutWithXopus"}

}


#Tests

function enableContentEditor_test(){

    readTargetXML


    if ($textFolderButtonbar -and $textInboxButtonBar -and $textLanguageDocumentButtonbar) {
        Write-Host $MyInvocation.MyCommand.Name -NoNewline 
        Write-Host " Passed"  -foregroundcolor "green" 
    }

    else {
         Write-Host $MyInvocation.MyCommand.Name -NoNewline 
         Write-Host " Failed"  -foregroundcolor "red"
    }
}

function disableContentEditor_test(){

    readTargetXML

    if (!$textFolderButtonbar -and !$textInboxButtonBar -and !$textLanguageDocumentButtonbar) {
        Write-Host $MyInvocation.MyCommand.Name -NoNewline 
        Write-Host " Passed"  -foregroundcolor "green" 
    }

    else {
         Write-Host $MyInvocation.MyCommand.Name -NoNewline 
         Write-Host " Failed"  -foregroundcolor "red"
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

    if ($ErrorMessage -Match "Could not find file" -and $Warning -Match $defaultWarning) {
        Write-Host $MyInvocation.MyCommand.Name -NoNewline 
        Write-Host " Passed"  -foregroundcolor "green" 
    }

    else {
         Write-Host $MyInvocation.MyCommand.Name -NoNewline 
         Write-Host " Failed"  -foregroundcolor "red"
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

    if ($ErrorMessage -Match "Could not find file" -and $Warning -Match $defaultWarning) {
        Write-Host $MyInvocation.MyCommand.Name -NoNewline 
        Write-Host " Passed"  -foregroundcolor "green" 
    }

    else {
         Write-Host $MyInvocation.MyCommand.Name -NoNewline 
         Write-Host " Failed"  -foregroundcolor "red"
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
    if ($ErrorMessage -Match "Root element is missing" -and $Warning -Match $defaultWarning) {
        Write-Host $MyInvocation.MyCommand.Name -NoNewline 
        Write-Host " Passed"  -foregroundcolor "green" 
    }

    else {
         Write-Host $MyInvocation.MyCommand.Name -NoNewline 
         Write-Host " Failed"  -foregroundcolor "red"
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
    if ($ErrorMessage -Match "Root element is missing" -and $Warning -Match $defaultWarning) {
        Write-Host $MyInvocation.MyCommand.Name -NoNewline 
        Write-Host " Passed"  -foregroundcolor "green" 
    }

    else {
         Write-Host $MyInvocation.MyCommand.Name -NoNewline 
         Write-Host " Failed"  -foregroundcolor "red"
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
}

else {
     Write-Host $MyInvocation.MyCommand.Name -NoNewline 
     Write-Host " Failed"  -foregroundcolor "red"
}
}

function disableDisabledContentEditor_test(){

    Disable-ISHUIContentEditor
    Disable-ISHUIContentEditor

    readTargetXML

    if (!$textFolderButtonbar -and !$textInboxButtonBar -and !$textLanguageDocumentButtonbar) {
        Write-Host $MyInvocation.MyCommand.Name -NoNewline 
        Write-Host " Passed"  -foregroundcolor "green" 
    }

    else {
         Write-Host $MyInvocation.MyCommand.Name -NoNewline 
         Write-Host " Failed"  -foregroundcolor "red"
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

            Set-ISHContentEditor -LicensePath D:\global.sdl.corp.txt

            if (Test-Path "$LicensePath\Editors\Xopus\license\global.sdl.corp.txt") {
                Write-Host $MyInvocation.MyCommand.Name -NoNewline
                Write-Host " Passed"  -foregroundcolor "green"

            }

            else {
                 Write-Host $MyInvocation.MyCommand.Name -NoNewline
                 Write-Host  " Failed"  -foregroundcolor "red"
            }
        }

        else {
            Write-Host $MyInvocation.MyCommand.Name -NoNewline
            Write-Host  " blocked"  -foregroundcolor "yellow"
        }
}

function SetContentEditorLicenseWhenLicenseExists_test(){
     if(!(Test-Path "$LicensePath\Editors\Xopus\license\global.sdl.corp.txt")){
        Copy-Item D:\global.sdl.corp.txt "$LicensePath\Editors\Xopus\license\global.sdl.corp.txt"
     }       

    if (Test-Path "$LicensePath\Editors\Xopus\license\global.sdl.corp.txt") {
                
        try
            {
                    Set-ISHContentEditor -LicensePath D:\global.sdl.corp.txt -WarningVariable Warning -ErrorAction Stop
        
            }
        catch 
            {
                $ErrorMessage = $_.Exception.Message
                      
            }

        if($ErrorMessage -Match "already exists" -and $Warning -Match $defaultWarning) {
            Write-Host $MyInvocation.MyCommand.Name -NoNewline 
            Write-Host " Passed"  -foregroundcolor "green" 
        }

        else {
                Write-Host $MyInvocation.MyCommand.Name -NoNewline 
                Write-Host " Failed"  -foregroundcolor "red"
               }
    }

    else {
        Write-Host $MyInvocation.MyCommand.Name -NoNewline 
        Write-Host " blocked"  -foregroundcolor "yellow"
    }
}

function SetContentEditorLicenseWhenLicenseNotExists_test(){
     if(!(Test-Path "$LicensePath\Editors\Xopus\license\global.sdl.corp.txt")){
        Copy-Item D:\global.sdl.corp.txt "$LicensePath\Editors\Xopus\license\global.sdl.corp.txt"
     }       

    if (Test-Path "$LicensePath\Editors\Xopus\license\global.sdl.corp.txt") {
                
        try
            {
                    Set-ISHContentEditor -LicensePath D:\_global.sdl.corp.txt -WarningVariable Warning -ErrorAction Stop
        
            }
        catch 
            {
                $ErrorMessage = $_.Exception.Message
                      
            }

        if($ErrorMessage -Match "Could not find file" -and $Warning -Match $defaultWarning) {
            Write-Host $MyInvocation.MyCommand.Name -NoNewline 
            Write-Host " Passed"  -foregroundcolor "green" 
        }

        else {
                Write-Host $MyInvocation.MyCommand.Name -NoNewline 
                Write-Host " Failed"  -foregroundcolor "red"
               }
    }

    else {
        Write-Host $MyInvocation.MyCommand.Name -NoNewline 
        Write-Host " blocked"  -foregroundcolor "yellow"
    }
}

#Test Calls
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