#!/bin/sh
set -eu

ROOT=$(cd "$(dirname "$0")/.." && pwd)
UNITY=${UNITY_EDITOR:-/opt/unity/Editor/Unity}
mkdir -p "$ROOT/Logs/Validation"

"$UNITY" -batchmode -nographics -quit \
  -projectPath "$ROOT" \
  -executeMethod ChapterOneArtFreezeValidator.RunBatchmode \
  -logFile "$ROOT/Logs/Validation/chapter1-art-freeze.log"

echo "CHAPTER I ART FREEZE AUDIT PASSED"
