using UnityEngine;

// Compatibility entry point retained for older bootstrap/scene references.
// Chapter I visual ownership is intentionally split by domain:
// coast -> CoastEnvironmentBuilder, shore motion -> ChapterOneShoreLife,
// battlefield dressing -> ChapterOneBattlefieldDetails,
// Troy gate -> TroyGateHeroBuilder, wall -> ChapterOneWallLife,
// city -> TroyCityBackdropPresentation, fire/lights -> TroyFireLifePresentation.
public static class ChapterOneVisualEnhancer
{
    static readonly Color Bronze = new Color(.58f,.36f,.13f);
    static readonly Color BurntEarth = new Color(.24f,.20f,.15f);
    static readonly Color FieldGold = new Color(.57f,.48f,.25f);
    static readonly Color Foam = new Color(.78f,.86f,.78f);
    static readonly Color GreekBlue = new Color(.31f,.39f,.49f);
    static readonly Color Olive = new Color(.31f,.39f,.18f);
    static readonly Color OliveDark = new Color(.20f,.30f,.14f);
    static readonly Color DryGrass = new Color(.57f,.49f,.26f);
    static readonly Color Reed = new Color(.40f,.48f,.23f);
    static readonly Color Wood = new Color(.30f,.17f,.07f);
    static readonly Color FlowerGold = new Color(.83f,.59f,.16f);
    static readonly Color FlowerRed = new Color(.63f,.20f,.10f);
    static readonly Color TroyRed = new Color(.55f,.07f,.04f);

    public static Transform Enhance()
    {
        GameObject root = new GameObject("Chapter01_CoastalDressing");
        AddGroundColorPass(root.transform);
        AddFoamAndTideLines(root.transform);
        AddCoastalScrub(root.transform);
        AddEdgeBushes(root.transform);
        AddDuneGrass(root.transform);
        AddBeachStakes(root.transform);
        AddGreekCampColor(root.transform);
        AddOliveGroves(root.transform);
        AddTroyApproach(root.transform);
        AddBattlefieldComposition(root.transform);
        return root.transform;
    }

    static void AddGroundColorPass(Transform parent)
    {
        Part(parent,"Battlefield Warm Sand Wash",PrimitiveType.Cube,new Vector3(-2.2f,-.135f,0f),new Vector3(16.8f,.026f,18.1f),new Color(.68f,.57f,.36f));
        Part(parent,"Trojan Dry Field Wash",PrimitiveType.Cube,new Vector3(7.1f,-.125f,0f),new Vector3(10.4f,.028f,18.0f),new Color(.52f,.45f,.27f));
        Part(parent,"Gate Shadow Stain",PrimitiveType.Sphere,new Vector3(11.35f,-.082f,.25f),new Vector3(2.7f,.025f,2.05f),BurntEarth,Quaternion.Euler(0f,-8f,0f));

        for (int i = 0; i < 8; i++)
        {
            float z = -7.2f + i * 2.05f;
            float x = -4.4f + Mathf.Sin(i * 1.7f) * 1.1f;
            Part(parent,"Wind Brushed Sand",PrimitiveType.Sphere,new Vector3(x,-.071f,z),
                new Vector3(2.1f + (i % 3) * .25f,.020f,.42f + (i % 2) * .13f),
                new Color(.74f,.64f,.42f) * (.92f + (i % 3) * .025f),
                Quaternion.Euler(0f,-12f + i * 8f,0f));
        }
    }

