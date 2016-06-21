<#
    .SYNOPSIS
        Grants the necessary permissions for the computer account to the $DATABASE$

    .DESCRIPTION
        This commandlet grants the necessary permissions for the computer account to the $DATABASE$
        The computer account will require to access the $DATABASE$ only when InfoShareSTS is 
        * configured for Windows Authentication
        * Integrated authentication is used with the $DATABASE$ connection string
        When the IIS application pool is configure with application pool identity then network resources are accessed with the computer account.
        This commandlet works with windows authentication only.

    .PARAMETER  Credential
        Specifies credential of user need to be created.

    .EXAMPLE
        $secpasswd = ConvertTo-SecureString "password" -AsPlainText -Force
        $credential = New-Object System.Management.Automation.PSCredential ("domain\User", $secpasswd)
        GrantComputerAccountPermissions.ps1 -Credential $credential

    .INPUTS
        None. You cannot pipe objects to GrantComputerAccountPermissions.ps1.

    .OUTPUTS
        None. GrantComputerAccountPermissions.ps1 does not generate any output.
#>
Param(
	[parameter(Mandatory=$false)]
	[PSCredential]$Credential=$null
)
 
$server="$DATASOURCE$"
$dbName="$DATABASE$"
$principal="$OSUSER$"
 
$psDriveName=$server.Replace("\","_")
 
try
{
 
    if (-not (Get-Module -ListAvailable -Name SQLPS)) 
    {
	    throw "SQLPS module is not installed. SQLPS module is available through SQL Server Management Studio"
    } 
    else
    {
        Import-Module SQLPS
    }
 
    if($Credential)
    {
        Write-Debug "Creating $psDriveName PSDrive for $server with credential"
        New-PSDrive -PSProvider SqlServer -Root "SQLSERVER:sql\$server\" -Name $psDriveName -Credential $Credential |Out-Null
        Write-Verbose "Created $psDriveName PSDrive for $server with credential"
    }
    else
    {
        Write-Debug "Creating $psDriveName PSDrive for $server. Using windows authentication."
        New-PSDrive -PSProvider SqlServer -Root "SQLSERVER:sql\$server\" -Name $psDriveName |Out-Null
        Write-Verbose "Created $psDriveName PSDrive for $server. Using windows authentication."
    }
 
    Push-Location ("$psDriveName"+":databases\")
 
    cd "MASTER"
    $serverPrincipalCount=Invoke-Sqlcmd -Query "SELECT COUNT(name) AS Count FROM sys.server_principals WHERE TYPE IN ('U', 'S') and Name='$principal'" |Select-Object -ExpandProperty Count
 
    if($serverPrincipalCount -eq 0)
    {
        # Create login for the computer account. 
        Invoke-Sqlcmd -Query "CREATE LOGIN [$principal] FROM WINDOWS WITH DEFAULT_DATABASE=[$dbName]"
        Write-Verbose "Create login for $principal"
    }
    else
    {
        Write-Warning "LOGIN $principal exists for server $server"
    }
 
    cd "..\$dbName"
 
 
    $databasePrincipalCount=Invoke-Sqlcmd -Query "SELECT COUNT(name) AS Count FROM sys.database_principals WHERE TYPE IN ('U', 'S') and Name='$principal'" |Select-Object -ExpandProperty Count
 
    if($databasePrincipalCount -eq 0)
    {
        # Add the user to the database
        Invoke-Sqlcmd -Query "CREATE USER [$principal] FOR LOGIN [$principal]"
 
        # Grant SELECT permision to the user
        Invoke-Sqlcmd -Query "GRANT SELECT TO [$principal]"
    }
    else
    {
        Write-Warning "User $principal exists for database $dbName"
    }
 
 
}
catch
{
    Write-Error $_
}
finally
{
    Pop-Location
    if(Get-PSDrive -Name $psDriveName -ErrorAction SilentlyContinue)
    {
        Remove-PSDrive $psDriveName
    }
} 