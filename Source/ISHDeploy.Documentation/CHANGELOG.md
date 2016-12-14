# Release notes

## stable-1.3

## stable-1.2
1. General 
    1. Cmdlets
		- For all cmdlets the '-ISHDeployment' parameter is now optional and calculated when omitted with the following reasoning:  **[Update]**
			- When there is only one available deployment detected then this one will be used.
			- Where there are more than one available deployments detected then the cmdlet will throw an error.
        - Improved cmdlet behavior when administrator privileges are required.
        - History file is enhanced with script info meta language. 
    2. Documentation
		- Renamed items in the top bar to **Tutorials** and **Cmdlet Documentation**. **[Update]**
		- New items in the top bar for access to topics such as **Getting started** and **Release notes**. **[Update]**
		- New icon in top bar. **[Update]**
		- New tutorial on how to work with one deployment. **[Update]**
        - Renamed and extended tutorial **Work with files** to **Work with files and packages**. **[Update]**
        - **Getting Started** has new information about required elevated administrator privileges. **[Update]**
1. Working with ISHCM web UI features
    1. Cmdlets
	    - `Set-ISHUIButtonBarItem`. **[New]**
	    - `Remove-ISHUIButtonBarItem`. **[New]**
	    - `Move-ISHUIButtonBarItem`. **[New]**
	    - `Move-ISHUIMainMenuBarItem`. **[New]**
	    - `Remove-ISHUIMainMenuBarItem`. **[New]**
	    - `Set-ISHUIMainMenuBarItem`. **[New]**
	    - `Move-ISHUISearchMenuBarItem`. **[New]**
	    - `Remove-ISHUISearchMenuBarItem`. **[New]**
	    - `Set-ISHUISearchMenuBarItem`. **[New]**
	    - `Set-ISHUISearchMenuBarItem`. **[New]**
	    - `Copy-ISHCMFile`. **[New]**
	    - `Expand-ISHCMPackage`. **[New]**
	    - `Set-ISHCMCUILResourceGroup`. **[New]**
        - `Backup-ISHDeployment`. **[New]**
	    - Renamed cmdlets for event monitor:  **[Update]**
			- From `Set-ISHUIEventMonitorTab` to `Set-ISHUIEventMonitorMenuBarItem`.
			- From `Move-ISHUIEventMonitorTab` to `Move-ISHUIEventMonitorMenuBarItem`.
			- From `Remove-ISHUIEventMonitorTab` to `Remove-ISHUIEventMonitorMenuBarItem`.
    2. Documentation
		- Reorganize former "Configuring features using the module" tutorial into three more specialized. **[Update]**
			- Toggling Web Components. **[Update]**
			- Configuring ISHCM web UI menu bars. **[Update]**
			- Configuring ISHCM web UI button bars. **[New]**
1. Integration STS
	1. Documentation
		- Fixed invalid parameter reference in tutorial **Implementing Vanilla certificate replacement**. **[Update]**
		- Added remarks for the importance of execution order in PowerShell examples in tutorial **Implementing Vanilla certificate replacement**. **[Update]**

## stable-1.1

**Summary of release**

- Control internal user authentication when the deployment is integrated with an external security token service (STS)
- Fine tune Cmdlets
- Bug fixing
- Solve previous known issue:
> When the deployment is configured for light weight windows authentication, the described certificate rollover leaves the system broken. The workaround is to re-execute the `Set-ISHSTSConfiguration -ISHDeployment $deploymentName -AuthenticationType Windows`.

**Cmdlets and documentation changes**

1. General 
	1. Cmdlets
		- Less warning messages. Warnings are issued when it�s important to share. **[Update]**
		- `Get-ISHDeploymentHistory` returns less. Get-* cmdlets are ignored. **[Update]**
	1. Documentation - Commands
		- Related links render as links and provide a quicker navigation experience. **[Update]**
1. Integration STS
	1. Cmdlets
		- `Get-ISHDeploymentHistory` returns less. Get-* cmdlets are ignored. **[Update]**
		- `Enable-ISHIntegrationSTSInternalAuthentication` and  `Disable-ISHIntegrationSTSInternalAuthentication` enable access for internal user authentication. **[New]**
	1. Documentation - Articles
		- Security Token Service\Integrating with Security Token Service mentions how to enable internal user authentication. **[Update]**
		- Security Token Service\ADFS\Integrating with Security Token Service mentions how to enable internal user authentication. **[Update]**
