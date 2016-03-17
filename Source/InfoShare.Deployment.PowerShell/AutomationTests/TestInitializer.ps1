. "$PSScriptRoot\Common.ps1"


$executingScriptDirectory = Split-Path -Path $MyInvocation.MyCommand.Definition -Parent
$ISHUIContentEditorPath = Join-Path $executingScriptDirectory "ISHUIContentEditor.ps1"
$ISHUIQualityAssistantPath =  Join-Path $executingScriptDirectory "ISHUIQualityAssistant.ps1"
$ISHUIExternalPreview = Join-Path $executingScriptDirectory "ISHUIExternalPreview.ps1"
$GETISHDeployment = Join-Path $executingScriptDirectory "GET-ISHDeployment.ps1"
$ISHHistory = Join-Path $executingScriptDirectory "ISHGetHistory.ps1"
$ISHTranlationJob = Join-Path $executingScriptDirectory "ISHUITranslationJob.ps1"
$ISHUndoDeployment = Join-Path $executingScriptDirectory "UndoVanila.ps1"

Invoke-Expression "$ISHUIContentEditorPath" 
Invoke-Expression $ISHUIQualityAssistantPath
Invoke-Expression $ISHUIExternalPreview
Invoke-Expression $GETISHDeployment
Invoke-Expression $ISHHistory
Invoke-Expression $ISHTranlationJob
Invoke-Expression $ISHUndoDeployment