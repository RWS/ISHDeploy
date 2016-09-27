param(
    $session = $null,
    $testingDeploymentName = "InfoShare"
)

. "$PSScriptRoot\Common.ps1"
$invalidSuffix = "Invalid"


$scriptBlockGetISHDeployment = {
    param (
        [Parameter(Mandatory=$false)]
        $ishDeployName 
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
    if ($ishDeployName){
        Get-ISHDeployment -Name $ishDeployName 
    }
    else {
        Get-ISHDeployment
    }
}

$testingDeployment = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHDeployment -Session $session -ArgumentList $testingDeploymentName
$xmlPath  = Join-Path $testingDeployment.WebPath ("\Web{0}\Author\ASP\XSL" -f $suffix )

$scriptBlockGetVersionValue = {
    param (
        $ishDeployName
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
    $currentInstall = (Get-ItemProperty -Path HKLM:\Software\Wow6432Node\Trisoft\InstallTool\InfoShare\$ishDeployName).Current
    $currentVersion = (Get-ItemProperty -Path HKLM:\Software\Wow6432Node\Trisoft\InstallTool\InfoShare\$ishDeployName\History\$currentInstall).Version
    Return $currentVersion 
    
}

$scriptBlockRemoveDeployment = {
    param (
        
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }

    $ishDeployments = Get-ISHDeployment
    if($ishDeployments.Count -gt 1){
        try{
            for($x =1; $x -lt $ishDeployments.Count; $x++){
                $name = $ishDeployments[$x].Name
                $source = "HKLM:\SOFTWARE\WOW6432Node\Trisoft\InstallTool\InfoShare\$name"
                $target = "HKLM:\SOFTWARE\WOW6432Node\Trisoft\InstallTool\InfoShare\Core"

                Copy-Item -Path $source -Destination $target -Recurse
                Remove-Item -Path $source -Recurse
            }
                Get-ISHDeploymentParameters
        }
        Finally{
            for($x =1; $x -lt $ishDeployments.Count; $x++){
                $name = $ishDeployments[$x].Name
                $source = "HKLM:\SOFTWARE\WOW6432Node\Trisoft\InstallTool\InfoShare\$name"
                $target2 = "HKLM:\SOFTWARE\WOW6432Node\Trisoft\InstallTool\InfoShare\Core\$name"
                
                Copy-Item -Path $target2 -Destination $source -Recurse
                Remove-Item -Path $target2 -Recurse
            }
        } 
    }
    else{
        Get-ISHDeploymentParameters
    }
}

$scriptBlockSetVersionValue = {
    param (
        $ishDeployName,
        $value
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
    $currentInstall = (Get-ItemProperty -Path HKLM:\Software\Wow6432Node\Trisoft\InstallTool\InfoShare\$ishDeployName).Current
    Set-ItemProperty -Path HKLM:\Software\Wow6432Node\Trisoft\InstallTool\InfoShare\$ishDeployName\History\$currentInstall -Name Version -Value $value
}

$scriptBlockGetHistory = {
    param (
        [Parameter(Mandatory=$false)]
        $ishDeployName 
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Get-ISHDeploymentHistory -ISHDeployment $ishDeploy
}

$scriptBlockGetAmountOfInstalledDeployments = {
    $RegistryInstallToolPath = "SOFTWARE\\Trisoft\\InstallTool"
    if ([System.Environment]::Is64BitOperatingSystem)
    {
        $RegistryInstallToolPath = "SOFTWARE\\Wow6432Node\\Trisoft\\InstallTool"
    }
    [Microsoft.Win32.RegistryKey]$installToolRegKey = [Microsoft.Win32.Registry]::LocalMachine.OpenSubKey($RegistryInstallToolPath);
    $amount = 0

    $installToolRegKey.OpenSubKey("InfoShare").GetSubKeyNames() | Where {$_ -ne "Core"} | ForEach-Object {  
        if ($installToolRegKey.OpenSubKey("InfoShare").OpenSubKey($_).GetValue("Current", "").ToString())
        {
            $amount++
        }
    }

    return $amount
}

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

# Restoring system to vanila state for not loosing files, touched in previous tests
UndoDeploymentBackToVanila $testingDeploymentName $true

Describe "Testing Get-ISHDeployment"{
    It "doesnot match 'InfoShare' pattern"{
		#Act/Assert
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHDeployment -Session $session -ArgumentList $invalidSuffix} | Should Throw 'The argument '+$invalidSuffix+' does not match the "^(InfoShare)" pattern. Supply an argument that matches "^(InfoShare)" and try the command again.'
    }

    It "returns correct deployment"{
		#Act
        $deploy = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHDeployment -Session $session -ArgumentList $testingDeploymentName
        #Assert
		$deploy.Name | Should Be $testingDeploymentName 
    }

    It "returns correct ammount of deployments"{
        #Act
        $deployments = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHDeployment -Session $session
        #Assert
        $amount = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetAmountOfInstalledDeployments -Session $session
		$deployments.Count | Should Be $amount
    }

    It "returns message when deployment is not found"{
        #Act/Assert
        $invalidProjectName = "InfoShare$invalidSuffix"
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHDeployment -Session $session -ArgumentList "InfoShare$invalidSuffix"} | Should Throw "Deployment with name InfoShare$invalidSuffix is not found on the system"
    }

    It "returns warning when CM version doesnot match"{
        #Arrange
        $current = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetVersionValue -Session $session -ArgumentList $testingDeploymentName
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetVersionValue -Session $session -ArgumentList $testingDeploymentName, "9.0.2417.0"
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHDeployment -Session $session -ArgumentList $testingDeploymentName  -WarningVariable Warning
        #Asssert
        $Warning | Should Match "does not correspond to deployment version"
        #Rollback
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetVersionValue -Session $session -ArgumentList $testingDeploymentName, $current 
    }

    It "Commandlets that accept ISHDeployment throw exception when CM version doesnot match"{
        #Arrange
        $current = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetVersionValue -Session $session -ArgumentList $testingDeploymentName
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetVersionValue -Session $session -ArgumentList $testingDeploymentName, "9.0.2417.0"
        #Act
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetHistory -Session $session -ArgumentList $testingDeploymentName} | Should Throw "does not correspond to deployment version "
		#Rollback
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetVersionValue -Session $session -ArgumentList $testingDeploymentName, $current 
    }
    
    It "Commandlets doesnot return Original Parameters"{
        #Arrange
        $deploy = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHDeployment -Session $session -ArgumentList $testingDeploymentName
        $deploy.ToString().Contains("OriginalParemeters") | Should be $False
    }

    It "Get-ISHDeployment returns WebSiteName"{
        #Arrange
        $deploy = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHDeployment -Session $session -ArgumentList $testingDeploymentName
        $inputparameters = Get-InputParameters $testingDeploymentName
        $deploy.WebSiteName -eq $inputparameters["websitename"] | Should be true
    }

    It "Get-ISHDeployment sets default deployment"{
        #Arrange
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockRemoveDeployment -Session $session} | Should not Throw
        
    }
    $amount = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetAmountOfInstalledDeployments -Session $session
    if ($amount -gt 1){
        It "Get-ISHDeployment don't set default deployment when more then one deployment"{
            #Arrange
            {Invoke-CommandRemoteOrLocal -ScriptBlock {Get-ISHDeploymentParameters} -Session $session} | Should Throw "More than one deployments detected. Please specify one"
        
        }
    }
    
}

