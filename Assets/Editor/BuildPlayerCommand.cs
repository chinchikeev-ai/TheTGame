using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.Rendering;

public static class BuildPlayerCommand
{
    public static void BuildWindowsVisibleMap()
    {
        string projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
        string buildDir = Path.Combine(projectRoot, "Builds", "Windows_VisibleMap");
        Directory.CreateDirectory(buildDir);

        for (int i = 0; i < QualitySettings.names.Length; i++)
        {
            if (QualitySettings.names[i] == "PC")
            {
                QualitySettings.SetQualityLevel(i, true);
                break;
            }
        }

        RenderPipelineAsset pipeline = QualitySettings.renderPipeline != null
            ? QualitySettings.renderPipeline
            : GraphicsSettings.defaultRenderPipeline;
        if (pipeline != null)
            GraphicsSettings.defaultRenderPipeline = pipeline;

        string[] scenes = EditorBuildSettings.scenes
            .Where(scene => scene.enabled)
            .Select(scene => scene.path)
            .ToArray();
        if (scenes.Length == 0)
            scenes = new[] { "Assets/Scenes/SampleScene.unity" };

        string exePath = Path.Combine(buildDir, "TheTGame_VisibleMap.exe");
        BuildReport report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = exePath,
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.CleanBuildCache | BuildOptions.DetailedBuildReport
        });

        BuildSummary summary = report.summary;
        Debug.Log($"Build result: {summary.result}; errors: {summary.totalErrors}; warnings: {summary.totalWarnings}; path: {exePath}");

        if (summary.result != BuildResult.Succeeded)
            EditorApplication.Exit(1);
    }
}
