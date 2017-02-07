# Scale up Translation Builder 
Set-ISHServiceTranslationBuilder -ISHDeployment $ishDeployment -Count 2

# Scale up Translation Organizer
Set-ISHServiceTranslationOrganizer -ISHDeployment $ishDeployment -Count 3