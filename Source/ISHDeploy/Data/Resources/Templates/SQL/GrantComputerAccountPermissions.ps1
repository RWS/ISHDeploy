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

    .PARAMETER  Computer
        Specifies computer name to open a persistent connection (PSSession) to. The default is the local computer.

    .PARAMETER  Session
        Specifies [PSSession] session object to use.

    .PARAMETER  Action
        Either `Set` or `Remove`

    .EXAMPLE
        $s01 = New-PSSession -ComputerName "10.91.5.39"
        Invoke-ADFSIntegrationISH -Session $s01 -Action "Set"

    .EXAMPLE
        Invoke-ADFSIntegrationISH -Computer "10.91.5.39" -Action "Set"

    .EXAMPLE
         $s03 = New-PSSession -ComputerName "10.91.5.39"
        Invoke-ADFSIntegrationISH -Session $s03 -Action "Remove"

    .EXAMPLE
        Invoke-ADFSIntegrationISH -Computer "10.91.5.39" -Action "Remove"

    .INPUTS
        None. You cannot pipe objects to Invoke-ADFSIntegrationISH.ps1.

    .OUTPUTS
        None. Invoke-ADFSIntegrationISH.ps1 does not generate any output.
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
        New-PSDrive -PSProvider SqlServer -Root "SQLSERVER:sql\MECDEVDB05\SQL2014SP1\" -Name $psDriveName |Out-Null
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