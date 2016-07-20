param(
    $session = $null,
    $testingDeploymentName = "InfoShare"
)

. "$PSScriptRoot\Common.ps1"
$computerName = If ($session) {$session.ComputerName} Else {$env:COMPUTERNAME}

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

$testingDeployment = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetDeployment -Session $session -ArgumentList $testingDeploymentName
$testCertificate = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockCreateCertificate -Session $session

$suffix = GetProjectSuffix($testingDeployment.Name)
$absolutePath = $testingDeployment.WebPath
$dbPath = ("\\$computerName\{0}\Web{1}\InfoShareSTS\App_Data\IdentityServerConfiguration-2.2.sdf" -f $testingDeployment.Webpath, $suffix).replace(":", "$")
$computerName = $computerName.split(".")[0]


$scriptBlockResetISHSTS = {
    param (
        [Parameter(Mandatory=$false)]
        $ishDeployName
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    Reset-ISHSTS -ISHDeployment $ishDeploy

}

$scriptBlockGetRelayingParty = {
    param (
        $ishDeployName,
        $ish,
        $lc,
        $bl
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }

    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    if ($ish) {
        Get-ISHSTSRelyingParty -ISHDeployment $ishDeploy -ISH
    }
    elseif ($lc) {
        Get-ISHSTSRelyingParty -ISHDeployment $ishDeploy -LC
    }
    elseif ($bl) {
        Get-ISHSTSRelyingParty -ISHDeployment $ishDeploy -BL
    }
    else{
        Get-ISHSTSRelyingParty -ISHDeployment $ishDeploy
    }
 
}

$scriptBlockSetRelayingParty = {
    param (
        $ishDeployName,
        $name,
        $realm,
        $encriptionCertificate,
        $lc,
        $bl
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    if ($lc){
        Set-ISHSTSRelyingParty -ISHDeployment $ishDeploy -Name $name -Realm $realm -LC
    }
    elseif ($bl){
        Set-ISHSTSRelyingParty -ISHDeployment $ishDeploy -Name $name -Realm $realm -BL
    }
    else{
         Set-ISHSTSRelyingParty -ISHDeployment $ishDeploy -Name $name -Realm $realm -EncryptingCertificate $encriptionCertificate
    }
}

$scriptBlockQuerry = {
    param (
        
        $suffix,
        $dbPath,
        $absolutePath,
        $command
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }
     #Create System.Data.SqlServerCe.dll path
    $sqlCEAssemblyPath=[System.IO.Path]::Combine("$absolutePath\Web$suffix\InfoShareSTS\bin","System.Data.SqlServerCe.dll")
    
    #Add SQL Server CE Engine
    $var = [Reflection.Assembly]::LoadFile($sqlCEAssemblyPath)

    #Create Connection String

    $connectionString="Data Source=$dbPath;"

    #Prepare Database Connection and Command
	$connection = New-Object "System.Data.SqlServerCe.SqlCeConnection" $connectionString
        
    $existCommand = New-Object "System.Data.SqlServerCe.SqlCeCommand"
	$existCommand.CommandType = [System.Data.CommandType]::Text
	$existCommand.Connection = $connection
	$existCommand.CommandText = "$command"

	#Execute Command
	try
	{
        $array = @()
		$connection.Open()
		$result =$existCommand.ExecuteReader()
        while ($result.Read()) {
            $object = New-Object PSObject
            $object | Add-Member –MemberType NoteProperty –Name Name –Value $result.GetValue(0).ToString()
            $object | Add-Member –MemberType NoteProperty –Name ID –Value $result.GetValue(1).ToString() 
            $object | Add-Member –MemberType NoteProperty –Name Enabled –Value $result.GetValue(2).ToString()
            $object | Add-Member –MemberType NoteProperty –Name Realm –Value $result.GetValue(3).ToString()
            $array += $object
        }
	}
    catch{
        Write-Host $Error
    }
	finally
	{
		$connection.Close()
		$connection.Dispose()
	}
     return $array 
}


function remoteQuerryDatabase() {
    param(
        [string]$command
    )
    $result = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockQuerry -Session $session -ArgumentList $suffix, $dbPath, $absolutePath, $command
    return $result
}



Describe "Testing ISHRelaying party"{
    BeforeEach {
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockUndoDeployment -Session $session -ArgumentList $testingDeploymentName
        }
    
     It "Get ISHSTSRelyingParty"{
        WebRequestToSTS $testingDeploymentName
        Test-Path $dbPath | Should be "True"

        $dbQuerryCommandSelect = "SELECT Name, Id, Enabled, Realm FROM RelyingParties"

        $rpList = remoteQuerryDatabase -command $dbQuerryCommandSelect -stringOutput $true
        $commandletList = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetRelayingParty -Session $session -ArgumentList $testingDeploymentName

        foreach ($rp in $rpList){
            $rpTextConcat += $rp.Name
        }
        foreach ($rp in $commandletList){
            $commandletTextConcat += $rp.Name
        }
        $rpTextConcat -eq $commandletTextConcat | Should be $true
    }
    
    It "Get ISHSTSRelyingParty with ISH key"{
       WebRequestToSTS $testingDeploymentName
        Test-Path $dbPath | Should be "True"

        $dbQuerryCommandSelect = "SELECT Name, Id, Enabled, Realm FROM RelyingParties WHERE Name Like 'ISH%'"

        $rpList = remoteQuerryDatabase -command $dbQuerryCommandSelect -stringOutput $true
      
        $commandletList = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetRelayingParty -Session $session -ArgumentList $testingDeploymentName 

        foreach ($rp in $rpList){
            $rpTextConcat += $rp.Name
        }
        foreach ($rp in $commandletList){
            $commandletTextConcat += $rp.Name
        }
        $rpTextConcat -eq $commandletTextConcat | Should be $true
    }

    It "Get ISHSTSRelyingParty with LC key"{
        WebRequestToSTS $testingDeploymentName
        Test-Path $dbPath | Should be "True"

        $dbQuerryCommandSelect = "SELECT Name, Id, Enabled, Realm FROM RelyingParties WHERE Name Like 'LC%'"

        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetRelayingParty -Session $session -ArgumentList $testingDeploymentName, "testNameLC", "testRealmLC", "testcert", $true, $false
            
        $commandletList = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetRelayingParty -Session $session -ArgumentList $testingDeploymentName, $false, $true, $false
        $rpList = remoteQuerryDatabase -command $dbQuerryCommandSelect -stringOutput $true

        foreach ($rp in $rpList){
            $rpTextConcat += $rp.Name
        }
        foreach ($rp in $commandletList){
            $commandletTextConcat += $rp.Name
        }
        $rpTextConcat -eq $commandletTextConcat | Should be $true
    }

     It "Get ISHSTSRelyingParty with BL key"{
       WebRequestToSTS $testingDeploymentName
        Test-Path $dbPath | Should be "True"

        $dbQuerryCommandSelect = "SELECT Name, Id, Enabled, Realm FROM RelyingParties WHERE Name Like 'BL%'"

        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetRelayingParty -Session $session -ArgumentList $testingDeploymentName, "testNameBL", "testRealmLC", "testcert", $false, $true
        $rpList = remoteQuerryDatabase -command $dbQuerryCommandSelect -stringOutput $true
      
        $commandletList = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetRelayingParty -Session $session -ArgumentList $testingDeploymentName, $false, $false, $true

        foreach ($rp in $rpList){
            $rpTextConcat += $rp.Name
        }
        foreach ($rp in $commandletList){
            $commandletTextConcat += $rp.Name
        }
        $rpTextConcat -eq $commandletTextConcat | Should be $true
    }

     It "Set ISHSTSRelyingParty"{
       WebRequestToSTS $testingDeploymentName
        Test-Path $dbPath | Should be "True"

        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetRelayingParty -Session $session -ArgumentList $testingDeploymentName, "testName", "testRealm", "testcert", $false, $false

      
        $commandletList = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetRelayingParty -Session $session -ArgumentList $testingDeploymentName 

        $commandletList.Name -contains "testName" | Should be $true
    }

    It "Set ISHSTSRelyingParty with LC switch"{
       WebRequestToSTS $testingDeploymentName
        Test-Path $dbPath | Should be "True"

        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetRelayingParty -Session $session -ArgumentList $testingDeploymentName, "testNameLC", "testRealmLC", "testcert", $true, $false

        $commandletList = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetRelayingParty -Session $session -ArgumentList $testingDeploymentName 

        $commandletList.Name -contains "LC: testNameLC" | Should be $true
    }

    It "Set ISHSTSRelyingParty with BL switch"{
        WebRequestToSTS $testingDeploymentName
        Test-Path $dbPath | Should be "True"

        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetRelayingParty -Session $session -ArgumentList $testingDeploymentName, "testNameBL", "testRealmBL", "testcert", $false, $true

        $commandletList = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetRelayingParty -Session $session -ArgumentList $testingDeploymentName 
        $commandletList.Name -contains "BL: testNameBL" | Should be $true
    }
    
    It "Get ISHSTSRelyingParty when db not exists"{
        
        if (Test-Path $dbPath){ 
            Remove-Item $dbPath
        }
  
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetRelayingParty -Session $session -ArgumentList $testingDeploymentName} | Should Throw 

    }

    It "Set ISHSTSRelyingParty when db not exists"{
        if (Test-Path $dbPath){ 
            Remove-Item $dbPath
        }
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetRelayingParty -Session $session -ArgumentList $testingDeploymentName, "testName", "testRealm", "testcert", $false, $false} | Should not Throw 
        Test-Path $dbPath | Should be "True"

    }
	Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockUndoDeployment -Session $session -ArgumentList $testingDeploymentName
}

