# Exporting translation jobs to the file system

This tutorial explains how to configure an export to the file system.

## Set deploymentName variable
First set deploymentName variable.

```powershell
$deploymentName="InfoShare"
```

## Configure the export to the file system

To configure the export to the file system use the `Set-ISHTranslationFileSystemExport` cmdlet. 

CopyCodeBlock(_nopublish\Example.Set-ISHTranslationFileSystemExport.ps1)


**Note:** Content Manager allows only one export to the file system configured. 

## Remove the integration with SDL TMS

Use `Remove-ISHIntegrationFileSystem  -ISHDeployment $deploymentName`.