param(
[string]$PathToModule,
[string]$PathToFolderThatMustBeTested,
[string]$ExcludeFolders,
[string]$ExcludeExtensions
)

Import-Module $PathToModule

Test-SDLOpenSourceHeader -FolderPath $PathToFolderThatMustBeTested -ExpandError -PassThru -ExcludeFolder $ExcludeFolders.Split(",") -ExcludeExtension $ExcludeExtensions.Split(",")
