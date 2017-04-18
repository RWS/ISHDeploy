# Scaling the services

A Vanilla deployment is delivered with the **Translation Builder** and **Translation Organizer** behaviors pre-configured. 
This tutorial explains how to configure the behaviors of each service.

## Set deploymentName variable
First set deploymentName variable.

```powershell
$deploymentName="InfoShare"
```

## Configure the Translation Buidler

To configure the behavior of the Translation Builder use the `Set-ISHServiceTranslationBuilder` cmdlet.
For example:

CopyCodeBlock(_nopublish\Vanilla.Set-ISHServiceTranslationBuilder.ps1)

Notice that all parameters are optional.


## Configure the Translation Organizer

To configure the behavior of the Translation Organizer use the `Set-ISHServiceTranslationOrganizer` cmdlet.
For example:

CopyCodeBlock(_nopublish\Vanilla.Set-ISHServiceTranslationOrganizer.ps1)

Notice that all parameters are optional.

To enable template synchronization execute:

CopyCodeBlock(_nopublish\Vanilla.Set-ISHServiceTranslationOrganizer.Template.ps1)

## Configure Translation Organizer to target any Content Manager deployment

Translation Organizer uses the SOAP API (ISHWS) of Content Manager to pull and push the translation jobs. Out of the box, it is configured to target the default ISHWS but `Set-ISHServiceTranslationOrganizer` can be used to change the default target. An example of such a case, is when when Translation Organizer executes on premise and Content Manager is hosted.

The following example configures a Translation Organizer against a deployment on `ish.example.com`. By default the authentication paradigm is username/password based and requires credentials.

CopyCodeBlock(_nopublish\OnPremise.Set-ISHServiceTranslationOrganizer.ps1)