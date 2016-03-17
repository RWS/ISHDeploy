. "$PSScriptRoot\Common.ps1"


$executingScriptDirectory = Split-Path -Path $MyInvocation.MyCommand.Definition -Parent
$ISHUIContentEditorPath = Join-Path $executingScriptDirectory "ISHUIContentEditor.ps1"
$ISHUIQualityAssistantPath =  Join-Path $executingScriptDirectory "ISHUIQualityAssistant.ps1"
$ISHUIExternalPreview = Join-Path $executingScriptDirectory "ISHUIExternalPreview.ps1"
$GETISHDeployment = Join-Path $executingScriptDirectory "Get-ISHDeployment.ps1"
$ISHHistory = Join-Path $executingScriptDirectory "ISHGetHistory.ps1"
$ISHTranlationJob = Join-Path $executingScriptDirectory "ISHUITranslationJob.ps1"
$UndoISHDeployment = Join-Path $executingScriptDirectory "UndoISHDeployment.ps1"
$ClearISHDeploymentHistory = Join-Path $executingScriptDirectory "ClearISHDeploymentHistory.ps1"

Invoke-Expression "$ISHUIContentEditorPath" 
Invoke-Expression $ISHUIQualityAssistantPath
Invoke-Expression $ISHUIExternalPreview
Invoke-Expression $GETISHDeployment
Invoke-Expression $ISHHistory
Invoke-Expression $ISHTranlationJob
Invoke-Expression $UndoISHDeployment
Invoke-Expression $ClearISHDeploymentHistory