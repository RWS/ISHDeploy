CLS
#Import-Module InfoShare.Deployment
Import-Module "C:\Stash Projects\Infoshare Deployment\Source\InfoShare.Deployment\bin\Debug\InfoShare.Deployment.dll"

#Test-ISHContentEditor

#Pause

Set-ISHProject "C:\Trisoft\RnDProjects\Trisoft\Test\Server.Web" sites

#Set-ISHContentEditor -LicensePath "C:\temp\localhost2.txt"

#Pause

Test-ISHContentEditor -Hostname "mecdevapp01.global.sdl.corp"