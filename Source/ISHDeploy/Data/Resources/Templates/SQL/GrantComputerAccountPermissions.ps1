# This script requires sys_admin permission on the server

# This script grants the necessary permissions for the computer account to the database
# The computer account will require to access the database only when InfoShareSTS is 
# * configured for Windows Authentication
# * Integrated authentication is used with the database connection string
# When the IIS application pool is configure with application pool identity then network resources are accessed with the computer account.

try
{
    Push-Location "sql\MECDEVDB05\SQL2014SP1\databases"

    cd "[MASTER]"

    # Create login for the computer account. 
    Invoke-Sqlcmd -Query "LOGIN [GLOBAL\MECDEV12QA01$] FROM WINDOWS WITH DEFAULT_DATABASE=[ISH13TEST]"

    cd "[ISH13TEST]"

    # Add the user to the database
    Invoke-Sqlcmd -Query "USER [GLOBAL\MECDEV12QA01$] FOR LOGIN [GLOBAL\MECDEV12QA01$]"

    cd "[ISH13TEST]"

    # Grant SELECT permision to the user
    Invoke-Sqlcmd -Query "SELECT TO [GLOBAL\MECDEV12QA01$]"
}
finally
{
    Pop-Location
}