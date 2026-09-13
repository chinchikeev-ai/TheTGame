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
            { "spear", BuildController(SpearControllerPath, clips, "spear", new[] { "spear", "thrust", "stab" }, false, false) },
            { "archer", BuildController(ArcherControllerPath, clips, "archer", new[] { "bow", "shoot", "arrow", "ranged" }, false, false) },
            { "skirmisher", BuildController(SkirmisherControllerPath, clips, "skirmisher", new[] { "dagger", "knife", "slash", "swing" }, false, false) },
            { "hector", BuildController(HectorControllerPath, clips, "hector", new[] { "spear", "thrust", "stab", "hero" }, true, false) },
            { "menelaus", BuildController(MenelausControllerPath, clips, "menelaus", new[] { "sword", "slash", "heavy", "attack" }, false, true) },
            { "ballista", BuildController(BallistaControllerPath, clips, "ballista", new[] { "interact", "work", "pull", "attack" }, false, false) },
            { "priest", BuildController(PriestControllerPath, clips, "priest", new[] { "cast", "spell", "magic", "chant" }, false, false) },
            { "firekeeper", BuildController(FireKeeperControllerPath, clips, "firekeeper", new[] { "throw", "attack", "work", "interact" }, false, false) }
        };

        int assigned = AssignControllers(controllers);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Chapter I role animation profiles built from {clips.Length} imported clips and assigned to {assigned} production candidates.");
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
        return result.ToArray();
    }

    static AnimatorController BuildController(string path, AnimationClip[] clips, string profileName, string[] roleTokens, bool hectorAbilities, bool commanderAction)
    {
        AnimationClip idle = Pick(clips, "idle", "stand");
        AnimationClip move = Pick(clips, "run", "walk", "move");
        AnimationClip attack = PickAction(clips, roleTokens, "attack", "slash", "swing", "stab", "thrust", "shoot", "fire", "melee", "interact");
        AnimationClip hit = Pick(clips, "hit", "hurt", "damage", "impact");
        AnimationClip death = Pick(clips, "death", "die", "defeat", "dead");
        AnimationClip downed = Pick(clips, "down", "knockdown", "fall");

        if (idle == null) idle = clips[0];
        if (move == null) move = idle;
        if (attack == null) attack = Pick(clips, "attack", "slash", "swing", "melee", "punch", "interact");
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

        if (string.Equals(profileName, "spear", StringComparison.Ordinal))
        {
            AnimationClip block = PickAction(clips, new[] { "shield", "block", "guard", "defend" }, "block", "guard", "defend", "idle");
            AnimationClip poke = PickAction(clips, new[] { "spear", "thrust", "stab" }, "thrust", "stab", "attack", "spear");
            AddAction(controller, machine, idleState, "Block", block != null ? block : idle, .90f);
            AddAction(controller, machine, idleState, "Poke", poke != null ? poke : attack, .86f);
        }

        if (string.Equals(profileName, "archer", StringComparison.Ordinal))
        {
            AnimationClip draw = PickAction(clips, new[] { "bow", "archer", "arrow" }, "draw", "aim", "ready", "attack");
            AnimationClip release = PickAction(clips, new[] { "bow", "archer", "arrow" }, "release", "shoot", "fire", "attack");
            AddAction(controller, machine, idleState, "Draw", draw != null ? draw : attack, .86f);
            AddAction(controller, machine, idleState, "Release", release != null ? release : attack, .82f);
        }

        if (string.Equals(profileName, "ballista", StringComparison.Ordinal))
        {
            AnimationClip reload = Pick(clips, "reload", "interact", "work", "pickup", "pull");
            AnimationClip tension = Pick(clips, "pull", "charge", "ready", "interact", "work");
            AnimationClip fire = Pick(clips, "attack", "push", "interact", "work");
            AddAction(controller, machine, idleState, "Reload", reload != null ? reload : attack, .88f);
            AddAction(controller, machine, idleState, "Tension", tension != null ? tension : attack, .88f);
            AddAction(controller, machine, idleState, "Fire", fire != null ? fire : attack, .82f);
        }

        if (string.Equals(profileName, "priest", StringComparison.Ordinal))
        {
            AnimationClip cast = Pick(clips, "cast", "spell", "magic", "attack", "interact");
            AnimationClip channel = Pick(clips, "chant", "cast", "spell", "idle", "interact");
            AddAction(controller, machine, idleState, "Cast", cast != null ? cast : attack, .88f);
            AddAction(controller, machine, idleState, "Channel", channel != null ? channel : idle, .92f);
        }

        if (string.Equals(profileName, "firekeeper", StringComparison.Ordinal))
        {
            AnimationClip stoke = Pick(clips, "interact", "work", "pickup", "attack");
            AnimationClip throwClip = Pick(clips, "throw", "attack", "swing", "interact");
            AddAction(controller, machine, idleState, "Stoke", stoke != null ? stoke : attack, .88f);
            AddAction(controller, machine, idleState, "Throw", throwClip != null ? throwClip : attack, .84f);
        }

        if (commanderAction)
        {
            AnimationClip command = Pick(clips, "taunt", "cheer", "cast", "spell", "shout", "attack");
            if (command == null) command = attack;
            AddAction(controller, machine, idleState, "Command", command, .88f);
        }

        if (hectorAbilities)
        {
            AnimationClip q = Pick(clips, "taunt", "cheer", "shout", "cast");
            AnimationClip e = PickAction(clips, new[] { "shield", "block", "defend" }, "block", "defend", "guard", "attack");
            AnimationClip r = PickAction(clips, new[] { "spear", "thrust", "stab" }, "attack", "stab", "thrust", "throw");
            AnimationClip f = Pick(clips, "heavy", "attack", "slash", "swing", "cast");
            AddAction(controller, machine, idleState, "AbilityQ", q != null ? q : attack, .90f);
            AddAction(controller, machine, idleState, "AbilityE", e != null ? e : attack, .90f);
            AddAction(controller, machine, idleState, "AbilityR", r != null ? r : attack, .88f);
            AddAction(controller, machine, idleState, "AbilityF", f != null ? f : attack, .92f);
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

    static AnimationClip PickAction(AnimationClip[] clips, string[] roleTokens, params string[] actionTokens)
    {
        if (roleTokens != null && roleTokens.Length > 0)
        {
            foreach (AnimationClip clip in clips)
            {
                string name = clip.name.ToLowerInvariant();
                if (ContainsAny(name, roleTokens) && ContainsAny(name, actionTokens)) return clip;
            }
        }
        return Pick(clips, actionTokens);
    }

    static AnimationClip Pick(AnimationClip[] clips, params string[] tokens)
    {
        foreach (string token in tokens)
        {
            string wanted = token.ToLowerInvariant();
            foreach (AnimationClip clip in clips)
                if (clip.name.ToLowerInvariant().Contains(wanted)) return clip;
        }
        return null;
    }

    static bool ContainsAny(string value, string[] tokens)
    {
        if (tokens == null) return false;
        foreach (string token in tokens)
            if (value.Contains(token.ToLowerInvariant())) return true;
        return false;
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
