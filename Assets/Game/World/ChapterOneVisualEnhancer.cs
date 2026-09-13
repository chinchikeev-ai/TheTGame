using UnityEngine;

// Compatibility entry point retained for older bootstrap/scene references.
// Chapter I visual ownership is intentionally split by domain:
// coast -> CoastEnvironmentBuilder, shore motion -> ChapterOneShoreLife,
// battlefield dressing -> ChapterOneBattlefieldDetails,
// Troy gate -> TroyGateHeroBuilder, wall -> ChapterOneWallLife,
// city -> TroyCityBackdropPresentation, fire/lights -> TroyFireLifePresentation.
public static class ChapterOneVisualEnhancer
{
    static readonly Color Olive = new Color(.25f,.30f,.15f);
    static readonly Color DryGrass = new Color(.45f,.40f,.22f);
    static readonly Color Reed = new Color(.34f,.38f,.20f);
    static readonly Color Wood = new Color(.29f,.17f,.08f);

    public static void Enhance()
    {
        if (GameObject.Find("Chapter01_CoastalDressing") != null) return;
        GameObject root = new GameObject("Chapter01_CoastalDressing");
        AddCoastalScrub(root.transform);
        AddDuneGrass(root.transform);
        AddBeachStakes(root.transform);
    }

    static void AddCoastalScrub(Transform parent)
    {
        Vector3[] scrub =
        {
            new Vector3(-8.8f,.05f,8.0f), new Vector3(-7.2f,.05f,-7.2f),
            new Vector3(-3.2f,.05f,8.5f), new Vector3(1.8f,.05f,-8.2f),
            new Vector3(6.2f,.05f,8.0f), new Vector3(9.6f,.05f,-7.4f),
            new Vector3(-1.1f,.05f,-9.1f), new Vector3(7.8f,.05f,9.0f)
        };
        for (int i = 0; i < scrub.Length; i++) CreateScrub(parent,scrub[i],.72f+(i%4)*.13f,i*31f);
    }

    static void AddDuneGrass(Transform parent)
    {
        Vector3[] clusters =
        {
            new Vector3(-7.5f,.04f,9.1f), new Vector3(-5.0f,.04f,-8.7f),
            new Vector3(.7f,.04f,9.4f), new Vector3(4.5f,.04f,-9.0f),
            new Vector3(8.6f,.04f,8.7f), new Vector3(11.1f,.04f,-8.0f),
            new Vector3(3.0f,.04f,8.9f), new Vector3(9.8f,.04f,7.9f)
        };

        for(int c=0;c<clusters.Length;c++)
        {
            int blades=5+(c%3);
            for(int i=0;i<blades;i++)
            {
                float angle=(c*43f+i*67f)*Mathf.Deg2Rad;
                Vector3 offset=new Vector3(Mathf.Cos(angle)*.20f,.11f,Mathf.Sin(angle)*.16f);
                GameObject blade=Part(parent,"Dune Grass",PrimitiveType.Cube,
                    clusters[c]+offset,
                    new Vector3(.025f,.24f+(i%3)*.055f,.035f),
                    (i%2==0?DryGrass:Reed)*(.88f+(i%3)*.045f));
                blade.transform.rotation=Quaternion.Euler((i%3)*4f,c*29f+i*19f,-15f+(i%5)*7f);
            }
        }
    }

    static void AddBeachStakes(Transform parent)
    {
        Vector3[] stakes =
        {
            new Vector3(-11.9f,.05f,-7.5f),
            new Vector3(-11.2f,.05f,-6.8f),
            new Vector3(-10.45f,.05f,-7.4f),
            new Vector3(-9.70f,.05f,-6.65f),
            new Vector3(-8.95f,.05f,-7.25f)
        };

        for (int i = 0; i < stakes.Length; i++)
        {
            GameObject stake = Part(parent,"Beach Stake",PrimitiveType.Cylinder,stakes[i],new Vector3(.032f,.48f+(i%2)*.10f,.032f),Wood);
            stake.transform.rotation = Quaternion.Euler(i%2==0?8f:-6f,i*7f,i%2==0?10f:-11f);
        }

        for(int i=0;i<stakes.Length-1;i++)
            RodBetween(parent,"Stake Rope",stakes[i]+Vector3.up*.62f,stakes[i+1]+Vector3.up*.58f,.012f,new Color(.31f,.25f,.17f));
    }

    static void CreateScrub(Transform parent, Vector3 position, float scale, float yaw)
    {
        GameObject root=new GameObject("Coastal Scrub Cluster");
        root.transform.SetParent(parent,false);
        root.transform.position=position;
        root.transform.rotation=Quaternion.Euler(0f,yaw,0f);

        for (int i = 0; i < 7; i++)
        {
            float x=(i-3)*.065f;
            float z=((i*5)%4-1.5f)*.07f;
            GameObject leaf=Part(root.transform,"Coastal Scrub",PrimitiveType.Cube,
                new Vector3(x,.13f,z),
                new Vector3(.032f,.27f+(i%3)*.045f,.055f)*scale,
                Olive*(.82f+(i%4)*.055f),true);
            leaf.transform.localRotation=Quaternion.Euler(i*5f,i*31f,(i-3)*9f);
        }

        Part(root.transform,"Scrub Core",PrimitiveType.Sphere,new Vector3(0f,.09f,0f),new Vector3(.24f,.10f,.20f)*scale,Olive*.72f,true);
    }

    static GameObject RodBetween(Transform parent,string name,Vector3 a,Vector3 b,float radius,Color color)
    {
        Vector3 delta=b-a;
        GameObject rod=Part(parent,name,PrimitiveType.Cylinder,(a+b)*.5f,new Vector3(radius,delta.magnitude*.5f,radius),color);
        if(delta.sqrMagnitude>.0001f) rod.transform.rotation=Quaternion.FromToRotation(Vector3.up,delta.normalized);
        return rod;
    }

    static GameObject Part(Transform parent,string name,PrimitiveType type,Vector3 position,Vector3 scale,Color color,bool local=false)
    {
        GameObject go=GameObject.CreatePrimitive(type);
        go.name=name;
        go.transform.SetParent(parent,false);
        if(local) go.transform.localPosition=position;
        else go.transform.position=position;
        go.transform.localScale=scale;
        Collider collider=go.GetComponent<Collider>();
        if(collider!=null) Object.Destroy(collider);
        TowerFactory.SetColor(go,color);
        return go;
    }
}
