param(
    $session = $null,
    $testingDeploymentName = "InfoShare"
)
. "$PSScriptRoot\Common.ps1"

$computerName = If ($session) {$session.ComputerName} Else {$env:COMPUTERNAME}

#region variables
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

# Generating file pathes to remote PC files
$testingDeployment = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetDeployment -Session $session -ArgumentList $testingDeploymentName
$LicensePath = Join-Path $testingDeployment.WebPath ("\Web{0}\Author\ASP" -f $testingDeployment.OriginalParameters.projectsuffix )
$LicensePath = $LicensePath.ToString().replace(":", "$")
$LicensePath = "\\$computerName\$LicensePath"
$xmlPath = Join-Path $LicensePath "\XSL"
$licenseFile = Join-Path $LicensePath "\Editors\Xopus\license\global.sdl.corp.txt"


$licenseKey = "4242016AB5B36C77352E3B4BF4E7196B7AD2ED29D65BC48CE0110CF9C14615F26B8DD41EC3FAD03CA81D0285368FD0F4BFB712CDDF66B6E3CCC98AA3A673B5EE1848E06713FBA88002B080B9593F56514017390933481305D7F051F9702C953567136CEDAE69E1C40F92CB2EE5FD8A8D7339638F39ABBBD035DB9022A9AE0FD51B4B5146D268A2D73674FAA17DE420D966C38637C906FBBC579CB8D3D9BE1A6182F51399AC43BB0A968B58C5B11A5250BBB6BCE627D80C144F95EE36B0CEFD7859991E87D1862D22087CA03DAE360A3D49671822023439AAE9FE4A1655845AC78EE06B18FBCB7E348899DDAB37CF66198AB0EC48B5DA76255F749ED23C17CC8322D8"
$domain = "global.sdl.corp"
$randomKey = Get-Random
#endregion

#region Script Blocks 