1. ISHSTS
    1. Cmdlets
		- `Set-ISHSTSConfiguration` will force the initialization of ISHSTS's database. **[Update]**
		- `Set-ISHSTSConfiguration` does check - is Windows Authentication feature installed on environment or not. **[Update]** 
		- `Set-ISHSTSRelyingParty` parameter `-Realm` is required to be a proper url with schema. **[Update]**

**Known issues**

- Due to internal dependencies to the product, avoid sequencing first `Set-ISHIntegrationSTSCertificate` and then `Set-ISHSTSConfiguration` because they will break the deployment.

## beta-0.9 (Pre-release)

1. Working with deployments
    1. Cmdlets
	      - All Get-ISH* cmdlets e.g. `Get-ISHDeployment` can be piped. e.g. `Get-ISHDeployment | ForEach-Object {$_}`. **[Update]**
1. Feature toggle
    1. Cmdlets
	      - Set-ISHUIEventMonitorTab can accept multiple user roles in parameter `-UserRole`. **[Update]**
1. ISHSTS
    1. Cmdlets
	      - Set-ISHSTSConfiguration. (Fixed) **[Update]**

**Known issues**

- When the deployment is configured for light weight windows authentication, the described certificate rollover leaves the system broken. The workaround is to re-execute the `Set-ISHSTSConfiguration -ISHDeployment $deploymentName -AuthenticationType Windows`.
- Set-ISHSTSConfiguration does not check if the windows authentication module is installed on IIS.

## alpha-0.2 (Pre-release)

1. General 
    1. Cmdlets
	      - All cmdlets accept also name as value for the `-ISHDeployment` parameter. e.g. `Enable-ISHUIContentEditor -ISHDeployment "InfoShare"`. **[Update]**
	      - `Get-ISHDeployment` returns the `WebSiteName` also. **[Update]**
	      - `Get-ISHDeploymentParameters` . **[New]**
1. ISHSTS
    1. Cmdlets
	      - Reset-ISHSTS. **[New]**
	      - Get-ISHSTSRelyingParty. **[New]**
	      - Set-ISHSTSRelyingParty. **[New]**
	      - Set-ISHSTSConfiguration. (Not working) **[New]**
    1. Articles
	      - Work with Security Token Service\ISHSTS\Integrating 3rd party service providers. **[New]**
	      - Work with Security Token Service\ISHSTS\Implementing light weight Windows Authentication. **[New]**
	      - Work with Security Token Service\ISHSTS\Resetting ISHSTS. **[New]**
1. Security Token Service (STS)
    1. Articles
	      - Work with Security Token Service\Certificates rollover/replacement\Implementing certificate replacement. **[New]**
	      - Work with Security Token Service\Certificates rollover/replacement\Implementing issuer certificate rollover with 3rd party STS. **[Update]**
	      - Work with Security Token Service\Certificates rollover/replacement\Implementing service certificate replacement with 3rd party STS. **[New]**

## alpha-0.1 (Pre-release)
Initial release
 
1. Working with deployments
    1. Cmdlets
	      - Clear-ISHDeploymentHistory
	      - Get-ISHDeployment
	      - Get-ISHDeploymentHistory
	      - Get-ISHPackageFolderPath
	      - Undo-ISHDeployment
    1. Articles
	      - Work with deployments  
	      - Work with files
	      - Work with Get-ISHPackageFolderPath  
1. Feature toggle
    1. Cmdlets
	      - Disable-ISHExternalPreview
	      - Disable-ISHUIContentEditor
	      - Disable-ISHUIQualityAssistant
	      - Disable-ISHUITranslationJob
	      - Enable-ISHExternalPreview
	      - Enable-ISHUIContentEditor
	      - Enable-ISHUIQualityAssistant
	      - Enable-ISHUITranslationJob
	      - Move-ISHUIEventMonitorTab
	      - Remove-ISHUIEventMonitorTab
	      - Set-ISHUIEventMonitorTab
	      - Test-ISHContentEditor
    1. Articles
	      - Work with UI Features  
1. Integration with Security Token Service (STS)
    1. Cmdlets
	      - Remove-ISHIntegrationSTSCertificate
	      - Save-ISHIntegrationSTSConfigurationPackage
	      - Set-ISHContentEditor
	      - Set-ISHIntegrationSTSCertificate
	      - Set-ISHIntegrationSTSWSFederation
	      - Set-ISHIntegrationSTSWSTrust
	      - Save-ISHIntegrationDBSTSSQLServerConfiguration
	      - Set-ISHAPIWCFServiceCertificate 
    1. Articles
	      - Security Token Service\Integrating with Security Token Service
	      - Security Token Service\ADFS\Integrating with Security Token Service
	      - Security Token Service\Certificates\Implementing issuer certificate rollover