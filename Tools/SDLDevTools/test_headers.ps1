$pathToModule = Join-Path $PSScriptRoot "Modules\SDLDevTools"
$pathToSourceFolder = $PSScriptRoot.Replace("Tools\SDLDevTools", "Source")
$pathToISHDeployFolder = Join-Path $pathToSourceFolder "ISHDeploy"

$excludeFolders = @("bin", "obj")
$excludeExtensions = @(".csproj", ".user", ".xml", ".dll", ".pdb", ".txt", ".manifest", ".md", ".sql", ".js", ".cache", ".targets", ".config")
Import-Module $pathToModule

Test-SDLOpenSourceHeader -FolderPath $pathToISHDeployFolder -ExpandError -PassThru -ExcludeFolder $excludeFolders -ExcludeExtension $excludeExtensions