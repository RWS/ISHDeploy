
ISHDeploy
==========

A small library for padding strings in JavaScript. Marmalade-free.

[![Powershell version support][shield-ps]](#)
[![Build status][shield-build]](#)
[![Code coverage][shield-coverage]](#)
[![Dependencies][shield-dependencies]](#)

Table of Contents
-----------------

  * [Requirements](#requirements)
  * [Usage](#usage)
  * [Contributing](#contributing)
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

$deploy = Get-ISHDeployment -Name 'InfoShareORA12'

Set-ISHUIEventMonitorTab -ISHDeployment $deploy -Label "EventTab" -Description "New Event Tab"
```

Contributing
------------

{ Contribution section }


License
-------

Copyright &copy; SDL 2016

[ps]: https://msdn.microsoft.com/en-us/powershell/mt173057.aspx
[shield-coverage]: https://img.shields.io/badge/coverage-100%25-brightgreen.svg
[shield-dependencies]: https://img.shields.io/badge/dependencies-up%20to%20date-brightgreen.svg
[shield-license]: https://img.shields.io/badge/license-MIT-blue.svg
[shield-ps]: https://img.shields.io/badge/powershell-3+-lightgrey.svg
[shield-build]: https://img.shields.io/badge/build-passing-brightgreen.svg