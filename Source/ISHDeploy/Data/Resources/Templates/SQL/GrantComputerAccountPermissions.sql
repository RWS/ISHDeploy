--This script requires sys_admin permission on the server
 
--This script grants the necessary permissions for the computer account to the $DATABASE$
--The computer account will require to access the $DATABASE$ only when InfoShareSTS is 
--* configured for Windows Authentication
--* Integrated authentication is used with the $DATABASE$ connection string
--When the IIS application pool is configure with application pool identity then network resources are accessed with the computer account.
 
--Create login for the computer account. 
USE [MASTER]
GO
if NOT EXISTS (SELECT name AS Count FROM sys.server_principals WHERE TYPE IN ('U', 'S') and Name='$PRINCIPAL$')
BEGIN
	CREATE LOGIN [$PRINCIPAL$] FROM WINDOWS WITH DEFAULT_DATABASE=[$DATABASE$]
END
GO
 
--Add the user to the $DATABASE$
USE [$DATABASE$]
GO
 
if NOT EXISTS (SELECT name AS Count FROM sys.database_principals WHERE TYPE IN ('U', 'S') and Name='$PRINCIPAL$')
BEGIN
	CREATE USER [$PRINCIPAL$] FOR LOGIN [$PRINCIPAL$]
	GRANT SELECT TO [$PRINCIPAL$]
END
GO
