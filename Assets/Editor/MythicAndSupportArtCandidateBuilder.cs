using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class MythicAndSupportArtCandidateBuilder
{
    const string CharacterRoot = "Assets/Game/Art/Characters/Resources/TroyProduction/Characters";
    const string GreekRoot = CharacterRoot + "/Greek";
    const string TrojanRoot = CharacterRoot + "/Trojan";
    const string MythicRoot = CharacterRoot + "/Mythic";

    [MenuItem("The Troy Game/Characters/Build Mythic and Trojan Support Candidates")]
    public static void BuildAll()
    {
        EnsureBaseCharacters();
        EnsureFolder(MythicRoot);

        BuildApolloPriest();
        BuildFireKeeper();
        BuildBallistaCrew();
        BuildCyclops();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Built Trojan support and Cyclops art candidates.");
    }

    static void EnsureBaseCharacters()
    {
        if (AssetDatabase.LoadAssetAtPath<GameObject>(TrojanRoot + "/Trojan_Infantry.prefab") == null)
            CartoonCharacterPrefabBuilder.BuildAll();
    }

    static void BuildApolloPriest()
    {
        Clone(TrojanRoot + "/Trojan_Archer.prefab", TrojanRoot, "Trojan_PriestApollo", root =>
        {
            StripCombatGear(root);
            Tint(root, new Color(.88f,.76f,.48f), .38f);
            Part(root.transform,"Priest White Robe",PrimitiveType.Cylinder,new Vector3(0f,.86f,0f),new Vector3(.31f,.52f,.31f),new Color(.88f,.82f,.66f));
            Part(root.transform,"Priest Gold Belt",PrimitiveType.Cylinder,new Vector3(0f,.72f,0f),new Vector3(.32f,.045f,.32f),new Color(.86f,.62f,.16f));
            Part(root.transform,"Sun Staff",PrimitiveType.Cylinder,new Vector3(.32f,.88f,.04f),new Vector3(.025f,.74f,.025f),new Color(.62f,.40f,.13f));
            Part(root.transform,"Sun Disc",PrimitiveType.Cylinder,new Vector3(.32f,1.62f,.04f),new Vector3(.18f,.035f,.18f),new Color(.95f,.72f,.18f),Quaternion.Euler(90f,0f,0f));
            SetIdentity(root,TroyFaction.Trojan,TroyVisualRole.Hero,"Trojan_PriestApollo","sun staff","ritual robes");
        });
    }

    static void BuildFireKeeper()
    {
        Clone(TrojanRoot + "/Trojan_Infantry.prefab", TrojanRoot, "Trojan_FireKeeper", root =>
        {
            StripCombatGear(root);
            Tint(root, new Color(.55f,.16f,.06f), .32f);
            Part(root.transform,"Fire Keeper Tunic",PrimitiveType.Cylinder,new Vector3(0f,.82f,0f),new Vector3(.30f,.48f,.30f),new Color(.50f,.09f,.035f));
            Part(root.transform,"Leather Apron",PrimitiveType.Cube,new Vector3(0f,.78f,-.24f),new Vector3(.34f,.46f,.035f),new Color(.24f,.13f,.07f));
            Part(root.transform,"Pitch Pot",PrimitiveType.Cylinder,new Vector3(-.38f,.52f,.12f),new Vector3(.16f,.22f,.16f),new Color(.17f,.10f,.055f));
            Part(root.transform,"Torch",PrimitiveType.Cylinder,new Vector3(.34f,.74f,.08f),new Vector3(.025f,.52f,.025f),new Color(.34f,.19f,.08f));
            Part(root.transform,"Torch Flame",PrimitiveType.Sphere,new Vector3(.34f,1.28f,.08f),new Vector3(.11f,.20f,.11f),new Color(1f,.32f,.035f));
            SetIdentity(root,TroyFaction.Trojan,TroyVisualRole.Infantry,"Trojan_FireKeeper","torch","pitch pot");
        });
    }

    static void BuildBallistaCrew()
    {
        Clone(TrojanRoot + "/Trojan_Infantry.prefab", TrojanRoot, "Trojan_BallistaCrew", root =>
        {
            StripCombatGear(root);
            Tint(root,new Color(.64f,.38f,.17f),.22f);
            Part(root.transform,"Crew Tool",PrimitiveType.Cube,new Vector3(.32f,.70f,.08f),new Vector3(.08f,.42f,.08f),new Color(.30f,.18f,.08f),Quaternion.Euler(0f,0f,-30f));
            Part(root.transform,"Bolt Bundle",PrimitiveType.Cylinder,new Vector3(-.34f,.70f,-.12f),new Vector3(.10f,.34f,.10f),new Color(.52f,.32f,.12f),Quaternion.Euler(8f,0f,12f));
            SetIdentity(root,TroyFaction.Trojan,TroyVisualRole.Infantry,"Trojan_BallistaCrew","winch tool","bolt bundle");
        });
    }

    static void BuildCyclops()
    {
        Clone(GreekRoot + "/Enemy_HeavyHoplite.prefab", MythicRoot, "Mythic_Cyclops", root =>
        {
            root.transform.localScale = Vector3.one * 2.25f;
            StripCombatGear(root);
            Tint(root,new Color(.52f,.42f,.34f),.34f);
            Part(root.transform,"Cyclops Brow",PrimitiveType.Cube,new Vector3(0f,1.75f,-.23f),new Vector3(.36f,.12f,.08f),new Color(.42f,.32f,.26f));
            Part(root.transform,"Cyclops Eye",PrimitiveType.Sphere,new Vector3(0f,1.72f,-.31f),new Vector3(.12f,.12f,.08f),new Color(.90f,.73f,.30f));
            Part(root.transform,"Cyclops Pupil",PrimitiveType.Sphere,new Vector3(0f,1.72f,-.38f),new Vector3(.045f,.045f,.025f),new Color(.10f,.08f,.06f));

            GameObject club = new GameObject("Cyclops Club");
            club.transform.SetParent(root.transform,false);
            club.transform.localPosition = new Vector3(.52f,.88f,.05f);
            club.transform.localRotation = Quaternion.Euler(0f,0f,-22f);
            Part(club.transform,"Club Shaft",PrimitiveType.Cylinder,Vector3.zero,new Vector3(.10f,.78f,.10f),new Color(.29f,.16f,.07f));
            Part(club.transform,"Club Head",PrimitiveType.Sphere,new Vector3(0f,.82f,0f),new Vector3(.30f,.38f,.30f),new Color(.25f,.14f,.06f));
            SetIdentity(root,TroyFaction.Trojan,TroyVisualRole.Hero,"Mythic_Cyclops","giant club","none");
        });
    }

    static void Clone(string sourcePath,string outputRoot,string name,Action<GameObject> decorate)
    {
        GameObject source = AssetDatabase.LoadAssetAtPath<GameObject>(sourcePath);
        if (source == null)
        {
            Debug.LogWarning($"Art candidate skipped: missing source {sourcePath}");
            return;
        }
        GameObject root = PrefabUtility.InstantiatePrefab(source) as GameObject;
        if (root == null) root = UnityEngine.Object.Instantiate(source);
        root.name = name;
        decorate(root);
        Save(root,outputRoot);
    }

    static void StripCombatGear(GameObject root)
    {
        foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
        {
            if (child == root.transform) continue;
            string n = child.name;
            if (n.IndexOf("Spear",StringComparison.OrdinalIgnoreCase)>=0 ||
                n.IndexOf("Gear_",StringComparison.OrdinalIgnoreCase)>=0 ||
                n.IndexOf("HeroCrest",StringComparison.OrdinalIgnoreCase)>=0 ||
                n.IndexOf("Bow",StringComparison.OrdinalIgnoreCase)>=0)
                child.gameObject.SetActive(false);
        }
    }

    static void SetIdentity(GameObject root,TroyFaction faction,TroyVisualRole role,string id,string primary,string offhand)
    {
        CharacterVisualIdentity identity = root.GetComponent<CharacterVisualIdentity>();
        if(identity==null) identity=root.AddComponent<CharacterVisualIdentity>();
        identity.faction=faction;
        identity.role=role;
        identity.characterId=id;
        identity.primaryWeapon=primary;
        identity.offhandItem=offhand;
    }

    static void Tint(GameObject root,Color tint,float amount)
    {
        foreach(Renderer renderer in root.GetComponentsInChildren<Renderer>(true))
        {
            Material[] materials=renderer.sharedMaterials;
            for(int i=0;i<materials.Length;i++)
            {
                Material source=materials[i]; if(source==null) continue;
                Material clone=new Material(source){name=source.name+"_SupportVariant"};
                if(clone.HasProperty("_BaseColor")) clone.SetColor("_BaseColor",Color.Lerp(clone.GetColor("_BaseColor"),tint,amount));
                if(clone.HasProperty("_Color")) clone.SetColor("_Color",Color.Lerp(clone.GetColor("_Color"),tint,amount));
                materials[i]=clone;
            }
            renderer.sharedMaterials=materials;
        }
    }

    static GameObject Part(Transform parent,string name,PrimitiveType type,Vector3 position,Vector3 scale,Color color,Quaternion? rotation=null)
    {
        GameObject go=GameObject.CreatePrimitive(type);
        go.name=name;
        go.transform.SetParent(parent,false);
        go.transform.localPosition=position;
        go.transform.localScale=scale;
        if(rotation.HasValue) go.transform.localRotation=rotation.Value;
        Collider collider=go.GetComponent<Collider>(); if(collider!=null) UnityEngine.Object.DestroyImmediate(collider);
        TowerFactory.SetColor(go,color);
        return go;
    }

    static void Save(GameObject root,string outputRoot)
    {
        EnsureFolder(outputRoot);
        string path=Path.Combine(outputRoot,root.name+".prefab").Replace('\\','/');
        PrefabUtility.SaveAsPrefabAsset(root,path);
        UnityEngine.Object.DestroyImmediate(root);
    }

    static void EnsureFolder(string path)
    {
        if(AssetDatabase.IsValidFolder(path)) return;
        string parent=Path.GetDirectoryName(path).Replace('\\','/');
        string name=Path.GetFileName(path);
        if(!AssetDatabase.IsValidFolder(parent)) EnsureFolder(parent);
        AssetDatabase.CreateFolder(parent,name);
    }
}
