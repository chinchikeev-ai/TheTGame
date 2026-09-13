using UnityEngine;

public static class WarriorArtDirector
{
    public static void EnhanceEnemy(GameObject root, EnemyArchetype type)
    {
        if (root == null || root.transform.Find("ArchetypeArt") != null) return;
        GameObject art = new GameObject("ArchetypeArt");
        art.transform.SetParent(root.transform,false);

        Color bronze = new Color(.72f,.52f,.20f);
        Color iron = new Color(.42f,.41f,.38f);
        Color red = new Color(.55f,.06f,.04f);
        Color dark = new Color(.19f,.16f,.13f);

        switch (type)
        {
            case EnemyArchetype.Runner:
                Part(art.transform,"Runner Shoulder L",PrimitiveType.Sphere,new Vector3(-.33f,1.34f,0f),new Vector3(.18f,.11f,.18f),bronze);
                Part(art.transform,"Runner Shoulder R",PrimitiveType.Sphere,new Vector3(.33f,1.34f,0f),new Vector3(.18f,.11f,.18f),bronze);
                Part(art.transform,"Runner Crest",PrimitiveType.Cube,new Vector3(0f,1.96f,-.02f),new Vector3(.08f,.20f,.30f),red);
                break;
            case EnemyArchetype.Archer:
                Part(art.transform,"Archer Hood",PrimitiveType.Sphere,new Vector3(0f,1.76f,0f),new Vector3(.37f,.25f,.36f),dark);
                Part(art.transform,"Arrow Bundle",PrimitiveType.Cylinder,new Vector3(-.29f,1.12f,-.30f),new Vector3(.13f,.42f,.13f),new Color(.35f,.22f,.10f),Quaternion.Euler(14f,0f,-16f));
                break;
            case EnemyArchetype.HeavyHoplite:
                AddHeavyArmor(art.transform,iron,bronze,false);
                break;
            case EnemyArchetype.ShieldBearer:
                AddHeavyArmor(art.transform,new Color(.38f,.33f,.27f),bronze,true);
                Part(art.transform,"ShieldBearer Plume",PrimitiveType.Cube,new Vector3(0f,2.07f,0f),new Vector3(.12f,.28f,.44f),new Color(.38f,.05f,.035f));
                break;
            case EnemyArchetype.Boss:
                AddHeavyArmor(art.transform,bronze,new Color(.90f,.66f,.22f),true);
                Part(art.transform,"Commander Crest",PrimitiveType.Cube,new Vector3(0f,2.15f,0f),new Vector3(.18f,.38f,.56f),red);
                Part(art.transform,"Commander Chest",PrimitiveType.Cube,new Vector3(0f,1.18f,.31f),new Vector3(.34f,.40f,.08f),new Color(.76f,.52f,.16f));
                break;
        }
    }

    static void AddHeavyArmor(Transform p, Color armor, Color trim, bool wide)
    {
        float shoulder = wide ? .43f : .38f;
        Part(p,"Pauldron L",PrimitiveType.Sphere,new Vector3(-shoulder,1.35f,0f),new Vector3(.24f,.14f,.22f),armor);
        Part(p,"Pauldron R",PrimitiveType.Sphere,new Vector3(shoulder,1.35f,0f),new Vector3(.24f,.14f,.22f),armor);
        Part(p,"Breastplate",PrimitiveType.Cube,new Vector3(0f,1.13f,.27f),new Vector3(.43f,.42f,.08f),armor);
        Part(p,"Chest Trim",PrimitiveType.Cube,new Vector3(0f,1.26f,.32f),new Vector3(.36f,.05f,.03f),trim);
        Part(p,"Greave L",PrimitiveType.Cube,new Vector3(-.16f,.27f,.11f),new Vector3(.14f,.34f,.07f),armor);
        Part(p,"Greave R",PrimitiveType.Cube,new Vector3(.16f,.27f,.11f),new Vector3(.14f,.34f,.07f),armor);
    }

    static GameObject Part(Transform parent,string name,PrimitiveType type,Vector3 pos,Vector3 scale,Color color,Quaternion? rot=null)
    {
        GameObject go=GameObject.CreatePrimitive(type); go.name=name; go.transform.SetParent(parent,false); go.transform.localPosition=pos; go.transform.localScale=scale; if(rot.HasValue) go.transform.localRotation=rot.Value;
        Collider c=go.GetComponent<Collider>(); if(c!=null) Object.Destroy(c); TowerFactory.SetColor(go,color); return go;
    }
}
