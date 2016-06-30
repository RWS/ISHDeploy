# Introduction

**{ModuleName}** is a PowerShell module that enables the **code as configuration** concept for [SDL](https://sdl.com/)'s [Knowledge Center](http://www.sdl.com/download/sdl-knowledge-center/60978/) **Content Manager** service. 
With code as configuration, PowerShell authors develop scripts that configure Content Manager vanilla deployments. 

As a very brief example the following script sets the license and enables the **Content Editor** for the default deployment.

```powershell
$deployment = Get-ISHDeployment -Name InfoShare
Set-ISHContentEditor -ISHDeployment $deployment -LicenseKey "licensekey" -Domain "ish.example.com"
Enable-ISHUIContentEditor -ISHDeployment $deployment
```

# Getting started
[{ModuleName}](https://www.powershellgallery.com/packages/{ModuleName}/) module is available to install from the [PowerShell galery](https://www.powershellgallery.com/) and requires PowerShell version 4.0. 
If your operating system does not include this version then install it following the instructions on [technet](http://social.technet.microsoft.com/wiki/contents/articles/21016.how-to-install-windows-powershell-4-0.aspx).

To work with the PowerShell galery the **PowerShellGet** module is required. **PowerShellGet** module is included in the PowerShell version 5 installation , therefore no action is necessary.
For PowerShell version install the module using the [Package Management](https://www.microsoft.com/en-us/download/details.aspx?id=51451) installer. 

To quickly check your installed PowerShell version execute
```powershell
$PSVersionTable
```

A version 4.0 console would return 
```
Name                           Value                                                                                   
----                           -----                                                                                   
PSVersion                      4.0                                                                                     
WSManStackVersion              3.0                                                                                     
SerializationVersion           1.1.0.1                                                                                 
CLRVersion                     4.0.30319.42000                                                                         
BuildVersion                   6.3.9600.17400                                                                          
PSCompatibleVersions           {1.0, 2.0, 3.0, 4.0}                                                                    
PSRemotingProtocolVersion      2.2                                                                                     
```

To install the {ModuleName} module execute
```powershell
Install-Module ISHDeploy.12.0.0 -Scope CurrentUser
```

For more detailed information, please read the [getting started](Getting Started.md) page and the rest of the pages in this portal.

# What is new in this release?

Pre-release version `0.1-alpha1` introduces the following cmdlets and articles:

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

Please review the module's entire [history](CHANGELOG.md).

