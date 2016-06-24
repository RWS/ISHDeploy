# ISHDeploy.Documentation

## Dependencies
Documentation generation uses PowerShell module [platyPS](https://www.powershellgallery.com/packages/platyPS/0.3.1.213) with version 0.3.1.213. 
At this moment higher version break the build.

Mote about [PlatyPS](https://blogs.msdn.microsoft.com/powershell/2016/02/05/platyps-write-external-help-files-in-markdown/)

## Pre process before the DOCFX build
When the ISHDeploy.Documentation builds the following actions take place before the DOCFX build:

1. `PlatyPS` exports the help for each cmdlet in `obj\doc\commands` in markdown format.
1. Each markdown `article` is copied in `obj\doc\articles` within a relevent directory to the original file. Then the markdown file gets infused with code fences matching the reference path. e.g. `CopyCodeBlockAndLink` or `CopyCodeBlock`
1. The root markdown files are copied in `obj\doc` and the `{ModuleName}` is replaced by the actual modulename from the build

Practically everything is copied into the `obj\doc` while respecting the original structure. For this reason the `docfx.json` references content and resources from this location.

