using System;
using System.Collections;
using UnityEngine;

// Presentation-only faction/material pass for the real Chapter I tactical view.
// It restages existing authored/procedural scenery after the normal presenters
// have spawned, then adds a few large readable faction landmarks. No gameplay
// colliders, routes, placement cells or combat data are modified here.
public sealed class ChapterOneFactionStaging : MonoBehaviour
{
    static readonly Color TrojanRed = new Color(.58f,.055f,.035f);
    static readonly Color TrojanDeepRed = new Color(.34f,.025f,.020f);
    static readonly Color TrojanWarmStone = new Color(.66f,.49f,.29f);
    static readonly Color TrojanPaleStone = new Color(.79f,.63f,.39f);
    static readonly Color TrojanBronze = new Color(.80f,.54f,.18f);
    static readonly Color TrojanGold = new Color(.90f,.65f,.22f);

    static readonly Color GreekBlue = new Color(.20f,.34f,.58f);
    static readonly Color GreekDarkBlue = new Color(.10f,.20f,.38f);
    static readonly Color GreekPaleCloth = new Color(.76f,.78f,.72f);
    static readonly Color GreekCoolBronze = new Color(.64f,.50f,.28f);

    static readonly Color Wood = new Color(.29f,.17f,.075f);
    static readonly Color Fire = new Color(1f,.34f,.045f);
    static readonly Color FireHot = new Color(1f,.61f,.08f);

    IEnumerator Start()
    {
        // Existing coast/gate/city presentation owners also build during Start.
        // Waiting two frames makes this a final art-direction layer instead of
        // competing with their construction order.
        yield return null;
        yield return null;

        if (GameManager.Instance != null && GameManager.Instance.MapNumber != 1)
        {
            UnityEngine.Object.Destroy(gameObject);
            yield break;
        }

        if (GameObject.Find("Chapter01_FactionStaging") != null)
        {
            UnityEngine.Object.Destroy(gameObject);
            yield break;
        }

        GameObject root = new GameObject("Chapter01_FactionStaging");
        RestageGreekMaterials();
        RestageTrojanMaterials();
        BuildGreekStandards(root.transform);
        BuildTrojanApproach(root.transform);
    }

    static void RestageGreekMaterials()
    {
        RestageGreekHierarchy(GameObject.Find("Chapter01_CoastEnvironment"));
        RestageGreekHierarchy(GameObject.Find("Chapter01_CoastalDressing"));
    }

    static void RestageGreekHierarchy(GameObject root)
    {
        if (root == null) return;

        foreach (Renderer renderer in root.GetComponentsInChildren<Renderer>(true))
        {
            if (renderer == null) continue;
            string n = renderer.gameObject.name;

            if (Contains(n,"Sail Left") || Contains(n,"Sail Right"))
                TowerFactory.SetColor(renderer.gameObject,GreekPaleCloth);
            else if (Contains(n,"Sail Center Stripe") || Contains(n,"Sail Top Border") || Contains(n,"Sail Bottom Border") || Contains(n,"Painted Stripe") || Contains(n,"Stern Fin"))
                TowerFactory.SetColor(renderer.gameObject,GreekBlue);
            else if (Contains(n,"Blue Command Canopy") || Contains(n,"Tent Left Roof") || Contains(n,"Tent Right Roof"))
                TowerFactory.SetColor(renderer.gameObject,GreekBlue);
            else if (Contains(n,"Tent Ground Cloth") || Contains(n,"Canopy Dark Border"))
                TowerFactory.SetColor(renderer.gameObject,GreekDarkBlue);
            else if (Contains(n,"Racked Greek Shield"))
                TowerFactory.SetColor(renderer.gameObject,GreekBlue);
            else if (Contains(n,"Rolled Cloth") || Contains(n,"Greek Camp Ground Cloth"))
                TowerFactory.SetColor(renderer.gameObject,GreekPaleCloth);
            else if (Contains(n,"Bronze") && (Contains(n,"Spear") || Contains(n,"Shield") || Contains(n,"Canopy") || Contains(n,"Mark")))
                TowerFactory.SetColor(renderer.gameObject,GreekCoolBronze);
        }
    }

    static void RestageTrojanMaterials()
    {
        RestageTrojanHierarchy(GameObject.Find("Chapter01_TroyGateHero"));
        RestageTrojanHierarchy(GameObject.Find("Chapter01_TroyCityBackdrop"));
    }

