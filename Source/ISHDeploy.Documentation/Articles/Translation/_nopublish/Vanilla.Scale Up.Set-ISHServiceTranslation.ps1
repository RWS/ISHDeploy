# Scale up Translation Builder 
Set-ISHServiceTranslationBuilder -ISHDeployment $deploymentName -Count 2

# Scale up Translation Organizer
Set-ISHServiceTranslationOrganizer -ISHDeployment $deploymentName -Count 3