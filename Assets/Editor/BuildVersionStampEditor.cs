using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public sealed class BuildVersionStampEditor : IPreprocessBuildWithReport, IPostprocessBuildWithReport
{
    const string StampFileName = "build_version.stamp";

    static string pendingStamp;

    public int callbackOrder => -1000;

    public void OnPreprocessBuild(BuildReport report)
    {
        pendingStamp = null;
        if (!IsWindowsPlayer(report.summary.platform)) return;

        if (!BuildVersionInfo.TryGetRepositoryIdentity(out string branch, out string sha))
        {
            Debug.LogWarning("Build version stamp: Git branch/SHA could not be resolved; player will show UNSTAMPED.");
            return;
        }

        pendingStamp = BuildVersionInfo.ComposeStampedVersion(branch, sha);
        Debug.Log("Build version stamp: " + pendingStamp);
    }

    public void OnPostprocessBuild(BuildReport report)
    {
        if (string.IsNullOrEmpty(pendingStamp)) return;
        if (report.summary.result != BuildResult.Succeeded)
        {
            pendingStamp = null;
            return;
        }

        try
        {
            string outputDirectory = Path.GetDirectoryName(report.summary.outputPath);
            if (string.IsNullOrEmpty(outputDirectory)) return;

            string dataFolder = Path.Combine(
                outputDirectory,
                Path.GetFileNameWithoutExtension(report.summary.outputPath) + "_Data");
            Directory.CreateDirectory(dataFolder);
            File.WriteAllText(Path.Combine(dataFolder, StampFileName), pendingStamp);
            Debug.Log("Build version stamp written to player data folder: " + pendingStamp);
        }
        catch (Exception ex)
        {
            Debug.LogWarning("Build version stamp could not be persisted: " + ex.Message);
        }
        finally
        {
            pendingStamp = null;
        }
    }

    static bool IsWindowsPlayer(BuildTarget target)
    {
        return target == BuildTarget.StandaloneWindows || target == BuildTarget.StandaloneWindows64;
    }
}