    static void RestageTrojanHierarchy(GameObject root)
    {
        if (root == null) return;

        foreach (Renderer renderer in root.GetComponentsInChildren<Renderer>(true))
        {
            if (renderer == null) continue;
            string n = renderer.gameObject.name;

            if (Contains(n,"Banner") || Contains(n,"Red Cloth") || Contains(n,"Roof"))
            {
                TowerFactory.SetColor(renderer.gameObject,Contains(n,"Roof") ? TrojanDeepRed : TrojanRed);
                continue;
            }

            if (Contains(n,"Gold") || Contains(n,"Bronze") || Contains(n,"Emblem") || Contains(n,"Band") || Contains(n,"Trim"))
            {
                TowerFactory.SetColor(renderer.gameObject,Contains(n,"Gold") ? TrojanGold : TrojanBronze);
                continue;
            }

            if (ContainsAny(n,"Wall","Stone","Palace","House","Citadel","Gatehouse","Tower","Temple","Terrace","Buttress","Jamb","Lintel","Crenellation","Masonry"))
            {
                int variant = Mathf.Abs(n.GetHashCode()) & 1;
                TowerFactory.SetColor(renderer.gameObject,variant == 0 ? TrojanWarmStone : TrojanPaleStone);
            }
        }
    }

    static void BuildGreekStandards(Transform parent)
    {
        Transform root = new GameObject("Greek Beachhead Identity").transform;
        root.SetParent(parent,false);

        CreateGreekStandard(root,new Vector3(-10.35f,.04f,5.05f),-7f,1.0f);
        CreateGreekStandard(root,new Vector3(-9.85f,.04f,-4.85f),8f,.94f);
        CreateGreekStandard(root,new Vector3(-7.95f,.04f,7.20f),4f,.82f);

        for (int side=-1; side<=1; side+=2)
        {
            Vector3 p = new Vector3(-8.55f,.035f,side*5.95f);
            Part(root,"Greek Pale Ground Cloth",PrimitiveType.Cube,p,new Vector3(1.20f,.018f,.62f),GreekPaleCloth,Quaternion.Euler(0f,side*11f,0f));
            GameObject shield = Part(root,"Greek Blue Ground Shield",PrimitiveType.Cylinder,p+new Vector3(side*.34f,.10f,.16f),new Vector3(.31f,.035f,.31f),GreekBlue,Quaternion.Euler(82f,side*14f,side*7f));
            Part(shield.transform,"Greek Ground Shield Boss",PrimitiveType.Sphere,new Vector3(0f,.075f,0f),new Vector3(.14f,.055f,.14f),GreekCoolBronze);
        }
    }

    static void BuildTrojanApproach(Transform parent)
    {
        Transform root = new GameObject("Trojan Gate Approach Identity").transform;
        root.SetParent(parent,false);
        float gateX = MapBuilder.CellToWorld(new Vector2Int(17,6)).x;

        CreateTrojanStandard(root,new Vector3(gateX-3.45f,.05f,-4.75f),9f,1.05f);
        CreateTrojanStandard(root,new Vector3(gateX-3.45f,.05f,5.95f),-9f,1.05f);
        CreateTrojanStandard(root,new Vector3(gateX-5.10f,.05f,-6.35f),6f,.86f);
        CreateTrojanStandard(root,new Vector3(gateX-5.10f,.05f,7.35f),-6f,.86f);

        CreateTrojanBrazier(root,new Vector3(gateX-2.55f,.08f,-3.10f),.92f);
        CreateTrojanBrazier(root,new Vector3(gateX-2.55f,.08f,4.60f),.92f);

        Vector3[] stones =
        {
            new Vector3(gateX-4.65f,.06f,-5.00f),
            new Vector3(gateX-4.15f,.06f,5.55f),
            new Vector3(gateX-2.95f,.06f,-5.55f),
            new Vector3(gateX-3.20f,.06f,6.20f)
        };
        for(int i=0;i<stones.Length;i++)
            Part(root,"Trojan Warm Approach Stone",PrimitiveType.Sphere,stones[i],new Vector3(.58f+(i%2)*.12f,.18f,.42f+(i%3)*.07f),i%2==0?TrojanWarmStone:TrojanPaleStone,Quaternion.Euler(0f,i*37f-11f,0f));
    }

