#!/bin/sh
set -eu

ROOT=$(cd "$(dirname "$0")/.." && pwd)
UNITY=${UNITY_EDITOR:-/opt/unity/Editor/Unity}
mkdir -p "$ROOT/Logs/Validation"

METHOD=CampaignModelAuditCommand.RunCurrentBatchmode
if [ "${BUILD_CANDIDATES:-0}" = "1" ]; then
  METHOD=CampaignModelAuditValidator.BuildAndRunBatchmode
fi

"$UNITY" -batchmode -nographics -projectPath "$ROOT" -executeMethod "$METHOD" -logFile "$ROOT/Logs/Validation/model-audit.log"

echo "CAMPAIGN MODEL AUDIT PASSED"
