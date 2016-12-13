param(
    $session = $null,
    $testingDeploymentName = "InfoShare"
)

. "$PSScriptRoot\Common.ps1"


$moduleName = Invoke-CommandRemoteOrLocal -ScriptBlock { (Get-Module "ISHDeploy.*").Name } -Session $session
$backupPath = "\\$computerName\C$\ProgramData\$moduleName\$($testingDeployment.Name)\Backup"

$xmlPath = Join-Path ($testingDeployment.WebPath.replace(":", "$")) ("Web{0}\Author\ASP\XSL" -f $suffix )
$xmlPath = "\\$computerName\$xmlPath"

$scriptBlockGet = {
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

$scriptBlockClean = {
    param (
        [Parameter(Mandatory=$false)]
        $ishDeployName 
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Clear-ISHDeploymentHistory -ISHDeployment $ishDeploy
}

# Script block for Disable-ISHUIQualityAssistant. Added here for generating backup files
$scriptBlockDisableQA = {
    param (
        [Parameter(Mandatory=$true)]
        $ishDeployName,
        [Parameter(Mandatory=$false)]
        [bool]$ishDeployParameterAsString = $false
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }

    if ($ishDeployParameterAsString -eq $true)
    {
        Disable-ISHUIQualityAssistant -ISHDeployment $ishDeployName
    }
    else
    {
        $ishDeploy = Get-ISHDeployment -Name $ishDeployName
        Disable-ISHUIQualityAssistant -ISHDeployment $ishDeploy
    }
}

$scriptBlockEnableContentEditor = {
    param (
        [Parameter(Mandatory=$true)]
        $ishDeployName,
        [Parameter(Mandatory=$false)]
        [bool]$ishDeployParameterAsString = $false
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }

    if ($ishDeployParameterAsString -eq $true)
    {
        Enable-ISHUIContentEditor -ISHDeployment $ishDeployName
    }
    else
    {
        $ishDeploy = Get-ISHDeployment -Name $ishDeployName
        Enable-ISHUIContentEditor -ISHDeployment $ishDeploy
    }
}

$scriptBlockGetPackageFolder = {
    param (
        [Parameter(Mandatory=$false)]
        $ishDeployName,
        [Parameter(Mandatory=$false)]
        $uncSwitch 
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    if($uncSwitch){
        Get-ISHPackageFolderPath -ISHDeployment $ishDeploy -UNC
    }
    else{
        Get-ISHPackageFolderPath -ISHDeployment $ishDeploy
    }
} 

$scriptBlockGetParameters = {
    param (
        [Parameter(Mandatory=$false)]
        $ishDeployName,
        $switshes
         
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Get-ISHDeploymentParameters -ISHDeployment $ishDeploy @switshes
}

function getExpectedHistory{
$text = '$deploymentName = ''InfoShare''
Disable-ISHUIQualityAssistant -ISHDeployment $deploymentName
'
  return $text.Replace("InfoShare", $testingDeployment.Name)
}

# Restoring system to vanila state for not loosing files, touched in previous tests
UndoDeploymentBackToVanila $testingDeploymentName $true

Describe "Testing Get-ISHDeploymentHistory"{
    BeforeEach {
		ArtifactCleaner -filePath $xmlPath -fileName "FolderButtonbar.xml"
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockClean -Session $session -ArgumentList $testingDeploymentName
    }

    It "Get ish deploy history"{
		# Try enabling Quality Assistant for generating backup files
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisableQA -Session $session -ArgumentList $testingDeploymentName
        $history = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGet -Session $session -ArgumentList $testingDeploymentName
        $expectedHistory =  getExpectedHistory
        $history.EndsWith($expectedHistory) | Should be "True"
    }

    It "Failed commandlets write no history"{
        Rename-Item "$xmlPath\FolderButtonbar.xml" "_FolderButtonbar.xml"
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisableQA -Session $session -ArgumentList $testingDeploymentName
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockEnableContentEditor -Session $session -ArgumentList $testingDeploymentName -WarningVariable Warning -ErrorAction Stop }| Should Throw "Could not find file"
        
		$history = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGet -Session $session -ArgumentList $testingDeploymentName
        $expectedHistory =  getExpectedHistory
        $history.EndsWith($expectedHistory) | Should be "True"
        
		Rename-Item "$xmlPath\_FolderButtonbar.xml" "FolderButtonbar.xml"
    }

    It "Commandlets that implements base class write no history"{
		$params = @{Original = $false; Changed = $true; Showpassword  = $false}
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisableQA -Session $session -ArgumentList $testingDeploymentName 
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGet -Session $session -ArgumentList $testingDeploymentName
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetDeployment -Session $session -ArgumentList $testingDeploymentName
		Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetPackageFolder -Session $session -ArgumentList $testingDeploymentName, $true
		Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetPackageFolder -Session $session -ArgumentList $testingDeploymentName
		$inputparameters = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetParameters -Session $session -ArgumentList $testingDeploymentName, $params
        $history = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGet -Session $session -ArgumentList $testingDeploymentName
        $expectedHistory =  getExpectedHistory
        $history.EndsWith($expectedHistory) | Should be "True"
    }

    It "Get-IshHistory works when there is no history"{
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGet -Session $session -ArgumentList $testingDeploymentName -WarningVariable Warning -ErrorAction Stop }| Should Not Throw

        $history = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGet -Session $session -ArgumentList $testingDeploymentName
        !$history | Should be "True"
    }

    It "Get ish deploy history - ISHDeployment parameter as a string"{
		# Try enabling Quality Assistant for generating backup files
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockDisableQA -Session $session -ArgumentList $testingDeploymentName, $true
        $history = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGet -Session $session -ArgumentList $testingDeploymentName
        $expectedHistory =  getExpectedHistory
        $history.EndsWith($expectedHistory) | Should be "True"
    }
}

