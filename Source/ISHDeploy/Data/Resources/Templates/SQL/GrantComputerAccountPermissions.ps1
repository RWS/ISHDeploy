# This script requires sys_admin permission on the server

# This script grants the necessary permissions for the computer account to the $DATABASE$
# The computer account will require to access the $DATABASE$ only when InfoShareSTS is 
# * configured for Windows Authentication
# * Integrated authentication is used with the $DATABASE$ connection string
# When the IIS application pool is configure with application pool identity then network resources are accessed with the computer account.

try
{
    Push-Location "sql\$DATASOURCE$\databases"

    cd "[MASTER]"

    # Create login for the computer account. 
    Invoke-Sqlcmd -Query "LOGIN [$OSUSER$] FROM WINDOWS WITH DEFAULT_DATABASE=[$DATABASE$]"

    cd "[$DATABASE$]"

    # Add the user to the $DATABASE$
    Invoke-Sqlcmd -Query "USER [$OSUSER$] FOR LOGIN [$OSUSER$]"

    cd "[$DATABASE$]"

    # Grant SELECT permision to the user
    Invoke-Sqlcmd -Query "SELECT TO [$OSUSER$]"
}
finally
{
    Pop-Location
}