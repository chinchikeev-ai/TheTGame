using UnityEngine;

public static class TroyGateHeroBuilder
{
    public static void Build()
    {
        if (GameObject.Find("Chapter01_TroyGateHero") != null) return;
        GameObject root = new GameObject("Chapter01_TroyGateHero");
        float gateX = MapBuilder.CellToWorld(new Vector2Int(17, 6)).x;
        Color sandstone = new Color(.63f, .51f, .31f);
        Color paleStone = new Color(.72f, .61f, .40f);
        Color darkStone = new Color(.42f, .34f, .22f);
        Color shadowStone = new Color(.30f, .25f, .18f);
        Color bronze = new Color(.72f, .48f, .16f);
        Color wood = new Color(.28f, .15f, .07f);
        Color trojanRed = new Color(.52f, .06f, .04f);

        Part(root.transform,"Gate Approach Apron",PrimitiveType.Cube,new Vector3(gateX-1.22f,.07f,0f),new Vector3(2.40f,.14f,3.25f),darkStone*.92f);
        Part(root.transform,"Gate Threshold",PrimitiveType.Cube,new Vector3(gateX-.72f,.14f,0f),new Vector3(.74f,.20f,2.15f),paleStone*.86f);
        for(int side=-1;side<=1;side+=2)
            Part(root.transform,"Gate Approach Stone",PrimitiveType.Cube,new Vector3(gateX-1.38f,.13f,side*1.55f),new Vector3(1.90f,.16f,.42f),sandstone*.86f);

        Part(root.transform,"Gatehouse Core",PrimitiveType.Cube,new Vector3(gateX+.72f,1.48f,0f),new Vector3(1.72f,2.95f,5.45f),sandstone);
        Part(root.transform,"Gatehouse Upper Terrace",PrimitiveType.Cube,new Vector3(gateX+.56f,2.82f,0f),new Vector3(1.95f,.42f,5.85f),paleStone);
        Part(root.transform,"Wall Walk",PrimitiveType.Cube,new Vector3(gateX+1.05f,3.06f,0f),new Vector3(1.62f,.18f,6.00f),darkStone);
        Part(root.transform,"Gatehouse Crown Core",PrimitiveType.Cube,new Vector3(gateX+.72f,3.52f,0f),new Vector3(1.35f,.70f,2.20f),sandstone*.97f);
        Part(root.transform,"Gatehouse Crown Cap",PrimitiveType.Cube,new Vector3(gateX+.61f,3.91f,0f),new Vector3(1.55f,.14f,2.44f),bronze*.72f);

        for(int side=-1;side<=1;side+=2)
        {
            float wingZ=side*6.55f;
            Part(root.transform,"Troy Wall Wing Base",PrimitiveType.Cube,new Vector3(gateX+1.02f,.35f,wingZ),new Vector3(1.72f,.70f,5.30f),darkStone);
            Part(root.transform,"Troy Wall Wing",PrimitiveType.Cube,new Vector3(gateX+.82f,1.40f,wingZ),new Vector3(1.45f,2.55f,5.10f),sandstone*.95f);
            Part(root.transform,"Wall Parapet",PrimitiveType.Cube,new Vector3(gateX+.62f,2.74f,wingZ),new Vector3(1.66f,.30f,5.20f),paleStone);

            for(int i=-4;i<=4;i++)
                Part(root.transform,"Wall Crenellation",PrimitiveType.Cube,new Vector3(gateX-.02f,3.10f,wingZ+i*.54f),new Vector3(.36f,.54f,.34f),paleStone);

            for(int i=-1;i<=1;i++)
            {
                float buttressZ=wingZ+i*1.75f;
                Part(root.transform,"Wall Buttress",PrimitiveType.Cube,new Vector3(gateX-.05f,.98f,buttressZ),new Vector3(.52f,1.92f,.56f),darkStone*1.04f);
                Part(root.transform,"Buttress Cap",PrimitiveType.Cube,new Vector3(gateX-.08f,1.98f,buttressZ),new Vector3(.62f,.18f,.66f),paleStone);
            }
        }

        Part(root.transform,"Gate Recess",PrimitiveType.Cube,new Vector3(gateX-.20f,1.05f,0f),new Vector3(.40f,2.10f,2.08f),shadowStone);
        Part(root.transform,"Gate Left Jamb",PrimitiveType.Cube,new Vector3(gateX-.43f,1.08f,-1.13f),new Vector3(.38f,2.18f,.42f),paleStone);
        Part(root.transform,"Gate Right Jamb",PrimitiveType.Cube,new Vector3(gateX-.43f,1.08f,1.13f),new Vector3(.38f,2.18f,.42f),paleStone);
        Part(root.transform,"Gate Lintel",PrimitiveType.Cube,new Vector3(gateX-.42f,2.16f,0f),new Vector3(.40f,.36f,2.68f),paleStone);
        Part(root.transform,"Gate Arch Step Left",PrimitiveType.Cube,new Vector3(gateX-.46f,2.34f,-.83f),new Vector3(.42f,.28f,.74f),paleStone*.95f);
        Part(root.transform,"Gate Arch Step Right",PrimitiveType.Cube,new Vector3(gateX-.46f,2.34f,.83f),new Vector3(.42f,.28f,.74f),paleStone*.95f);
        Part(root.transform,"Gate Arch Crown",PrimitiveType.Cube,new Vector3(gateX-.46f,2.48f,0f),new Vector3(.42f,.30f,1.05f),paleStone);
        Part(root.transform,"Gate Doors",PrimitiveType.Cube,new Vector3(gateX-.47f,.92f,0f),new Vector3(.20f,1.78f,1.76f),wood);

        for(int z=-1;z<=1;z+=2)
            Part(root.transform,"Door Bronze Band",PrimitiveType.Cube,new Vector3(gateX-.59f,.92f,z*.46f),new Vector3(.05f,1.66f,.07f),bronze);
        for(int y=0;y<4;y++)
            Part(root.transform,"Gate Crossbar",PrimitiveType.Cube,new Vector3(gateX-.60f,.34f+y*.38f,0f),new Vector3(.05f,.075f,1.73f),bronze*.92f);

        for(int row=0;row<5;row++)
        {
            float y=.46f+row*.52f;
            Part(root.transform,"Gate Masonry Course",PrimitiveType.Cube,new Vector3(gateX-.18f,y,-2.12f),new Vector3(.08f,.055f,1.14f),darkStone*1.02f);
            Part(root.transform,"Gate Masonry Course",PrimitiveType.Cube,new Vector3(gateX-.18f,y,2.12f),new Vector3(.08f,.055f,1.14f),darkStone*1.02f);
        }

        for(int side=-1;side<=1;side+=2)
        {
            float z=side*3.58f;
            Part(root.transform,"Hero Gate Tower Base",PrimitiveType.Cylinder,new Vector3(gateX+.42f,.24f,z),new Vector3(1.52f,.18f,1.52f),darkStone);
            Part(root.transform,"Hero Gate Tower",PrimitiveType.Cylinder,new Vector3(gateX+.42f,1.88f,z),new Vector3(1.40f,1.72f,1.40f),sandstone*.97f);
            Part(root.transform,"Tower Mid Ring",PrimitiveType.Cylinder,new Vector3(gateX+.42f,2.36f,z),new Vector3(1.43f,.09f,1.43f),darkStone*.92f);
            Part(root.transform,"Tower Crown",PrimitiveType.Cylinder,new Vector3(gateX+.42f,3.66f,z),new Vector3(1.57f,.18f,1.57f),paleStone);

            for(int i=0;i<8;i++)
            {
                float a=i*Mathf.PI*2f/8f;
                Vector3 p=new Vector3(gateX+.42f,3.95f,z)+new Vector3(Mathf.Cos(a)*1.20f,0f,Mathf.Sin(a)*1.20f);
                Part(root.transform,"Tower Crenellation",PrimitiveType.Cube,p,new Vector3(.34f,.50f,.34f),paleStone);
            }

            Part(root.transform,"Tower Arrow Slit",PrimitiveType.Cube,new Vector3(gateX-.98f,1.92f,z-.34f),new Vector3(.035f,.34f,.10f),shadowStone*.55f);
            Part(root.transform,"Tower Arrow Slit",PrimitiveType.Cube,new Vector3(gateX-.98f,1.92f,z+.34f),new Vector3(.035f,.34f,.10f),shadowStone*.55f);
            Part(root.transform,"Tower Bronze Shield",PrimitiveType.Cylinder,new Vector3(gateX-.99f,2.70f,z),new Vector3(.30f,.055f,.30f),bronze).transform.rotation=Quaternion.Euler(90f,0f,0f);
            AddBanner(root.transform,new Vector3(gateX-1.00f,2.42f,z),trojanRed,bronze);
        }

        for(int i=-4;i<=4;i++)
        {
            if(i==0) continue;
            Part(root.transform,"Gatehouse Crenellation",PrimitiveType.Cube,new Vector3(gateX-.10f,3.34f,i*.58f),new Vector3(.36f,.56f,.36f),paleStone);
        }

        AddBanner(root.transform,new Vector3(gateX-.18f,3.52f,-1.12f),trojanRed,bronze);
        AddBanner(root.transform,new Vector3(gateX-.18f,3.52f,1.12f),trojanRed,bronze);

        GameObject emblem=Part(root.transform,"Lion Emblem Back",PrimitiveType.Cylinder,new Vector3(gateX-.40f,2.78f,0f),new Vector3(.58f,.08f,.58f),bronze);
        emblem.transform.rotation=Quaternion.Euler(90f,0f,0f);
        Part(root.transform,"Lion Emblem Body",PrimitiveType.Cube,new Vector3(gateX-.50f,2.77f,0f),new Vector3(.08f,.30f,.44f),trojanRed);
        Part(root.transform,"Lion Emblem Head",PrimitiveType.Sphere,new Vector3(gateX-.57f,2.96f,0f),new Vector3(.12f,.16f,.16f),trojanRed);

        for(int side=-1;side<=1;side+=2)
        {
            Vector3 p=new Vector3(gateX-.82f,1.25f,side*2.30f);
            Part(root.transform,"Gate Brazier Pedestal",PrimitiveType.Cube,p-new Vector3(0f,.46f,0f),new Vector3(.42f,.78f,.42f),darkStone);
            Part(root.transform,"Gate Brazier",PrimitiveType.Cylinder,p,new Vector3(.34f,.14f,.34f),bronze*.72f);
            GameObject flame=Part(root.transform,"Gate Flame",PrimitiveType.Sphere,p+new Vector3(0f,.34f,0f),new Vector3(.22f,.42f,.22f),new Color(1f,.32f,.04f));
            flame.AddComponent<ChapterOneAmbientMotion>().kind=ChapterOneAmbientMotion.MotionKind.Flame;
            Light light=flame.AddComponent<Light>(); light.type=LightType.Point; light.color=new Color(1f,.42f,.10f); light.range=5.5f; light.intensity=2f;
        }
    }

