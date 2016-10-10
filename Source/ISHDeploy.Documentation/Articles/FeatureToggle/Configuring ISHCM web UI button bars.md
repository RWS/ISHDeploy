# Configuring ISHCM web UI button bars
 
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

CopyCodeBlock(01.CreateButtonBarItem.ps1)  
   
To change this item to 

- add a filter that shows the button only for
  - Illustrations
  - Maps
  - Libraries 
- place it first

CopyCodeBlock(02.ModifyButtonBarItem.ps1)  

With the `Set-ISHUIButtonBarItem`, `Move-ISHUIButtonBarItem` and `Remove-ISHUIButtonBarItem` each level is targetted with different parameter set that is driven by one of the following mandatory switch parameters:
- `-Logical` (Top)
- `-Version` (Bottom horizontal)
- `-Language` (Bottom and right vertical)

In these examples one configuration was applied for each button on all levels for simplicity.
To create different behavior per levels, use different parameters with e.g. `Set-ISHUIButtonBarItem` when using a different parameter set.