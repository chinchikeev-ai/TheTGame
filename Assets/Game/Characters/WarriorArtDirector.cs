using UnityEngine;

public static class WarriorArtDirector
{
    public static void EnhanceEnemy(GameObject root, EnemyArchetype type)
    {
        if (root == null || root.transform.Find("ArchetypeArt") != null) return;
        GameObject art = new GameObject("ArchetypeArt");
        art.transform.SetParent(root.transform,false);

        Color bronze = new Color(.72f,.52f,.20f);
        Color brightBronze = new Color(.86f,.64f,.24f);
        Color iron = new Color(.42f,.41f,.38f);
        Color greekBlue = new Color(.20f,.34f,.58f);
        Color paleBlue = new Color(.46f,.61f,.78f);
        Color paleCloth = new Color(.76f,.74f,.66f);
        Color dark = new Color(.19f,.16f,.13f);
        Color wood = new Color(.34f,.21f,.10f);

        switch (type)
        {
            case EnemyArchetype.Infantry:
                AddInfantryRead(art.transform,bronze,greekBlue,wood);
                break;

            case EnemyArchetype.Runner:
                Part(art.transform,"Role_Runner_ShoulderL",PrimitiveType.Sphere,new Vector3(-.31f,1.32f,0f),new Vector3(.15f,.09f,.15f),bronze);
                Part(art.transform,"Role_Runner_ShoulderR",PrimitiveType.Sphere,new Vector3(.31f,1.32f,0f),new Vector3(.15f,.09f,.15f),bronze);
                Part(art.transform,"Role_Runner_ShortCrest",PrimitiveType.Cube,new Vector3(0f,1.94f,-.02f),new Vector3(.07f,.18f,.26f),greekBlue);
                Part(art.transform,"Role_Runner_BlueSash",PrimitiveType.Cube,new Vector3(.09f,1.02f,-.20f),new Vector3(.13f,.46f,.030f),paleBlue,Quaternion.Euler(4f,0f,-20f));
                Part(art.transform,"Role_Runner_LightKnife",PrimitiveType.Cube,new Vector3(.43f,.91f,.10f),new Vector3(.055f,.46f,.028f),iron,Quaternion.Euler(0f,0f,-22f));
                break;

            case EnemyArchetype.Archer:
                Part(art.transform,"Role_Archer_Hood",PrimitiveType.Sphere,new Vector3(0f,1.76f,0f),new Vector3(.36f,.24f,.35f),dark);
                Part(art.transform,"Role_Archer_BlueScarf",PrimitiveType.Cube,new Vector3(0f,1.46f,-.16f),new Vector3(.38f,.12f,.035f),greekBlue);
                Part(art.transform,"Role_Archer_ArrowBundle",PrimitiveType.Cylinder,new Vector3(-.31f,1.12f,-.31f),new Vector3(.14f,.48f,.14f),wood,Quaternion.Euler(14f,0f,-16f));
                AddLargeBow(art.transform,new Vector3(.48f,1.10f,.08f),wood,paleCloth);
                break;

            case EnemyArchetype.HeavyHoplite:
                AddHeavyArmor(art.transform,iron,bronze,false);
                Part(art.transform,"Role_HeavyHoplite_BlueCrest",PrimitiveType.Cube,new Vector3(0f,2.02f,0f),new Vector3(.12f,.28f,.46f),greekBlue);
                Part(art.transform,"Role_HeavyHoplite_HeavyShield",PrimitiveType.Cylinder,new Vector3(-.54f,1.03f,.11f),new Vector3(.52f,.070f,.52f),iron,Quaternion.Euler(90f,0f,0f));
                Part(art.transform,"Role_HeavyHoplite_ShieldBoss",PrimitiveType.Sphere,new Vector3(-.54f,1.03f,.18f),new Vector3(.18f,.10f,.18f),brightBronze);
                break;

            case EnemyArchetype.ShieldBearer:
                AddHeavyArmor(art.transform,new Color(.38f,.33f,.27f),bronze,true);
                Part(art.transform,"Role_ShieldBearer_Plume",PrimitiveType.Cube,new Vector3(0f,2.09f,0f),new Vector3(.14f,.32f,.50f),greekBlue);
                Part(art.transform,"Role_ShieldBearer_WallShield",PrimitiveType.Cylinder,new Vector3(-.57f,1.03f,.11f),new Vector3(.64f,.080f,.72f),greekBlue,Quaternion.Euler(90f,0f,0f));
                Part(art.transform,"Role_ShieldBearer_ShieldBoss",PrimitiveType.Sphere,new Vector3(-.57f,1.03f,.19f),new Vector3(.22f,.11f,.22f),brightBronze);
                Part(art.transform,"Role_ShieldBearer_PaleBand",PrimitiveType.Cube,new Vector3(-.57f,1.03f,.205f),new Vector3(.44f,.035f,.10f),paleCloth);
                break;

            case EnemyArchetype.Boss:
                AddHeavyArmor(art.transform,bronze,brightBronze,true);
                Part(art.transform,"Role_Boss_CommanderCrest",PrimitiveType.Cube,new Vector3(0f,2.17f,0f),new Vector3(.21f,.44f,.64f),new Color(.48f,.045f,.035f));
                Part(art.transform,"Role_Boss_CommanderChest",PrimitiveType.Cube,new Vector3(0f,1.19f,.32f),new Vector3(.43f,.44f,.09f),brightBronze);
                Part(art.transform,"Role_Boss_BlueMantle",PrimitiveType.Cube,new Vector3(0f,1.12f,-.28f),new Vector3(.70f,1.02f,.055f),greekBlue);
                Part(art.transform,"Role_Boss_CommandShield",PrimitiveType.Cylinder,new Vector3(-.61f,1.08f,.10f),new Vector3(.67f,.082f,.67f),greekBlue,Quaternion.Euler(90f,0f,0f));
                Part(art.transform,"Role_Boss_ShieldBoss",PrimitiveType.Sphere,new Vector3(-.61f,1.08f,.19f),new Vector3(.24f,.12f,.24f),brightBronze);
                Part(art.transform,"Role_Boss_CommandAura",PrimitiveType.Cylinder,new Vector3(0f,.035f,0f),new Vector3(.90f,.025f,.90f),new Color(.15f,.28f,.56f));
                break;
        }
    }

