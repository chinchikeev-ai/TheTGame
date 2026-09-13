$ErrorActionPreference = "Stop"
$ProjectRoot = Split-Path -Parent $PSScriptRoot
$Unity = $env:UNITY_EDITOR
if ([string]::IsNullOrWhiteSpace($Unity) -or -not (Test-Path $Unity)) {
    throw "Unity Editor not found. Set UNITY_EDITOR to Unity.exe."
}
$Logs = Join-Path $ProjectRoot "Logs\Validation"
New-Item -ItemType Directory -Force -Path $Logs | Out-Null
$process = Start-Process -FilePath $Unity -ArgumentList @(
    "-batchmode", "-nographics", "-quit",
    "-projectPath", $ProjectRoot,
    "-executeMethod", "ChapterOneArtFreezeValidator.RunBatchmode",
    "-logFile", (Join-Path $Logs "chapter1-art-freeze.log")
) -Wait -PassThru -NoNewWindow
if ($process.ExitCode -ne 0) { throw "Chapter I art freeze audit failed with exit code $($process.ExitCode)" }
Write-Host "CHAPTER I ART FREEZE AUDIT PASSED" -ForegroundColor Green
