# Work with one deployment
 
This article explains a possible simpler interaction with the module's cmdlets when there is only one Content Manager deployment.
 
## Cmdlets without ISHDeployment parameter. 

All cmdlets with the exception of `Get-ISHDeployment` offer the `-ISHDeployment` parameter to help them target a deployment. 
When there is only one deployment available then it is possible to ommit the `-ISHDeployment` and the module will automatically use the one.

For example lets assume that `Get-ISHDeployment` outputs one deployment

```powershell
Get-ISHDeployment|Format-Table Name
```

    Name                SoftwareVersion DatabaseType    AccessHostName                                
    ----                --------------- ------------    --------------                                
    InfoShare           12.0.2417.0     sqlserver2014   ish.example.com                               

In this case the following two invocation of `Get-ISHDeploymentHistory` are both equal

- `Get-ISHDeploymentHistory -ISHDeployment "InfoShare"`
- `Get-ISHDeploymentHistory`

Although the system automatically figures out this one deployment named `InfoShare`, the history file will contain records with explicit value for the `-ISHDeployment` parameter.

For example either of the following lines will generate the same entry in the history

- `Enable-ISHUIContentEditor -ISHDeployment "InfoShare"`
- `Enable-ISHUIContentEditor`

And the history will be

```powershell
# 20160314
$deploymentName = "InfoShare"
Enable-ISHUIContentEditor -ISHDeployment $deploymentName
```

