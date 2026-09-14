using UnityEngine;

public sealed class TroyCityBackdropPresentation : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoCreate()
    {
        if (FindFirstObjectByType<TroyCityBackdropPresentation>() == null)
            new GameObject("TroyCityBackdropPresentation").AddComponent<TroyCityBackdropPresentation>();
    }

    void Start()
    {
        if (GameManager.Instance != null && GameManager.Instance.MapNumber != 1) return;
        if (GameObject.Find("Chapter01_TroyCityBackdrop") != null) return;

        GameObject root = new GameObject("Chapter01_TroyCityBackdrop");
        float gateX = MapBuilder.CellToWorld(new Vector2Int(17,6)).x;
        Color stone = new Color(.54f,.41f,.24f);
        Color warmStone = new Color(.66f,.49f,.28f);
        Color paleStone = new Color(.77f,.61f,.37f);
        Color roof = new Color(.48f,.13f,.055f);
        Color gold = new Color(.84f,.54f,.15f);
        Color earth = new Color(.37f,.30f,.21f);
        Color distantStone = new Color(.48f,.39f,.27f);
        Color cypress = new Color(.16f,.25f,.14f);

        BuildLowerCity(root.transform,gateX,stone,warmStone,paleStone,roof);
        BuildInnerWall(root.transform,gateX,stone,warmStone,paleStone,gold);
        BuildCitadel(root.transform,gateX,stone,warmStone,paleStone,roof,gold,earth);
        BuildDistantSkyline(root.transform,gateX,distantStone,warmStone,paleStone,roof,gold,earth,cypress);
    }

    void BuildLowerCity(Transform parent,float gateX,Color stone,Color warmStone,Color paleStone,Color roof)
    {
        Vector3[] houses = {
            new Vector3(gateX+3.0f,0f,-8.0f), new Vector3(gateX+3.6f,0f,-5.8f), new Vector3(gateX+3.1f,0f,-3.4f),
            new Vector3(gateX+3.8f,0f,-1.2f), new Vector3(gateX+3.3f,0f,1.2f), new Vector3(gateX+3.0f,0f,3.7f),
            new Vector3(gateX+3.7f,0f,5.8f), new Vector3(gateX+3.1f,0f,8.0f), new Vector3(gateX+4.8f,0f,-6.8f),
            new Vector3(gateX+4.7f,0f,-2.8f), new Vector3(gateX+4.9f,0f,2.8f), new Vector3(gateX+4.7f,0f,6.8f)
        };

        for (int i = 0; i < houses.Length; i++)
        {
            float h = 1.4f + (i % 4) * .28f;
            float w = 1.0f + (i % 3) * .16f;
            float d = 1.15f + ((i + 1) % 3) * .17f;
            CreateHouse(parent,houses[i],w,h,d,i%3==0?paleStone:i%2==0?warmStone:stone,roof,-10f+i*11f);
        }
    }

    void BuildInnerWall(Transform parent,float gateX,Color stone,Color warmStone,Color paleStone,Color gold)
    {
        float innerWallX = gateX + 5.9f;
        Part(parent,"Troy Inner Wall",PrimitiveType.Cube,new Vector3(innerWallX,1.55f,0f),new Vector3(.78f,3.1f,15.7f),stone);
        Part(parent,"Inner Wall Walk",PrimitiveType.Cube,new Vector3(innerWallX-.1f,3.16f,0f),new Vector3(.96f,.18f,15.9f),paleStone*.84f);
        for(int z=-12;z<=12;z+=2)
            Part(parent,"Inner Wall Merlon",PrimitiveType.Cube,new Vector3(innerWallX-.46f,3.48f,z*.58f),new Vector3(.30f,.52f,.34f),paleStone);

        for(int side=-1;side<=1;side+=2)
        {
            float z=side*6.8f;
            Part(parent,"Inner Wall Tower",PrimitiveType.Cylinder,new Vector3(innerWallX,1.95f,z),new Vector3(1.12f,1.95f,1.12f),warmStone);
            Part(parent,"Inner Tower Crown",PrimitiveType.Cylinder,new Vector3(innerWallX,4.02f,z),new Vector3(1.28f,.16f,1.28f),gold*.82f);
        }
    }

    void BuildCitadel(Transform parent,float gateX,Color stone,Color warmStone,Color paleStone,Color roof,Color gold,Color earth)
    {
        Part(parent,"Citadel Hill Lower",PrimitiveType.Sphere,new Vector3(gateX+8.1f,.45f,0f),new Vector3(4.4f,.68f,6.3f),earth);
        Part(parent,"Citadel Hill Upper",PrimitiveType.Sphere,new Vector3(gateX+8.8f,1.05f,0f),new Vector3(3.5f,.82f,4.9f),earth*1.08f);
        Part(parent,"Citadel Terrace",PrimitiveType.Cube,new Vector3(gateX+8.35f,1.55f,0f),new Vector3(4.35f,.42f,8.2f),stone);
        Part(parent,"Citadel Terrace Cap",PrimitiveType.Cube,new Vector3(gateX+8.2f,1.80f,0f),new Vector3(4.55f,.12f,8.45f),paleStone*.86f);
        Part(parent,"Palace Lower",PrimitiveType.Cube,new Vector3(gateX+8.85f,3.05f,0f),new Vector3(3.10f,2.55f,6.65f),warmStone);
        Part(parent,"Palace Upper",PrimitiveType.Cube,new Vector3(gateX+9.20f,4.55f,0f),new Vector3(2.15f,1.25f,4.90f),paleStone);
        Part(parent,"Palace Roof",PrimitiveType.Cube,new Vector3(gateX+9.12f,5.28f,0f),new Vector3(2.38f,.22f,5.15f),roof);
        Part(parent,"Palace Crown",PrimitiveType.Cube,new Vector3(gateX+9.15f,5.48f,0f),new Vector3(2.55f,.10f,5.30f),gold*.82f);

        for(int side=-1;side<=1;side+=2)
        {
            float z=side*3.45f;
            Part(parent,"Citadel Corner Tower",PrimitiveType.Cylinder,new Vector3(gateX+8.10f,3.45f,z),new Vector3(.92f,1.72f,.92f),stone*1.08f);
            Part(parent,"Citadel Tower Crown",PrimitiveType.Cylinder,new Vector3(gateX+8.10f,5.23f,z),new Vector3(1.04f,.14f,1.04f),gold*.78f);
        }
    }

    void BuildDistantSkyline(Transform parent,float gateX,Color distantStone,Color warmStone,Color paleStone,Color roof,Color gold,Color earth,Color cypress)
    {
        Part(parent,"Troy Back Ridge",PrimitiveType.Sphere,new Vector3(gateX+12.6f,1.15f,0f),new Vector3(7.2f,1.25f,10.8f),earth*.93f);
        Part(parent,"Troy Far Ridge",PrimitiveType.Sphere,new Vector3(gateX+16.2f,1.65f,1.2f),new Vector3(6.2f,1.35f,12.0f),earth*.78f);

        Vector3[] backHouses=
        {
            new Vector3(gateX+9.7f,1.55f,-7.4f), new Vector3(gateX+10.4f,1.75f,-5.3f),
            new Vector3(gateX+11.0f,1.65f,5.6f), new Vector3(gateX+10.0f,1.55f,7.5f),
            new Vector3(gateX+12.0f,2.0f,-8.0f), new Vector3(gateX+12.4f,2.15f,7.8f),
            new Vector3(gateX+14.0f,2.25f,-5.8f), new Vector3(gateX+14.4f,2.35f,5.4f)
        };
        for(int i=0;i<backHouses.Length;i++)
        {
            float height=1.35f+(i%3)*.30f;
            CreateHouse(parent,backHouses[i],1.0f+(i%2)*.22f,height,1.05f+(i%3)*.14f,i%2==0?distantStone:warmStone*.88f,roof*.92f,-18f+i*13f);
        }

        CreateWatchtower(parent,new Vector3(gateX+11.4f,2.0f,-3.8f),2.7f,distantStone,paleStone,gold);
        CreateWatchtower(parent,new Vector3(gateX+12.2f,2.1f,3.9f),3.0f,warmStone*.88f,paleStone,gold);
        CreateWatchtower(parent,new Vector3(gateX+15.1f,2.45f,.6f),3.4f,distantStone*.86f,paleStone*.88f,gold*.78f);

        CreateTemple(parent,new Vector3(gateX+13.4f,2.45f,-2.2f),paleStone,warmStone,roof,gold);

        Vector3[] trees=
        {
            new Vector3(gateX+7.1f,1.8f,-7.7f), new Vector3(gateX+7.4f,1.8f,7.5f),
            new Vector3(gateX+10.8f,2.0f,-6.6f), new Vector3(gateX+11.3f,2.1f,6.5f),
            new Vector3(gateX+13.0f,2.35f,4.8f), new Vector3(gateX+14.2f,2.45f,-4.6f),
            new Vector3(gateX+15.2f,2.65f,7.0f), new Vector3(gateX+15.6f,2.65f,-7.2f)
        };
        for(int i=0;i<trees.Length;i++) CreateCypress(parent,trees[i],.82f+(i%3)*.12f,cypress*(.92f+(i%2)*.07f));
    }

    void CreateTemple(Transform parent,Vector3 pos,Color paleStone,Color warmStone,Color roof,Color gold)
    {
        GameObject root=new GameObject("Troy Distant Temple");
        root.transform.SetParent(parent,false);
        root.transform.position=pos;
        root.transform.rotation=Quaternion.Euler(0f,-7f,0f);
        LocalPart(root.transform,"Temple Terrace",PrimitiveType.Cube,new Vector3(0f,.12f,0f),new Vector3(3.8f,.24f,3.0f),warmStone*.85f);
        LocalPart(root.transform,"Temple Floor",PrimitiveType.Cube,new Vector3(0f,.30f,0f),new Vector3(3.35f,.14f,2.55f),paleStone*.92f);
        for(int side=-1;side<=1;side+=2)
            for(int i=-1;i<=1;i++)
                LocalPart(root.transform,"Temple Column",PrimitiveType.Cylinder,new Vector3(i*.92f,1.25f,side*.91f),new Vector3(.16f,.95f,.16f),paleStone);
        LocalPart(root.transform,"Temple Lintel",PrimitiveType.Cube,new Vector3(0f,2.25f,0f),new Vector3(3.35f,.25f,2.45f),warmStone);
        LocalPart(root.transform,"Temple Roof",PrimitiveType.Cube,new Vector3(0f,2.52f,0f),new Vector3(3.75f,.24f,2.85f),roof);
        LocalPart(root.transform,"Temple Gold Ridge",PrimitiveType.Cube,new Vector3(0f,2.70f,0f),new Vector3(.18f,.12f,2.92f),gold*.82f);
    }

    void CreateWatchtower(Transform parent,Vector3 pos,float height,Color stone,Color cap,Color gold)
    {
        Part(parent,"Distant Troy Watchtower",PrimitiveType.Cylinder,pos+Vector3.up*(height*.5f),new Vector3(.72f,height*.5f,.72f),stone);
        Part(parent,"Watchtower Crown",PrimitiveType.Cylinder,pos+Vector3.up*(height+.12f),new Vector3(.88f,.16f,.88f),cap);
        Part(parent,"Watchtower Gold Band",PrimitiveType.Cylinder,pos+Vector3.up*(height-.18f),new Vector3(.76f,.08f,.76f),gold*.72f);
    }

    void CreateCypress(Transform parent,Vector3 pos,float scale,Color foliage)
    {
        GameObject root=new GameObject("Troy Cypress");
        root.transform.SetParent(parent,false);
        root.transform.position=pos;
        root.transform.localScale=Vector3.one*scale;
        LocalPart(root.transform,"Cypress Trunk",PrimitiveType.Cylinder,new Vector3(0f,.48f,0f),new Vector3(.065f,.48f,.065f),new Color(.27f,.17f,.09f));
        LocalPart(root.transform,"Cypress Lower",PrimitiveType.Capsule,new Vector3(0f,1.18f,0f),new Vector3(.34f,.70f,.34f),foliage);
        LocalPart(root.transform,"Cypress Upper",PrimitiveType.Capsule,new Vector3(0f,1.78f,0f),new Vector3(.23f,.58f,.23f),foliage*1.04f);
    }

    void CreateHouse(Transform parent,Vector3 pos,float width,float height,float depth,Color wall,Color roof,float yaw)
    {
        GameObject house=new GameObject("Troy House Cluster");
        house.transform.SetParent(parent,false); house.transform.position=pos; house.transform.rotation=Quaternion.Euler(0f,yaw,0f);
        LocalPart(house.transform,"Troy House",PrimitiveType.Cube,new Vector3(0f,height*.5f,0f),new Vector3(width,height,depth),wall);
        LocalPart(house.transform,"Troy Roof",PrimitiveType.Cube,new Vector3(0f,height+.14f,0f),new Vector3(width*1.08f,.20f,depth*1.09f),roof);
        LocalPart(house.transform,"Roof Ridge",PrimitiveType.Cube,new Vector3(0f,height+.27f,0f),new Vector3(width*.16f,.11f,depth*1.12f),roof*.82f);
    }

    GameObject Part(Transform parent,string name,PrimitiveType type,Vector3 pos,Vector3 scale,Color color)
    {
        GameObject go=GameObject.CreatePrimitive(type); go.name=name; go.transform.SetParent(parent,false); go.transform.position=pos; go.transform.localScale=scale;
        Collider c=go.GetComponent<Collider>(); if(c!=null) Destroy(c); TowerFactory.SetColor(go,color); return go;
    }

    GameObject LocalPart(Transform parent,string name,PrimitiveType type,Vector3 pos,Vector3 scale,Color color)
    {
        GameObject go=GameObject.CreatePrimitive(type); go.name=name; go.transform.SetParent(parent,false); go.transform.localPosition=pos; go.transform.localScale=scale;
        Collider c=go.GetComponent<Collider>(); if(c!=null) Destroy(c); TowerFactory.SetColor(go,color); return go;
    }
}
