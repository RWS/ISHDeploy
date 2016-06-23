param(
    $session = $null,
    $testingDeploymentName = "InfoShare"
)
. "$PSScriptRoot\Common.ps1"

$computerName = If ($session) {$session.ComputerName} Else {$env:COMPUTERNAME}

# Test variables
$licenseKey=Get-TestDataValue "xopusLicenseKey"
$domain=Get-TestDataValue "xopusLicenseDomain"

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

$suffix = GetProjectSuffix($testingDeployment.Name)
$LicensePath = Join-Path $testingDeployment.WebPath ("\Web{0}\Author\ASP" -f $suffix )
$LicensePath = $LicensePath.ToString().replace(":", "$")
$LicensePath = "\\$computerName\$LicensePath"
$xmlPath = Join-Path $LicensePath "\XSL"
$licenseFile = Join-Path $LicensePath "\Editors\Xopus\license\$domain.txt"

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