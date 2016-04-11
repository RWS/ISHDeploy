# Work with deployments
 
This article explains how to work with available deployments on a server
 
## Querying 
### Query all the server for deployments
To query for all deployments use the following command
```powershell
$deployments=Get-ISHDeployment
```
 
- When the server has no active deployments then `$deployments` will be empty.
- When the server has one deployment then `$deployments` will hold a reference to the one deployment, typically known as the default deployment.
- When the server has multiple deployments then you can get a nice report using the `Format-Table` commandlet.
 
```powershell
$deployments|Format-Table Name,SoftwareVersion,DatabaseType,AccessHostname
```
    Name                SoftwareVersion DatabaseType    AccessHostName                                
    ----                --------------- ------------    --------------                                
    InfoShare           12.0.2417.0     sqlserver2014   ish.example.com                               
    InfoShareSQLADFSU   12.0.2417.0     sqlserver2014   ish.example.com                               
    InfoShareSQLADFSW   12.0.2417.0     sqlserver2014   ish.example.com                               
    InfoShareSQLW       12.0.2417.0     sqlserver2014   ish.example.com                               
 
### Query the server for specific deployment
To query for one specific deployment use the following command when the `projectsuffix` is empty.
```powershell
Get-ISHDeployment -Name InfoShare
```
This outputs the following
 
    OriginalParameters : A hash collection with all key/value parameters used by InstallTool.exe
    SoftwareVersion    : 12.0.2417.0
    Name               : InfoShare
    AppPath            : C:\InfoShare\12.0\ISH
    WebPath            : C:\InfoShare\12.0\ISH
    DataPath           : C:\InfoShare\12.0\ISH
    ConnectString      : A connection string
    DatabaseType       : sqlserver2014
    WebNameCM          : C:\InfoShare\12.0\ISH\Web\Author
    WebNameWS          : C:\InfoShare\12.0\ISH\Web\InfoShareWS
    WebNameSTS         : C:\InfoShare\12.0\ISH\Web\InfoShareSTS
    AccessHostName     : ish.example.com
 
If the `projectsuffix` was not empty then concatenate it after `InfoShare`. For example
```powershell
Get-ISHDeployment -Name InfoShareSQL
```
 
 
### Using a deployment
All commandlets that target a specific deployment are driven from a parameter `-ISHDeployment` that expects as value an instance from the `Get-ISHDeployment` output. 
To provide `-ISHDeployment` parameter to commandlets you need to initialize variable with Get-ISHDeployment output, for example:
```powershell
$deployment = Get-ISHDeployment -Name InfoShare
```
 
### Get the history of a deployment
Infoshare.Deployment tracks all actions done on a Vanilla deployment through this module. 
 
For the above `$deployment` you can get the history like this
 
```powershell
Get-ISHDeploymentHistory -ISHDeployment $deployment
```
 
If you would execute the following.
 
CopyCodeBlockAndLink(FeatureToggle.ps1)
 
and then again `Get-ISHDeploymentHistory -ISHDeployment $deployment` outputs as an example of a history file.

```powershell
# 20160314
$deployment = Get-ISHDeployment -Name 'InfoShare'
Set-ISHContentEditor -ISHDeployment $deployment -LicenseKey "licensekey" -Domain "ish.example.com"
Enable-ISHUIContentEditor -ISHDeployment $deployment
Enable-ISHUIQualityAssistant -ISHDeployment $deployment
Enable-ISHExternalPreview -ISHDeployment $deployment -ExternalId "externalid"
Disable-ISHUITranslationJob -ISHDeployment $deployment
```

 
## Developing a deployment
To help engineers write and debug scripts a commandlet `Undo-ISHDeployment` is provided.
 
All commandlets in this module track and keep a backup of all vanilla files being modified.
This allows the module to undo all changes without the requirement to uninstall and then re-install.
 
```powershell
Undo-ISHDeployment -ISHDeployment $deployment
```
 
## Clear history artifacts
Because the module's codebase is not connected with the `InstallTool.exe` when you unistall the module's artificats will not be removed.
The `Clear-ISHDeploymentHistory` takes care of this by removing all artifacts. You must use this commandlet before uninstalling.
 
```powershell
Clear-ISHDeploymentHistory -ISHDeployment $deployment
```