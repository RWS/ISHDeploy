--This script requires sys_admin permission on the server

--This script grants the necessary permissions for the computer account to the database
--The computer account will require to access the database only when InfoShareSTS is 
--* configured for Windows Authentication
--* Integrated authentication is used with the database connection string
--When the IIS application pool is configure with application pool identity then network resources are accessed with the computer account.

USE [MASTER]
GO
--Create login for the computer account. 
CREATE LOGIN [GLOBAL\MECDEV12QA01$] FROM WINDOWS WITH DEFAULT_DATABASE=[ISH13TEST]
GO
USE [ISH13TEST]
GO
--Add the user to the database
CREATE USER [GLOBAL\MECDEV12QA01$] FOR LOGIN [GLOBAL\MECDEV12QA01$]
GO
USE [ISH13TEST]
GO
--Grant SELECT permision to the user
GRANT SELECT TO [GLOBAL\MECDEV12QA01$]
GO

