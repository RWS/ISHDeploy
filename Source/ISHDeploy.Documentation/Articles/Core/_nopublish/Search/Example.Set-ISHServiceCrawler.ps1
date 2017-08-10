# Manually set the hostname
$hostname="ish.example.com"
# As an alternative, derive the hostname from the deployment's configuration
# $hostname=Get-ISHDeployment -ISHDeployment $deploymentName |Select-Object -ExpandProperty AccessHostName

# Manually set the catalog
$catalog="InfoShare"
# As an alternative, use the same name as with the deployment name
# $catalog=$deploymentName

Set-ISHServiceCrawler -ISHDeployment $deploymentName -Catalog $catalog -Hostname $hostname