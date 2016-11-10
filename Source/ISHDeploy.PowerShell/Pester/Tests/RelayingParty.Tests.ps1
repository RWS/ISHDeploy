param(
    $session = $null,
    $testingDeploymentName = "InfoShare"
)

. "$PSScriptRoot\Common.ps1"
$computerName = If ($session) {$session.ComputerName} Else {[System.Net.Dns]::GetHostName()}

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
    Start-Sleep -Milliseconds 7000
}

$scriptBlockSetRelayingParty = {
    param (
        $ishDeployName,
        $name,
        $realm,
        $lc,
        $bl,
        $encriptionCertificate = $null
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
    elseif ($encriptionCertificate){
         Set-ISHSTSRelyingParty -ISHDeployment $ishDeploy -Name $name -Realm $realm -EncryptingCertificate $encriptionCertificate
    }
    else {
         Set-ISHSTSRelyingParty -ISHDeployment $ishDeploy -Name $name -Realm $realm
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

$scriptBlockTestDBPath = {
    param (
        $dbPath
    )
    
    return Test-Path $dbPath 
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
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockResetISHSTS -Session $session -ArgumentList $testingDeploymentName
    }
    
    It "Get ISHSTSRelyingParty"{
        WebRequestToSTS $testingDeploymentName
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockTestDBPath -Session $session -ArgumentList $dbPath | Should be "True"

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
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockTestDBPath -Session $session -ArgumentList $dbPath | Should be "True"

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
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockTestDBPath -Session $session -ArgumentList $dbPath | Should be "True"

        $dbQuerryCommandSelect = "SELECT Name, Id, Enabled, Realm FROM RelyingParties WHERE Name Like 'LC%'"

        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetRelayingParty -Session $session -ArgumentList $testingDeploymentName, "testNameLC", "https://testRealmLC.sdl.com", $true, $false, "testcert"
            
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
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockTestDBPath -Session $session -ArgumentList $dbPath | Should be "True"

        $dbQuerryCommandSelect = "SELECT Name, Id, Enabled, Realm FROM RelyingParties WHERE Name Like 'BL%'"

        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetRelayingParty -Session $session -ArgumentList $testingDeploymentName, "testNameBL", "https://testRealmLC.sdl.com", $false, $true, "testcert"
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
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockTestDBPath -Session $session -ArgumentList $dbPath | Should be "True"

        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetRelayingParty -Session $session -ArgumentList $testingDeploymentName, "testName", "https://testRealm.sdl.com", $false, $false, "testcert"

      
        $commandletList = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetRelayingParty -Session $session -ArgumentList $testingDeploymentName 

        $commandletList.Name -contains "testName" | Should be $true
    }

	It "Set ISHSTSRelyingParty updates RP when realm exists"{
        WebRequestToSTS $testingDeploymentName
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockTestDBPath -Session $session -ArgumentList $dbPath | Should be "True"

        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetRelayingParty -Session $session -ArgumentList $testingDeploymentName, "testName", "https://testRealm.sdl.com", $false, $false, "testcert"
		Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetRelayingParty -Session $session -ArgumentList $testingDeploymentName, "SecondName", "https://testRealm.sdl.com", $false, $false, "testcert"
      
        $commandletList = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetRelayingParty -Session $session -ArgumentList $testingDeploymentName 

        $commandletList.Name -contains "SecondName" | Should be $true
		$commandletList.Name -contains "testName" | Should be $false
    }

    It "Set ISHSTSRelyingParty with LC switch"{
        WebRequestToSTS $testingDeploymentName
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockTestDBPath -Session $session -ArgumentList $dbPath | Should be "True"

        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetRelayingParty -Session $session -ArgumentList $testingDeploymentName, "testNameLC", "https://testRealmLC.sdl.com", $true, $false, "testcert"

        $commandletList = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetRelayingParty -Session $session -ArgumentList $testingDeploymentName 

        $commandletList.Name -contains "LC: testNameLC" | Should be $true
    }

    It "Set ISHSTSRelyingParty with BL switch"{
        WebRequestToSTS $testingDeploymentName
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockTestDBPath -Session $session -ArgumentList $dbPath | Should be "True"

        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetRelayingParty -Session $session -ArgumentList $testingDeploymentName, "testNameBL", "https://testRealmBL.sdl.com", $false, $true, "testcert"

        $commandletList = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetRelayingParty -Session $session -ArgumentList $testingDeploymentName 
        $commandletList.Name -contains "BL: testNameBL" | Should be $true
    }
 
    It "Set ISHSTSRelyingParty - if EncryptingCertificate is Null"{
       WebRequestToSTS $testingDeploymentName
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockTestDBPath -Session $session -ArgumentList $dbPath | Should be "True"

        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetRelayingParty -Session $session -ArgumentList $testingDeploymentName, "testNameEncryptingCertificateIsNull", "https://testRealm.sdl.com", $false, $false
      
        $commandletList = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetRelayingParty -Session $session -ArgumentList $testingDeploymentName 

        $rp = $commandletList | Where-Object { $_.Name -eq "testNameEncryptingCertificateIsNull" }
        $rp.EncryptingCertificate | Should be $null
    }
    
    It "Get ISHSTSRelyingParty when db not exists"{
        
        if (Test-Path $dbPath){ 
            Remove-Item $dbPath
        }
  
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetRelayingParty -Session $session -ArgumentList $testingDeploymentName} | Should not Throw 
    }

    It "Set ISHSTSRelyingParty when db not exists"{
        Invoke-CommandRemoteOrLocal -ScriptBlock {if (Test-Path $dbPath){ Remove-Item $dbPath }} -Session $session -ArgumentList $dbPath
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetRelayingParty -Session $session -ArgumentList $testingDeploymentName, "testName", "https://testRealm.sdl.com", $false, $false, "testcert"} | Should not Throw 
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockTestDBPath -Session $session -ArgumentList $dbPath | Should be "True"
    }
    
    It "Set ISHSTSRelyingParty - Realm validate pattern"{
       WebRequestToSTS $testingDeploymentName
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockTestDBPath -Session $session -ArgumentList $dbPath | Should be "True"

        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetRelayingParty -Session $session -ArgumentList $testingDeploymentName, "testName", "https://testRealm.sdl.com", $false, $false, "testcert"
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetRelayingParty -Session $session -ArgumentList $testingDeploymentName, "testName2", "https://testRealm.com", $false, $false, "testcert"
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetRelayingParty -Session $session -ArgumentList $testingDeploymentName, "testName3", "https://testRealm.global.sdl.corp", $false, $false, "testcert"
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetRelayingParty -Session $session -ArgumentList $testingDeploymentName, "testName4", "https://testRealm", $false, $false, "testcert" } | Should Throw "Cannot validate argument on parameter 'Realm'. The argument `"https://testRealm`" does not match the"
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetRelayingParty -Session $session -ArgumentList $testingDeploymentName, "testName5", "https://testRealm.c", $false, $false, "testcert" } | Should Throw "Cannot validate argument on parameter 'Realm'. The argument `"https://testRealm.c`" does not match the"
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetRelayingParty -Session $session -ArgumentList $testingDeploymentName, "testName7", "https://testRealmApi25.global.sdl.corp/InfoShareWS/Wcf/API25/User.svc", $false, $false, "testcert"
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetRelayingParty -Session $session -ArgumentList $testingDeploymentName, "testName8", "http://testRealm.corp", $false, $false, "testcert"
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetRelayingParty -Session $session -ArgumentList $testingDeploymentName, "testName9", "http://testRealmApi25.global.sdl.corp/InfoShareWS/Wcf/API25/User.svc", $false, $false, "testcert"
    }

    Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockResetISHSTS -Session $session -ArgumentList $testingDeploymentName
}

