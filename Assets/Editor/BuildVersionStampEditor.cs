using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public sealed class BuildVersionStampEditor : IPreprocessBuildWithReport, IPostprocessBuildWithReport
{
    static string previousVersion;
    static bool stamped;

    public int callbackOrder => -1000;

    public void OnPreprocessBuild(BuildReport report)
    {
        stamped = false;
        if (!IsWindowsPlayer(report.summary.platform)) return;

        if (!BuildVersionInfo.TryGetRepositoryIdentity(out string branch, out string sha))
        {
            Debug.LogWarning("Build version stamp: Git branch/SHA could not be resolved; player will show UNSTAMPED.");
            return;
        }

        previousVersion = PlayerSettings.bundleVersion;
        PlayerSettings.bundleVersion = BuildVersionInfo.ComposeStampedVersion(branch, sha);
        stamped = true;
        Debug.Log("Build version stamp: " + PlayerSettings.bundleVersion);
    }

    public void OnPostprocessBuild(BuildReport report)
    {
        if (!stamped) return;
        PlayerSettings.bundleVersion = previousVersion;
        stamped = false;
    }

    static bool IsWindowsPlayer(BuildTarget target)
    {
        return target == BuildTarget.StandaloneWindows || target == BuildTarget.StandaloneWindows64;
    }
}
