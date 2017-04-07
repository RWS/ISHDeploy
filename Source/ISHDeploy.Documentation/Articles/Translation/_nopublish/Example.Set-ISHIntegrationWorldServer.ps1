# Uri and credentials for SDL World Server
$uri="http://worldserver.example.com:8080/ws/services"
$credential=Get-Credential -Message "SDL WorldServer integration credential" 

# Set the integration
Set-ISHIntegrationWorldServer -ISHDeployment $deploymentName -Name WorldServer -Uri $uri -Credential $credential -Mappings $mappings