using UnityEngine;

public static class ChapterOneVisualEnhancer
{
    public static void Enhance()
    {
        if (GameObject.Find("Chapter01_VisualEnhancements") != null) return;
        GameObject root = new GameObject("Chapter01_VisualEnhancements");
        AnimateExistingEnvironment();
        AddBeachLife(root.transform);
        AddTrojanSkyline(root.transform);
        AddBattleDebris(root.transform);
    }

    static void AnimateExistingEnvironment()
    {
        foreach (Transform t in Object.FindObjectsByType<Transform>(FindObjectsSortMode.None))
        {
            if (t == null) continue;
            string n = t.name;
            if (n.Contains("Sea Glint") || n.Contains("Foam")) AttachMotion(t.gameObject, ChapterOneAmbientMotion.MotionKind.Sea);
            else if (n.Contains("Flame")) AttachMotion(t.gameObject, ChapterOneAmbientMotion.MotionKind.Flame);
            else if (n.Contains("Banner")) AttachMotion(t.gameObject, ChapterOneAmbientMotion.MotionKind.Banner);
            else if (n.Contains("Smoke")) AttachMotion(t.gameObject, ChapterOneAmbientMotion.MotionKind.Smoke);
        }
    }

    static void AddBeachLife(Transform parent)
    {
        Vector3[] scrub = {
            new Vector3(-8.8f,.05f,8.0f), new Vector3(-7.2f,.05f,-7.2f), new Vector3(-3.2f,.05f,8.5f),
            new Vector3(1.8f,.05f,-8.2f), new Vector3(6.2f,.05f,8.0f), new Vector3(9.6f,.05f,-7.4f)
        };
        for (int i=0;i<scrub.Length;i++) CreateScrub(parent,scrub[i],.8f+(i%3)*.15f);

        for (int i=0;i<5;i++)
        {
            Vector3 p = new Vector3(-12.0f + i*.7f,.05f,-7.6f + (i%2)*.8f);
            GameObject stake = Part(parent,"Beach Stake",PrimitiveType.Cylinder,p,new Vector3(.035f,.52f,.035f),new Color(.30f,.18f,.08f));
            stake.transform.rotation = Quaternion.Euler(10f,0f,(i%2==0?12f:-12f));
        }
    }

    static void AddTrojanSkyline(Transform parent)
    {
        float gateX=MapBuilder.CellToWorld(new Vector2Int(17,6)).x;
        Color stone = new Color(.58f,.47f,.30f);
        Color roof = new Color(.50f,.18f,.10f);

        for (int i=-3;i<=3;i++)
        {
            float z=i*2.4f;
            float x=gateX+3.0f+Mathf.Abs(i%2)*.8f;
            float h=1.0f+(Mathf.Abs(i)%3)*.25f;
            Part(parent,"Troy House",PrimitiveType.Cube,new Vector3(x,h*.48f,z),new Vector3(1.45f,h,1.55f),stone*.92f);
            GameObject roofObj=Part(parent,"Troy Roof",PrimitiveType.Cylinder,new Vector3(x,h+0.22f,z),new Vector3(.9f,.22f,.9f),roof);
            roofObj.transform.rotation=Quaternion.Euler(0f,30f,0f);
        }

        Part(parent,"Citadel Mass",PrimitiveType.Cube,new Vector3(gateX+5.0f,1.45f,0f),new Vector3(3.0f,2.9f,7.0f),stone*.82f);
        for(int z=-2;z<=2;z+=2)
            Part(parent,"Citadel Crown",PrimitiveType.Cube,new Vector3(gateX+4.55f,3.05f,z*1.25f),new Vector3(.85f,.45f,.85f),stone*1.08f);
    }

    static void AddBattleDebris(Transform parent)
    {
        Color bronze=new Color(.63f,.43f,.17f);
        Color wood=new Color(.29f,.17f,.08f);
        Vector3[] debris={new Vector3(-9.8f,.07f,3.2f),new Vector3(-8.7f,.07f,-2.8f),new Vector3(-5.4f,.07f,6.7f),new Vector3(3.0f,.07f,-6.8f)};
        for(int i=0;i<debris.Length;i++)
        {
            GameObject shield=Part(parent,"Abandoned Shield",PrimitiveType.Cylinder,debris[i],new Vector3(.30f,.05f,.30f),bronze);
            shield.transform.rotation=Quaternion.Euler(90f,i*23f,0f);
            GameObject spear=Part(parent,"Broken Spear",PrimitiveType.Cylinder,debris[i]+new Vector3(.32f,.05f,.10f),new Vector3(.025f,.48f,.025f),wood);
            spear.transform.rotation=Quaternion.Euler(76f,i*18f,8f);
        }
    }

    static void CreateScrub(Transform parent, Vector3 position, float scale)
    {
        Color green=new Color(.26f,.31f,.16f);
        for(int i=0;i<5;i++)
        {
            GameObject leaf=Part(parent,"Coastal Scrub",PrimitiveType.Cube,position+new Vector3((i-2)*.08f,.12f,((i%2)-.5f)*.12f),new Vector3(.04f,.30f,.07f)*scale,green*(.88f+i*.025f));
            leaf.transform.rotation=Quaternion.Euler(i*7f,i*31f,(i-2)*11f);
        }
    }

    static void AttachMotion(GameObject go, ChapterOneAmbientMotion.MotionKind kind)
    {
        if (go.GetComponent<ChapterOneAmbientMotion>() != null) return;
        go.AddComponent<ChapterOneAmbientMotion>().kind=kind;
    }

    static GameObject Part(Transform parent,string name,PrimitiveType type,Vector3 position,Vector3 scale,Color color)
    {
        GameObject go=GameObject.CreatePrimitive(type);
        go.name=name;
        go.transform.SetParent(parent,false);
        go.transform.position=position;
        go.transform.localScale=scale;
        Object.Destroy(go.GetComponent<Collider>());
        TowerFactory.SetColor(go,color);
        return go;
    }
}
