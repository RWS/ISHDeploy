# Integrating translation jobs with SDL TMS

This tutorial explains how to configure an integration with [SDL TMS](http://www.sdl.com/solution/language/translation-management/tms/).

## Set deploymentName variable
First set deploymentName variable.

```powershell
$deploymentName="InfoShare"
```
## Create mappings between SDL TMS languages and Content Manager languages

As **SDL TMS** and **SDL Knowledge Center Content Manager** use different identifiers for the languages, we need to create a mapping.

Use the `New-ISHIntegrationTMSMapping` cmdlet to create one mapping. 
Add multiple mappings into an array to describe all the necessary mappings. 

For example:

CopyCodeBlock(_nopublish\Example.New-ISHIntegrationTMSMapping.ps1)

## Create templates 

Use the `New-ISHIntegrationTMSTemplate` cmdlet to create one template. 
Add multiple templates into an array to describe all the necessary mappings. 

For example:

CopyCodeBlock(_nopublish\Example.New-ISHIntegrationTMSTemplate.ps1)

## Create request and group metadata

Use the `New-ISHFieldMetadata` cmdlet to create one metadata field representation. 
Add multiple fields into an array to describe all the necessary fields. 

For example:

CopyCodeBlock(_nopublish\Example.New-ISHFieldMetadata.ps1)

## Configure the integration with SDL TMS

To configure the integration with SDL TMS use the `Set-ISHIntegrationTMS` cmdlet. 
The integration requires the following information

- The uri for TMS's service.
- The credentials to access the TMS service.
- The mapping configuration created above.
- The template configuration created above.
- The metadata configuration created above that is pushed to TMS.
- The metadata configuration created above that is to group items in TMS.

CopyCodeBlock(_nopublish\Example.Set-ISHIntegrationTMS.ps1)

**Note:** Content Manager allows only one connector for SDL TMS configured. 

## Remove the integration with SDL TMS

Use `Remove-ISHIntegrationTMS  -ISHDeployment $ishDeployment`.