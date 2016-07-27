# Configuring features using the module
 
This article explains how to control UI features such as **Content Editor**, **Quality Assistant**, **Translation Job** and **External Preview**.
 
## Control individual features 

### Set deploymentName variable
First set deploymentName variable.

```powershell
$deploymentName="InfoShare"
```

### Configure Content Editor
Set the license for Content Editor for domain e.g. `ish.example.com`
```powershell
Set-ISHContentEditor -ISHDeployment $deploymentName -LicenseKey "licensekey" -Domain "ish.example.com"
```

By default the **Content Editor** is disabled in the UI. To enable it:

```powershell
Enable-ISHUIContentEditor -ISHDeployment $deploymentName
```


### Configure Quality Assistant
By default the **Quality Assistant** is disabled in the UI. To enable it:

```powershell
Enable-ISHUIQualityAssistant -ISHDeployment $deploymentName
```


### Configure External Preview
By default the **External Preview** is disabled. To enable it for a user with `ExternalId` e.g. `externalid`:

```powershell
Enable-ISHExternalPreview -ISHDeployment $deploymentName -ExternalId "externalid"
```

The `ExternalId` value must match an existing user's `FISHEXTERNALID`.

### Configure Translation Job
By default the **Translation Job** is enabled in the UI. To disable it:

```powershell
Disable-ISHUITranslationJob -ISHDeployment $deploymentName
```

### Configure Event monitor options
To create a new tab event type `CUSTOM`:

CopyCodeBlock(01.CreateCustomTab.ps1)  
   
To change this tab to show event types `CUSTOM1` and `CUSTOM2` and make it appear first

CopyCodeBlock(02.ModifyCustomTab.ps1)  
   
The above scripts use the splatting technique to avoid lengthy lines.

## Putting it all together
The following script is an example of changing the state of all the above features with one script

CopyCodeBlockAndLink(FeatureToggle.ps1)