    static void AddInfantryRead(Transform p, Color bronze, Color blue, Color wood)
    {
        Part(p,"Role_Infantry_BlueCrest",PrimitiveType.Cube,new Vector3(0f,1.98f,0f),new Vector3(.09f,.22f,.36f),blue);
        Part(p,"Role_Infantry_RoundShield",PrimitiveType.Cylinder,new Vector3(-.49f,1.01f,.10f),new Vector3(.42f,.062f,.42f),blue,Quaternion.Euler(90f,0f,0f));
        Part(p,"Role_Infantry_ShieldBoss",PrimitiveType.Sphere,new Vector3(-.49f,1.01f,.17f),new Vector3(.16f,.09f,.16f),bronze);
        Part(p,"Role_Infantry_Spear",PrimitiveType.Cylinder,new Vector3(.47f,1.08f,.02f),new Vector3(.030f,1.12f,.030f),wood,Quaternion.Euler(7f,0f,-8f));
    }

    static void AddLargeBow(Transform p, Vector3 center, Color wood, Color stringColor)
    {
        GameObject upper=Part(p,"Role_Archer_BowUpper",PrimitiveType.Cylinder,center+new Vector3(0f,.25f,0f),new Vector3(.032f,.42f,.032f),wood,Quaternion.Euler(0f,0f,26f));
        GameObject lower=Part(p,"Role_Archer_BowLower",PrimitiveType.Cylinder,center+new Vector3(0f,-.25f,0f),new Vector3(.032f,.42f,.032f),wood,Quaternion.Euler(0f,0f,-26f));
        Part(p,"Role_Archer_BowString",PrimitiveType.Cylinder,center+new Vector3(-.12f,0f,0f),new Vector3(.009f,.70f,.009f),stringColor);
        upper.transform.localPosition+=new Vector3(0f,.02f,0f);
        lower.transform.localPosition-=new Vector3(0f,.02f,0f);
    }

    static void AddHeavyArmor(Transform p, Color armor, Color trim, bool wide)
    {
        float shoulder = wide ? .45f : .39f;
        Part(p,"Pauldron L",PrimitiveType.Sphere,new Vector3(-shoulder,1.35f,0f),new Vector3(.25f,.15f,.23f),armor);
        Part(p,"Pauldron R",PrimitiveType.Sphere,new Vector3(shoulder,1.35f,0f),new Vector3(.25f,.15f,.23f),armor);
        Part(p,"Breastplate",PrimitiveType.Cube,new Vector3(0f,1.13f,.27f),new Vector3(wide?.48f:.44f,.44f,.08f),armor);
        Part(p,"Chest Trim",PrimitiveType.Cube,new Vector3(0f,1.26f,.32f),new Vector3(.38f,.055f,.03f),trim);
        Part(p,"Greave L",PrimitiveType.Cube,new Vector3(-.16f,.27f,.11f),new Vector3(.15f,.35f,.075f),armor);
        Part(p,"Greave R",PrimitiveType.Cube,new Vector3(.16f,.27f,.11f),new Vector3(.15f,.35f,.075f),armor);
    }

    static GameObject Part(Transform parent,string name,PrimitiveType type,Vector3 pos,Vector3 scale,Color color,Quaternion? rot=null)
    {
        GameObject go=GameObject.CreatePrimitive(type);
        go.name=name;
        go.transform.SetParent(parent,false);
        go.transform.localPosition=pos;
        go.transform.localScale=scale;
        if(rot.HasValue) go.transform.localRotation=rot.Value;
        Collider c=go.GetComponent<Collider>();
        if(c!=null) Object.Destroy(c);
        TowerFactory.SetColor(go,color);
        return go;
    }
}
