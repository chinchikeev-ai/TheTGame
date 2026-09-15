using System;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class MainBranchSyncGuard
{
    static MainBranchSyncGuard()
    {
        EditorApplication.delayCall += CheckMainSync;
    }

    [MenuItem("The Troy Game/Sync/Check origin main")]
    public static void CheckMainSync()
    {
        if (!BuildVersionInfo.RefreshEditorMainSyncState(out string branch, out string localSha, out string remoteSha, out bool dirty))
        {
            Debug.LogWarning("MAIN SYNC: could not fetch/resolve origin/main. Local files were not changed.");
            return;
        }

        bool onMain = string.Equals(branch, "main", StringComparison.OrdinalIgnoreCase);
        bool synced = string.Equals(localSha, remoteSha, StringComparison.OrdinalIgnoreCase);

        if (!onMain)
        {
            Debug.LogWarning($"MAIN SYNC: current branch is {branch}. LOCAL {localSha}, origin/main {remoteSha}. No files were changed.");
            return;
        }

        if (!synced)
        {
            Debug.LogError($"MAIN SYNC OUTDATED: Unity is on LOCAL {localSha}, while origin/main is {remoteSha}. The latest main changes are NOT applied locally. Working tree: {(dirty ? "DIRTY" : "CLEAN")}. No pull/reset was performed.");
            return;
        }

        Debug.Log($"MAIN SYNC OK: LOCAL {localSha} == origin/main {remoteSha}. Working tree: {(dirty ? "DIRTY" : "CLEAN")}.");
    }
}
