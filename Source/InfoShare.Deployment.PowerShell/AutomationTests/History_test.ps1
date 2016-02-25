$deployment = Get-ISHDeployment -Deployment 'SQL2014'
Enable-ISHUIContentEditor -ISHDeployment $deployment
Enable-ISHUIQualityAssistant -ISHDeployment $deployment
