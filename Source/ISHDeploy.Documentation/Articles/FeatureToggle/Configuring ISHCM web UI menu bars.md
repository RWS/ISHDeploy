# Configuring ISHCM web UI menu bars
 
This article explains how to configure the menu bars in **ISHCM** Web UI for 
- Main Menu
- Event Monitor
- Search

## Set deploymentName variable
First set deploymentName variable.

```powershell
$deploymentName="InfoShare"
```

## Configure event monitor
To create a new menu item `CUSTOM`:

CopyCodeBlock(01.CreateCustomTab.ps1)  
   
To change this item to show event types `CUSTOM1` and `CUSTOM2` and make it appear first

CopyCodeBlock(02.ModifyCustomTab.ps1)  
   
The above scripts use the splatting technique to avoid lengthy lines.