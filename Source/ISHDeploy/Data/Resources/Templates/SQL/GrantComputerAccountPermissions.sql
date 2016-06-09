--This script requires sys_admin permission on the server

--This script grants the necessary permissions for the computer account to the $DATABASE$
--The computer account will require to access the $DATABASE$ only when InfoShareSTS is 
--* configured for Windows Authentication
--* Integrated authentication is used with the $DATABASE$ connection string
--When the IIS application pool is configure with application pool identity then network resources are accessed with the computer account.

USE [MASTER]
GO
--Create login for the computer account. 
CREATE LOGIN [$OSUSER$] FROM WINDOWS WITH DEFAULT_DATABASE=[$DATABASE$]
GO
USE [$DATABASE$]
GO
--Add the user to the $DATABASE$
CREATE USER [$OSUSER$] FOR LOGIN [$OSUSER$]
GO
USE [$DATABASE$]
GO
--Grant SELECT permision to the user
GRANT SELECT TO [$OSUSER$]
GO

