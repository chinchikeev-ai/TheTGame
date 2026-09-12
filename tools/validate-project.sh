#!/bin/sh
set -eu

ROOT=$(cd "$(dirname "$0")/.." && pwd)
UNITY=${UNITY_EDITOR:-/opt/unity/Editor/Unity}
mkdir -p "$ROOT/Logs/Validation" "$ROOT/TestResults"

python3 "$ROOT/tools/check-architecture.py"

"$UNITY" -batchmode -nographics -quit -projectPath "$ROOT" -executeMethod CommandLineValidation.RunArchitectureChecks -logFile "$ROOT/Logs/Validation/architecture.log"
"$UNITY" -batchmode -nographics -quit -projectPath "$ROOT" -runTests -testPlatform EditMode -testResults "$ROOT/TestResults/editmode.xml" -logFile "$ROOT/Logs/Validation/editmode.log"
"$UNITY" -batchmode -nographics -quit -projectPath "$ROOT" -runTests -testPlatform PlayMode -testResults "$ROOT/TestResults/playmode.xml" -logFile "$ROOT/Logs/Validation/playmode.log"

if [ "${SKIP_BUILD:-0}" != "1" ]; then
  "$UNITY" -batchmode -nographics -quit -projectPath "$ROOT" -executeMethod BuildPlayerCommand.BuildWindowsVisibleMap -logFile "$ROOT/Logs/Validation/build.log"
fi

echo "VALIDATION PASSED"
