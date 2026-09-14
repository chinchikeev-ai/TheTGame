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
        TrojanCoreUnitVisualPass.ApplyAll();
        EnsureFolder(MythicRoot);

        BuildApolloPriest();
        BuildFireKeeper();
        BuildBallistaCrew();
        BuildCyclops();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        ChapterOneCharacterAnimationBuilder.BuildAll();
        Debug.Log("Built visually differentiated Trojan support and Cyclops candidates and refreshed Chapter I animation profiles.");
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
            DisableNamed(root, "LateBronzeAgeKit");
            DisableNamed(root, "TrojanUnitVisualIdentityPass");
            SetVisualScale(root, new Vector3(.78f, .87f, .78f));
            Tint(root, new Color(.88f,.76f,.48f), .38f);

            Color robe = new Color(.88f,.82f,.66f);
            Color gold = new Color(.92f,.69f,.20f);
            Color oldHair = new Color(.70f,.68f,.61f);
            Color skinShadow = new Color(.55f,.40f,.30f);

            Part(root.transform,"Priest White Robe",PrimitiveType.Cylinder,new Vector3(0f,.75f,0f),new Vector3(.27f,.46f,.27f),robe);
            Part(root.transform,"Priest Gold Belt",PrimitiveType.Cylinder,new Vector3(0f,.70f,0f),new Vector3(.285f,.038f,.285f),new Color(.86f,.62f,.16f));
            Part(root.transform,"Apollo Stole Left",PrimitiveType.Cube,new Vector3(-.14f,.94f,.19f),new Vector3(.055f,.43f,.026f),gold,Quaternion.Euler(4f,0f,4f));
            Part(root.transform,"Apollo Stole Right",PrimitiveType.Cube,new Vector3(.14f,.94f,.19f),new Vector3(.055f,.43f,.026f),gold,Quaternion.Euler(4f,0f,-4f));

            // Old, thin face read: bald crown, long grey beard and heavy age brows.
            Part(root.transform,"Priest Bald Crown",PrimitiveType.Sphere,new Vector3(0f,1.53f,0f),new Vector3(.20f,.12f,.20f),new Color(.75f,.58f,.43f));
            Part(root.transform,"Priest Grey Brow",PrimitiveType.Cube,new Vector3(0f,1.52f,.205f),new Vector3(.16f,.025f,.025f),oldHair);
            Part(root.transform,"Priest Long Grey Beard",PrimitiveType.Cube,new Vector3(0f,1.36f,.205f),new Vector3(.13f,.22f,.045f),oldHair,Quaternion.Euler(5f,0f,0f));
            Part(root.transform,"Priest Long Nose",PrimitiveType.Cube,new Vector3(0f,1.46f,.235f),new Vector3(.028f,.085f,.035f),skinShadow);

            Part(root.transform,"Sun Staff",PrimitiveType.Cylinder,new Vector3(.30f,.77f,.04f),new Vector3(.022f,.67f,.022f),new Color(.62f,.40f,.13f));
            Part(root.transform,"Sun Disc",PrimitiveType.Cylinder,new Vector3(.30f,1.44f,.04f),new Vector3(.15f,.030f,.15f),new Color(.95f,.72f,.18f),Quaternion.Euler(90f,0f,0f));
            SetIdentity(root,TroyFaction.Trojan,TroyVisualRole.Hero,"Trojan_PriestApollo","sun staff","ritual robes");
        });
    }

    static void BuildFireKeeper()
    {
        Clone(TrojanRoot + "/Trojan_Infantry.prefab", TrojanRoot, "Trojan_FireKeeper", root =>
        {
            StripCombatGear(root);
            DisableNamed(root, "LateBronzeAgeKit");
            DisableNamed(root, "TrojanUnitVisualIdentityPass");
            SetVisualScale(root, new Vector3(.94f, 1.00f, .92f));
            Tint(root, new Color(.55f,.16f,.06f), .32f);

            Color tunic = new Color(.50f,.09f,.035f);
            Color leather = new Color(.24f,.13f,.07f);
            Color glass = new Color(.34f,.20f,.10f);
            Color rag = new Color(.72f,.56f,.30f);
            Color flame = new Color(1f,.32f,.035f);
            Color eye = new Color(.91f,.84f,.68f);
            Color pupil = new Color(.08f,.045f,.025f);
            Color hair = new Color(.20f,.10f,.045f);

            Part(root.transform,"Fire Keeper Tunic",PrimitiveType.Cylinder,new Vector3(0f,.82f,0f),new Vector3(.30f,.48f,.30f),tunic);
            Part(root.transform,"Scorched Leather Apron",PrimitiveType.Cube,new Vector3(0f,.78f,.24f),new Vector3(.34f,.46f,.035f),leather);
            Part(root.transform,"Bottle Satchel",PrimitiveType.Cube,new Vector3(-.30f,.76f,.02f),new Vector3(.16f,.25f,.16f),leather);

            AddBottle(root.transform,"Belt Fire Bottle 1",new Vector3(-.34f,.68f,.12f),Quaternion.Euler(0f,0f,12f),glass,rag,false);
            AddBottle(root.transform,"Belt Fire Bottle 2",new Vector3(-.29f,.85f,-.04f),Quaternion.Euler(0f,0f,-18f),glass,rag,false);
            AddBottle(root.transform,"Lit Fire Bottle",new Vector3(.39f,1.10f,.10f),Quaternion.Euler(0f,0f,-24f),glass,rag,true);

            // Deliberately manic face, readable even when the body remains a shared candidate rig.
            Part(root.transform,"Pyromaniac Eye L",PrimitiveType.Sphere,new Vector3(-.075f,1.69f,.255f),new Vector3(.065f,.070f,.035f),eye);
            Part(root.transform,"Pyromaniac Eye R",PrimitiveType.Sphere,new Vector3(.075f,1.69f,.255f),new Vector3(.065f,.070f,.035f),eye);
            Part(root.transform,"Pyromaniac Pupil L",PrimitiveType.Sphere,new Vector3(-.066f,1.69f,.286f),new Vector3(.025f,.030f,.016f),pupil);
            Part(root.transform,"Pyromaniac Pupil R",PrimitiveType.Sphere,new Vector3(.084f,1.69f,.286f),new Vector3(.025f,.030f,.016f),pupil);
            Part(root.transform,"Mad Brow L",PrimitiveType.Cube,new Vector3(-.075f,1.77f,.275f),new Vector3(.085f,.020f,.018f),hair,Quaternion.Euler(0f,0f,-18f));
            Part(root.transform,"Mad Brow R",PrimitiveType.Cube,new Vector3(.075f,1.77f,.275f),new Vector3(.085f,.020f,.018f),hair,Quaternion.Euler(0f,0f,18f));
            Part(root.transform,"Wild Hair",PrimitiveType.Cube,new Vector3(0f,1.89f,-.01f),new Vector3(.24f,.16f,.20f),hair,Quaternion.Euler(0f,0f,8f));
            Part(root.transform,"Wild Hair Tuft",PrimitiveType.Cube,new Vector3(.15f,1.98f,-.01f),new Vector3(.07f,.17f,.07f),hair,Quaternion.Euler(0f,0f,-25f));

            SetIdentity(root,TroyFaction.Trojan,TroyVisualRole.Infantry,"Trojan_FireKeeper","flaming bottle","bottle satchel");
        });
    }

    static void AddBottle(Transform parent,string name,Vector3 position,Quaternion rotation,Color glass,Color rag,bool lit)
    {
        GameObject root = new GameObject(name);
        root.transform.SetParent(parent,false);
        root.transform.localPosition=position;
        root.transform.localRotation=rotation;
        Part(root.transform,"Bottle Body",PrimitiveType.Sphere,Vector3.zero,new Vector3(.12f,.17f,.12f),glass);
        Part(root.transform,"Bottle Neck",PrimitiveType.Cylinder,new Vector3(0f,.16f,0f),new Vector3(.045f,.10f,.045f),glass*.85f);
        Part(root.transform,"Bottle Rag",PrimitiveType.Cube,new Vector3(0f,.27f,0f),new Vector3(.035f,.13f,.035f),rag,Quaternion.Euler(0f,0f,12f));
        if(lit) Part(root.transform,"Bottle Flame",PrimitiveType.Sphere,new Vector3(0f,.40f,0f),new Vector3(.09f,.16f,.09f),new Color(1f,.30f,.03f));
    }

    static void BuildBallistaCrew()
    {
        BuildBallistaEngineer("Trojan_BallistaCrew"); // compatibility path used by older runtime/content.
        BuildBallistaEngineer("Trojan_BallistaCrew_Engineer");
        BuildBallistaLoader();
    }

    static void BuildBallistaEngineer(string prefabName)
    {
        Clone(TrojanRoot + "/Trojan_Infantry.prefab", TrojanRoot, prefabName, root =>
        {
            StripCombatGear(root);
            DisableNamed(root, "TrojanUnitVisualIdentityPass");
            SetVisualScale(root,new Vector3(.88f,1.02f,.88f));
            Tint(root,new Color(.64f,.38f,.17f),.22f);
            Part(root.transform,"Engineer Leather Harness",PrimitiveType.Cube,new Vector3(0f,1.05f,.22f),new Vector3(.32f,.07f,.03f),new Color(.30f,.18f,.08f),Quaternion.Euler(0f,0f,-12f));
            Part(root.transform,"Engineer Measuring Tool",PrimitiveType.Cube,new Vector3(.32f,.72f,.08f),new Vector3(.07f,.39f,.07f),new Color(.30f,.18f,.08f),Quaternion.Euler(0f,0f,-30f));
            Part(root.transform,"Winch Handle",PrimitiveType.Cylinder,new Vector3(.38f,.64f,-.08f),new Vector3(.025f,.28f,.025f),new Color(.58f,.38f,.14f),Quaternion.Euler(0f,0f,58f));
            Part(root.transform,"Engineer Sharp Brow",PrimitiveType.Cube,new Vector3(0f,1.68f,.24f),new Vector3(.17f,.025f,.02f),new Color(.19f,.10f,.055f),Quaternion.Euler(0f,0f,-5f));
            SetIdentity(root,TroyFaction.Trojan,TroyVisualRole.Infantry,prefabName,"winch tool","engineering kit");
        });
    }

    static void BuildBallistaLoader()
    {
        Clone(TrojanRoot + "/Trojan_Infantry.prefab", TrojanRoot, "Trojan_BallistaCrew_Loader", root =>
        {
            StripCombatGear(root);
            DisableNamed(root, "TrojanUnitVisualIdentityPass");
            SetVisualScale(root,new Vector3(1.10f,.98f,1.08f));
            Tint(root,new Color(.58f,.32f,.15f),.18f);
            Part(root.transform,"Loader Leather Strap",PrimitiveType.Cube,new Vector3(-.10f,1.04f,.22f),new Vector3(.08f,.48f,.03f),new Color(.28f,.16f,.075f),Quaternion.Euler(0f,0f,18f));
            Part(root.transform,"Heavy Bolt Bundle",PrimitiveType.Cylinder,new Vector3(-.36f,.78f,-.10f),new Vector3(.13f,.40f,.13f),new Color(.52f,.32f,.12f),Quaternion.Euler(8f,0f,12f));
            Part(root.transform,"Loader Broad Beard",PrimitiveType.Sphere,new Vector3(0f,1.54f,.23f),new Vector3(.19f,.14f,.06f),new Color(.20f,.11f,.06f));
            SetIdentity(root,TroyFaction.Trojan,TroyVisualRole.Infantry,"Trojan_BallistaCrew_Loader","ballista bolts","bolt bundle");
        });
    }

    static void BuildCyclops()
    {
        Clone(GreekRoot + "/Enemy_HeavyHoplite.prefab", MythicRoot, "Mythic_Cyclops", root =>
        {
            root.transform.localScale = Vector3.one * 2.25f;
            StripCombatGear(root);
            DisableNamed(root, "LateBronzeAgeKit");
            Tint(root,new Color(.52f,.42f,.34f),.34f);

            Color skinDark = new Color(.42f,.32f,.26f);
            Color eye = new Color(.90f,.73f,.30f);
            Color pupil = new Color(.10f,.08f,.06f);
            Color stone = new Color(.34f,.33f,.30f);
            Color cloth = new Color(.38f,.20f,.10f);

            Part(root.transform,"Cyclops Massive Brow",PrimitiveType.Cube,new Vector3(0f,1.76f,.235f),new Vector3(.39f,.13f,.075f),skinDark);
            Part(root.transform,"Cyclops Eye",PrimitiveType.Sphere,new Vector3(0f,1.70f,.305f),new Vector3(.13f,.13f,.075f),eye);
            Part(root.transform,"Cyclops Pupil",PrimitiveType.Sphere,new Vector3(0f,1.70f,.365f),new Vector3(.050f,.050f,.024f),pupil);
            Part(root.transform,"Cyclops Heavy Shoulder",PrimitiveType.Sphere,new Vector3(.37f,1.30f,.01f),new Vector3(.28f,.22f,.26f),skinDark);
            Part(root.transform,"Cyclops Loincloth",PrimitiveType.Cube,new Vector3(0f,.65f,.16f),new Vector3(.43f,.28f,.08f),cloth);

            GameObject boulder = new GameObject("Cyclops Throwing Boulder");
            boulder.transform.SetParent(root.transform,false);
            boulder.transform.localPosition = new Vector3(.56f,1.50f,.08f);
            boulder.transform.localRotation = Quaternion.Euler(8f,0f,-12f);
            Part(boulder.transform,"Boulder Core",PrimitiveType.Sphere,Vector3.zero,new Vector3(.48f,.44f,.46f),stone);
            Part(boulder.transform,"Boulder Lump A",PrimitiveType.Sphere,new Vector3(.24f,.08f,.04f),new Vector3(.23f,.20f,.21f),stone*.92f);
            Part(boulder.transform,"Boulder Lump B",PrimitiveType.Sphere,new Vector3(-.18f,.16f,-.08f),new Vector3(.19f,.18f,.20f),stone*1.06f);

            SetIdentity(root,TroyFaction.Trojan,TroyVisualRole.Hero,"Mythic_Cyclops","throwing boulder","none");
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

    static void DisableNamed(GameObject root,string objectName)
    {
        foreach(Transform child in root.GetComponentsInChildren<Transform>(true))
            if(child!=root.transform && string.Equals(child.name,objectName,StringComparison.Ordinal)) child.gameObject.SetActive(false);
    }

    static void SetVisualScale(GameObject root,Vector3 scale)
    {
        Transform visual=root.transform.Find("Visual");
        if(visual!=null) visual.localScale=scale;
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
