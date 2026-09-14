#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class ChapterOneQaSummaryAutoReporter
{
    static string lastProcessedReportPath;

    static ChapterOneQaSummaryAutoReporter()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state != PlayModeStateChange.ExitingPlayMode) return;

        string reportPath = ChapterOnePlaythroughReporter.LastReportPath;
        if (string.IsNullOrWhiteSpace(reportPath) || !File.Exists(reportPath)) return;
        if (string.Equals(lastProcessedReportPath, reportPath, StringComparison.OrdinalIgnoreCase)) return;

        lastProcessedReportPath = reportPath;
        EditorApplication.delayCall += () => GenerateSummary(reportPath);
    }

    static void GenerateSummary(string reportPath)
    {
        if (string.IsNullOrWhiteSpace(reportPath) || !File.Exists(reportPath)) return;

        try
        {
            ChapterOneGameplayFreezeValidator.Check(reportPath, false);
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
