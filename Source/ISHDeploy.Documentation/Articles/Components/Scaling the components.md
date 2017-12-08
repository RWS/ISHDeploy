# Scaling the components

This tutorial explains how to scale the components of a Content Manager deployment.

Not all components are scalable. The following components are the ones that can be scaled:

- Crawler.
- TranslationBuilder.
- TranslationOrganizer.
- BackgroundTask. This component is scaled per role.

In general, scaling is applied by setting the `-Count` parameter for each related cmdlet of each component. 

Let's take a look in the following example for component **TranslationBuilder**. Similar principals apply for the rest of the components.

## Set deploymentName variable
First set deploymentName variable.

```powershell
$deploymentName="InfoShare"
```
## Scaling the Crawler component

The following example scales this component to `2`.

CopyCodeBlock(_nopublish\Crawler\Example.Set-ISHServiceCrawler.ps1)

## Scaling the TranslationBuilder component

The following example scales this component to `2`.

CopyCodeBlock(_nopublish\TranslationBuilder\Example.Set-ISHServiceTranslationBuilder.ps1)

## Scaling the TranslationOrganizer component

The following example scales this component to `2`.

CopyCodeBlock(_nopublish\TranslationOrganizer\Example.Set-ISHServiceTranslationOrganizer.ps1)

## Scaling the BackgroundTask default role component

The following example scales this component to `2`.

CopyCodeBlock(_nopublish\BackgroundTask\Example.Set-ISHServiceBackgroundTask.Default.ps1)

## Scaling a BackgroundTask custom role component

Assuming a new `Custom` role BackgroundTask component is registered, the following example scales this component to `2`.

CopyCodeBlock(_nopublish\BackgroundTask\Example.Set-ISHServiceBackgroundTask.Custom.ps1)

## Getting an holistic overview of the scaling configuration

The following query gets the status of all services based on their scaling configuration

CopyCodeBlock(_nopublish\Example.Get-ISHService.ps1)

```text
Name                                           Status                 Type
----                                           ------                 ----
Trisoft InfoShareSQL Crawler One              Stopped              Crawler
Trisoft InfoShareSQL Crawler Two              Stopped              Crawler
Trisoft InfoShareSQL TranslationBuilder One   Stopped   TranslationBuilder
Trisoft InfoShareSQL TranslationBuilder Two   Stopped   TranslationBuilder
Trisoft InfoShareSQL TranslationOrganizer One Stopped TranslationOrganizer
Trisoft InfoShareSQL TranslationOrganizer Two Stopped TranslationOrganizer

Name                                            Status           Type Sequence Role   
----                                            ------           ---- -------- ----   
Trisoft InfoShareSQL BackgroundTask One        Stopped BackgroundTask        1 Default
Trisoft InfoShareSQL BackgroundTask Two        Stopped BackgroundTask        2 Default
Trisoft InfoShareSQL BackgroundTask Custom One Stopped BackgroundTask        1 Custom 
Trisoft InfoShareSQL BackgroundTask Custom Two Stopped BackgroundTask        2 Custom 
```


