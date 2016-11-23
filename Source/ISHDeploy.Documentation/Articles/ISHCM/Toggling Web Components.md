# Toggling Web Components
 
This article explains how to control the features of **ISHCM** 
- Content Editor
- Quality Assistant
- External Preview
- Translation Job

## Set deploymentName variable
First set deploymentName variable.

```powershell
$deploymentName="InfoShare"
```

## Configure Content Editor
Set the license for Content Editor for domain e.g. `ish.example.com`
```powershell
Set-ISHContentEditor -ISHDeployment $deploymentName -LicenseKey "licensekey" -Domain "ish.example.com"
```

By default the **Content Editor** is disabled in the UI. To enable it:

```powershell
Enable-ISHUIContentEditor -ISHDeployment $deploymentName
```

## Configure Quality Assistant
By default the **Quality Assistant** is disabled in the UI. To enable it:

```powershell
Enable-ISHUIQualityAssistant -ISHDeployment $deploymentName
```

## Configure External Preview
By default the **External Preview** is disabled. To enable it for a user with `ExternalId` e.g. `externalid`:

```powershell
Enable-ISHExternalPreview -ISHDeployment $deploymentName -ExternalId "externalid"
```

The `ExternalId` value must match an existing user's `FISHEXTERNALID`.

## Configure Translation Job
By default the **Translation Job** is enabled in the UI. To disable it:

```powershell
Disable-ISHUITranslationJob -ISHDeployment $deploymentName
```