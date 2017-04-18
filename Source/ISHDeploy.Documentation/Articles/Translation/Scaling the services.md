# Scaling the services

A Vanilla deployment is delivered with the **Translation Builder** and **Translation Organizer** in a disabled state. 
This tutorial explains how to enable, disable and scale the above.

## Set deploymentName variable
First set deploymentName variable.

```powershell
$deploymentName="InfoShare"
```

## Get state

The module offers two cmdlets that return the state of each
- `Get-ISHServiceTranslationBuilder`
- `Get-ISHServiceTranslationOrganizer`

Here is an example:

CopyCodeBlock(_nopublish\Vanilla.Get-ISHServiceTranslation.ps1)

```text
Name                                           Status                 Type Sequence
----                                           ------                 ---- --------
Trisoft InfoShareSQL TranslationBuilder One   Stopped   TranslationBuilder        1
Trisoft InfoShareSQL TranslationOrganizer One Stopped TranslationOrganizer        1
```

This is also the state that the `Undo-ISHDeployment` will return to.

## Enable the translation services

To enable the Translation Builder use the `Enable-ISHServiceTranslationBuilder`. 
To enable the Translation Organizer use the `Enable-ISHServiceTranslationOrganizer`. 

For example to enable both execute:

CopyCodeBlock(_nopublish\Vanilla.Enable-ISHServiceTranslation.ps1)

And then the state becomes:

```text
Name                                           Status                 Type Sequence
----                                           ------                 ---- --------
Trisoft InfoShareSQL TranslationBuilder One   Running   TranslationBuilder        1
Trisoft InfoShareSQL TranslationOrganizer One Running TranslationOrganizer        1
```

## Disable the translation services

To disable the Translation Builder use the `Disable-ISHServiceTranslationBuilder`. 
To disable the Translation Organizer use the `Disable-ISHServiceTranslationOrganizer`. 

For example to disable both execute:

CopyCodeBlock(_nopublish\Vanilla.Disable-ISHServiceTranslation.ps1)

And then the state becomes:

```text
Name                                           Status                 Type Sequence
----                                           ------                 ---- --------
Trisoft InfoShareSQL TranslationBuilder One   Stopped   TranslationBuilder        1
Trisoft InfoShareSQL TranslationOrganizer One Stopped TranslationOrganizer        1
```

An alternative could had been to execute `Undo-ISHDeployment`.

## Scaling the translation services

The number of services defines the throughput of Translation Builder and Translation Organizer. 
To scale up the Translation Builder use the `Set-ISHServiceTranslationBuilder` while specifying the count. 
To scale up the Translation Organizer use the `Set-ISHServiceTranslationOrganizer` while specifying the count. 

For example to scale the Translation Builder to 2 and the Translation Organizer to 3 and then enable them execute :

CopyCodeBlock(_nopublish\Vanilla.Scale Up.Set-ISHServiceTranslation.ps1)

Then the state looks like this:

```text
Name                                             Status                 Type Sequence
----                                             ------                 ---- --------
Trisoft InfoShareSQL TranslationBuilder One     Running   TranslationBuilder        1
Trisoft InfoShareSQL TranslationBuilder Two     Running   TranslationBuilder        2
Trisoft InfoShareSQL TranslationOrganizer One   Running TranslationOrganizer        1
Trisoft InfoShareSQL TranslationOrganizer Two   Running TranslationOrganizer        2
Trisoft InfoShareSQL TranslationOrganizer Three Running TranslationOrganizer        3
```

To scale down the Translation Organizer to 2 execute `Set-ISHServiceTranslationOrganizer -ISHDeployment $deploymentName -Count 2` and the state becomes:
```text
Name                                           Status                 Type Sequence
----                                           ------                 ---- --------
Trisoft InfoShareSQL TranslationBuilder One   Running   TranslationBuilder        1
Trisoft InfoShareSQL TranslationBuilder Two   Running   TranslationBuilder        2
Trisoft InfoShareSQL TranslationOrganizer One Running TranslationOrganizer        1
Trisoft InfoShareSQL TranslationOrganizer Two Running TranslationOrganizer        2
```

Notice that when disabling and enabling, the module will **only** control the state of the services and not the scaling numbers. 
This is major difference with `Undo-ISHDeployment` that will first scale down each service to one and then disable.