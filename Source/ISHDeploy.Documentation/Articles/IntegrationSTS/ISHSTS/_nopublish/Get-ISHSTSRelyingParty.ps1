# Get all relying parties
Get-ISHSTSRelyingParty -ISHDeployment $deployment

# Get only Knowledge Center specific relying parties
Get-ISHSTSRelyingParty -ISHDeployment $deployment -ISH -BL -LC

# Get only Content Manager specific relying parties
Get-ISHSTSRelyingParty -ISHDeployment $deployment -ISH

# Get only Content Delivery specific relying parties
Get-ISHSTSRelyingParty -ISHDeployment $deployment -LC

# Get only Quality Assistant specific relying parties
Get-ISHSTSRelyingParty -ISHDeployment $deployment -BL