# ISHDeploy

[![Powershell version support][shield-ps]](#)
[![Build Status](http://kiev-green-bld.global.sdl.corp:8080/job/ISHDeploy%20Daily%20Develop.12.0.1/badge/icon)](http://kiev-green-bld.global.sdl.corp:8080/job/ISHDeploy%20Daily%20Develop.12.0.1/)
[![Code coverage][shield-coverage]](#)
[![Dependencies][shield-dependencies]](#)

Table of Contents
-----------------

  * [Requirements](#requirements)
  * [Usage](#usage)
  * [Contributing](#contributing)
  * [More Info](#more-info)
  * [License](#license)


Requirements
------------

ISHDeploy requires the following to run:

* [Powershell][ps] 3.0+
* Administrative rights
* InfoShare Content Manager

Usage
-----

To use this module, it should me imported.

```powershell
Import-Module "ISHDeploy"

$deployment = Get-ISHDeployment -Name 'InfoShareORA12'

Set-ISHUIEventMonitorTab -ISHDeployment $deployment -Label "EventTab" -Description "New Event Tab"
```

Contributing
------------

{ Contribution section }

More Info
------------

For more info please see:

  * [Maintaining Versions][versions]
  * [Module Layers Organization][layers]
  * [How-To Publish Module][publish-module]


License
-------

Copyright &copy; SDL 2016

[versions]: ./Notes/Maintain%20versions.md
[layers]: ./Notes/Module%20Layers.md
[publish-module]: ./Notes/Publish%20Module%20To%20PSGallery.md

[ps]: https://msdn.microsoft.com/en-us/powershell/mt173057.aspx
[shield-coverage]: https://img.shields.io/badge/coverage-100%25-brightgreen.svg
[shield-dependencies]: https://img.shields.io/badge/dependencies-up%20to%20date-brightgreen.svg
[shield-license]: https://img.shields.io/badge/license-MIT-blue.svg
[shield-ps]: https://img.shields.io/badge/powershell-3+-lightgrey.svg
[shield-build]: https://img.shields.io/badge/build-passing-brightgreen.svg