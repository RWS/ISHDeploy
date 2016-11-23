# Configuring ISHCM web UI button bars
 
This article explains how to configure the button bars in **ISHCM** Web UI for the following levels in the repository view.
- Logical (Top)
- Version (Bottom horizontal)
- Language (Bottom and right vertical)

Scripts will use the splatting technique to avoid lengthy lines.

## Set deploymentName variable
First set deploymentName variable.

```powershell
$deploymentName="InfoShare"
```

## Configure a bar

To create a new button for all three places and all ishtypes that executes the javascript function `custom1()`:

CopyCodeBlock(_nopublish\01.CreateButtonBarItem.ps1)  

`Set-ISHUIButtonBarItem` supports additional optional parameters:

- `-CheckAccess` shows the button only if the user has valid access.
- `-HideText` doesn't render the text.
   
To change this item to 

- add a filter that shows the button only for
  - Illustrations
  - Maps
  - Libraries 
- place it first

CopyCodeBlock(_nopublish\02.ModifyButtonBarItem.ps1)  

With the `Set-ISHUIButtonBarItem`, `Move-ISHUIButtonBarItem` and `Remove-ISHUIButtonBarItem` each level is targetted with different parameter set that is driven by one of the following mandatory switch parameters:
- `-Logical` (Top)
- `-Version` (Bottom horizontal)
- `-Language` (Bottom and right vertical)

In these examples one configuration was applied for each button on all levels for simplicity.
To create different behavior per levels, use different parameters with e.g. `Set-ISHUIButtonBarItem` when using a different parameter set.

## Registering the dependencies

`Set-ISHUIButtonBarItem` allows only to specify a javascript function to call when the button is activated. 
All functions must be defined in a custom javascript file. 
To make sure that the javascript file(s) are available when the button is clicked we need to register them with the default `Trisoft.Extensions` resource group.
This is achieved with `Set-ISHCMCUILResourceGroup`.

Assuming the `namespace.custom1` function from the above example is defined in a `namespace.custom1.js` to register it execute

CopyCodeBlock(_nopublish\01.RegisterResourceGroup.ps1)

Keep in mind that as the `Set-ISHCMCUILResourceGroup` uses the verb `Set` it will overwrite the group.

The custom file `namespace.custom1.js` must be present in the Custom folder of ISHCM as explained in tutorial [Work with files and packages.md](../Module/Work with files and packages.md).

## Full example

This is an example of 

1. Upload the customization package in the staging folder
2. Copy the customization in the ISHCM folder
3. Register the javascript with the default group
4. Add a button bar

CopyCodeBlock(_nopublish\03.CreateButtonBarItem - Full.ps1)

