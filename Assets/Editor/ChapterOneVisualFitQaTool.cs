#if UNITY_EDITOR
using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class ChapterOneVisualFitQaTool
{
    const string ManifestPath = "Assets/Game/QA/CHAPTER_I_GAMEPLAY_ACCEPTANCE.json";

    [Serializable]
    sealed class VisualFitSection
    {
        public bool ru1920x1080;
        public bool en1920x1080;
        public bool ru1366or1376x768;
        public bool en1366or1376x768;
        public string notes;
    }

    [Serializable]
    sealed class Root
    {
        public VisualFitSection visualFit;
    }

    [MenuItem("TheTroyGame/Validation/Visual Fit/1920x1080 RU")]
    public static void Set1920Ru() => ApplyPreset(1920, 1080, true);

    [MenuItem("TheTroyGame/Validation/Visual Fit/1920x1080 EN")]
    public static void Set1920En() => ApplyPreset(1920, 1080, false);

    [MenuItem("TheTroyGame/Validation/Visual Fit/1376x768 RU")]
    public static void Set1376Ru() => ApplyPreset(1376, 768, true);

    [MenuItem("TheTroyGame/Validation/Visual Fit/1376x768 EN")]
    public static void Set1376En() => ApplyPreset(1376, 768, false);

    [MenuItem("TheTroyGame/Validation/Visual Fit/Generate Chapter I QA Checklist")]
    public static void GenerateChecklist()
    {
        Root root = LoadManifest();
        VisualFitSection fit = root?.visualFit ?? new VisualFitSection();
        string directory = Path.Combine(Application.persistentDataPath, "Logs");
        Directory.CreateDirectory(directory);
        string path = Path.Combine(directory, "ChapterI_VisualFit_QA.md");

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("# Chapter I Visual-Fit QA");
        sb.AppendLine();
        sb.AppendLine("Run each preset in real Play Mode and inspect the full Chapter I flow, not only the initial frame.");
        sb.AppendLine();
        sb.AppendLine($"- [{Mark(fit.ru1920x1080)}] 1920x1080 RU");
        sb.AppendLine($"- [{Mark(fit.en1920x1080)}] 1920x1080 EN");
        sb.AppendLine($"- [{Mark(fit.ru1366or1376x768)}] 1376x768 RU (acceptance field also allows 1366x768)");
        sb.AppendLine($"- [{Mark(fit.en1366or1376x768)}] 1376x768 EN (acceptance field also allows 1366x768)");
        sb.AppendLine();
        sb.AppendLine("## Inspect on every preset");
        sb.AppendLine();
        sb.AppendLine("- [ ] top resources remain readable and do not overlap boss/wave regions");
        sb.AppendLine("- [ ] encounter/wave status remains fully visible");
        sb.AppendLine("- [ ] boss HUD does not collide with top resource panels");
        sb.AppendLine("- [ ] Hector HUD and Q/E/R/F labels remain readable");
        sb.AppendLine("- [ ] defense picker is reachable and does not cover critical battlefield space");
        sb.AppendLine("- [ ] contextual selected-defense menu remains on-screen near edge build points");
        sb.AppendLine("- [ ] tooltips remain inside the usable viewport");
        sb.AppendLine("- [ ] Menelaus warning/aura/boss presentation remains readable behind HUD");
        sb.AppendLine("- [ ] result screen fits without clipped RU/EN text");
        sb.AppendLine("- [ ] no critical text truncation, overlap, or inaccessible battlefield caused by 16:9 scaling");
        sb.AppendLine();
        sb.AppendLine("After each real visual pass, update `Assets/Game/QA/CHAPTER_I_GAMEPLAY_ACCEPTANCE.json` and add concise evidence/limitations to `visualFit.notes`. The tool never marks a preset accepted automatically.");

        File.WriteAllText(path, sb.ToString(), new UTF8Encoding(false));
        Debug.Log($"[CHAPTER I VISUAL FIT] Checklist generated: {path}");
        EditorUtility.RevealInFinder(path);
    }

    [MenuItem("TheTroyGame/Validation/Visual Fit/Check Visual-Fit Acceptance")]
    public static void CheckAcceptance()
    {
        Root root = LoadManifest();
        VisualFitSection fit = root?.visualFit;
        if (fit == null)
        {
            Debug.LogError("[CHAPTER I VISUAL FIT] BLOCKED - visualFit section missing from gameplay acceptance manifest.");
            return;
        }

        int missing = 0;
        missing += Missing(fit.ru1920x1080, "1920x1080 RU");
        missing += Missing(fit.en1920x1080, "1920x1080 EN");
        missing += Missing(fit.ru1366or1376x768, "1366/1376x768 RU");
        missing += Missing(fit.en1366or1376x768, "1366/1376x768 EN");
        if (string.IsNullOrWhiteSpace(fit.notes))
        {
            Debug.LogError("[CHAPTER I VISUAL FIT] Missing visualFit.notes evidence/limitations.");
            missing++;
        }

        if (missing == 0) Debug.Log("[CHAPTER I VISUAL FIT] PASS - all four required RU/EN 16:9 visual-fit passes are explicitly accepted.");
        else Debug.LogError($"[CHAPTER I VISUAL FIT] BLOCKED - {missing} acceptance item(s) missing.");
    }

    static void ApplyPreset(int width, int height, bool russian)
    {
        if (!EditorApplication.isPlaying)
        {
            Debug.LogError("[CHAPTER I VISUAL FIT] Enter Play Mode before applying a visual-fit preset.");
            return;
        }

        GameLanguage.SetRussian(russian);
        Screen.SetResolution(width, height, false);
        Debug.Log($"[CHAPTER I VISUAL FIT] Applied {width}x{height} {GameLanguage.Code}. Inspect the full flow before marking acceptance.");
    }

    static Root LoadManifest()
    {
        if (!File.Exists(ManifestPath)) throw new FileNotFoundException("Chapter I gameplay acceptance manifest missing.", ManifestPath);
        Root root = JsonUtility.FromJson<Root>(File.ReadAllText(ManifestPath));
        if (root == null) throw new InvalidDataException($"Could not parse {ManifestPath}");
        return root;
    }

    static int Missing(bool accepted, string label)
    {
        if (accepted) return 0;
        Debug.LogError($"[CHAPTER I VISUAL FIT] Missing acceptance: {label}");
        return 1;
    }

    static string Mark(bool accepted) => accepted ? "x" : " ";
}
#endif
