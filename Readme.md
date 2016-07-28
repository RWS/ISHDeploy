
# ISHDeploy

# Table of Contents

- [Introduction](#introduction)
- [Requirements](#requirements)
- [License](#license)

## Introduction

**ISHDeploy** is a PowerShell module that provides the necessary tooling to create, update and maintain [SDL Knowledge Center](http://www.sdl.com/download/sdl-knowledge-center/60978/) Content Manager deployments.
With ISHDeploy the deployments can be expressed in PowerShell scripts. The module's cmdlets aim towards enabling the **code as configuration** principal. 
With each cmdlet, the author of the script focuses on expressing intention without knowledge about the configuration implementation details. 
This allows developing scripts that are cross version applicable as long as the referenced feature is compatible in Content Manager.

For example the following segment enables the Content Editor.
```powershell
$xopusLicenseKey="license key for ish.example.com"
$xopusLicenseDomain='ish.example.com"
Set-ISHContentEditor -ISHDeployment $deploymentName -LicenseKey "$xopusLicenseKey" -Domain $xopusLicenseDomain
Enable-ISHUIContentEditor -ISHDeployment $deploymentName
```

The module is available in [PowerShell gallery](https://www.powershellgallery.com/items?q=ISHDeploy&x=0&y=0). 

The PowerShell module name matches the specific version of [SDL Knowledge Center](http://www.sdl.com/download/sdl-knowledge-center/60978/) release version. For example 
- **ISHDeploy.12.0.0** targets the release of **12.0.0**

Every cmdlet provides rich help that can be explored with the `Get-Help` PowerShell command.

Additionally, a [documentation portal](https://github.com/sdl/ISHDeploy/) is available with the following content:
- Getting started information.
- Article base on various subjects e.g. *Integration with security token service*.
- The online equivelant of each cmdlet's help.  

## Requirements

ISHDeploy requires the following to run:

* [Powershell](https://msdn.microsoft.com/en-us/powershell/mt173057.aspx) version 4.0 or higher.
* Administrative user rights.
* An [SDL Knowledge Center](http://www.sdl.com/download/sdl-knowledge-center/60978/) content manager deployment.

The cmdlets that query the deployment work with different versions but the ones that modify files don't. 

## License
Copyright (c) 2014 All Rights Reserved by the SDL Group.

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.