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
        Color stone = new Color(.48f,.37f,.23f);
        Color warmStone = new Color(.60f,.45f,.25f);
        Color roof = new Color(.43f,.12f,.055f);
        Color gold = new Color(.78f,.49f,.13f);

        for (int i=-4;i<=4;i++)
        {
            float z=i*2.25f;
            float x=gateX+3.7f+(i%2==0?.5f:0f);
            float h=1.8f+(Mathf.Abs(i)%3)*.45f;
            Part(root.transform,"Troy House",PrimitiveType.Cube,new Vector3(x,h*.5f,z),new Vector3(1.4f,h,1.45f),i%2==0?warmStone:stone);
            Part(root.transform,"Troy Roof",PrimitiveType.Cube,new Vector3(x,h+.18f,z),new Vector3(1.55f,.22f,1.58f),roof);
        }

        for (int i=-1;i<=1;i++)
        {
            float z=i*4.0f;
            Part(root.transform,"Inner Troy Tower",PrimitiveType.Cylinder,new Vector3(gateX+5.8f,2.2f,z),new Vector3(1.15f,2.2f,1.15f),warmStone);
            Part(root.transform,"Tower Crown",PrimitiveType.Cylinder,new Vector3(gateX+5.8f,4.45f,z),new Vector3(1.3f,.14f,1.3f),gold);
        }

        Part(root.transform,"Citadel Mass",PrimitiveType.Cube,new Vector3(gateX+8.0f,2.3f,0f),new Vector3(3.8f,4.6f,7.8f),new Color(.39f,.30f,.20f));
        Part(root.transform,"Citadel Crown",PrimitiveType.Cube,new Vector3(gateX+8.0f,4.8f,0f),new Vector3(4.1f,.35f,8.1f),gold*.85f);
    }

    GameObject Part(Transform parent,string name,PrimitiveType type,Vector3 pos,Vector3 scale,Color color)
    {
        GameObject go=GameObject.CreatePrimitive(type);
        go.name=name;
        go.transform.SetParent(parent,false);
        go.transform.position=pos;
        go.transform.localScale=scale;
        Collider c=go.GetComponent<Collider>();
        if(c!=null) Destroy(c);
        TowerFactory.SetColor(go,color);
        return go;
    }
}
