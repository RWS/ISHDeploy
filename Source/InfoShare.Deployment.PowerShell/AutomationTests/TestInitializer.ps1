. "$PSScriptRoot\Common.ps1"


$executingScriptDirectory = Split-Path -Path $MyInvocation.MyCommand.Definition -Parent
$ISHUIContentEditorPath = Join-Path $executingScriptDirectory "ISHUIContentEditor.ps1"
$ISHUIQualityAssistantPath =  Join-Path $executingScriptDirectory "ISHUIQualityAssistant.ps1"

Invoke-Expression "$ISHUIContentEditorPath" 
Invoke-Expression $ISHUIQualityAssistantPath
