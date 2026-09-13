using System.IO;
using UnityEditor;
using UnityEngine;

public static class CampaignCivilianVariantBuilder
{
    const string Root = "Assets/Game/Art/Characters/Resources/TroyProduction/Characters/Trojan";
    const string BasePath = Root + "/Trojan_Civilian.prefab";

    [MenuItem("The Troy Game/Characters/Build Trojan Civilian Variants")]
    public static void BuildAll()
    {
        if (AssetDatabase.LoadAssetAtPath<GameObject>(BasePath) == null) CampaignArtCandidateBuilder.BuildAll();
        Build("Trojan_Civilian_Worker",Vector3.one,new Color(.34f,.20f,.08f),"Basket",new Vector3(-.34f,.50f,.08f));
        Build("Trojan_Civilian_Elder",Vector3.one*.94f,new Color(.55f,.39f,.20f),"Walking Staff",new Vector3(.34f,.72f,.08f));
        Build("Trojan_Civilian_Young",Vector3.one*.78f,new Color(.52f,.12f,.08f),"Cloth Bundle",new Vector3(-.28f,.55f,.10f));
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    static void Build(string name,Vector3 scale,Color propColor,string propName,Vector3 propPosition)
    {
        GameObject source = AssetDatabase.LoadAssetAtPath<GameObject>(BasePath);
        if (source == null) return;
        GameObject root = PrefabUtility.InstantiatePrefab(source) as GameObject;
        if (root == null) root = Object.Instantiate(source);
        root.name = name;
        root.transform.localScale = scale;

        PrimitiveType type = propName == "Walking Staff" ? PrimitiveType.Cylinder : PrimitiveType.Cube;
        GameObject prop = GameObject.CreatePrimitive(type);
        prop.name = propName;
        prop.transform.SetParent(root.transform,false);
        prop.transform.localPosition = propPosition;
        prop.transform.localScale = propName == "Walking Staff" ? new Vector3(.025f,.62f,.025f) : new Vector3(.22f,.24f,.18f);
        Collider collider = prop.GetComponent<Collider>();
        if (collider != null) Object.DestroyImmediate(collider);
        TowerFactory.SetColor(prop,propColor);

        PrefabUtility.SaveAsPrefabAsset(root,Path.Combine(Root,name+".prefab").Replace('\\','/'));
        Object.DestroyImmediate(root);
    }
}