    static void AddBanner(Transform parent,Vector3 position,Color cloth,Color trim)
    {
        Part(parent,"Gate Banner Pole",PrimitiveType.Cylinder,position,new Vector3(.045f,.90f,.045f),new Color(.28f,.16f,.07f));
        GameObject banner=Part(parent,"Gate Banner",PrimitiveType.Cube,position+new Vector3(-.08f,.30f,0f),new Vector3(.08f,.58f,.56f),cloth);
        Part(parent,"Banner Trim",PrimitiveType.Cube,position+new Vector3(-.125f,.30f,0f),new Vector3(.02f,.07f,.56f),trim);
        Part(parent,"Banner Tail",PrimitiveType.Cube,position+new Vector3(-.09f,-.04f,.19f),new Vector3(.07f,.19f,.18f),cloth*.82f);
        banner.AddComponent<ChapterOneAmbientMotion>().kind=ChapterOneAmbientMotion.MotionKind.Banner;
    }

    static GameObject Part(Transform parent,string name,PrimitiveType type,Vector3 position,Vector3 scale,Color color)
    {
        GameObject go=GameObject.CreatePrimitive(type);
        go.name=name; go.transform.SetParent(parent,false); go.transform.position=position; go.transform.localScale=scale;
        Collider collider=go.GetComponent<Collider>(); if(collider!=null) Object.Destroy(collider); TowerFactory.SetColor(go,color); return go;
    }
}
