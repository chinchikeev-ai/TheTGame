using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public static class HectorProductionAnimationBinder
{
    const string ThirdPartyRoot = "Assets/ThirdParty/KayKitAdventurers";
    const string HectorControllerPath = "Assets/Game/Art/Characters/Animation/ChapterOne_Hector.controller";

    static readonly BindingSpec[] Bindings =
    {
        new BindingSpec("Poke", new[] { "melee attack stab", "attack stab", "stab", "thrust", "poke" }, new[] { "bow", "shoot", "cast", "spell" }),
        new BindingSpec("Block", new[] { "block", "blocking", "defend", "guard" }, new[] { "bow", "shoot", "cast", "spell" }),
        new BindingSpec("AbilityQ", new[] { "cheer", "taunt", "shout", "victory" }, new[] { "death", "defeat", "down", "hurt" }),
        new BindingSpec("AbilityE", new[] { "block", "blocking", "defend", "guard" }, new[] { "bow", "shoot", "cast", "spell" }),
        new BindingSpec("ShieldHold", new[] { "block", "blocking", "defend", "guard" }, new[] { "bow", "shoot", "cast", "spell" }),
        new BindingSpec("AbilityR", new[] { "throw", "hurl", "toss", "javelin" }, new[] { "bow", "shoot", "cast", "spell", "magic" }),
        new BindingSpec("AbilityF", new[] { "heavy attack", "attack heavy", "heavy", "power attack" }, new[] { "bow", "shoot", "death", "defeat" }),
        new BindingSpec("Downed", new[] { "laying down idle", "lying down idle", "down idle", "laying", "lying" }, new[] { "stand", "get up", "getting up", "walk", "run" })
    };

    readonly struct BindingSpec
    {
        public readonly string stateName;
        public readonly string[] preferredPhrases;
        public readonly string[] avoidPhrases;

        public BindingSpec(string stateName, string[] preferredPhrases, string[] avoidPhrases)
        {
            this.stateName = stateName;
            this.preferredPhrases = preferredPhrases;
            this.avoidPhrases = avoidPhrases;
        }
    }

    [MenuItem("The Troy Game/Characters/Bind Hector Production Animation Candidates")]
    public static void Apply()
    {
        ApplyIfAvailable(true);
    }

    public static bool ApplyIfAvailable(bool logIfMissing = false)
    {
        AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(HectorControllerPath);
        if (controller == null)
        {
            if (logIfMissing)
                Debug.LogWarning("Hector production animation binding skipped: build ChapterOne_Hector.controller first.");
            return false;
        }

        AnimationClip[] clips = LoadAllSourceClips();
        if (clips.Length == 0)
        {
            if (logIfMissing)
                Debug.LogWarning("Hector production animation binding skipped: no imported KayKit clips were found.");
            return false;
        }

        AnimatorStateMachine machine = controller.layers[0].stateMachine;
        bool changed = false;
        foreach (BindingSpec binding in Bindings)
        {
            AnimatorState state = FindState(machine, binding.stateName);
            if (state == null)
            {
                Debug.LogWarning($"Hector production animation binding: state={binding.stateName} is missing from {HectorControllerPath}.");
                continue;
            }

            AnimationClip selected = PickPreferred(clips, binding.preferredPhrases, binding.avoidPhrases);
            if (selected == null)
            {
                Debug.LogWarning($"Hector production animation binding: no specialized clip matched state={binding.stateName}; keeping existing={Name(state.motion)}. Play Mode QA remains required.");
                continue;
            }

            if (state.motion != selected)
            {
                state.motion = selected;
                EditorUtility.SetDirty(state);
                changed = true;
            }

            Debug.Log($"Hector production animation binding: state={binding.stateName} clip={selected.name} source={AssetDatabase.GetAssetPath(selected)}");
        }

        if (changed)
        {
            EditorUtility.SetDirty(controller);
            AssetDatabase.SaveAssets();
        }

        return changed;
    }

    static AnimationClip[] LoadAllSourceClips()
    {
        var result = new List<AnimationClip>();
        var seen = new HashSet<string>();
        string[] guids = AssetDatabase.FindAssets("", new[] { ThirdPartyRoot });
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (!path.EndsWith(".fbx", StringComparison.OrdinalIgnoreCase)) continue;

            foreach (UnityEngine.Object asset in AssetDatabase.LoadAllAssetsAtPath(path))
            {
                AnimationClip clip = asset as AnimationClip;
                if (clip == null || clip.name.StartsWith("__preview__", StringComparison.OrdinalIgnoreCase) || clip.length <= .05f) continue;
                string key = clip.name;
                if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(clip, out string assetGuid, out long localId))
                    key = assetGuid + ":" + localId;
                if (seen.Add(key)) result.Add(clip);
            }
        }
        return result.ToArray();
    }

    static AnimationClip PickPreferred(AnimationClip[] clips, string[] preferredPhrases, string[] avoidPhrases)
    {
        AnimationClip best = null;
        int bestScore = int.MinValue;
        foreach (AnimationClip clip in clips)
        {
            if (clip == null) continue;
            string normalized = Normalize(clip.name);
            if (ContainsAny(normalized, avoidPhrases)) continue;

            int phraseRank = FirstMatch(normalized, preferredPhrases);
            if (phraseRank < 0) continue;

            int score = 1000 - phraseRank * 100;
            string path = AssetDatabase.GetAssetPath(clip) ?? string.Empty;
            if (path.EndsWith("/Knight.fbx", StringComparison.OrdinalIgnoreCase)) score += 40;
            if (normalized == Normalize(preferredPhrases[phraseRank])) score += 20;

            if (best == null || score > bestScore || (score == bestScore && Compare(clip, best) < 0))
            {
                best = clip;
                bestScore = score;
            }
        }
        return best;
    }

    static AnimatorState FindState(AnimatorStateMachine machine, string stateName)
    {
        foreach (ChildAnimatorState child in machine.states)
            if (child.state != null && string.Equals(child.state.name, stateName, StringComparison.Ordinal)) return child.state;
        return null;
    }

    static int FirstMatch(string normalizedValue, string[] phrases)
    {
        if (phrases == null) return -1;
        for (int i = 0; i < phrases.Length; i++)
        {
            string normalizedPhrase = Normalize(phrases[i]);
            if (!string.IsNullOrEmpty(normalizedPhrase) && normalizedValue.Contains(normalizedPhrase)) return i;
        }
        return -1;
    }

    static bool ContainsAny(string normalizedValue, string[] phrases) => FirstMatch(normalizedValue, phrases) >= 0;

    static string Normalize(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return string.Empty;
        char[] buffer = value.ToLowerInvariant().ToCharArray();
        for (int i = 0; i < buffer.Length; i++)
            if (!char.IsLetterOrDigit(buffer[i])) buffer[i] = ' ';
        return string.Join(" ", new string(buffer).Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
    }

    static int Compare(AnimationClip a, AnimationClip b)
    {
        int nameCompare = string.Compare(a.name, b.name, StringComparison.OrdinalIgnoreCase);
        if (nameCompare != 0) return nameCompare;
        return string.Compare(AssetDatabase.GetAssetPath(a), AssetDatabase.GetAssetPath(b), StringComparison.Ordinal);
    }

    static string Name(Motion motion) => motion != null ? motion.name : "none";
}
