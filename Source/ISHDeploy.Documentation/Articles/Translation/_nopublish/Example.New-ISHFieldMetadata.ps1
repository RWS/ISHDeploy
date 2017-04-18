# Create an array of requested metadata

$requestMetadata=@(
    New-ISHFieldMetadata -Name FAUTHOR -Level lng -ValueType value
    New-ISHFieldMetadata -Name DOC-LANGUAGE -Level lng -ValueType value
)

# Create an array of group metadata

$groupMetadata=@(
    New-ISHFieldMetadata -Name FAUTHOR -Level lng -ValueType value
    New-ISHFieldMetadata -Name DOC-LANGUAGE -Level lng -ValueType value
)


