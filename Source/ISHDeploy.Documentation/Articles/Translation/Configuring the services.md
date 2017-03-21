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