# Script block for Enable-ISHUIContentEditor
$scriptBlockEnable = {
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

# Script block for Disable-ISHUIContentEditor
$scriptBlockDisable = {
    param (
        [Parameter(Mandatory=$true)]
        $ishDeployname 
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Disable-ISHUIContentEditor -ISHDeployment $ishDeploy
}

# Script block for Set-ISHContentEditor
$scriptBlockSetLicense = {
    param (
        [Parameter(Mandatory=$true)]
        $ishDeployName ,
        [Parameter(Mandatory=$true)]
        $domainName,
        [Parameter(Mandatory=$true)]
        $license
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }  
    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Set-ISHContentEditor -Domain $domainName -LicenseKey $license -ISHDeployment $ishDeploy
}

# Script block for Test-ISHContentEditor
$scriptBlockTestLicense = {
    param (
        [Parameter(Mandatory=$true)]
        $ishDeployName,
        [Parameter(Mandatory=$true)]
        $domainName
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Test-ISHContentEditor -Domain $domainName  -ISHDeployment $ishDeploy
}

#endregion



# Function reads target files and their content, searches for specified nodes in xm
function readTargetXML() {
	[xml]$XmlFolderButtonbar = Get-Content "$xmlPath\FolderButtonbar.xml" -ErrorAction SilentlyContinue
	[xml]$XmlInboxButtonBar = Get-Content "$xmlPath\InboxButtonBar.xml" -ErrorAction SilentlyContinue
	[xml]$XmlLanguageDocumentButtonbar = Get-Content "$xmlPath\LanguageDocumentButtonbar.xml" -ErrorAction SilentlyContinue
	$global:textFolderButtonbar = $XmlFolderButtonbar.BUTTONBAR.BUTTON.INPUT | ? {$_.NAME -eq "CheckOutWithXopus"}
	$global:textInboxButtonBar = $XmlInboxButtonBar.BUTTONBAR.BUTTON.INPUT | ? {$_.NAME -eq "CheckOutWithXopus"}
	$global:textLanguageDocumentButtonbar = $XmlLanguageDocumentButtonbar.BUTTONBAR.BUTTON.INPUT | ? {$_.NAME -eq "CheckOutWithXopus"}

	if($textFolderButtonbar -and $textInboxButtonBar -and $textLanguageDocumentButtonbar){
		Return "Enabled"
	}
	else{
		Return "Disabled"
	}

}


Describe "Testing ISHUIContentEditor"{
    BeforeEach {
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockUndoDeployment -Session $session -ArgumentList $testingDeploymentName
    }

    It "enables Disabled Content Editor"{
		#Arrange
        # Make sure, that Content Editor is disabled, otherwise - make it disabled 
        $precondition = readTargetXML
        if ($precondition -eq "Enabled"){Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisable -Session $session -ArgumentList $testingDeploymentName }

        # Now Content Editor should be fo sure disabled. Otherwise test fails
        $precondition = readTargetXML
        $precondition | Should Be "Disabled"
		#Act
        # Try enabling Content Editor
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnable -Session $session -ArgumentList $testingDeploymentName
        $result = readTargetXML
		#Assert
        $result | Should Be "Enabled"
    }

    It "disables enabled Content Editor" {
		#Arrange
        # Make sure, that Content Editor is disabled, otherwise - make it disabled 
        $precondition = readTargetXML
        
		if ($precondition -eq "Disabled") {
			Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnable -Session $session -ArgumentList $testingDeploymentName
		}

        # Now Content Editor should be fo sure disabled. Otherwise test fails
        $precondition = readTargetXML
        $precondition | Should Be "Enabled"
		#Act
        # Try disabling Content Editor
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisable -Session $session -ArgumentList $testingDeploymentName
		$result = readTargetXML
		#Assert
        $result | Should Be "Disabled"
    }

    It "enables Content Editor with no XML"{
        #Arange
        Rename-Item "$xmlPath\FolderButtonbar.xml" "_FolderButtonbar.xml"
		#Act/Assert
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnable -Session $session -ArgumentList $testingDeploymentName -WarningVariable Warning -ErrorAction Stop }| Should Throw "Could not find file"
        #Rollback
        Rename-Item "$xmlPath\_FolderButtonbar.xml" "FolderButtonbar.xml"
    }

    It "disables Content Editor with no XML"{
        #Arange
        Rename-Item "$xmlPath\FolderButtonbar.xml" "_FolderButtonbar.xml"
		#Act/Assert
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisable -Session $session -ArgumentList $testingDeploymentName -WarningVariable Warning -ErrorAction Stop }| Should Throw "Could not find file"
        #Rollback
        Rename-Item "$xmlPath\_FolderButtonbar.xml" "FolderButtonbar.xml"
    }

    It "enables Content Editor with wrong XML"{
        #Arrange
        # Running valid scenario commandlet to out files into backup folder before they will ba manually modified in test
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnable -Session $session -ArgumentList $testingDeploymentName
        Rename-Item "$xmlPath\FolderButtonbar.xml" "_FolderButtonbar.xml"
        New-Item "$xmlPath\FolderButtonbar.xml" -type file |Out-Null

        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnable -Session $session -ArgumentList $testingDeploymentName -WarningVariable Warning -ErrorAction Stop }| Should Throw "Root element is missing"

        #Rollback
        Remove-Item "$xmlPath\FolderButtonbar.xml"
        Rename-Item "$xmlPath\_FolderButtonbar.xml" "FolderButtonbar.xml"
     }

    It "disables Content Editor with wrong XML"{
        #Arrange
        # Running valid scenario commandlet to out files into backup folder before they will ba manually modified in test
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnable -Session $session -ArgumentList $testingDeploymentName
        Rename-Item "$xmlPath\FolderButtonbar.xml" "_FolderButtonbar.xml"
        New-Item "$xmlPath\FolderButtonbar.xml" -type file |Out-Null
		
		#Act/Assert
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisable -Session $session -ArgumentList $testingDeploymentName -WarningVariable Warning -ErrorAction Stop }| Should Throw "Root element is missing"

        #Rollback
        Remove-Item "$xmlPath\FolderButtonbar.xml"
        Rename-Item "$xmlPath\_FolderButtonbar.xml" "FolderButtonbar.xml"
     }

     It "enables enabled Content Editor"{
		#Arrange
        # Make sure, that Content Editor is disabled, otherwise - make it disabled 
        $precondition = readTargetXML
        if ($precondition -eq "Disabled"){Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnable -Session $session -ArgumentList $testingDeploymentName }

        # Now Content Editor should be fo sure disabled. Otherwise test fails
        $precondition = readTargetXML
        $precondition | Should Be "Enabled"
		#Act
        # Try enabling Content Editor
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnable -Session $session -ArgumentList $testingDeploymentName
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnable -Session $session -ArgumentList $testingDeploymentName -ErrorAction Stop } | Should Not Throw
        $result = readTargetXML
		#Assert
        $result | Should Be "Enabled"
    }

    It "disables disabled Content Editor"{
		#Arrange
        # Make sure, that Content Editor is disabled, otherwise - make it disabled 
        $precondition = readTargetXML
        if ($precondition -eq "Enabled"){Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisable -Session $session -ArgumentList $testingDeploymentName }

        # Now Content Editor should be fo sure disabled. Otherwise test fails
        $precondition = readTargetXML
        $precondition | Should Be "Disabled"
		#Act
        # Try disabling Content Editor
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisable -Session $session -ArgumentList $testingDeploymentName
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisable -Session $session -ArgumentList $testingDeploymentName -ErrorAction Stop } | Should Not Throw
        $result = readTargetXML
		#Assert
        $result | Should Be "Disabled"
    }

    It "checks Set license with valid key"{
		#Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetLicense -Session $session -ArgumentList  $testingDeploymentName, $domain, $licenseKey -ErrorAction Stop 
        RemotePathCheck $licenseFile | Should Be "True"
		#Assert
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockTestLicense -Session $session -ArgumentList  $testingDeploymentName, $domain -ErrorAction Stop | Should Be "True"
    }

    It "checks Set and test license with empty key"{
		#Act
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetLicense -Session $session -ArgumentList  $testingDeploymentName, $domain, "" -ErrorAction Stop } | Should Throw "Cannot validate argument on parameter 'LicenseKey'"
        RemotePathCheck $licenseFile | Should Be "False"
		#Assert
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockTestLicense -Session $session -ArgumentList $testingDeploymentName, $domain -ErrorAction Stop | Should Be "False"
    } 
    
    It "checks Set and test license with invalid key"{
		#Act
       {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetLicense -Session $session -ArgumentList $testingDeploymentName, $domain, $randomKey -ErrorAction Stop } | Should Not Throw 
	   RemotePathCheck $licenseFile | Should Be "True"
       #Assert
	   Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockTestLicense -Session $session -ArgumentList $testingDeploymentName, $domain -ErrorAction Stop | Should Be "True"
    }   
    
}