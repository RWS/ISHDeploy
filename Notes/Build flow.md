# ISHDeploy build flow

This repository contains one Visual Studion solution file [ISHDeploy.sln](..\Source\ISHDeploy.sln).

The solution has the following projects

- **ISHDeploy** is the C# project for the binary PowerShell module.
  - Each cmdlet `.cs` class is annotated with the expected PowerShell help. The help is extracted to `maml` files with the [XmlDoc2CmdletDoc](https://github.com/red-gate/XmlDoc2CmdletDoc) tool.
- **ISHDeploy.Tests** is the unit tests project for **ISHDeploy**.
- **ISHDeploy.Documentation** is the project for the documentation portal developed in markdown and powered by [DOCFX](https://dotnet.github.io/docfx/). More in [Documentation.md](Documentation.md).
- **ISHDeploy.PowerShell** is the project with the integration tests developed in PowerShell and powered by [Pester](https://github.com/pester/Pester).

## Build

When the solution is build the following happens:

1. **ISHDeploy** and **ISHDeploy.Tests** builds. This is relatively straightforward. 'Build.props' drives some metadata information that are required for the build.
  - Upon building the `XmlDoc2CmdletDoc` is executed to generate the module's '.maml' file help.
1. **ISHDeploy.Documentation** builds with the following steps
  1. `PlatyPS` exports the help from each cmdlet to a markdown file.
  1. The markdown files are processed. 
    - Article files get infused with their PowerShell examples. 
    - Root markdown get the module name replaced. 
  1. DOXFX builds the static html files

## Build server

The build is automated using a Jenkins build server powered by DSL job plugin and groovy scripts. When a build job executes the following happen:

1. 'Build.props' is adjusted.
1. [ISHDeploy.sln](..\Source\ISHDeploy.sln) is build.
1. Binaries are signed.
1. **ISHDeploy.Tests** unit tests are executed.
1. Module is copied to remote server.
1. **ISHDeploy.PowerShell** integration tests are executed.
1. Module is published to internal repository.
1. Documentation portal is published to an IIS Virtual directory.