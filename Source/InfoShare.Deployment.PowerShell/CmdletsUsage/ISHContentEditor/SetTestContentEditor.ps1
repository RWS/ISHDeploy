CLS
Import-Module InfoShare.Deployment

$info = @{
  "WebPath" = "F:\InfoShare\";
  "Suffix" = ''
}

#Set-ISHContentEditor -LicensePath "C:\temp\localhost.txt"

#Pause

Test-ISHContentEditor -Hostname "localhost" -ISHDeployment $info 