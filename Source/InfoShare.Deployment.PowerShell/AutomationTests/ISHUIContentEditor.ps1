Import-Module InfoShare.Deployment
CLS
Set-ISHProject -InstallPath "D:\InfoShare" -Suffix "SQL2014"

$xmlPath = "D:\InfoShare\WebSQL2014\Author\ASP\XSL"
$LicensePath = "D:\InfoShare\WebSQL2014\Author\ASP"
$WarningPreference = “Continue"
$defaultWarning = "Operation fails. Rolling back all changes..."
#Core function

#region Style
$a = "<style>"

$a = $a+   "  body {font-family: Verdana, Geneva, Arial, Helvetica, sans-serif;}"

$a = $a+     "table{ border-collapse: collapse; border: none; font: 10pt Verdana, Geneva, Arial, Helvetica, sans-serif; color: black; margin-bottom: 10px;}"
 
$a = $a+    " table td{font-size: 16px; padding-left: 0px; padding-right: 20px; text-align: left;}"
 
$a = $a+     "table th {  font-size: 20px;  font-weight: bold; padding-left: 0px;padding-right: 20px; text-align: left;background: green; }"
 
$a = $a+     "h2{ clear: both; font-size: 130%;color:#354B5E; }"
 
$a = $a+     "h3{ clear: both;font-size: 75%; margin-left: 20px;  margin-top: 30px;  color:#475F77; }"
 
$a = $a+     "p{ margin-left: 20px; font-size: 12px; }"
$a = $a+     "table.list{ float: left; }"
 
 $a = $a+    "table.list td:nth-child(1){font-weight: bold;border-right: 1px grey solid;text-align: center;}"
 
 $a = $a+    "table.list td:nth-child(2){ padding-left: 7px; } table tr:nth-child(even) td:nth-child(even){ background: #BBBBBB; }"
 $a = $a+   " table tr:nth-child(odd) td:nth-child(odd){ background: #F2F2F2; }"
 $a = $a+    "table tr:nth-child(even) td:nth-child(odd){ background: #DDDDDD; }"
 $a = $a+    "table tr:nth-child(odd) td:nth-child(even){ background: #E5E5E5; }"
 $a = $a+    "div.column { width: 320px; float: left; }"
  $a = $a+  " div.first{ padding-right: 20px; border-right: 1px grey solid; }"
  $a = $a+   "div.second{ margin-left: 30px; }"
  $a = $a+   "table{ margin-left: 20px; }"

  $a = $a+   "</style>"



#endregion 

function readTargetXML(){
[xml]$XmlFolderButtonbar = Get-Content "$xmlPath\FolderButtonbar.xml" -ErrorAction SilentlyContinue
[xml]$XmlInboxButtonBar = Get-Content "$xmlPath\InboxButtonBar.xml" -ErrorAction SilentlyContinue
[xml]$XmlLanguageDocumentButtonbar = Get-Content "$xmlPath\LanguageDocumentButtonbar.xml" -ErrorAction SilentlyContinue
$global:textFolderButtonbar = $XmlFolderButtonbar.BUTTONBAR.BUTTON.INPUT | ? {$_.NAME -eq "CheckOutWithXopus"}
$global:textInboxButtonBar = $XmlInboxButtonBar.BUTTONBAR.BUTTON.INPUT | ? {$_.NAME -eq "CheckOutWithXopus"}
$global:textLanguageDocumentButtonbar = $XmlLanguageDocumentButtonbar.BUTTONBAR.BUTTON.INPUT | ? {$_.NAME -eq "CheckOutWithXopus"}

}

$logArray = @()

