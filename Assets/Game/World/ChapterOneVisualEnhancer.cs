using UnityEngine;

public static class ChapterOneVisualEnhancer
{
    public static void Enhance()
    {
        if (GameObject.Find("Chapter01_VisualEnhancements") != null) return;
        GameObject root = new GameObject("Chapter01_VisualEnhancements");
        AddBeachLife(root.transform);
        AddTrojanSkyline(root.transform);
        AddBattleDebris(root.transform);
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
        Color paleStone = new Color(.68f,.56f,.37f);
        Color roof = new Color(.50f,.18f,.10f);
        Color bronze = new Color(.68f,.46f,.17f);
        Color red = new Color(.52f,.06f,.04f);

        // Dense stepped houses establish Troy as a city beyond the defensive gate.
        for (int i=-4;i<=4;i++)
        {
            float z=i*2.15f;
            float x=gateX+3.0f+(Mathf.Abs(i)%3)*.72f;
            float h=1.0f+(Mathf.Abs(i)%3)*.25f;
            Part(parent,"Troy House",PrimitiveType.Cube,new Vector3(x,h*.48f,z),new Vector3(1.40f,h,1.45f),stone*(.90f+(i%2)*.03f));
            GameObject roofObj=Part(parent,"Troy Roof",PrimitiveType.Cylinder,new Vector3(x,h+0.22f,z),new Vector3(.88f,.22f,.88f),roof);
            roofObj.transform.rotation=Quaternion.Euler(0f,30f,0f);
            if(i%2==0)
                Part(parent,"House Banner",PrimitiveType.Cube,new Vector3(x-.78f,h*.82f,z),new Vector3(.06f,.36f,.28f),red);
        }

        // Lower terrace visually connects the houses to the citadel mass.
        Part(parent,"Citadel Lower Terrace",PrimitiveType.Cube,new Vector3(gateX+4.85f,.42f,0f),new Vector3(4.25f,.84f,9.10f),stone*.72f);
        Part(parent,"Citadel Middle Terrace",PrimitiveType.Cube,new Vector3(gateX+5.35f,1.02f,0f),new Vector3(3.65f,.62f,8.10f),stone*.78f);
        Part(parent,"Citadel Mass",PrimitiveType.Cube,new Vector3(gateX+5.65f,1.92f,0f),new Vector3(3.10f,2.35f,7.15f),stone*.82f);

        for(int z=-2;z<=2;z+=2)
            Part(parent,"Citadel Crown",PrimitiveType.Cube,new Vector3(gateX+4.95f,3.22f,z*1.30f),new Vector3(.82f,.45f,.82f),paleStone);

        // Temple / palace focus rises above the skyline and is visible behind the gate.
        Vector3 temple = new Vector3(gateX+7.35f,2.45f,3.15f);
        Part(parent,"Troy Temple Plinth",PrimitiveType.Cube,temple+new Vector3(0f,-1.48f,0f),new Vector3(2.35f,.36f,2.65f),paleStone*.92f);
        for(int side=-1;side<=1;side+=2)
        for(int depth=-1;depth<=1;depth+=2)
            Part(parent,"Temple Column",PrimitiveType.Cylinder,temple+new Vector3(side*.72f,-.54f,depth*.78f),new Vector3(.12f,.88f,.12f),paleStone);
        Part(parent,"Temple Entablature",PrimitiveType.Cube,temple+new Vector3(0f,.35f,0f),new Vector3(2.15f,.24f,2.35f),paleStone);
        GameObject templeRoof=Part(parent,"Temple Roof",PrimitiveType.Cylinder,temple+new Vector3(0f,.68f,0f),new Vector3(1.35f,.30f,1.45f),roof);
        templeRoof.transform.rotation=Quaternion.Euler(0f,30f,0f);
        Part(parent,"Temple Sun Disc",PrimitiveType.Cylinder,temple+new Vector3(-1.10f,.72f,0f),new Vector3(.28f,.05f,.28f),bronze).transform.rotation=Quaternion.Euler(90f,0f,0f);

        // Back wall silhouettes fill gaps at the top of the gameplay frame.
        for(int i=-5;i<=5;i++)
        {
            float z=i*1.82f;
            Part(parent,"Inner Troy Wall",PrimitiveType.Cube,new Vector3(gateX+7.85f,1.05f,z),new Vector3(.60f,2.10f,1.65f),stone*.70f);
            if(i%2==0)
                Part(parent,"Inner Wall Merlon",PrimitiveType.Cube,new Vector3(gateX+7.52f,2.28f,z),new Vector3(.42f,.52f,.42f),paleStone*.92f);
        }
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
