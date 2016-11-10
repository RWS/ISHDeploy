# Configuring ISHCM web UI menu bars
 
This article explains how to configure the menu bars in **ISHCM** Web UI for 
- Main Menu
- Event Monitor
- Search

Scripts will use the splatting technique to avoid lengthy lines.

## Set deploymentName variable
First set deploymentName variable.

```powershell
$deploymentName="InfoShare"
```

## Configure main menu
To create a new menu item that targets custom page `custom.html`:

CopyCodeBlock(01.CreateMainMenuBarItem.ps1)  
   
To change this item to 

- target a newer version of the page with name `custom.v2.html`
- add the user role `Author`
- place it first

CopyCodeBlock(02.ModifyMainMenuBarItem.ps1)  
   

## Configure event monitor
To create a new menu item `CUSTOM` for user role `Administrator`:

CopyCodeBlock(01.CreateEventMonitorMenuBarItem.ps1)  
   
By default the cmdlet will place the new item before the one with label `All Events`. To change this item to 

- show event types `CUSTOM1` and `CUSTOM2`
- add the user role `Author`
- place it first

CopyCodeBlock(02.ModifyEventMonitorMenuBarItem.ps1)

## Configure search menu
To create a new menu item `Custom Search` for user role `Administrator`:

CopyCodeBlock(01.CreateSearchMenuBarItem.ps1)  
   
By default the cmdlet will place the new item before the one with label `Locate`. To change this item to 

- change the search type to publication
- add the user role `Author`
- place it first

CopyCodeBlock(02.ModifySearchMenuBarItem.ps1)