function logger{
    Param( [Parameter(Position=0,Mandatory=$true)][string] $test_id,
    [Parameter(Position=1,Mandatory=$true)][string] $test_name,
    [Parameter(Position=2,Mandatory=$true)][string] $test_result,
    [Parameter(Position=3,Mandatory=$true)][string] $test_exception
    )

    $logObject = New-Object -TypeName PSObject
    $logObject | Add-Member -Name 'test_id' -MemberType Noteproperty -Value $test_id
    $logObject | Add-Member -Name 'test_name' -MemberType Noteproperty -Value $test_name
    $logObject | Add-Member -Name 'test_result' -MemberType Noteproperty -Value $test_result
    $logObject | Add-Member -Name 'test_exception' -MemberType Noteproperty -Value $test_exception
    $global:logArray += $logObject

    
     
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

    if ($ErrorMessage -Match "Could not find file" -and $Warning -Match $defaultWarning) {
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

    if ($ErrorMessage -Match "Could not find file" -and $Warning -Match $defaultWarning) {
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
    if ($ErrorMessage -Match "Root element is missing" -and $Warning -Match $defaultWarning) {
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
    if ($ErrorMessage -Match "Root element is missing" -and $Warning -Match $defaultWarning) {
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

            Set-ISHContentEditor -LicensePath D:\global.sdl.corp.txt

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
            Set-ISHContentEditor -LicensePath D:\_global.sdl.corp.txt -WarningVariable Warning -ErrorAction Stop
        
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
        Copy-Item D:\global.sdl.corp.txt "$LicensePath\Editors\Xopus\license\global.sdl.corp.txt"
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

function TestContentEditorLicenseWhenNoLicense_test(){
        
      if(Test-Path "$LicensePath\Editors\Xopus\license\global.sdl.corp.txt"){
        Remove-Item "$LicensePath\Editors\Xopus\license\global.sdl.corp.txt" -force
     }   
          
         
    try
        {
           $testResponse =  Test-ISHContentEditor -Hostname global.sdl.corp -WarningVariable Warning -ErrorAction Stop
        
        }
    catch 
        {
            $ErrorMessage = $_.Exception.Message
        }

   
  
    if($testResponse -Match "False") {
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

function TestContentEditorLicenseWithWrongHostName_test(){
        
      if(!(Test-Path "$LicensePath\Editors\Xopus\license\global.sdl.corp.txt")){
        Copy-Item D:\global.sdl.corp.txt "$LicensePath\Editors\Xopus\license\global.sdl.corp.txt"
     }          
    try
        {
           $testResponse =  Test-ISHContentEditor -Hostname global.sdl.cor -WarningVariable Warning -ErrorAction Stop
        
        }
    catch 
        {
            $ErrorMessage = $_.Exception.Message
        }

    
    if($ErrorMessage -Match 'The license file for host "global.sdl.cor" not found') {
        Write-Host $MyInvocation.MyCommand.Name -NoNewline 
        Write-Host " Passed"  -foregroundcolor "green" 
        logger "14" $MyInvocation.MyCommand.Name "Passed" " "
    }

    else {
            Write-Host $MyInvocation.MyCommand.Name -NoNewline 
            Write-Host " Failed"  -foregroundcolor "red"
            logger "14" $MyInvocation.MyCommand.Name "Failed" " "
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
TestContentEditorLicenseWhenNoLicense_test
TestContentEditorLicenseWithWrongHostName_test

#endregion


$logArray | ConvertTo-HTML -Head $a| Out-File D:\Test2.htm

#region output Aftereffects
(Get-Content D:\Test2.htm) | 
Foreach-Object {$_ -replace '<td>Failed</td>',"<td style='color: red'>Failed</td>"}  | 
Out-File D:\Test2.htm

(Get-Content D:\Test2.htm) | 
Foreach-Object {$_ -replace '<td>Passed</td>',"<td style='color: green'>Passed</td>"}  | 
Out-File D:\Test2.htm

(Get-Content D:\Test2.htm) | 
Foreach-Object {$_ -replace '<td>Blocked</td>',"<td style='color: yellow'>Blocked</td>"}  | 
Out-File D:\Test2.htm
#endregion

Invoke-Expression D:\Test2.htm

 
