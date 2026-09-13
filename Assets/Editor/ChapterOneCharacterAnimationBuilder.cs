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
    const string ControllerPath = AnimationRoot + "/ChapterOneCharacter.controller";

    [MenuItem("The Troy Game/Characters/Build Chapter I Animation Controller")]
    public static void BuildAll()
    {
        string sourcePath = FindAnimationSource();
        if (string.IsNullOrEmpty(sourcePath))
        {
            Debug.LogWarning("Chapter I animation build skipped: no KayKit character FBX with embedded clips was found.");
            return;
        }

        AnimationClip[] clips = LoadClips(sourcePath);
        if (clips.Length == 0)
        {
            Debug.LogWarning("Chapter I animation build skipped: source FBX contains no imported AnimationClip sub-assets.");
            return;
        }

        EnsureFolder(AnimationRoot);
        AnimatorController controller = BuildController(clips);
        if (controller == null) return;

        int assigned = AssignController(controller);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Chapter I character animation controller built from {Path.GetFileName(sourcePath)} and assigned to {assigned} production candidates.");
    }

    static string FindAnimationSource()
    {
        string[] guids = AssetDatabase.FindAssets("t:GameObject", new[] { ThirdPartyRoot });
        string fallback = null;
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (!path.EndsWith(".fbx", StringComparison.OrdinalIgnoreCase)) continue;
            if (!path.Contains("/Characters/")) continue;
            if (fallback == null) fallback = path;
            if (Path.GetFileNameWithoutExtension(path).Equals("Knight", StringComparison.OrdinalIgnoreCase))
                return path;
        }
        return fallback;
    }

    static AnimationClip[] LoadClips(string sourcePath)
    {
        var result = new List<AnimationClip>();
        foreach (UnityEngine.Object asset in AssetDatabase.LoadAllAssetsAtPath(sourcePath))
        {
            AnimationClip clip = asset as AnimationClip;
            if (clip == null || clip.name.StartsWith("__preview__", StringComparison.OrdinalIgnoreCase) || clip.length <= .05f) continue;
            result.Add(clip);
        }
        return result.ToArray();
    }

    static AnimatorController BuildController(AnimationClip[] clips)
    {
        AnimationClip idle = Pick(clips, "idle", "stand");
        AnimationClip move = Pick(clips, "walk", "run", "move");
        AnimationClip attack = Pick(clips, "attack", "slash", "melee", "swing", "punch");
        AnimationClip hit = Pick(clips, "hit", "hurt", "damage", "impact");
        AnimationClip death = Pick(clips, "death", "die", "defeat", "dead");
        AnimationClip downed = Pick(clips, "down", "knockdown", "fall");
        if (downed == null) downed = death;
        if (idle == null) idle = clips[0];
        if (move == null) move = idle;

        AssetDatabase.DeleteAsset(ControllerPath);
        AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(ControllerPath);
        if (controller == null)
        {
            Debug.LogError("Failed to create Chapter I AnimatorController.");
            return null;
        }

        AnimatorControllerLayer layer = controller.layers[0];
        AnimatorStateMachine machine = layer.stateMachine;
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

        if (attack != null)
        {
            controller.AddParameter("Attack", AnimatorControllerParameterType.Trigger);
            AddTriggeredState(machine, idleState, "Attack", attack, "Attack", .88f);
        }
        if (hit != null)
        {
            controller.AddParameter("Hit", AnimatorControllerParameterType.Trigger);
            AddTriggeredState(machine, idleState, "Hit", hit, "Hit", .82f);
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
        Debug.Log($"Chapter I animation clips selected: idle={Name(idle)}, move={Name(move)}, attack={Name(attack)}, hit={Name(hit)}, death={Name(death)}, downed={Name(downed)}");
        return controller;
    }

    static void AddTriggeredState(AnimatorStateMachine machine, AnimatorState idleState, string stateName, AnimationClip clip, string parameter, float exitTime)
    {
        AnimatorState state = machine.AddState(stateName);
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

    static int AssignController(AnimatorController controller)
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

    static AnimationClip Pick(AnimationClip[] clips, params string[] tokens)
    {
        foreach (string token in tokens)
        {
            foreach (AnimationClip clip in clips)
            {
                string name = clip.name.ToLowerInvariant();
                if (name.Contains(token.ToLowerInvariant())) return clip;
            }
        }
        return null;
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
