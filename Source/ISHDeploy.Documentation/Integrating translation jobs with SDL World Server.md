# Integrating translation jobs with SDL World Server

This tutorial explains how to configure an integration with [SDL WorldServer](http://www.sdl.com/solution/language/translation-management/worldserver/).

## Set deploymentName variable
First set deploymentName variable.

```powershell
$deploymentName="InfoShare"
```

## Create mappings between SDL World Server locales and Content Manager languages

As **SDL World Server** and **SDL Knowledge Center Content Manager** use different identifiers for the languages, we need to create a mapping.

Use the `New-ISHIntegrationWorldServerMapping` cmdlet to create one mapping. 
Add multiple mappings into an array to describe all the necessary mappings. 

For example:

CopyCodeBlock(_nopublish\Example.New-ISHIntegrationWorldServerMapping.ps1)

## Configure the integration with SDL WorldServer

To configure the integration with SDL WorldServer use the `Set-ISHIntegrationWorldServer` cmdlet. 
The integration requires the following information

- The uri for WorldServer's service.
- The credentials to access the WorldServer service.
- The mapping configuration created above.

CopyCodeBlock(_nopublish\Example.Set-ISHIntegrationWorldServer.ps1)

## Remove the integration with SDL WorldServer

Use `Remove-ISHIntegrationWorldServer  -ISHDeployment $ishDeployment`.