# Get all relying parties
Get-ISHSTSRelyingParty -ISHDeployment $deploymentName

# Get only Knowledge Center specific relying parties
Get-ISHSTSRelyingParty -ISHDeployment $deploymentName -ISH -BL -LC

# Get only Content Manager specific relying parties
Get-ISHSTSRelyingParty -ISHDeployment $deploymentName -ISH

# Get only Content Delivery specific relying parties
Get-ISHSTSRelyingParty -ISHDeployment $deploymentName -LC

# Get only Quality Assistant specific relying parties
Get-ISHSTSRelyingParty -ISHDeployment $deploymentName -BL