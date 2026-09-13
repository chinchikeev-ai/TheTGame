using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public static class ChapterOneCharacterAnimationBuilder
{
    const string ThirdPartyRoot = "Assets/ThirdParty/KayKitAdventurers";
    const string CandidateRoot = "Assets/Game/Art/Characters/Resources/TroyProduction/Characters";
    const string AnimationRoot = "Assets/Game/Art/Characters/Animation";

    const string GenericControllerPath = AnimationRoot + "/ChapterOneCharacter.controller";
    const string SpearControllerPath = AnimationRoot + "/ChapterOne_Spear.controller";
    const string ArcherControllerPath = AnimationRoot + "/ChapterOne_Archer.controller";
    const string SkirmisherControllerPath = AnimationRoot + "/ChapterOne_Skirmisher.controller";
    const string HectorControllerPath = AnimationRoot + "/ChapterOne_Hector.controller";
    const string MenelausControllerPath = AnimationRoot + "/ChapterOne_Menelaus.controller";
    const string BallistaControllerPath = AnimationRoot + "/ChapterOne_BallistaCrew.controller";
    const string PriestControllerPath = AnimationRoot + "/ChapterOne_PriestApollo.controller";
    const string FireKeeperControllerPath = AnimationRoot + "/ChapterOne_FireKeeper.controller";

    static readonly string[] SpearRoleTokens = { "spear", "polearm", "thrust", "stab" };
    static readonly string[] SpearActionTokens = { "thrust", "stab", "poke", "attack", "melee" };
    static readonly string[] SpearAvoidTokens = { "bow", "shoot", "ranged", "cast", "spell", "magic" };

    static readonly string[] ShieldRoleTokens = { "shield", "guard", "defend", "block" };
    static readonly string[] ShieldActionTokens = { "block", "brace", "defend", "guard", "idle" };
    static readonly string[] ShieldAvoidTokens = { "bow", "shoot", "cast", "spell", "magic" };

    static readonly string[] BowRoleTokens = { "bow", "archer", "arrow", "ranged" };
    static readonly string[] BowDrawTokens = { "draw", "aim", "ready", "charge" };
    static readonly string[] BowReleaseTokens = { "release", "shoot", "fire", "attack" };
    static readonly string[] BowDrawAvoidTokens = { "release", "shoot", "fire", "melee", "slash" };
    static readonly string[] BowReleaseAvoidTokens = { "draw", "aim", "ready", "melee", "slash" };

    [MenuItem("The Troy Game/Characters/Build Chapter I Animation Controller")]
    public static void BuildAll()
    {
        AnimationClip[] clips = LoadAllSourceClips();
        if (clips.Length == 0)
        {
            Debug.LogWarning("Chapter I animation build skipped: no imported KayKit animation clips were found.");
            return;
        }

        EnsureFolder(AnimationRoot);

        var controllers = new Dictionary<string, RuntimeAnimatorController>
        {
            { "generic", BuildController(GenericControllerPath, clips, "generic", null, false, false) },
            { "spear", BuildController(SpearControllerPath, clips, "spear", SpearRoleTokens, false, false) },
            { "archer", BuildController(ArcherControllerPath, clips, "archer", BowRoleTokens, false, false) },
            { "skirmisher", BuildController(SkirmisherControllerPath, clips, "skirmisher", new[] { "dagger", "knife", "slash", "swing" }, false, false) },
            { "hector", BuildController(HectorControllerPath, clips, "hector", SpearRoleTokens, true, false) },
            { "menelaus", BuildController(MenelausControllerPath, clips, "menelaus", new[] { "sword", "slash", "heavy", "attack" }, false, true) },
            { "ballista", BuildController(BallistaControllerPath, clips, "ballista", new[] { "interact", "work", "pull", "attack" }, false, false) },
            { "priest", BuildController(PriestControllerPath, clips, "priest", new[] { "cast", "spell", "magic", "chant" }, false, false) },
            { "firekeeper", BuildController(FireKeeperControllerPath, clips, "firekeeper", new[] { "throw", "attack", "work", "interact" }, false, false) }
        };

        int assigned = AssignControllers(controllers);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Chapter I role animation profiles built from {clips.Length} imported clips and assigned to {assigned} production candidates. Specialized bindings are candidate mappings until real Play Mode timing/pose QA passes.");
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
                string clipKey = clip.name;
                if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(clip, out string assetGuid, out long localId))
                    clipKey = assetGuid + ":" + localId;
                if (seen.Add(clipKey)) result.Add(clip);
            }
        }

        result.Sort(CompareClips);
        return result.ToArray();
    }

    static AnimatorController BuildController(string path, AnimationClip[] clips, string profileName, string[] roleTokens, bool hectorAbilities, bool commanderAction)
    {
        AnimationClip idle = PickBest(clips, "idle", "stand");
        AnimationClip move = PickBest(clips, "run", "walk", "move");
        AnimationClip attack = PickBestAction(clips, roleTokens,
            new[] { "attack", "slash", "swing", "stab", "thrust", "shoot", "fire", "melee", "interact" }, null);
        AnimationClip hit = PickBest(clips, "hit", "hurt", "damage", "impact");
        AnimationClip death = PickBest(clips, "death", "die", "defeat", "dead");
        AnimationClip downed = PickBest(clips, "down", "knockdown", "fall");

        if (idle == null) idle = clips[0];
        if (move == null) move = idle;
        if (attack == null) attack = PickBest(clips, "attack", "slash", "swing", "melee", "punch", "interact");
        if (downed == null) downed = death;

        AssetDatabase.DeleteAsset(path);
        AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(path);
        if (controller == null)
        {
            Debug.LogError($"Failed to create Chapter I AnimatorController profile={profileName} path={path}");
            return null;
        }

        AnimatorStateMachine machine = controller.layers[0].stateMachine;
        AnimatorState idleState = machine.AddState("Idle");
        idleState.motion = idle;
        machine.defaultState = idleState;

        controller.AddParameter("Speed", AnimatorControllerParameterType.Float);
        AnimatorState moveState = machine.AddState("Move");
        moveState.motion = move;
        AnimatorStateTransition toMove = idleState.AddTransition(moveState);
        toMove.hasExitTime = false;
        toMove.duration = .10f;
        toMove.AddCondition(AnimatorConditionMode.Greater, .10f, "Speed");
        AnimatorStateTransition toIdle = moveState.AddTransition(idleState);
        toIdle.hasExitTime = false;
        toIdle.duration = .10f;
        toIdle.AddCondition(AnimatorConditionMode.Less, .10f, "Speed");

        AddAction(controller, machine, idleState, "Attack", attack, .86f);
        AddAction(controller, machine, idleState, "Hit", hit, .78f);

        AnimationClip spearPoke = null;
        AnimationClip shieldBlock = null;
        bool spearProfile = string.Equals(profileName, "spear", StringComparison.Ordinal) ||
                            string.Equals(profileName, "hector", StringComparison.Ordinal);
        if (spearProfile)
        {
            spearPoke = PickBestAction(clips, SpearRoleTokens, SpearActionTokens, SpearAvoidTokens);
            shieldBlock = PickBestAction(clips, ShieldRoleTokens, ShieldActionTokens, ShieldAvoidTokens);
            AddAction(controller, machine, idleState, "Poke", spearPoke != null ? spearPoke : attack, .86f);
            AddAction(controller, machine, idleState, "Block", shieldBlock != null ? shieldBlock : idle, .90f);
            ReportBinding(profileName, "Poke", spearPoke, attack);
            ReportBinding(profileName, "Block", shieldBlock, idle);
        }

        if (string.Equals(profileName, "archer", StringComparison.Ordinal))
        {
            AnimationClip draw = PickBestAction(clips, BowRoleTokens, BowDrawTokens, BowDrawAvoidTokens);
            var excluded = new HashSet<AnimationClip>();
            if (draw != null) excluded.Add(draw);
            AnimationClip release = PickBestAction(clips, BowRoleTokens, BowReleaseTokens, BowReleaseAvoidTokens, excluded);

            AnimationClip drawBinding = draw != null ? draw : attack;
            AnimationClip releaseBinding = release != null ? release : attack;
            AddAction(controller, machine, idleState, "Draw", drawBinding, .86f);
            AddAction(controller, machine, idleState, "Release", releaseBinding, .82f);
            ReportBinding(profileName, "Draw", draw, attack);
            ReportBinding(profileName, "Release", release, attack);
            if (drawBinding != null && drawBinding == releaseBinding)
                Debug.LogWarning($"Chapter I animation profile={profileName}: Draw and Release both resolve to {Name(drawBinding)}. Dedicated bow clips are still required for production acceptance.");
        }

        if (string.Equals(profileName, "ballista", StringComparison.Ordinal))
        {
            AnimationClip reload = PickBest(clips, "reload", "interact", "work", "pickup", "pull");
            AnimationClip tension = PickBest(clips, "pull", "charge", "ready", "interact", "work");
            AnimationClip fire = PickBest(clips, "attack", "push", "interact", "work");
            AddAction(controller, machine, idleState, "Reload", reload != null ? reload : attack, .88f);
            AddAction(controller, machine, idleState, "Tension", tension != null ? tension : attack, .88f);
            AddAction(controller, machine, idleState, "Fire", fire != null ? fire : attack, .82f);
        }

        if (string.Equals(profileName, "priest", StringComparison.Ordinal))
        {
            AnimationClip cast = PickBest(clips, "cast", "spell", "magic", "attack", "interact");
            AnimationClip channel = PickBest(clips, "chant", "cast", "spell", "idle", "interact");
            AddAction(controller, machine, idleState, "Cast", cast != null ? cast : attack, .88f);
            AddAction(controller, machine, idleState, "Channel", channel != null ? channel : idle, .92f);
        }

        if (string.Equals(profileName, "firekeeper", StringComparison.Ordinal))
        {
            AnimationClip stoke = PickBest(clips, "interact", "work", "pickup", "attack");
            AnimationClip throwClip = PickBest(clips, "throw", "attack", "swing", "interact");
            AddAction(controller, machine, idleState, "Stoke", stoke != null ? stoke : attack, .88f);
            AddAction(controller, machine, idleState, "Throw", throwClip != null ? throwClip : attack, .84f);
        }

        if (commanderAction)
        {
            AnimationClip command = PickBestAction(clips,
                new[] { "command", "taunt", "cheer", "shout", "hero" },
                new[] { "command", "taunt", "cheer", "shout", "cast", "attack" },
                new[] { "death", "die", "down", "hurt" });
            AddAction(controller, machine, idleState, "Command", command != null ? command : attack, .88f);
            ReportBinding(profileName, "Command", command, attack);
        }

        if (hectorAbilities)
        {
            AnimationClip q = PickBest(clips, "taunt", "cheer", "shout", "cast");
            AnimationClip e = shieldBlock != null ? shieldBlock :
                PickBestAction(clips, ShieldRoleTokens, ShieldActionTokens, ShieldAvoidTokens);
            AnimationClip r = spearPoke != null ? spearPoke :
                PickBestAction(clips, SpearRoleTokens, new[] { "throw", "thrust", "stab", "attack" }, SpearAvoidTokens);
            AnimationClip f = PickBestAction(clips,
                new[] { "heavy", "hero", "attack" },
                new[] { "heavy", "attack", "slash", "swing", "cast" },
                new[] { "bow", "shoot", "death", "die" });
            AddAction(controller, machine, idleState, "AbilityQ", q != null ? q : attack, .90f);
            AddAction(controller, machine, idleState, "AbilityE", e != null ? e : attack, .90f);
            AddAction(controller, machine, idleState, "AbilityR", r != null ? r : attack, .88f);
            AddAction(controller, machine, idleState, "AbilityF", f != null ? f : attack, .92f);
            ReportBinding(profileName, "AbilityE", e, attack);
            ReportBinding(profileName, "AbilityR", r, attack);
        }

        if (death != null)
        {
            controller.AddParameter("Die", AnimatorControllerParameterType.Trigger);
            AnimatorState deathState = machine.AddState("Death");
            deathState.motion = death;
            AnimatorStateTransition transition = machine.AddAnyStateTransition(deathState);
            transition.hasExitTime = false;
            transition.duration = .06f;
            transition.AddCondition(AnimatorConditionMode.If, 0f, "Die");
        }

        if (downed != null)
        {
            controller.AddParameter("IsDowned", AnimatorControllerParameterType.Bool);
            AnimatorState downState = machine.AddState("Downed");
            downState.motion = downed;
            downState.speed = .70f;
            AnimatorStateTransition enter = machine.AddAnyStateTransition(downState);
            enter.hasExitTime = false;
            enter.duration = .08f;
            enter.AddCondition(AnimatorConditionMode.If, 0f, "IsDowned");
            AnimatorStateTransition recover = downState.AddTransition(idleState);
            recover.hasExitTime = false;
            recover.duration = .18f;
            recover.AddCondition(AnimatorConditionMode.IfNot, 0f, "IsDowned");
        }

        EditorUtility.SetDirty(controller);
        Debug.Log($"Chapter I animation profile={profileName}: idle={Name(idle)}, move={Name(move)}, attack={Name(attack)}, hit={Name(hit)}, death={Name(death)}, downed={Name(downed)}");
        return controller;
    }

    static void AddAction(AnimatorController controller, AnimatorStateMachine machine, AnimatorState idleState, string parameter, AnimationClip clip, float exitTime)
    {
        if (clip == null) return;
        controller.AddParameter(parameter, AnimatorControllerParameterType.Trigger);
        AnimatorState state = machine.AddState(parameter);
        state.motion = clip;
        AnimatorStateTransition enter = machine.AddAnyStateTransition(state);
        enter.hasExitTime = false;
        enter.duration = .05f;
        enter.AddCondition(AnimatorConditionMode.If, 0f, parameter);

        AnimatorStateTransition exit = state.AddTransition(idleState);
        exit.hasExitTime = true;
        exit.exitTime = exitTime;
        exit.duration = .08f;
    }

    static int AssignControllers(Dictionary<string, RuntimeAnimatorController> controllers)
    {
        if (!AssetDatabase.IsValidFolder(CandidateRoot)) return 0;
        int assigned = 0;
        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { CandidateRoot });
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject root = PrefabUtility.LoadPrefabContents(path);
            try
            {
                Animator animator = root.GetComponentInChildren<Animator>(true);
                if (animator == null) continue;

                CharacterVisualIdentity identity = root.GetComponent<CharacterVisualIdentity>();
                if (identity == null) identity = root.GetComponentInChildren<CharacterVisualIdentity>(true);
                string profile = ResolveProfile(identity);
                RuntimeAnimatorController controller;
                if (!controllers.TryGetValue(profile, out controller) || controller == null)
                    controller = controllers["generic"];
                if (controller == null) continue;

                animator.runtimeAnimatorController = controller;
                animator.applyRootMotion = false;
                PrefabUtility.SaveAsPrefabAsset(root, path);
                assigned++;
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }
        return assigned;
    }

    static string ResolveProfile(CharacterVisualIdentity identity)
    {
        if (identity == null) return "generic";
        string id = identity.characterId ?? string.Empty;
        if (id.Equals("Hector", StringComparison.OrdinalIgnoreCase)) return "hector";
        if (id.Equals("Menelaus", StringComparison.OrdinalIgnoreCase) || identity.role == TroyVisualRole.Commander) return "menelaus";
        if (id.Equals("Trojan_BallistaCrew", StringComparison.OrdinalIgnoreCase)) return "ballista";
        if (id.Equals("Trojan_PriestApollo", StringComparison.OrdinalIgnoreCase)) return "priest";
        if (id.Equals("Trojan_FireKeeper", StringComparison.OrdinalIgnoreCase)) return "firekeeper";
        if (identity.role == TroyVisualRole.Archer) return "archer";
        if (identity.role == TroyVisualRole.Runner) return "skirmisher";
        if (identity.role == TroyVisualRole.Infantry || identity.role == TroyVisualRole.Heavy || identity.role == TroyVisualRole.ShieldBearer) return "spear";
        return "generic";
    }

    static AnimationClip PickBestAction(AnimationClip[] clips, string[] roleTokens, string[] actionTokens, string[] avoidTokens, HashSet<AnimationClip> excluded = null)
    {
        AnimationClip roleMatch = PickBestScored(clips, roleTokens, actionTokens, avoidTokens, excluded, true);
        if (roleMatch != null) return roleMatch;
        return PickBestScored(clips, roleTokens, actionTokens, avoidTokens, excluded, false);
    }

    static AnimationClip PickBest(AnimationClip[] clips, params string[] actionTokens)
    {
        return PickBestScored(clips, null, actionTokens, null, null, false);
    }

    static AnimationClip PickBestScored(AnimationClip[] clips, string[] roleTokens, string[] actionTokens, string[] avoidTokens,
        HashSet<AnimationClip> excluded, bool requireRoleMatch)
    {
        AnimationClip best = null;
        int bestScore = int.MinValue;
        foreach (AnimationClip clip in clips)
        {
            if (clip == null || (excluded != null && excluded.Contains(clip))) continue;
            string name = clip.name.ToLowerInvariant();
            if (ContainsAny(name, avoidTokens)) continue;

            int actionMatches = CountMatches(name, actionTokens);
            if (actionTokens != null && actionTokens.Length > 0 && actionMatches == 0) continue;
            int roleMatches = CountMatches(name, roleTokens);
            if (requireRoleMatch && roleTokens != null && roleTokens.Length > 0 && roleMatches == 0) continue;

            int score = actionMatches * 30 + roleMatches * 12;
            if (StartsWithAny(name, actionTokens)) score += 8;
            if (StartsWithAny(name, roleTokens)) score += 4;

            if (best == null || score > bestScore || (score == bestScore && CompareClips(clip, best) < 0))
            {
                best = clip;
                bestScore = score;
            }
        }
        return best;
    }

    static int CountMatches(string value, string[] tokens)
    {
        if (tokens == null) return 0;
        int count = 0;
        foreach (string token in tokens)
            if (!string.IsNullOrEmpty(token) && value.Contains(token.ToLowerInvariant())) count++;
        return count;
    }

    static bool ContainsAny(string value, string[] tokens) => CountMatches(value, tokens) > 0;

    static bool StartsWithAny(string value, string[] tokens)
    {
        if (tokens == null) return false;
        foreach (string token in tokens)
            if (!string.IsNullOrEmpty(token) && value.StartsWith(token.ToLowerInvariant(), StringComparison.Ordinal)) return true;
        return false;
    }

    static int CompareClips(AnimationClip a, AnimationClip b)
    {
        if (ReferenceEquals(a, b)) return 0;
        if (a == null) return 1;
        if (b == null) return -1;
        int nameCompare = string.Compare(a.name, b.name, StringComparison.OrdinalIgnoreCase);
        if (nameCompare != 0) return nameCompare;
        string pathA = AssetDatabase.GetAssetPath(a) ?? string.Empty;
        string pathB = AssetDatabase.GetAssetPath(b) ?? string.Empty;
        return string.Compare(pathA, pathB, StringComparison.Ordinal);
    }

    static void ReportBinding(string profileName, string actionName, AnimationClip specialized, AnimationClip fallback)
    {
        AnimationClip chosen = specialized != null ? specialized : fallback;
        string source = chosen != null ? AssetDatabase.GetAssetPath(chosen) : "none";
        if (specialized == null)
        {
            Debug.LogWarning($"Chapter I animation binding profile={profileName} action={actionName}: no specialized source clip matched; fallback={Name(fallback)} source={source}. This remains a placeholder binding pending authored animation QA.");
            return;
        }

        Debug.Log($"Chapter I animation binding profile={profileName} action={actionName}: clip={Name(specialized)} source={source}");
    }

    static string Name(AnimationClip clip) => clip != null ? clip.name : "none";

    static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path)) return;
        string parent = Path.GetDirectoryName(path).Replace('\\', '/');
        string name = Path.GetFileName(path);
        if (!AssetDatabase.IsValidFolder(parent)) EnsureFolder(parent);
        AssetDatabase.CreateFolder(parent, name);
    }
}
