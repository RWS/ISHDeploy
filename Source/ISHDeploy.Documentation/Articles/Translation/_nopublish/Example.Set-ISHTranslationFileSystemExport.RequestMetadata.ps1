$requestMetadata=@(
    New-ISHFieldMetadata -Name FAUTHOR -Level lng -ValueType value
    New-ISHFieldMetadata -Name DOC-LANGUAGE -Level lng -ValueType value
)

# Set the export
Set-ISHTranslationFileSystemExport -ISHDeployment $deploymentName -Name FileSystem -ExportFolderPath $exportFolderPath -RequestMetadata $requestMetadata