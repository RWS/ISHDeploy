param(
[string]$PathToFolderThatMustBeTested,
[string]$ExcludeFolders,
[string]$ExcludeExtensions
)

$PathToModule = Join-Path $PSScriptRoot "Modules\SDLDevTools"
Import-Module $PathToModule

Test-SDLOpenSourceHeader -FolderPath $PathToFolderThatMustBeTested -ExpandError -PassThru -ExcludeFolder $ExcludeFolders.Split(",") -ExcludeExtension $ExcludeExtensions.Split(",")
