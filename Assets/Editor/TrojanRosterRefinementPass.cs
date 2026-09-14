using System;
using UnityEditor;
using UnityEngine;

// Final presentation-only binding pass for the generated Trojan roster candidates.
// Generated props are attached to the closest authored rig bone where possible so
// they follow candidate animation instead of floating in root space.
public static class TrojanRosterRefinementPass
{
    const string CharacterRoot = "Assets/Game/Art/Characters/Resources/TroyProduction/Characters";
    const string TrojanRoot = CharacterRoot + "/Trojan/";
    const string MythicRoot = CharacterRoot + "/Mythic/";

    [MenuItem("The Troy Game/Characters/Apply Trojan Roster Refinement Pass")]
    public static void ApplyAll()
    {
        RefinePriest();
        RefineFireKeeper();
        RefineBallistaCrew();
        RefineCyclops();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // The distinct ballista prefabs intentionally share the role id used by the
        // existing animation resolver. Re-run assignment after normalizing identity.
        ChapterOneCharacterAnimationBuilder.BuildAll();
        Debug.Log("Applied Trojan roster refinement pass: support props now follow candidate rig bones where available and ballista variants share the ballista animation profile identity.");
    }

    static void RefinePriest()
    {
        Apply(TrojanRoot + "Trojan_PriestApollo.prefab", root =>
        {
            Transform head = Head(root);
            Transform torso = Torso(root);
            Transform rightHand = Hand(root, true);

            BindNamed(root, head,
                "Priest Bald Crown",
                "Priest Grey Brow",
                "Priest Long Grey Beard",
                "Priest Long Nose");

            BindNamed(root, torso,
                "Priest White Robe",
                "Priest Gold Belt",
                "Apollo Stole Left",
                "Apollo Stole Right");

            BindNamed(root, rightHand,
                "Sun Staff",
                "Sun Disc");
        });
    }

    static void RefineFireKeeper()
    {
        Apply(TrojanRoot + "Trojan_FireKeeper.prefab", root =>
        {
            Transform head = Head(root);
            Transform torso = Torso(root);
            Transform rightHand = Hand(root, true);

            BindNamed(root, head,
                "Pyromaniac Eye L",
                "Pyromaniac Eye R",
                "Pyromaniac Pupil L",
                "Pyromaniac Pupil R",
                "Mad Brow L",
                "Mad Brow R",
                "Wild Hair",
                "Wild Hair Tuft");

            BindNamed(root, torso,
                "Fire Keeper Tunic",
                "Scorched Leather Apron",
                "Bottle Satchel",
                "Belt Fire Bottle 1",
                "Belt Fire Bottle 2");

            // The lit bottle must move with the throwing hand or the Throw candidate
            // reads as a floating prop from the gameplay camera.
            BindNamed(root, rightHand, "Lit Fire Bottle");
        });
    }

    static void RefineBallistaCrew()
    {
        RefineBallistaEngineer(TrojanRoot + "Trojan_BallistaCrew.prefab");
        RefineBallistaEngineer(TrojanRoot + "Trojan_BallistaCrew_Engineer.prefab");

        Apply(TrojanRoot + "Trojan_BallistaCrew_Loader.prefab", root =>
        {
            NormalizeBallistaIdentity(root);
            BindNamed(root, Head(root), "Loader Broad Beard");
            BindNamed(root, Torso(root), "Loader Leather Strap", "Heavy Bolt Bundle");
        });
    }

    static void RefineBallistaEngineer(string path)
    {
        Apply(path, root =>
        {
            NormalizeBallistaIdentity(root);
            BindNamed(root, Head(root), "Engineer Sharp Brow");
            BindNamed(root, Torso(root), "Engineer Leather Harness");
            BindNamed(root, Hand(root, false), "Engineer Measuring Tool");
            BindNamed(root, Hand(root, true), "Winch Handle");
        });
    }

    static void RefineCyclops()
    {
        Apply(MythicRoot + "Mythic_Cyclops.prefab", root =>
        {
            BindNamed(root, Head(root),
                "Cyclops Massive Brow",
                "Cyclops Eye",
                "Cyclops Pupil");

            BindNamed(root, Torso(root),
                "Cyclops Heavy Shoulder",
                "Cyclops Loincloth");

            // Boulder follows the throwing hand so the candidate animation has a
            // readable full-body wind-up even before final authored throw clips exist.
            BindNamed(root, Hand(root, true), "Cyclops Throwing Boulder");
        });
    }

    static void NormalizeBallistaIdentity(GameObject root)
    {
        CharacterVisualIdentity identity = root.GetComponent<CharacterVisualIdentity>();
        if (identity == null) identity = root.GetComponentInChildren<CharacterVisualIdentity>(true);
        if (identity == null) return;

        identity.characterId = "Trojan_BallistaCrew";
        identity.primaryWeapon = "ballista work tools";
        EditorUtility.SetDirty(identity);
    }

    static void Apply(string path, Action<GameObject> refine)
    {
        if (AssetDatabase.LoadAssetAtPath<GameObject>(path) == null)
        {
            Debug.LogWarning("Trojan roster refinement skipped missing prefab: " + path);
            return;
        }

        GameObject root = PrefabUtility.LoadPrefabContents(path);
        try
        {
            refine(root);
            PrefabUtility.SaveAsPrefabAsset(root, path);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    static void BindNamed(GameObject root, Transform target, params string[] names)
    {
        if (target == null) return;
        foreach (string name in names)
        {
            Transform item = FindByName(root, name);
            if (item == null || item == target || item.IsChildOf(target)) continue;
            item.SetParent(target, true);
        }
    }

    static Transform Head(GameObject root)
    {
        return FindTransform(root, new[]
        {
            "mixamorig:head",
            "headtop_end",
            "head"
        });
    }

    static Transform Torso(GameObject root)
    {
        return FindTransform(root, new[]
        {
            "mixamorig:spine2",
            "upperchest",
            "spine_02",
            "spine2",
            "chest"
        });
    }

    static Transform Hand(GameObject root, bool right)
    {
        string[] hints = right
            ? new[] { "mixamorig:righthand", "righthand", "right_hand", "hand_r", "hand.r" }
            : new[] { "mixamorig:lefthand", "lefthand", "left_hand", "hand_l", "hand.l" };
        return FindTransform(root, hints);
    }

    static Transform FindTransform(GameObject root, string[] hints)
    {
        Transform[] all = root.GetComponentsInChildren<Transform>(true);
        foreach (string hint in hints)
        {
            string wanted = hint.ToLowerInvariant();
            foreach (Transform candidate in all)
            {
                if (candidate == null) continue;
                if (candidate.name.ToLowerInvariant().Contains(wanted)) return candidate;
            }
        }
        return null;
    }

    static Transform FindByName(GameObject root, string objectName)
    {
        foreach (Transform candidate in root.GetComponentsInChildren<Transform>(true))
        {
            if (candidate == null) continue;
            if (string.Equals(candidate.name, objectName, StringComparison.Ordinal)) return candidate;
        }
        return null;
    }
}
