. "$PSScriptRoot\Common.ps1"


$executingScriptDirectory = Split-Path -Path $MyInvocation.MyCommand.Definition -Parent
$ISHUIContentEditorPath = Join-Path $executingScriptDirectory "ISHUIContentEditor.ps1"
$ISHUIQualityAssistantPath =  Join-Path $executingScriptDirectory "ISHUIQualityAssistant.ps1"
$ISHUIExternalPreview = Join-Path $executingScriptDirectory "ISHUIExternalPreview.ps1"
$GETISHDeployment = Join-Path $executingScriptDirectory "GET-ISHDeployment.ps1"

Invoke-Expression "$ISHUIContentEditorPath" 
Invoke-Expression $ISHUIQualityAssistantPath
Invoke-Expression $ISHUIExternalPreview
Invoke-Expression $GETISHDeployment
