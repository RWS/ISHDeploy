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
 
    SoftwareVersion : 12.0.2929.1
    Name            : InfoShare
    AppPath         : C:\InfoShare
    WebPath         : C:\InfoShare
    DataPath        : C:\InfoShare
    DatabaseType    : sqlserver2012
    AccessHostName  : ish.example.com
    WebAppNameCM    : InfoShareAuthor
    WebAppNameWS    : InfoShareWS
    WebAppNameSTS   : InfoShareSTS
 
If the `projectsuffix` was not empty then concatenate it after `InfoShare`. For example
```powershell
Get-ISHDeployment -Name InfoShareSQL
```

### Using a deployment
All commandlets that target a specific deployment are driven from a parameter `-ISHDeployment` that expects as value a name of deployment or an instance from the `Get-ISHDeployment` output.
To provide `-ISHDeployment` parameter to commandlets you need either to initialize variable with Get-ISHDeployment output, for example:
```powershell
$deployment = Get-ISHDeployment -Name InfoShare
Get-ISHDeploymentHistory -ISHDeployment $deployment
```
Or specify deployment name
```powershell
Get-ISHDeploymentHistory -ISHDeployment "InfoShare"
```
Using deployment name instead of deployment instance is the preffered way for remote invocation purposes.
 
### Get the history of a deployment
Infoshare.Deployment tracks all actions done on a Vanilla deployment through this module. 
 
You can get the history like this
 
```powershell
$deploymentName = "InfoShare"
Get-ISHDeploymentHistory -ISHDeployment $deploymentName
```
 
If you would execute the following.
 
CopyCodeBlockAndLink(FeatureToggle.ps1)
 
and then again `Get-ISHDeploymentHistory -ISHDeployment $deploymentName` outputs as an example of a history file.

```powershell
# 20160314
$deploymentName = "InfoShare"
Set-ISHContentEditor -ISHDeployment $deploymentName -LicenseKey "licensekey" -Domain "ish.example.com"
Enable-ISHUIContentEditor -ISHDeployment $deploymentName
Enable-ISHUIQualityAssistant -ISHDeployment $deploymentName
Enable-ISHExternalPreview -ISHDeployment $deploymentName -ExternalId "externalid"
Disable-ISHUITranslationJob -ISHDeployment $deploymentName
```

 
## Developing a deployment
To help engineers write and debug scripts a commandlet `Undo-ISHDeployment` is provided.
 
All commandlets in this module track and keep a backup of all vanilla files being modified.
This allows the module to undo all changes without the requirement to uninstall and then re-install.
 
```powershell
Undo-ISHDeployment -ISHDeployment $deploymentName
```
 
## Clear history artifacts
Because the module's codebase is not connected with the `InstallTool.exe` when you unistall the module's artificats will not be removed.
The `Clear-ISHDeploymentHistory` takes care of this by removing all artifacts. You must use this commandlet before uninstalling.
 
```powershell
Clear-ISHDeploymentHistory -ISHDeployment $deploymentName
```