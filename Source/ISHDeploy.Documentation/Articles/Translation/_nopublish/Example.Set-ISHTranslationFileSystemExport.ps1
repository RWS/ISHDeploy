# The export path on the file system
$exportFolderPath=""

# Set the export
Set-ISHTranslationFileSystemExport -ISHDeployment $deploymentName -Name FileSystem -ExportFolderPath $exportFolderPath