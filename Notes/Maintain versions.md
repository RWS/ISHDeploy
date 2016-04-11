# Instructions and notes for each version of ISHDeploy module
===========================================================

# Pulling a branch into multiple develop branches

* Make sure that all references numbers are correct
* Module name
* Manifest and Nuspec
* DocPortal
* Readme.md

# Subjects known as candidates for messy pulls

* Articles that reference the module version
* Articles that reference the module name

# When releasing a branch

* Make sure that all software version in the documentation are accurate
* Make sure that the module depends on the correct version of the released product

# How we handle build versions and module name

There is `Build.props` file in solution items (next to ISHDeploy.sln file) which handles module name, version and company specific information.
Module name is different for each branch, for instance:

    Branch Name       Module Name
    -----------       -----------
    develop.12.0      ISHDeploy.12.0
    develop.12.1      ISHDeploy.12.1
    develop.13.0      ISHDeploy.13.0

Module version consists of 4 parts: `major.minor.build.revision`

* `major` and `minor` - specific for release builds.
* `build` - calculated in `Build.props` file for each build and has short and long formats.
Short format: 1200 * (CurrentYear - 2015) + 100 * CurrentMonth + CurrentDay.
Long format starts with "short format" plus current time in format "HHmmss".
* `revision` - numeric value that will correspond to semantic versioning. For instance: alpha version = 1, beta version = 2.

`Build.props` file provides values that used for generating:
 * Correct output module name (see `ISHDeploy.csproj`);
 * AssemblyInfo.cs file (see `ISHDeploy\Properties\AssemblyInfo.targets`);
 * ISHDeploy.x.xx.psd1 (module manifest) file (see `ISHDeploy\Properties\ModuleManifest.targets`);

Also, `ModuleName` property from is used for debug information and post build events in project files.