    static void CreateGreekStandard(Transform parent,Vector3 position,float yaw,float scale)
    {
        Transform root = new GameObject("Greek Blue Standard").transform;
        root.SetParent(parent,false);
        root.position=position;
        root.rotation=Quaternion.Euler(0f,yaw,0f);
        root.localScale=Vector3.one*scale;

        Part(root,"Greek Standard Pole",PrimitiveType.Cylinder,new Vector3(0f,.80f,0f),new Vector3(.028f,.80f,.028f),Wood);
        GameObject banner=Part(root,"Greek Blue Banner",PrimitiveType.Cube,new Vector3(.16f,1.22f,0f),new Vector3(.31f,.48f,.045f),GreekBlue);
        Part(root,"Greek Pale Banner Stripe",PrimitiveType.Cube,new Vector3(.16f,1.22f,-.028f),new Vector3(.07f,.48f,.018f),GreekPaleCloth);
        Part(root,"Greek Bronze Finial",PrimitiveType.Sphere,new Vector3(0f,1.64f,0f),Vector3.one*.10f,GreekCoolBronze);
        banner.AddComponent<ChapterOneAmbientMotion>().kind=ChapterOneAmbientMotion.MotionKind.Banner;
    }

    static void CreateTrojanStandard(Transform parent,Vector3 position,float yaw,float scale)
    {
        Transform root = new GameObject("Trojan Red Standard").transform;
        root.SetParent(parent,false);
        root.position=position;
        root.rotation=Quaternion.Euler(0f,yaw,0f);
        root.localScale=Vector3.one*scale;

        Part(root,"Trojan Standard Pole",PrimitiveType.Cylinder,new Vector3(0f,.86f,0f),new Vector3(.030f,.86f,.030f),Wood);
        GameObject banner=Part(root,"Trojan Red Banner",PrimitiveType.Cube,new Vector3(.18f,1.31f,0f),new Vector3(.36f,.56f,.050f),TrojanRed);
        Part(root,"Trojan Banner Bronze Top",PrimitiveType.Cube,new Vector3(.18f,1.60f,0f),new Vector3(.38f,.055f,.060f),TrojanBronze);
        Part(root,"Trojan Banner Gold Emblem",PrimitiveType.Cylinder,new Vector3(.18f,1.34f,-.032f),new Vector3(.095f,.022f,.095f),TrojanGold,Quaternion.Euler(90f,0f,0f));
        Part(root,"Trojan Bronze Finial",PrimitiveType.Sphere,new Vector3(0f,1.78f,0f),Vector3.one*.11f,TrojanBronze);
        banner.AddComponent<ChapterOneAmbientMotion>().kind=ChapterOneAmbientMotion.MotionKind.Banner;
    }

    static void CreateTrojanBrazier(Transform parent,Vector3 position,float scale)
    {
        Transform root = new GameObject("Trojan Approach Brazier").transform;
        root.SetParent(parent,false);
        root.position=position;
        root.localScale=Vector3.one*scale;

        Part(root,"Brazier Stone Pedestal",PrimitiveType.Cube,new Vector3(0f,.28f,0f),new Vector3(.40f,.55f,.40f),TrojanWarmStone);
        Part(root,"Brazier Bronze Bowl",PrimitiveType.Cylinder,new Vector3(0f,.62f,0f),new Vector3(.34f,.12f,.34f),TrojanBronze);
        GameObject flame=Part(root,"Brazier Flame",PrimitiveType.Sphere,new Vector3(0f,.91f,0f),new Vector3(.22f,.42f,.22f),Fire);
        Part(root,"Brazier Hot Core",PrimitiveType.Sphere,new Vector3(.03f,.88f,-.02f),new Vector3(.12f,.27f,.12f),FireHot);
        flame.AddComponent<ChapterOneAmbientMotion>().kind=ChapterOneAmbientMotion.MotionKind.Flame;
    }

    static bool Contains(string value,string token)
    {
        return value.IndexOf(token,StringComparison.OrdinalIgnoreCase)>=0;
    }

    static bool ContainsAny(string value,params string[] tokens)
    {
        for(int i=0;i<tokens.Length;i++)
            if(Contains(value,tokens[i])) return true;
        return false;
    }

    static GameObject Part(Transform parent,string name,PrimitiveType type,Vector3 position,Vector3 scale,Color color,Quaternion? rotation=null)
    {
        GameObject go=GameObject.CreatePrimitive(type);
        go.name=name;
        go.transform.SetParent(parent,false);
        go.transform.localPosition=position;
        go.transform.localScale=scale;
        if(rotation.HasValue) go.transform.localRotation=rotation.Value;
        Collider collider=go.GetComponent<Collider>();
        if(collider!=null) UnityEngine.Object.Destroy(collider);
        TowerFactory.SetColor(go,color);
        return go;
    }
}
