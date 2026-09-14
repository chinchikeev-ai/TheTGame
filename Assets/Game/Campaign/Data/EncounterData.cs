using System;
using UnityEngine;

[Serializable]
public sealed class EncounterSpawnGroup
{
    [Tooltip("Ordered enemy pattern. Difficulty scaling consumes this pattern from the front and may cycle it only if pressure exceeds the authored pattern length.")]
    public EnemyArchetype[] pattern = { EnemyArchetype.Infantry };
    [Min(1)] public int repeats = 1;
    [Tooltip("Optional nominal count used for difficulty scaling. 0 = pattern length x repeats.")]
    [Min(0)] public int baseCount;
    [Tooltip("-1 = alternate/round-robin across available routes using global spawn order.")]
    public int route = -1;
    public int routeOffset;
    [Min(0f)] public float startDelay;
    [Tooltip("<= 0 uses EncounterData.spawnInterval.")]
    public float spawnInterval;
    [Min(0.01f)] public float hpMultiplier = 1f;
    [Min(0.01f)] public float speedMultiplier = 1f;
    [Tooltip("Optional runtime behavior profile, for example 'menelaus'.")]
    public string behaviorId;

    public int PatternCount
    {
        get
        {
            int patternLength = pattern != null ? pattern.Length : 0;
            return Mathf.Max(0, patternLength * Mathf.Max(1, repeats));
        }
    }

    public int BaseCount => baseCount > 0 ? baseCount : PatternCount;
}

[CreateAssetMenu(menuName = "TheTroyGame/Encounter Data")]
public sealed class EncounterData : ScriptableObject
{
    public string encounterId = "chapter_01_01";
    public int encounterNumber = 1;

    [Header("Pacing")]
    [Min(0f)] public float preparationTime = 20f;
    [Min(1f)] public float targetDuration = 60f;
    [Min(0.01f)] public float spawnInterval = 3f;

    [Header("Pressure")]
    [Min(0.01f)] public float hpMultiplier = 1f;
    [Min(0.01f)] public float speedMultiplier = 1f;

    [Header("Authored spawn plan")]
    public EncounterSpawnGroup[] spawnGroups;

    public int BaseEnemyCount
    {
        get
        {
            int count = 0;
            if (spawnGroups == null) return 0;
            for (int i = 0; i < spawnGroups.Length; i++)
                if (spawnGroups[i] != null) count += spawnGroups[i].BaseCount;
            return count;
        }
    }

    public bool HasBoss
    {
        get
        {
            if (spawnGroups == null) return false;
            for (int g = 0; g < spawnGroups.Length; g++)
            {
                EnemyArchetype[] pattern = spawnGroups[g]?.pattern;
                if (pattern == null) continue;
                for (int i = 0; i < pattern.Length; i++)
                    if (pattern[i] == EnemyArchetype.Boss) return true;
            }
            return false;
        }
    }

    public bool Contains(EnemyArchetype archetype)
    {
        if (spawnGroups == null) return false;
        for (int g = 0; g < spawnGroups.Length; g++)
        {
            EnemyArchetype[] pattern = spawnGroups[g]?.pattern;
            if (pattern == null) continue;
            for (int i = 0; i < pattern.Length; i++)
                if (pattern[i] == archetype) return true;
        }
        return false;
    }
}
