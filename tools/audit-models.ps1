param([switch]$BuildCandidates)

$ErrorActionPreference = "Stop"
$ProjectRoot = Split-Path -Parent $PSScriptRoot
$Unity = $env:UNITY_EDITOR
if ([string]::IsNullOrWhiteSpace($Unity)) { throw "Set UNITY_EDITOR to Unity.exe" }

$Logs = Join-Path $ProjectRoot "Logs\Validation"
New-Item -ItemType Directory -Force -Path $Logs | Out-Null
$Method = "CampaignModelAuditCommand.RunCurrentBatchmode"
if ($BuildCandidates) { $Method = "CampaignModelAuditValidator.BuildAndRunBatchmode" }

& $Unity -batchmode -nographics -projectPath $ProjectRoot -executeMethod $Method -logFile (Join-Path $Logs "model-audit.log")
if ($LASTEXITCODE -ne 0) { throw "Campaign model audit failed. See Logs\Validation\model-audit.log" }
Write-Host "CAMPAIGN MODEL AUDIT PASSED"
