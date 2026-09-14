#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class ChapterOneQaSummaryAutoReporter
{
    const string PendingReportKey = "TheTroyGame.ChapterOneQaSummary.PendingReport";
    const string LastProcessedReportKey = "TheTroyGame.ChapterOneQaSummary.LastProcessedReport";

    static ChapterOneQaSummaryAutoReporter()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

        if (!EditorApplication.isPlayingOrWillChangePlaymode)
            EditorApplication.delayCall += ProcessPendingReport;
    }

    static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            string reportPath = ChapterOnePlaythroughReporter.LastReportPath;
            if (!string.IsNullOrWhiteSpace(reportPath) && File.Exists(reportPath))
                SessionState.SetString(PendingReportKey, reportPath);
            return;
        }

        if (state == PlayModeStateChange.EnteredEditMode)
            EditorApplication.delayCall += ProcessPendingReport;
    }

    static void ProcessPendingReport()
    {
        string reportPath = SessionState.GetString(PendingReportKey, string.Empty);
        if (string.IsNullOrWhiteSpace(reportPath) || !File.Exists(reportPath)) return;

        string lastProcessed = SessionState.GetString(LastProcessedReportKey, string.Empty);
        if (string.Equals(lastProcessed, reportPath, StringComparison.OrdinalIgnoreCase))
        {
            SessionState.EraseString(PendingReportKey);
            return;
        }

        try
        {
            ChapterOneGameplayFreezeValidator.Check(reportPath, false);
            SessionState.SetString(LastProcessedReportKey, reportPath);
            SessionState.EraseString(PendingReportKey);
            Debug.Log($"[CHAPTER I QA SUMMARY] Automatic QA summary generated for {Path.GetFileName(reportPath)}.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[CHAPTER I QA SUMMARY] Automatic summary failed for {Path.GetFileName(reportPath)}: {ex.GetType().Name}: {ex.Message}");
            Debug.LogException(ex);
        }
    }
}
#endif
