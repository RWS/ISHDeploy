# This script requires sys_admin permission on the server

# This script grants the necessary permissions for the computer account to the $DATABASE$
# The computer account will require to access the $DATABASE$ only when InfoShareSTS is 
# * configured for Windows Authentication
# * Integrated authentication is used with the $DATABASE$ connection string
# When the IIS application pool is configure with application pool identity then network resources are accessed with the computer account.

try
{
    if (-not (Get-Module -ListAvailable -Name SQLPS)) 
    {
	    throw "SQLPS module is not installed."
    } 

    Push-Location "SQLSERVER:sql\$DATASOURCE$\databases"

    cd "MASTER"

    # Create login for the computer account. 
    Invoke-Sqlcmd -Query "CREATE LOGIN [$OSUSER$] FROM WINDOWS WITH DEFAULT_DATABASE=[$DATABASE$]"

    cd "..\$DATABASE$"

    # Add the user to the $DATABASE$
    Invoke-Sqlcmd -Query "CREATE USER [$OSUSER$] FOR LOGIN [$OSUSER$]"

    # Grant SELECT permision to the user
    Invoke-Sqlcmd -Query "GRANT SELECT TO [$OSUSER$]"
}
catch
{
    Write-Error $_
}
finally
{
    Pop-Location
}