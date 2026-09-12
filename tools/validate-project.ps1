param(
    [switch]$SkipBuild
)

$ErrorActionPreference = "Stop"
$ProjectRoot = Split-Path -Parent $PSScriptRoot
$UnityVersion = "6000.6.0f1"
$Unity = $env:UNITY_EDITOR

if ([string]::IsNullOrWhiteSpace($Unity)) {
    $candidate = "C:\Program Files\Unity\Hub\Editor\$UnityVersion\Editor\Unity.exe"
    if (Test-Path $candidate) { $Unity = $candidate }
}

if ([string]::IsNullOrWhiteSpace($Unity) -or -not (Test-Path $Unity)) {
    throw "Unity Editor not found. Set UNITY_EDITOR to Unity.exe. Expected project version: $UnityVersion"
}

$Logs = Join-Path $ProjectRoot "Logs\Validation"
$Results = Join-Path $ProjectRoot "TestResults"
New-Item -ItemType Directory -Force -Path $Logs | Out-Null
New-Item -ItemType Directory -Force -Path $Results | Out-Null

function Invoke-Step([string]$Name, [string[]]$Args) {
    Write-Host "==> $Name"
    $process = Start-Process -FilePath $Unity -ArgumentList $Args -Wait -PassThru -NoNewWindow
    if ($process.ExitCode -ne 0) {
        throw "$Name failed with exit code $($process.ExitCode)"
    }
}

Write-Host "==> Fast architecture guard"
python (Join-Path $PSScriptRoot "check-architecture.py")
if ($LASTEXITCODE -ne 0) { throw "Architecture guard failed" }

Invoke-Step "Unity architecture validation" @(
    "-batchmode", "-nographics", "-quit",
    "-projectPath", $ProjectRoot,
    "-executeMethod", "CommandLineValidation.RunArchitectureChecks",
    "-logFile", (Join-Path $Logs "architecture.log")
)

Invoke-Step "EditMode tests" @(
    "-batchmode", "-nographics", "-quit",
    "-projectPath", $ProjectRoot,
    "-runTests", "-testPlatform", "EditMode",
    "-testResults", (Join-Path $Results "editmode.xml"),
    "-logFile", (Join-Path $Logs "editmode.log")
)

Invoke-Step "PlayMode tests" @(
    "-batchmode", "-nographics", "-quit",
    "-projectPath", $ProjectRoot,
    "-runTests", "-testPlatform", "PlayMode",
    "-testResults", (Join-Path $Results "playmode.xml"),
    "-logFile", (Join-Path $Logs "playmode.log")
)

if (-not $SkipBuild) {
    Invoke-Step "Windows build" @(
        "-batchmode", "-nographics", "-quit",
        "-projectPath", $ProjectRoot,
        "-executeMethod", "BuildPlayerCommand.BuildWindowsVisibleMap",
        "-logFile", (Join-Path $Logs "build.log")
    )
}

Write-Host "VALIDATION PASSED" -ForegroundColor Green
