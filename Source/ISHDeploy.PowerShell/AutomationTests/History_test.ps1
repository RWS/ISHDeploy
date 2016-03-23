$deployment = Get-ISHDeployment -Name 'InfoShareSQL2014'
Enable-ISHUIContentEditor -ISHDeployment $deployment
Enable-ISHUIQualityAssistant -ISHDeployment $deployment
