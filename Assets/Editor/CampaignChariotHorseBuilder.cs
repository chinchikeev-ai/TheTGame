using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public static class CampaignChariotHorseBuilder
{
    const string ChariotPath = "Assets/Game/Art/Vehicles/Resources/TroyProduction/Vehicles/Vehicle_Chariot.prefab";
    const string HorsePath = "Assets/Game/Art/Vehicles/Source/Quaternius/Horse.fbx";
    const string ControllerPath = "Assets/Game/Art/Vehicles/Animation/ChariotHorse.controller";

    [MenuItem("The Troy Game/Characters/Upgrade Chariot With Animated Horses")]
    public static void Build()
    {
        CampaignHorseSourceInstaller.Install();
        if (AssetDatabase.LoadAssetAtPath<GameObject>(ChariotPath) == null) CampaignArtCandidateBuilder.BuildAll();
        GameObject horseSource = AssetDatabase.LoadAssetAtPath<GameObject>(HorsePath);
        GameObject chariot = PrefabUtility.LoadPrefabContents(ChariotPath);
        if (horseSource == null || chariot == null) throw new InvalidOperationException("Horse or chariot source is missing.");
        ClearOldHorseParts(chariot.transform);
        RuntimeAnimatorController controller = BuildController();
        AddHorse(chariot.transform,horseSource,controller,new Vector3(1.62f,.02f,-.42f),"Horse_Animated_Left");
        AddHorse(chariot.transform,horseSource,controller,new Vector3(1.62f,.02f,.42f),"Horse_Animated_Right");
        PrefabUtility.SaveAsPrefabAsset(chariot,ChariotPath);
        PrefabUtility.UnloadPrefabContents(chariot);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    static void AddHorse(Transform parent,GameObject source,RuntimeAnimatorController controller,Vector3 p,string name)
    {
        GameObject horse = PrefabUtility.InstantiatePrefab(source) as GameObject;
        if (horse == null) horse = UnityEngine.Object.Instantiate(source);
        horse.name = name;
        horse.transform.SetParent(parent,false);
        horse.transform.localPosition = p;
        horse.transform.localRotation = Quaternion.Euler(0f,90f,0f);
        horse.transform.localScale = Vector3.one * .82f;
        foreach (Collider collider in horse.GetComponentsInChildren<Collider>(true)) UnityEngine.Object.DestroyImmediate(collider);
        Animator animator = horse.GetComponent<Animator>();
        if (animator == null) animator = horse.AddComponent<Animator>();
        animator.runtimeAnimatorController = controller;
        animator.applyRootMotion = false;
    }

    static void ClearOldHorseParts(Transform root)
    {
        Transform[] all = root.GetComponentsInChildren<Transform>(true);
        for (int i=all.Length-1;i>=0;i--)
        {
            if (all[i] == root) continue;
            string n = all[i].name;
            if (n.StartsWith("Horse ",StringComparison.Ordinal) || n == "Harness" || n.StartsWith("Horse_Animated_",StringComparison.Ordinal)) UnityEngine.Object.DestroyImmediate(all[i].gameObject);
        }
    }

    static RuntimeAnimatorController BuildController()
    {
        EnsureFolder(Path.GetDirectoryName(ControllerPath).Replace('\\','/'));
        AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(ControllerPath);
        if (controller == null) controller = AnimatorController.CreateAnimatorControllerAtPath(ControllerPath);
        AnimationClip[] clips = AssetDatabase.LoadAllAssetsAtPath(HorsePath).OfType<AnimationClip>().Where(c => !c.name.StartsWith("__preview__",StringComparison.OrdinalIgnoreCase)).ToArray();
        AnimationClip clip = Pick(clips,"run","gallop","trot","walk","idle");
        if (clip != null)
        {
            AnimatorStateMachine machine = controller.layers[0].stateMachine;
            foreach (ChildAnimatorState child in machine.states) machine.RemoveState(child.state);
            AnimatorState state = machine.AddState("ChariotLocomotion");
            state.motion = clip;
            machine.defaultState = state;
        }
        EditorUtility.SetDirty(controller);
        return controller;
    }

    static AnimationClip Pick(AnimationClip[] clips,params string[] tokens)
    {
        foreach (string token in tokens)
        {
            AnimationClip clip = clips.FirstOrDefault(c => c.name.IndexOf(token,StringComparison.OrdinalIgnoreCase) >= 0);
            if (clip != null) return clip;
        }
        return clips.FirstOrDefault();
    }

    static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path)) return;
        string parent = Path.GetDirectoryName(path).Replace('\\','/');
        if (!AssetDatabase.IsValidFolder(parent)) EnsureFolder(parent);
        AssetDatabase.CreateFolder(parent,Path.GetFileName(path));
    }
}
