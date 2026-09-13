using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class CampaignArtCandidateBuilder
{
    const string CharacterRoot = "Assets/Game/Art/Characters/Resources/TroyProduction/Characters";
    const string GreekRoot = CharacterRoot + "/Greek";
    const string TrojanRoot = CharacterRoot + "/Trojan";
    const string HeroRoot = CharacterRoot + "/Heroes";
    const string VehicleRoot = "Assets/Game/Art/Vehicles/Resources/TroyProduction/Vehicles";
    const string SiegeRoot = "Assets/Game/Art/Vehicles/Resources/TroyProduction/Siege";
    const string PropRoot = "Assets/Game/Art/Props/Resources/TroyProduction/Props";

    [MenuItem("The Troy Game/Characters/Build Campaign Art Candidates")]
    public static void BuildAll()
    {
        EnsureBaseCharacters();
        EnsureFolder(VehicleRoot);
        EnsureFolder(SiegeRoot);
        EnsureFolder(PropRoot);

        BuildGreekCharacterVariants();
        BuildNamedHeroes();
        BuildTrojanCivilian();
        BuildChariot();
        BuildBatteringRam();
        BuildSiegeTower();
        BuildTrojanHorse();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Built campaign art candidates for Chapters II-VII: Greek variants, named heroes, chariot, siege engines, civilians and Trojan Horse.");
    }

    static void EnsureBaseCharacters()
    {
        if (AssetDatabase.LoadAssetAtPath<GameObject>(GreekRoot + "/Enemy_Infantry.prefab") == null)
            CartoonCharacterPrefabBuilder.BuildAll();
        ChapterOneCharacterAnimationBuilder.BuildAll();
    }

    static void BuildGreekCharacterVariants()
    {
        CloneCharacter(GreekRoot + "/Enemy_Infantry.prefab", GreekRoot, "Enemy_Spearman", new Color(.42f,.52f,.70f), TroyVisualRole.Infantry, "long spear", "round shield", root =>
        {
            AddCrest(root,new Color(.32f,.42f,.62f),.75f);
        });

        CloneCharacter(GreekRoot + "/Enemy_Runner.prefab", GreekRoot, "Enemy_Scout", new Color(.38f,.47f,.60f), TroyVisualRole.Runner, "short blade", "scout kit", root =>
        {
            AddSatchel(root,new Color(.25f,.15f,.08f));
        });

        CloneCharacter(GreekRoot + "/Enemy_Runner.prefab", GreekRoot, "Enemy_LightSwordsman", new Color(.48f,.52f,.62f), TroyVisualRole.Runner, "short sword", "light shield", root =>
        {
            AddCrest(root,new Color(.42f,.45f,.52f),.52f);
        });

        CloneCharacter(GreekRoot + "/Enemy_HeavyHoplite.prefab", GreekRoot, "Enemy_Hoplite", new Color(.45f,.54f,.68f), TroyVisualRole.Heavy, "spear", "hoplon", root =>
        {
            AddCrest(root,new Color(.25f,.38f,.62f),.88f);
        });

        CloneCharacter(GreekRoot + "/Enemy_HeavyHoplite.prefab", GreekRoot, "Enemy_Myrmidon", new Color(.30f,.30f,.34f), TroyVisualRole.Heavy, "spear", "black shield", root =>
        {
            AddCrest(root,new Color(.10f,.10f,.12f),1.05f);
            AddArmorPlate(root,new Color(.20f,.20f,.23f),new Color(.72f,.50f,.16f));
        });

        CloneCharacter(GreekRoot + "/Enemy_HeavyHoplite.prefab", GreekRoot, "Enemy_MyrmidonVeteran", new Color(.23f,.23f,.27f), TroyVisualRole.Commander, "veteran spear", "black-gold shield", root =>
        {
            AddCrest(root,new Color(.76f,.50f,.12f),1.18f);
            AddArmorPlate(root,new Color(.16f,.16f,.18f),new Color(.82f,.59f,.20f));
            AddCape(root,new Color(.12f,.12f,.15f));
        });

        CloneCharacter(GreekRoot + "/Enemy_Runner.prefab", GreekRoot, "Enemy_Sapper", new Color(.40f,.43f,.48f), TroyVisualRole.Runner, "tool", "demolition satchel", root =>
        {
            AddSatchel(root,new Color(.22f,.13f,.065f));
            Part(root.transform,"Sapper Tool",PrimitiveType.Cube,new Vector3(.30f,.72f,.10f),new Vector3(.08f,.34f,.08f),new Color(.30f,.20f,.10f),Quaternion.Euler(0f,0f,-35f));
        });

        CloneCharacter(GreekRoot + "/Enemy_Boss.prefab", GreekRoot, "Enemy_GreekCaptain", new Color(.34f,.46f,.66f), TroyVisualRole.Commander, "command sword", "captain shield", root =>
        {
            AddCrest(root,new Color(.25f,.40f,.72f),1.10f);
            AddCape(root,new Color(.20f,.30f,.54f));
        });

        CloneCharacter(GreekRoot + "/Enemy_HeavyHoplite.prefab", GreekRoot, "Enemy_HeroCompanion", new Color(.50f,.48f,.42f), TroyVisualRole.Heavy, "hero spear", "reinforced shield", root =>
        {
            AddCrest(root,new Color(.74f,.54f,.20f),.95f);
            AddArmorPlate(root,new Color(.52f,.41f,.24f),new Color(.78f,.56f,.20f));
        });

        CloneCharacter(GreekRoot + "/Enemy_Infantry.prefab", GreekRoot, "Enemy_RamCrew", new Color(.44f,.40f,.32f), TroyVisualRole.Infantry, "ram pole", "work shield", root =>
        {
            AddSatchel(root,new Color(.24f,.14f,.07f));
        });
    }

    static void BuildNamedHeroes()
    {
        CloneCharacter(HeroRoot + "/Hero_Achilles.prefab", HeroRoot, "Hero_Ajax", new Color(.42f,.47f,.58f), TroyVisualRole.Hero, "heavy spear", "towering shield", root =>
        {
            AddCrest(root,new Color(.32f,.38f,.52f),1.25f);
            GameObject shield=Part(root.transform,"Ajax Great Shield",PrimitiveType.Cylinder,new Vector3(-.42f,1.00f,.18f),new Vector3(.48f,.07f,.48f),new Color(.52f,.39f,.20f),Quaternion.Euler(90f,0f,8f));
            Part(shield.transform,"Ajax Shield Boss",PrimitiveType.Sphere,new Vector3(0f,.08f,0f),new Vector3(.18f,.10f,.18f),new Color(.75f,.52f,.18f));
            AddCape(root,new Color(.24f,.31f,.48f));
        });

        CloneCharacter(HeroRoot + "/Hero_Menelaus.prefab", HeroRoot, "Hero_Odysseus", new Color(.30f,.38f,.48f), TroyVisualRole.Hero, "short sword", "deception shield", root =>
        {
            DisableNamedChildren(root,"HeroCrest");
            AddCloak(root,new Color(.18f,.24f,.34f));
            AddSatchel(root,new Color(.22f,.14f,.08f));
        });
    }

    static void BuildTrojanCivilian()
    {
        CloneCharacter(TrojanRoot + "/Trojan_Infantry.prefab", TrojanRoot, "Trojan_Civilian", new Color(.70f,.50f,.34f), TroyVisualRole.Infantry, "none", "none", root =>
        {
            DisableNamedChildren(root,"Spear","Gear_","HeroCrest","LateBronzeAgeKit");
            Part(root.transform,"Civilian Tunic",PrimitiveType.Cylinder,new Vector3(0f,.86f,0f),new Vector3(.30f,.48f,.30f),new Color(.62f,.32f,.20f));
            Part(root.transform,"Civilian Belt",PrimitiveType.Cylinder,new Vector3(0f,.74f,0f),new Vector3(.31f,.05f,.31f),new Color(.25f,.13f,.06f));
        });
    }

    static void BuildChariot()
    {
        GameObject root=new GameObject("Vehicle_Chariot");
        Color wood=new Color(.34f,.18f,.075f); Color bronze=new Color(.68f,.46f,.17f); Color red=new Color(.50f,.06f,.04f);
        Part(root.transform,"Chariot Body",PrimitiveType.Cube,new Vector3(-.42f,.54f,0f),new Vector3(.88f,.64f,1.18f),wood);
        Part(root.transform,"Bronze Rail",PrimitiveType.Cube,new Vector3(-.42f,.92f,0f),new Vector3(.90f,.08f,1.22f),bronze);
        Part(root.transform,"Yoke",PrimitiveType.Cube,new Vector3(.62f,.56f,0f),new Vector3(1.28f,.10f,.10f),wood);
        for(int side=-1;side<=1;side+=2)
        {
            Part(root.transform,"Wheel",PrimitiveType.Cylinder,new Vector3(-.42f,.38f,side*.70f),new Vector3(.48f,.10f,.48f),wood,Quaternion.Euler(90f,0f,0f));
            Part(root.transform,"Wheel Hub",PrimitiveType.Cylinder,new Vector3(-.42f,.38f,side*.77f),new Vector3(.15f,.13f,.15f),bronze,Quaternion.Euler(90f,0f,0f));
            BuildHorse(root.transform,new Vector3(1.62f,.42f,side*.42f),side);
        }
        Part(root.transform,"Chariot Banner",PrimitiveType.Cube,new Vector3(-.88f,1.26f,0f),new Vector3(.08f,.54f,.42f),red);
        AddCharacterCrew(root.transform,GreekRoot+"/Enemy_GreekCaptain.prefab",new Vector3(-.48f,.58f,0f),.70f,90f);
        Save(root,VehicleRoot);
    }

    static void BuildHorse(Transform parent,Vector3 basePosition,int side)
    {
        Color horse=side<0 ? new Color(.36f,.27f,.20f) : new Color(.50f,.43f,.34f);
        Part(parent,"Horse Body",PrimitiveType.Capsule,basePosition+new Vector3(0f,.55f,0f),new Vector3(.40f,.62f,.34f),horse,Quaternion.Euler(0f,0f,90f));
        Part(parent,"Horse Neck",PrimitiveType.Capsule,basePosition+new Vector3(.42f,.91f,0f),new Vector3(.23f,.43f,.23f),horse,Quaternion.Euler(0f,0f,-28f));
        Part(parent,"Horse Head",PrimitiveType.Cube,basePosition+new Vector3(.68f,1.20f,0f),new Vector3(.36f,.28f,.26f),horse);
        for(int leg=-1;leg<=1;leg+=2)
            Part(parent,"Horse Leg",PrimitiveType.Cylinder,basePosition+new Vector3(leg*.22f,.05f,0f),new Vector3(.07f,.48f,.07f),horse);
        Part(parent,"Harness",PrimitiveType.Cube,basePosition+new Vector3(.12f,.67f,0f),new Vector3(.08f,.52f,.38f),new Color(.20f,.10f,.05f));
    }

    static void BuildBatteringRam()
    {
        GameObject root=new GameObject("Siege_BatteringRam");
        Color wood=new Color(.30f,.17f,.075f); Color darkWood=new Color(.22f,.12f,.055f); Color bronze=new Color(.62f,.42f,.17f);
        Part(root.transform,"Ram Beam",PrimitiveType.Cylinder,new Vector3(.15f,.82f,0f),new Vector3(.18f,1.85f,.18f),darkWood,Quaternion.Euler(0f,0f,90f));
        Part(root.transform,"Ram Head",PrimitiveType.Sphere,new Vector3(2.00f,.82f,0f),new Vector3(.32f,.28f,.28f),bronze);
        for(int side=-1;side<=1;side+=2)
        {
            float z=side*.72f;
            Part(root.transform,"Frame Rail",PrimitiveType.Cube,new Vector3(0f,.95f,z),new Vector3(2.45f,.12f,.12f),wood);
            for(int x=-1;x<=1;x++) Part(root.transform,"Frame Post",PrimitiveType.Cube,new Vector3(x*.85f,.78f,z),new Vector3(.12f,1.35f,.12f),wood);
            for(int x=-1;x<=1;x+=2)
                Part(root.transform,"Ram Wheel",PrimitiveType.Cylinder,new Vector3(x*.90f,.26f,z),new Vector3(.30f,.10f,.30f),wood,Quaternion.Euler(90f,0f,0f));
        }
        Part(root.transform,"Hide Roof",PrimitiveType.Cube,new Vector3(0f,1.62f,0f),new Vector3(2.25f,.14f,1.75f),new Color(.43f,.33f,.22f));
        AddCharacterCrew(root.transform,GreekRoot+"/Enemy_RamCrew.prefab",new Vector3(-.62f,.26f,-.40f),.62f,90f);
        AddCharacterCrew(root.transform,GreekRoot+"/Enemy_RamCrew.prefab",new Vector3(-.62f,.26f,.40f),.62f,90f);
        Save(root,SiegeRoot);
    }

    static void BuildSiegeTower()
    {
        GameObject root=new GameObject("Siege_SiegeTower");
        Color wood=new Color(.31f,.18f,.08f); Color dark=new Color(.22f,.12f,.055f); Color hide=new Color(.46f,.35f,.23f);
        Part(root.transform,"Tower Core",PrimitiveType.Cube,new Vector3(0f,1.75f,0f),new Vector3(1.65f,3.45f,1.65f),wood);
        Part(root.transform,"Front Hide",PrimitiveType.Cube,new Vector3(-.88f,1.76f,0f),new Vector3(.12f,3.15f,1.44f),hide);
        Part(root.transform,"Top Platform",PrimitiveType.Cube,new Vector3(0f,3.55f,0f),new Vector3(1.92f,.18f,1.92f),dark);
        for(int corner=-1;corner<=1;corner+=2)
        {
            Part(root.transform,"Tower Wheel",PrimitiveType.Cylinder,new Vector3(-.35f,.34f,corner*.92f),new Vector3(.42f,.12f,.42f),dark,Quaternion.Euler(90f,0f,0f));
            Part(root.transform,"Top Merlon",PrimitiveType.Cube,new Vector3(-.55f,3.90f,corner*.62f),new Vector3(.34f,.54f,.34f),wood);
            Part(root.transform,"Top Merlon",PrimitiveType.Cube,new Vector3(.55f,3.90f,corner*.62f),new Vector3(.34f,.54f,.34f),wood);
        }
        for(int rung=0;rung<7;rung++) Part(root.transform,"Ladder Rung",PrimitiveType.Cube,new Vector3(.88f,.55f+rung*.42f,0f),new Vector3(.08f,.06f,1.05f),dark);
        Part(root.transform,"Ladder Rail A",PrimitiveType.Cube,new Vector3(.88f,1.80f,-.50f),new Vector3(.08f,2.65f,.08f),dark);
        Part(root.transform,"Ladder Rail B",PrimitiveType.Cube,new Vector3(.88f,1.80f,.50f),new Vector3(.08f,2.65f,.08f),dark);
        AddCharacterCrew(root.transform,GreekRoot+"/Enemy_Infantry.prefab",new Vector3(-.10f,3.58f,0f),.58f,90f);
        Save(root,SiegeRoot);
    }

    static void BuildTrojanHorse()
    {
        GameObject root=new GameObject("Prop_TrojanHorse");
        Color wood=new Color(.38f,.22f,.10f); Color dark=new Color(.24f,.13f,.06f); Color bronze=new Color(.62f,.42f,.16f);
        Part(root.transform,"Horse Body",PrimitiveType.Capsule,new Vector3(0f,2.05f,0f),new Vector3(1.35f,1.65f,1.10f),wood,Quaternion.Euler(0f,0f,90f));
        Part(root.transform,"Horse Chest",PrimitiveType.Sphere,new Vector3(.92f,2.18f,0f),new Vector3(.92f,1.05f,.92f),wood);
        Part(root.transform,"Horse Neck",PrimitiveType.Capsule,new Vector3(1.30f,3.20f,0f),new Vector3(.54f,1.18f,.54f),wood,Quaternion.Euler(0f,0f,-18f));
        Part(root.transform,"Horse Head",PrimitiveType.Cube,new Vector3(1.62f,4.15f,0f),new Vector3(1.00f,.68f,.72f),wood,Quaternion.Euler(0f,0f,-8f));
        Part(root.transform,"Horse Muzzle",PrimitiveType.Cube,new Vector3(2.15f,3.98f,0f),new Vector3(.62f,.38f,.52f),dark);
        Part(root.transform,"Ear L",PrimitiveType.Cube,new Vector3(1.40f,4.64f,-.24f),new Vector3(.16f,.42f,.14f),dark,Quaternion.Euler(0f,0f,-14f));
        Part(root.transform,"Ear R",PrimitiveType.Cube,new Vector3(1.40f,4.64f,.24f),new Vector3(.16f,.42f,.14f),dark,Quaternion.Euler(0f,0f,-14f));
        for(int leg=-1;leg<=1;leg+=2)
        for(int side=-1;side<=1;side+=2)
            Part(root.transform,"Horse Leg",PrimitiveType.Cube,new Vector3(leg*.72f,.78f,side*.54f),new Vector3(.32f,1.55f,.32f),wood);
        for(int i=0;i<7;i++) Part(root.transform,"Wooden Mane",PrimitiveType.Cube,new Vector3(.92f+i*.08f,3.30f+i*.13f,0f),new Vector3(.14f,.34f,.16f),dark,Quaternion.Euler(0f,0f,-18f));
        Part(root.transform,"Hidden Hatch",PrimitiveType.Cube,new Vector3(-.10f,2.12f,-1.10f),new Vector3(.90f,.78f,.08f),dark);
        Part(root.transform,"Bronze Eye L",PrimitiveType.Sphere,new Vector3(1.98f,4.28f,-.38f),Vector3.one*.10f,bronze);
        Part(root.transform,"Bronze Eye R",PrimitiveType.Sphere,new Vector3(1.98f,4.28f,.38f),Vector3.one*.10f,bronze);
        Save(root,PropRoot);
    }

    static void CloneCharacter(string sourcePath,string outputRoot,string name,Color tint,TroyVisualRole role,string primary,string offhand,Action<GameObject> decorate)
    {
        GameObject source=AssetDatabase.LoadAssetAtPath<GameObject>(sourcePath);
        if(source==null){Debug.LogWarning($"Campaign art candidate skipped: missing source {sourcePath}"); return;}
        GameObject root=PrefabUtility.InstantiatePrefab(source) as GameObject;
        if(root==null) root=UnityEngine.Object.Instantiate(source);
        root.name=name;
        ApplyTint(root,tint,.24f);
        CharacterVisualIdentity identity=root.GetComponent<CharacterVisualIdentity>();
        if(identity==null) identity=root.AddComponent<CharacterVisualIdentity>();
        identity.characterId=name;
        identity.role=role;
        identity.primaryWeapon=primary;
        identity.offhandItem=offhand;
        decorate?.Invoke(root);
        Save(root,outputRoot);
    }

    static void AddCharacterCrew(Transform parent,string sourcePath,Vector3 position,float scale,float yaw)
    {
        GameObject source=AssetDatabase.LoadAssetAtPath<GameObject>(sourcePath); if(source==null) return;
        GameObject crew=PrefabUtility.InstantiatePrefab(source) as GameObject; if(crew==null) return;
        crew.transform.SetParent(parent,false); crew.transform.localPosition=position; crew.transform.localScale=Vector3.one*scale; crew.transform.localRotation=Quaternion.Euler(0f,yaw,0f);
        foreach(Collider collider in crew.GetComponentsInChildren<Collider>(true)) UnityEngine.Object.DestroyImmediate(collider);
    }

    static void AddCrest(GameObject root,Color color,float scale)
    {
        Part(root.transform,"Campaign Crest",PrimitiveType.Cube,new Vector3(0f,1.96f,0f),new Vector3(.12f,.34f*scale,.34f*scale),color);
    }

    static void AddArmorPlate(GameObject root,Color armor,Color trim)
    {
        Part(root.transform,"Campaign Armor Plate",PrimitiveType.Cube,new Vector3(0f,1.14f,-.02f),new Vector3(.50f,.38f,.25f),armor);
        Part(root.transform,"Campaign Armor Trim",PrimitiveType.Cube,new Vector3(0f,.90f,-.03f),new Vector3(.52f,.06f,.27f),trim);
    }

    static void AddCape(GameObject root,Color color)
    {
        Part(root.transform,"Campaign Cape",PrimitiveType.Cube,new Vector3(0f,1.10f,-.25f),new Vector3(.46f,.68f,.04f),color,Quaternion.Euler(8f,0f,0f));
    }

    static void AddCloak(GameObject root,Color color)
    {
        Part(root.transform,"Odysseus Cloak",PrimitiveType.Cube,new Vector3(-.03f,1.08f,-.27f),new Vector3(.52f,.78f,.04f),color,Quaternion.Euler(9f,0f,0f));
        Part(root.transform,"Odysseus Hood",PrimitiveType.Sphere,new Vector3(0f,1.72f,-.06f),new Vector3(.32f,.26f,.31f),color);
    }

    static void AddSatchel(GameObject root,Color color)
    {
        Part(root.transform,"Satchel",PrimitiveType.Cube,new Vector3(-.38f,.90f,-.12f),new Vector3(.24f,.28f,.16f),color,Quaternion.Euler(0f,8f,-12f));
    }

    static void DisableNamedChildren(GameObject root,params string[] tokens)
    {
        Transform[] all=root.GetComponentsInChildren<Transform>(true);
        foreach(Transform child in all)
        {
            if(child==root.transform) continue;
            foreach(string token in tokens)
            {
                if(child.name.IndexOf(token,StringComparison.OrdinalIgnoreCase)>=0){child.gameObject.SetActive(false); break;}
            }
        }
    }

    static void ApplyTint(GameObject root,Color tint,float amount)
    {
        foreach(Renderer renderer in root.GetComponentsInChildren<Renderer>(true))
        {
            Material[] materials=renderer.sharedMaterials;
            for(int i=0;i<materials.Length;i++)
            {
                Material source=materials[i]; if(source==null) continue;
                Material clone=new Material(source){name=source.name+"_CampaignVariant"};
                if(clone.HasProperty("_BaseColor")) clone.SetColor("_BaseColor",Color.Lerp(clone.GetColor("_BaseColor"),tint,amount));
                if(clone.HasProperty("_Color")) clone.SetColor("_Color",Color.Lerp(clone.GetColor("_Color"),tint,amount));
                materials[i]=clone;
            }
            renderer.sharedMaterials=materials;
        }
    }

    static GameObject Part(Transform parent,string name,PrimitiveType type,Vector3 position,Vector3 scale,Color color,Quaternion? rotation=null)
    {
        GameObject go=GameObject.CreatePrimitive(type); go.name=name; go.transform.SetParent(parent,false); go.transform.localPosition=position; go.transform.localScale=scale; if(rotation.HasValue) go.transform.localRotation=rotation.Value;
        Collider collider=go.GetComponent<Collider>(); if(collider!=null) UnityEngine.Object.DestroyImmediate(collider); TowerFactory.SetColor(go,color); return go;
    }

    static void Save(GameObject root,string outputRoot)
    {
        EnsureFolder(outputRoot); string path=Path.Combine(outputRoot,root.name+".prefab").Replace('\\','/'); PrefabUtility.SaveAsPrefabAsset(root,path); UnityEngine.Object.DestroyImmediate(root);
    }

    static void EnsureFolder(string path)
    {
        if(AssetDatabase.IsValidFolder(path)) return; string parent=Path.GetDirectoryName(path).Replace('\\','/'); string name=Path.GetFileName(path); if(!AssetDatabase.IsValidFolder(parent)) EnsureFolder(parent); AssetDatabase.CreateFolder(parent,name);
    }
}
