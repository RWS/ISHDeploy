param(
    $session = $null,
    $testingDeploymentName = "InfoShareSQL2014"
)
. "$PSScriptRoot\Common.ps1"

#region variables

$testConnectionString = "Provider=SQLOLEDB.1;Password=isource;Persist Security Info=True;User ID=isource;Initial Catalog=test;Data Source=testSource\TestDB"
$testConnectionStringOracle = "Provider=OraOLEDB.Oracle.1;Password=isource;Persist Security Info=True;User ID=isource;Data Source=ISH12PROD.MECDEVDB05"
$testDBType = "SQLServer2014"
#endregion


$scriptBlockSetISHIntegrationDB = {
    param (
        $ishDeployName,
        $parameters

    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }

    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    
    Set-ISHIntegrationDB -ISHDeployment $ishDeploy @parameters

}
$scriptBlockGetISHIntegrationDB = {
    param (
        $ishDeployName
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }

    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    
    Get-ISHIntegrationDB -ISHDeployment $ishDeploy

}

$scriptBlockTestISHIntegrationDB = {
    param (
        $ishDeployName,
        $string
    )
    if($PSSenderInfo) {
        $DebugPreference=$Using:DebugPreference
        $VerbosePreference=$Using:VerbosePreference 
    }

    $ishDeploy = Get-ISHDeployment -Name $ishDeployName
    if($string) {
        Test-ISHIntegrationDB -ISHDeployment $ishDeployName -ConnectionString $string
    }
    else {
        Test-ISHIntegrationDB -ISHDeployment $ishDeployName
    }
}
#endregion


# Function reads target files and their content, searches for specified nodes in xml


$scriptBlockGetConnectionString = {
    param (
        [Parameter(Mandatory=$true)]
        $projectName 
    )

    $RegistryTridkPath = "SOFTWARE\\Trisoft\\Tridk\\TridkApp"
    if ([System.Environment]::Is64BitOperatingSystem)
    {
        $RegistryTridkPath = "SOFTWARE\\Wow6432Node\\Trisoft\\Tridk\\TridkApp"
    }
    [Microsoft.Win32.RegistryKey]$RegistryTridkPath = [Microsoft.Win32.Registry]::LocalMachine.OpenSubKey($RegistryTridkPath);
    $suffix= $projectName.Replace("InfoShare", "")
    $connectAuthor = $RegistryTridkPath.OpenSubKey("InfoShareAuthor$suffix").GetValue("Connect")
    $dbTypeAuthor = $RegistryTridkPath.OpenSubKey("InfoShareAuthor$suffix").GetValue("ComponentName")
    $connectBuilder = $RegistryTridkPath.OpenSubKey("InfoShareBuilders$suffix").GetValue("Connect")
    $dbTypeBuilder = $RegistryTridkPath.OpenSubKey("InfoShareBuilders$suffix").GetValue("ComponentName")

    
    $result =  @{}

    $result["DatabaseTypeAuthor"] = $dbTypeAuthor
    $result["ConnectionStringAuthor"] = $connectAuthor
    $result["DatabaseTypeBuilder"] = $dbTypeAuthor
    $result["ConnectionStringBuilder"] = $connectAuthor
    return $result
    
}
function Get-ConnectionString
{
    param (
        [Parameter(Mandatory=$true)]
        $projectName
    ) 
    $result=Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetConnectionString -Session $session -ArgumentList $projectName
    $global:connectionStringAuthorFromRegistry = $result["ConnectionStringAuthor"]
    $global:dbTypeAuthorFromRegistry = $result["DatabaseTypeAuthor"]
    $global:connectionStringBuilderFromRegistry = $result["ConnectionStringBuilder"]
    $global:dbTypeBuilderFromRegistry = $result["DatabaseTypeBuilder"]
}


Describe "Testing ISHIntegrationDB"{
    BeforeEach {
        
        UndoDeploymentBackToVanila $testingDeploymentName $true
    }

    It "Set ISHIntegrationDB with raw connection string"{       
        #Act

        $params = @{ConnectionString = $testConnectionString;DatabaseType = $testDBType; Raw=$true}
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationDB -Session $session -ArgumentList $testingDeploymentName, $params
        
        #Assert
        Get-ConnectionString $testingDeploymentName
        
        $connectionStringAuthorFromRegistry| Should be $testConnectionString
        $dbTypeAuthorFromRegistry | Should be $testDBType
        $connectionStringBuilderFromRegistry| Should be $testConnectionString
        $dbTypeBuilderFromRegistry | Should be $testDBType
    }

    It "Get ISHIntegrationDB returns proper connection string"{       
        #Act
        $stringFromCommandlet = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetISHIntegrationDB -Session $session -ArgumentList $testingDeploymentName
        
        #Assert
        Get-ConnectionString $testingDeploymentName
        
        $connectionStringAuthorFromRegistry| Should be $stringFromCommandlet.RawConnectionString
        $connectionStringBuilderFromRegistry| Should be $stringFromCommandlet.RawConnectionString

    }

    It "Set ISHIntegrationDBn don't throw error on Oracle"{       
        #Act
        #Act

        $params = @{ConnectionString = $testConnectionStringOracle;DatabaseType = "oracle"; Raw=$true}
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationDB -Session $session -ArgumentList $testingDeploymentName, $params
        
        #Assert
        Get-ConnectionString $testingDeploymentName
        
        $connectionStringAuthorFromRegistry| Should be $testConnectionStringOracle
        $dbTypeAuthorFromRegistry | Should be "oracle"
        $connectionStringBuilderFromRegistry| Should be $testConnectionStringOracle
        $dbTypeBuilderFromRegistry | Should be "oracle"

    }

    It "Test ISHIntegrationDBn output Warning on Oracle"{       
        #Act
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockTestISHIntegrationDB -Session $session -ArgumentList $testingDeploymentName, $testConnectionStringOracle -WarningVariable Warning
        
        #Assert
        $Warning | Should Match "Connection check doesn't support Oracle database" 
    }

    It "Test ISHIntegrationDBn works for SQL"{       
        #Act
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockTestISHIntegrationDB -Session $session -ArgumentList $testingDeploymentName} | Should not Throw
    }

    It "Test ISHIntegrationDBn throws error when ConnectionString is empty or NULL"{     
    
        $scriptBlock = {
            param (
                $ishDeployName
            )
            if($PSSenderInfo) {
                $DebugPreference=$Using:DebugPreference
                $VerbosePreference=$Using:VerbosePreference 
            }

            Test-ISHIntegrationDB -ISHDeployment $ishDeployName -ConnectionString ""
        }  
        #Act
        {Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlock -Session $session -ArgumentList $testingDeploymentName} | Should Throw
    }

    It "Set ISHIntegrationDB writes history"{       
        #Act

        $params = @{ConnectionString = $testConnectionString;DatabaseType = $testDBType; Raw=$true}
        Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockSetISHIntegrationDB -Session $session -ArgumentList $testingDeploymentName, $params
        
        #Assert
        $history = Invoke-CommandRemoteOrLocal -ScriptBlock $scriptBlockGetHistory -Session $session -ArgumentList $testingDeploymentName
        $history.Contains('Set-ISHIntegrationDB -ISHDeployment $deploymentName') | Should be "True"     
    }
     UndoDeploymentBackToVanila $testingDeploymentName $true
}