    static void AddFoamAndTideLines(Transform parent)
    {
        for (int i = 0; i < 9; i++)
        {
            float z = -8.4f + i * 2.1f;
            float x = -12.95f + Mathf.Sin(i * 1.34f) * .22f;
            Part(parent,"White Shore Foam",PrimitiveType.Sphere,new Vector3(x,-.025f,z),
                new Vector3(.52f,.018f,1.25f + (i % 3) * .20f),
                Foam * (.88f + (i % 2) * .06f),
                Quaternion.Euler(0f,5f + i * 7f,0f));

            Part(parent,"Old Tide Line",PrimitiveType.Sphere,new Vector3(-11.35f,-.030f,z+.45f),
                new Vector3(.30f,.014f,.85f),
                new Color(.33f,.30f,.23f),
                Quaternion.Euler(0f,-11f + i * 5f,0f));
        }
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

    static void AddGreekCampColor(Transform parent)
    {
        Vector3[] cloths =
        {
            new Vector3(-10.35f,.075f,7.55f), new Vector3(-9.65f,.075f,5.55f),
            new Vector3(-9.85f,.075f,-5.10f), new Vector3(-8.60f,.075f,-6.95f),
            new Vector3(-7.25f,.075f,8.20f), new Vector3(-7.10f,.075f,-7.75f)
        };

        for (int i = 0; i < cloths.Length; i++)
        {
            Color color = i % 2 == 0 ? GreekBlue : new Color(.61f,.57f,.45f);
            Part(parent,"Greek Camp Ground Cloth",PrimitiveType.Cube,cloths[i],
                new Vector3(.96f,.018f,.58f),color * (.82f + (i % 3) * .045f),
                Quaternion.Euler(0f,-18f + i * 17f,0f));
        }

        CreateCampFire(parent,new Vector3(-8.9f,.05f,3.85f),1f);
        CreateCampFire(parent,new Vector3(-8.3f,.05f,-3.65f),.86f);
    }

    static void AddOliveGroves(Transform parent)
    {
        Vector3[] trees =
        {
            new Vector3(-1.9f,.03f,7.35f), new Vector3(.3f,.03f,7.75f),
            new Vector3(2.8f,.03f,7.15f), new Vector3(5.6f,.03f,7.70f),
            new Vector3(-.8f,.03f,-7.55f), new Vector3(1.8f,.03f,-7.25f),
            new Vector3(4.7f,.03f,-7.75f), new Vector3(7.3f,.03f,-7.15f)
        };

        for (int i = 0; i < trees.Length; i++)
            CreateOliveTree(parent,trees[i],.82f + (i % 3) * .12f,i * 37f);
    }

    static void AddTroyApproach(Transform parent)
    {
        for (int i = 0; i < 7; i++)
        {
            float z = -6.9f + i * 2.3f;
            Part(parent,"Trojan Field Strip",PrimitiveType.Cube,new Vector3(7.7f,-.055f,z),
                new Vector3(4.7f,.018f,.20f),FieldGold * (.82f + (i % 3) * .045f),
                Quaternion.Euler(0f,2f + i * 4f,0f));
        }

        for (int side = -1; side <= 1; side += 2)
            for (int i = 0; i < 4; i++)
            {
                Vector3 p = new Vector3(8.8f + i * .82f,.04f,side * (1.85f + i * .66f));
                CreateTrojanBanner(parent,p,side * (8f + i * 5f),.82f + i * .045f);
            }

        CreateGateCauseway(parent,new Vector3(9.25f,-.045f,0f),new Vector3(12.45f,-.045f,0f));
    }

    static void AddBattlefieldComposition(Transform parent)
    {
        Vector3[] stains =
        {
            new Vector3(-5.4f,-.036f,5.15f), new Vector3(-3.0f,-.036f,-4.95f),
            new Vector3(.2f,-.036f,3.0f), new Vector3(3.7f,-.036f,-2.75f),
            new Vector3(6.1f,-.036f,1.95f)
        };
        for (int i = 0; i < stains.Length; i++)
            Part(parent,"Battle Scuffed Ground",PrimitiveType.Sphere,stains[i],
                new Vector3(1.05f + (i % 2) * .28f,.014f,.58f + (i % 3) * .13f),
                BurntEarth * (.82f + (i % 3) * .045f),
                Quaternion.Euler(0f,i * 31f,0f));

        CreateShieldWallHint(parent,new Vector3(-6.25f,.12f,.65f),-9f);
        CreateShieldWallHint(parent,new Vector3(3.6f,.12f,.95f),11f);
    }

    static void CreateOliveTree(Transform parent, Vector3 position, float scale, float yaw)
    {
        GameObject root = new GameObject("Olive Tree");
        root.transform.SetParent(parent,false);
        root.transform.position = position;
        root.transform.rotation = Quaternion.Euler(0f,yaw,0f);
        root.transform.localScale = Vector3.one * scale;

        Part(root.transform,"Olive Trunk",PrimitiveType.Cylinder,new Vector3(0f,.42f,0f),new Vector3(.08f,.42f,.08f),Wood * .88f,true);
        Part(root.transform,"Olive Crown",PrimitiveType.Sphere,new Vector3(-.10f,.88f,.02f),new Vector3(.58f,.28f,.48f),Olive * 1.04f,true);
        Part(root.transform,"Olive Crown",PrimitiveType.Sphere,new Vector3(.20f,.78f,-.12f),new Vector3(.50f,.24f,.42f),Olive * .88f,true);
        Part(root.transform,"Olive Crown",PrimitiveType.Sphere,new Vector3(.03f,1.02f,.18f),new Vector3(.42f,.22f,.36f),Olive * 1.12f,true);
    }

    static void CreateCampFire(Transform parent, Vector3 position, float scale)
    {
        GameObject root = new GameObject("Greek Camp Fire");
        root.transform.SetParent(parent,false);
        root.transform.position = position;
        root.transform.localScale = Vector3.one * scale;

        for (int i = 0; i < 6; i++)
        {
            float angle = i * Mathf.PI * 2f / 6f;
            Vector3 p = new Vector3(Mathf.Cos(angle)*.24f,.045f,Mathf.Sin(angle)*.20f);
            Part(root.transform,"Fire Ring Stone",PrimitiveType.Sphere,p,new Vector3(.14f,.06f,.11f),new Color(.29f,.28f,.25f),true);
        }

        GameObject flame = Part(root.transform,"Small Camp Flame",PrimitiveType.Sphere,new Vector3(0f,.20f,0f),new Vector3(.18f,.28f,.18f),new Color(1f,.38f,.05f),true);
        flame.AddComponent<ChapterOneAmbientMotion>().kind = ChapterOneAmbientMotion.MotionKind.Flame;
        Light light = flame.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = new Color(1f,.46f,.18f);
        light.range = 3.2f;
        light.intensity = 1.05f;
    }

    static void CreateTrojanBanner(Transform parent, Vector3 position, float yaw, float scale)
    {
        GameObject root = new GameObject("Trojan Approach Banner");
        root.transform.SetParent(parent,false);
        root.transform.position = position;
        root.transform.rotation = Quaternion.Euler(0f,yaw,0f);
        root.transform.localScale = Vector3.one * scale;

        Part(root.transform,"Banner Pole",PrimitiveType.Cylinder,new Vector3(0f,.74f,0f),new Vector3(.032f,.74f,.032f),Wood,true);
        GameObject banner = Part(root.transform,"Red Banner",PrimitiveType.Cube,new Vector3(-.06f,1.06f,.25f),new Vector3(.04f,.42f,.50f),TroyRed,true);
        banner.AddComponent<ChapterOneAmbientMotion>().kind = ChapterOneAmbientMotion.MotionKind.Banner;
        Part(root.transform,"Banner Bronze Mark",PrimitiveType.Cube,new Vector3(-.085f,1.08f,.25f),new Vector3(.018f,.06f,.36f),Bronze,true);
    }

    static void CreateGateCauseway(Transform parent, Vector3 start, Vector3 end)
    {
        RodBetween(parent,"Stone Causeway Edge",start + new Vector3(0f,.035f,.62f),end + new Vector3(0f,.035f,.62f),.035f,new Color(.41f,.36f,.27f));
        RodBetween(parent,"Stone Causeway Edge",start + new Vector3(0f,.035f,-.62f),end + new Vector3(0f,.035f,-.62f),.035f,new Color(.41f,.36f,.27f));

        for (int i = 0; i < 7; i++)
        {
            float t = (i + .5f) / 7f;
            Vector3 p = Vector3.Lerp(start,end,t);
            Part(parent,"Causeway Paver",PrimitiveType.Cube,p + Vector3.up * .028f,
                new Vector3(.32f,.035f,1.02f),new Color(.47f,.40f,.29f) * (.90f + (i % 3) * .045f),
                Quaternion.Euler(0f,(i % 2 == 0 ? 4f : -5f),0f));
        }
    }

    static void CreateShieldWallHint(Transform parent, Vector3 position, float yaw)
    {
        GameObject root = new GameObject("Fallen Shield Line");
        root.transform.SetParent(parent,false);
        root.transform.position = position;
        root.transform.rotation = Quaternion.Euler(0f,yaw,0f);

        for (int i = 0; i < 5; i++)
        {
            Color color = i % 2 == 0 ? GreekBlue : TroyRed;
            Part(root.transform,"Fallen Shield Accent",PrimitiveType.Cylinder,new Vector3((i-2)*.38f,.02f,0f),
                new Vector3(.18f,.018f,.18f),color * .82f,Quaternion.Euler(90f,i*13f,0f),true);
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

    static GameObject Part(Transform parent,string name,PrimitiveType type,Vector3 position,Vector3 scale,Color color,bool local)
    {
        return Part(parent,name,type,position,scale,color,null,local);
    }

    static GameObject Part(Transform parent,string name,PrimitiveType type,Vector3 position,Vector3 scale,Color color,Quaternion? rotation=null,bool local=false)
    {
        GameObject go=GameObject.CreatePrimitive(type);
        go.name=name;
        go.transform.SetParent(parent,false);
        if(local) go.transform.localPosition=position;
        else go.transform.position=position;
        go.transform.localScale=scale;
        if(rotation.HasValue)
        {
            if(local) go.transform.localRotation=rotation.Value;
            else go.transform.rotation=rotation.Value;
        }
        Collider collider=go.GetComponent<Collider>();
        if(collider!=null) Object.Destroy(collider);
        TowerFactory.SetColor(go,color);
        return go;
    }
}
