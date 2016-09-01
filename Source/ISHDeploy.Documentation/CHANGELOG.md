# Version notes

## release-1.0

1. General 
  1. Documentation
    - Related links render as links and provide a quick navigation.	   

1. Integration STS
  1. Cmdlets
	- `Enable-ISHIntegrationSTSInternalAuthentication` and  `Disablee-ISHIntegrationSTSInternalAuthentication` enable access for internal users. **[New]**
  1. Articles
    - Security Token Service\Integrating with Security Token Service mentiones how to enable internal user authentication. **[Update]**
	- Security Token Service\ADFS\Integrating with Security Token Service mentiones how to enable internal user authentication. **[Update]**

1. ISHSTS
    1. Cmdlets
	      - Set-ISHSTSConfiguration does check - is Windows Authentication feature installed on environment or not.
		  - Set-ISHSTSRelyingParty `-Realm` is required to be a proper url with schema.

**Fixed known issues**

- When the deployment is configured for light weight windows authentication, the described certificate rollover leaves the system broken. The workaround is to re-execute the `Set-ISHSTSConfiguration -ISHDeployment $deploymentName -AuthenticationType Windows`.


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