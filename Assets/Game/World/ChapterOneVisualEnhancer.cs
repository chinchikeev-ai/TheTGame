using UnityEngine;

// Compatibility entry point retained for older bootstrap/scene references.
// Chapter I visual ownership is intentionally split by domain:
// coast -> CoastEnvironmentBuilder, shore motion -> ChapterOneShoreLife,
// battlefield dressing -> ChapterOneBattlefieldDetails,
// Troy gate -> TroyGateHeroBuilder, wall -> ChapterOneWallLife,
// city -> TroyCityBackdropPresentation, fire/lights -> TroyFireLifePresentation.
public static class ChapterOneVisualEnhancer
{
    static readonly Color Olive = new Color(.31f,.39f,.18f);
    static readonly Color OliveDark = new Color(.20f,.30f,.14f);
    static readonly Color DryGrass = new Color(.57f,.49f,.26f);
    static readonly Color Reed = new Color(.40f,.48f,.23f);
    static readonly Color Wood = new Color(.30f,.17f,.07f);
    static readonly Color FlowerGold = new Color(.83f,.59f,.16f);
    static readonly Color FlowerRed = new Color(.63f,.20f,.10f);

    public static void Enhance()
    {
        if (GameObject.Find("Chapter01_CoastalDressing") != null) return;
        GameObject root = new GameObject("Chapter01_CoastalDressing");
        AddCoastalScrub(root.transform);
        AddEdgeBushes(root.transform);
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

    static void AddEdgeBushes(Transform parent)
    {
        Vector3[] bushes=
        {
            new Vector3(-5.9f,.05f,8.55f), new Vector3(-4.2f,.05f,-8.55f),
            new Vector3(.2f,.05f,8.85f), new Vector3(3.2f,.05f,-8.65f),
            new Vector3(5.1f,.05f,8.65f), new Vector3(7.4f,.05f,-8.3f),
            new Vector3(9.2f,.05f,8.0f), new Vector3(10.7f,.05f,-7.35f)
        };
        for(int i=0;i<bushes.Length;i++) CreateRoundBush(parent,bushes[i],.78f+(i%3)*.12f,i*41f,i%2==0);
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
                    (i%2==0?DryGrass:Reed)*(.90f+(i%3)*.045f));
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

    static void CreateRoundBush(Transform parent,Vector3 position,float scale,float yaw,bool flowers)
    {
        GameObject root=new GameObject("Stylized Edge Bush");
        root.transform.SetParent(parent,false);
        root.transform.position=position;
        root.transform.rotation=Quaternion.Euler(0f,yaw,0f);
        root.transform.localScale=Vector3.one*scale;

        Part(root.transform,"Bush Core",PrimitiveType.Sphere,new Vector3(0f,.18f,0f),new Vector3(.42f,.28f,.36f),OliveDark,true);
        Part(root.transform,"Bush Crown",PrimitiveType.Sphere,new Vector3(-.20f,.28f,.03f),new Vector3(.31f,.28f,.30f),Olive,true);
        Part(root.transform,"Bush Crown",PrimitiveType.Sphere,new Vector3(.19f,.30f,-.04f),new Vector3(.33f,.30f,.28f),Olive*1.07f,true);
        Part(root.transform,"Bush Crown",PrimitiveType.Sphere,new Vector3(.02f,.38f,.15f),new Vector3(.30f,.30f,.29f),Olive*.95f,true);

        if(!flowers) return;
        for(int i=0;i<4;i++)
        {
            float a=(35f+i*83f)*Mathf.Deg2Rad;
            Part(root.transform,"Battlefield Wildflower",PrimitiveType.Sphere,
                new Vector3(Mathf.Cos(a)*.25f,.48f+(i%2)*.05f,Mathf.Sin(a)*.20f),Vector3.one*.045f,
                i%2==0?FlowerGold:FlowerRed,true);
        }
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
                Olive*(.84f+(i%4)*.055f),true);
            leaf.transform.localRotation=Quaternion.Euler(i*5f,i*31f,(i-3)*9f);
        }

        Part(root.transform,"Scrub Core",PrimitiveType.Sphere,new Vector3(0f,.09f,0f),new Vector3(.24f,.10f,.20f)*scale,OliveDark,true);
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
