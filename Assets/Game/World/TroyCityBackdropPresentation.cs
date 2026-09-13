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
        Color stone = new Color(.47f,.36f,.22f);
        Color warmStone = new Color(.60f,.45f,.25f);
        Color paleStone = new Color(.70f,.55f,.32f);
        Color roof = new Color(.42f,.11f,.05f);
        Color gold = new Color(.78f,.49f,.13f);
        Color earth = new Color(.31f,.26f,.19f);

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
            CreateHouse(root.transform,houses[i],w,h,d,i%3==0?paleStone:i%2==0?warmStone:stone,roof,-10f+i*11f);
        }

        float innerWallX = gateX + 5.9f;
        Part(root.transform,"Troy Inner Wall",PrimitiveType.Cube,new Vector3(innerWallX,1.55f,0f),new Vector3(.78f,3.1f,15.7f),stone);
        Part(root.transform,"Inner Wall Walk",PrimitiveType.Cube,new Vector3(innerWallX-.1f,3.16f,0f),new Vector3(.96f,.18f,15.9f),paleStone*.84f);
        for(int z=-12;z<=12;z+=2)
            Part(root.transform,"Inner Wall Merlon",PrimitiveType.Cube,new Vector3(innerWallX-.46f,3.48f,z*.58f),new Vector3(.30f,.52f,.34f),paleStone);

        for(int side=-1;side<=1;side+=2)
        {
            float z=side*6.8f;
            Part(root.transform,"Inner Wall Tower",PrimitiveType.Cylinder,new Vector3(innerWallX,1.95f,z),new Vector3(1.12f,1.95f,1.12f),warmStone);
            Part(root.transform,"Inner Tower Crown",PrimitiveType.Cylinder,new Vector3(innerWallX,4.02f,z),new Vector3(1.28f,.16f,1.28f),gold*.82f);
        }

        Part(root.transform,"Citadel Hill Lower",PrimitiveType.Sphere,new Vector3(gateX+8.1f,.45f,0f),new Vector3(4.4f,.68f,6.3f),earth);
        Part(root.transform,"Citadel Hill Upper",PrimitiveType.Sphere,new Vector3(gateX+8.8f,1.05f,0f),new Vector3(3.5f,.82f,4.9f),earth*1.08f);
        Part(root.transform,"Citadel Terrace",PrimitiveType.Cube,new Vector3(gateX+8.35f,1.55f,0f),new Vector3(4.35f,.42f,8.2f),stone);
        Part(root.transform,"Citadel Terrace Cap",PrimitiveType.Cube,new Vector3(gateX+8.2f,1.80f,0f),new Vector3(4.55f,.12f,8.45f),paleStone*.86f);
        Part(root.transform,"Palace Lower",PrimitiveType.Cube,new Vector3(gateX+8.85f,3.05f,0f),new Vector3(3.10f,2.55f,6.65f),warmStone);
        Part(root.transform,"Palace Upper",PrimitiveType.Cube,new Vector3(gateX+9.20f,4.55f,0f),new Vector3(2.15f,1.25f,4.90f),paleStone);
        Part(root.transform,"Palace Roof",PrimitiveType.Cube,new Vector3(gateX+9.12f,5.28f,0f),new Vector3(2.38f,.22f,5.15f),roof);
        Part(root.transform,"Palace Crown",PrimitiveType.Cube,new Vector3(gateX+9.15f,5.48f,0f),new Vector3(2.55f,.10f,5.30f),gold*.82f);

        for(int side=-1;side<=1;side+=2)
        {
            float z=side*3.45f;
            Part(root.transform,"Citadel Corner Tower",PrimitiveType.Cylinder,new Vector3(gateX+8.10f,3.45f,z),new Vector3(.92f,1.72f,.92f),stone*1.08f);
            Part(root.transform,"Citadel Tower Crown",PrimitiveType.Cylinder,new Vector3(gateX+8.10f,5.23f,z),new Vector3(1.04f,.14f,1.04f),gold*.78f);
        }
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
