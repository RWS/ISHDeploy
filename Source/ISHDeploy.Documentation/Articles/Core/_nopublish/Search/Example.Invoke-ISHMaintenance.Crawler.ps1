# Register with the crawler's current configuration
Invoke-ISHMaintenance -ISHDeployment $deploymentName -Crawler -Register

# Unregister all crawlers
Invoke-ISHMaintenance -ISHDeployment $deploymentName -Crawler -UnRegisterAll

# Start a full reindex
Invoke-ISHMaintenance -ISHDeployment $deploymentName -Crawler -ReIndex