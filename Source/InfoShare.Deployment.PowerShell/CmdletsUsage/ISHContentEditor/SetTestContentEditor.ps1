CLS
#Import-Module InfoShare.Deployment
Import-Module "C:\Stash Projects\Infoshare Deployment\Source\InfoShare.Deployment\bin\Debug\InfoShare.Deployment.dll"

#Test-ISHContentEditor

#Pause

$info = @{
  "AuthorFolderPath" = "C:\Trisoft\RnDProjects\Trisoft\Test\Server.Web\Websites\Trisoft.InfoShare.Web";
  "InstallPath" = "F:\InfoShare";
  "Suffix" = ''
}

Set-ISHContentEditor -IshProject $info -LicensePath "C:\temp\localhost.txt"

#Pause

#Test-ISHContentEditor