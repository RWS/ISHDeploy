# Managing and tracking components

This tutorial explains how to manage and track the components of a Content Manager deployment.

## Component composition of a deployment

A component expresses a logical grouping of processes that are responsible to execute a specific flow of Content Manager. Each deployment is composed by the following components:

- Web.
- COM+.
- Crawler.
- FullTextIndex.
- TranslationBuilder.
- TranslationOrganizer.
- BackgroundTask. This component is tracked per role.

Each components can be enabled or disabled with cmdlets offered by the module. Additionally the module offers related cmdlets to retrieve the state of each component.

The enabled or disabled state of a component expresses what happens when a deployment is started. 

- When disabled, then the component's related processes will not start. 
- When enabled, then the component's related processes will start.

## Set deploymentName variable
First set deploymentName variable.

```powershell
$deploymentName="InfoShare"
```

## Getting the overall component status

The module offers two `Get-ISHComponent` cmdlet to help track all the components status. Per component, an additional set of cmdlets is offered to look deeper into the component.

Here is an example:

CopyCodeBlock(_nopublish\Example.Get-ISHComponent.ps1)

```text
                Name IsEnabled Role   
                ---- --------- ----   
                  CM      True        
                  WS      True        
                 STS      True        
             COMPlus      True        
      BackgroundTask     False Default
             Crawler     False        
          SolrLucene     False        
  TranslationBuilder     False        
TranslationOrganizer     False        
```

**Notice** that is the state that the `Undo-ISHDeployment` will revert the deployment to.

## Managing the Web component

The module offers the following cmdlets to manage the Web component. The Web component groups the IIS application pools that drive the **ISHCM**, **ISHWS** and **ISHSTS** web applications.

- `Get-ISHIISAppPool`
- `Enable-ISHIISAppPool`
- `Disable-ISHIISAppPool`

Here is an example:

CopyCodeBlock(_nopublish\Web\Example.Get-ISHIISAppPool.ps1)

```text
ApplicationPoolName     WebApplicationName  Status
-------------------     ------------------  ------
TrisoftAppPoolishcm     ishcm              Started
TrisoftAppPoolishws     ishws              Started
TrisoftAppPoolishsts    ishsts             Started
```

To disable the Web component execute

CopyCodeBlock(_nopublish\Web\Example.Disable-ISHIISAppPool.ps1)

Then the state becomes 

```text
ApplicationPoolName     WebApplicationName  Status
-------------------     ------------------  ------
TrisoftAppPoolishcm     ishcm              Stopped
TrisoftAppPoolishws     ishws              Stopped
TrisoftAppPoolishsts    ishsts             Stopped
```

To enable the Web component execute

CopyCodeBlock(_nopublish\Web\Example.Enable-ISHIISAppPool.ps1)

## Managing the COM+ component

The module offers the following cmdlets to manage the COM+ component.

- `Get-ISHCOMPlus`
- `Enable-ISHCOMPlus`
- `Disable-ISHCOMPlus`

Here is an example:

CopyCodeBlock(_nopublish\COMPlus\Example.Get-ISHCOMPlus.ps1)

```text
Name                         Status     ActivationType
----                         ------     --------------
Trisoft-InfoShare-AuthorIso Enabled LibraryApplication
Trisoft-Utilities           Enabled LibraryApplication
Trisoft-TriDK               Enabled LibraryApplication
Trisoft-InfoShare-Author    Enabled  ServerApplication
```

To disable the COM+ component execute

CopyCodeBlock(_nopublish\COMPlus\Example.Disable-ISHCOMPlus.ps1)

Then the state becomes 

```text
Name                          Status     ActivationType
----                          ------     --------------
Trisoft-InfoShare-AuthorIso Disabled LibraryApplication
Trisoft-Utilities           Disabled LibraryApplication
Trisoft-TriDK               Disabled LibraryApplication
Trisoft-InfoShare-Author    Disabled  ServerApplication
```

To enable the COM+ component execute

CopyCodeBlock(_nopublish\COMPlus\Example.Enable-ISHCOMPlus.ps1)

**Notice** that the COM+ components are shared among multiple deployments on the same server. That means that when modifying COM+ for a specific deployment, then other deployments on the same server are affected. For this reason each cmdlet emits warning messages such as 

```text
WARNING: The disabling of COM+ component `Trisoft-Utilities` has implications across all deployments.
```

## Managing the Crawler component

The module offers the following cmdlets to manage the Crawler component.

- `Get-ISHServiceCrawler`
- `Enable-ISHServiceCrawler`
- `Disable-ISHServiceCrawler`

Here is an example:

CopyCodeBlock(_nopublish\Crawler\Example.Get-ISHServiceCrawler.ps1)

```text
Name                              Status    Type Sequence
----                              ------    ---- --------
Trisoft InfoShareSQL Crawler One Stopped Crawler        1
```

To enable the Crawler component execute

CopyCodeBlock(_nopublish\Crawler\Example.Enable-ISHServiceCrawler.ps1)

Then the state becomes 

```text
Name                              Status    Type Sequence
----                              ------    ---- --------
Trisoft InfoShareSQL Crawler One Started Crawler        1
```

**Notice** that by default the Crawler component depends on the FullTextIndex component and for this reason when enabling Crawler, the FullTextIndex component will be enabled also.

To disable the Crawler component execute

CopyCodeBlock(_nopublish\Crawler\Example.Disable-ISHServiceCrawler.ps1)

## Managing the FullTextIndex component

The module offers the following cmdlets to manage the FullTextIndex component.

- `Get-ISHServiceFullTextIndex`
- `Enable-ISHServiceFullTextIndex`
- `Disable-ISHServiceFullTextIndex`

Here is an example:

CopyCodeBlock(_nopublish\FullTextIndex\Example.Get-ISHServiceFullTextIndex.ps1)

```text
Name                                    Status          Type Sequence
----                                    ------          ---- --------
Trisoft InfoShareSQL FullTextIndex One Stopped FullTextIndex        1
```

To enable the FullTextIndex component execute

CopyCodeBlock(_nopublish\FullTextIndex\Example.Enable-ISHServiceFullTextIndex.ps1)

Then the state becomes 

```text
Name                                    Status          Type Sequence
----                                    ------          ---- --------
Trisoft InfoShareSQL FullTextIndex One Started FullTextIndex        1
```

To disable the FullTextIndex component execute

CopyCodeBlock(_nopublish\FullTextIndex\Example.Disable-ISHServiceFullTextIndex.ps1)

## Managing the TranslationBuilder component

The module offers the following cmdlets to manage the TranslationBuilder component.

- `Get-ISHServiceTranslationBuilder`
- `Enable-ISHServiceTranslationBuilder`
- `Disable-ISHServiceTranslationBuilder`

Here is an example:

CopyCodeBlock(_nopublish\TranslationBuilder\Example.Get-ISHServiceTranslationBuilder.ps1)

```text
Name                                         Status               Type Sequence
----                                         ------               ---- --------
Trisoft InfoShareSQL TranslationBuilder One Stopped TranslationBuilder        1
```

To enable the TranslationBuilder component execute

CopyCodeBlock(_nopublish\TranslationBuilder\Example.Enable-ISHServiceTranslationBuilder.ps1)

Then the state becomes 

```text
Name                                         Status               Type Sequence
----                                         ------               ---- --------
Trisoft InfoShareSQL TranslationBuilder One Started TranslationBuilder        1
```

To disable the TranslationBuilder component execute

CopyCodeBlock(_nopublish\TranslationBuilder\Example.Disable-ISHServiceTranslationBuilder.ps1)

## Managing the TranslationOrganizer component

The module offers the following cmdlets to manage the ISHServiceTranslationOrganizer component.

- `Get-ISHServiceTranslationOrganizer`
- `Enable-ISHServiceTranslationOrganizer`
- `Disable-ISHServiceTranslationOrganizer`

Here is an example:

CopyCodeBlock(_nopublish\TranslationOrganizer\Example.Get-ISHServiceTranslationOrganizer.ps1)

```text
Name                                           Status                 Type Sequence
----                                           ------                 ---- --------
Trisoft InfoShareSQL TranslationOrganizer One Stopped TranslationOrganizer        1
```

To enable the TranslationOrganizer component execute

CopyCodeBlock(_nopublish\TranslationOrganizer\Example.Enable-ISHServiceTranslationOrganizer.ps1)

Then the state becomes 

```text
Name                                           Status                 Type Sequence
----                                           ------                 ---- --------
Trisoft InfoShareSQL TranslationOrganizer One Started TranslationOrganizer        1
```

To disable the TranslationOrganizer component execute

CopyCodeBlock(_nopublish\TranslationOrganizer\Example.Disable-ISHServiceTranslationOrganizer.ps1)

## Managing the BackgroundTask default role component

The module offers the following cmdlets to manage the BackgroundTask component.

- `Get-ISHServiceBackgroundTask`
- `Enable-ISHServiceBackgroundTask`
- `Disable-ISHServiceBackgroundTask`

In comparison with the rest of the components, the BackgroundTask is a component per role. For this reason all related cmdlets offer the `-Role` parameter. 

By default, the Vanilla deployment is configured with the role `Default` and for this reason the `-Role` parameter is optional with default value `Default`.

Here is an example:

CopyCodeBlock(_nopublish\BackgroundTask\Example.Get-ISHServiceBackgroundTask.ps1)

```text
Name                                     Status           Type Sequence Role   
----                                     ------           ---- -------- ----   
Trisoft InfoShareSQL BackgroundTask One Stopped BackgroundTask        1 Default
```

To enable the BackgroundTask component execute

CopyCodeBlock(_nopublish\BackgroundTask\Example.Enable-ISHServiceBackgroundTask.ps1)

Then the state becomes 

```text
Name                                     Status           Type Sequence Role   
----                                     ------           ---- -------- ----   
Trisoft InfoShareSQL BackgroundTask One Started BackgroundTask        1 Default
```

To disable the BackgroundTask component execute

CopyCodeBlock(_nopublish\BackgroundTask\Example.Disable-ISHServiceBackgroundTask.ps1)

## Adding and removing a custom BackgroundTask component

The module offers the following cmdlets to add and remove a new custom BackgroundTask component.

- `Add-ISHServiceBackgroundTask`
- `Remove-ISHServiceBackgroundTask`

Once a new custom BackgroundTask component is registered, the above cmdlets apply by explicitly specifying the `-Role` parameter

Here is an example:

CopyCodeBlock(_nopublish\BackgroundTask\Example.Add-ISHServiceBackgroundTask.ps1)

```text
Name                                            Status           Type Sequence Role   
----                                            ------           ---- -------- ----   
Trisoft InfoShareSQL BackgroundTask One        Stopped BackgroundTask        1 Default
Trisoft InfoShareSQL BackgroundTask Custom One Stopped BackgroundTask        1 Custom 
```

With this extra custom component registered, the overall component would become.

```text

                Name IsEnabled Role   
                ---- --------- ----   
                  CM     False        
                  WS     False        
                 STS     False        
             COMPlus     False        
      BackgroundTask     False Default
             Crawler     False        
          SolrLucene     False        
  TranslationBuilder     False        
TranslationOrganizer     False        
      BackgroundTask     False Custom 
```

To remove the custom component execute

CopyCodeBlock(_nopublish\BackgroundTask\Example.Remove-ISHServiceBackgroundTask.ps1)