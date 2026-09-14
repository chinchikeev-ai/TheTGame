using System.Collections.Generic;
using UnityEngine;

public class MapBuilder : MonoBehaviour
{
    public const int GridWidth = 18;
    public const int GridHeight = 12;
    public const float CellSize = 1.5f;

    static readonly Color PackedEarth = new Color(.39f,.315f,.215f);
    static readonly Color WornEarth = new Color(.51f,.41f,.285f);
    static readonly Color RutEarth = new Color(.29f,.235f,.17f);
    static readonly Color RoadStone = new Color(.38f,.36f,.31f);

    public Transform[][] Paths { get; private set; }

    readonly HashSet<Vector2Int> roadCells = new HashSet<Vector2Int>();
    readonly HashSet<Vector2Int> blockedCells = new HashSet<Vector2Int>();

    public Transform[] BuildMap()
    {
        if (TryAdoptExistingPaths()) return Paths[0];

        DefineLayout();
        CoastEnvironmentBuilder.Build();
        CreateGrid();
        CreateGameplayAnchors();
        CreateBuildPoints();
        Paths = CreatePaths();
        return Paths[0];
    }

    bool TryAdoptExistingPaths()
    {
        GameObject routeA = GameObject.Find("Route_A");
        GameObject routeB = GameObject.Find("Route_B");
        if (routeA == null || routeB == null) return false;

        Transform[] a = ReadPath(routeA.transform);
        Transform[] b = ReadPath(routeB.transform);
        if (a.Length == 0 || b.Length == 0) return false;

        DefineLayout();
        Paths = new[] { a, b };
        return true;
    }

    static Transform[] ReadPath(Transform root)
    {
        Transform[] path = new Transform[root.childCount];
        for (int i = 0; i < root.childCount; i++) path[i] = root.GetChild(i);
        return path;
    }

    void DefineLayout()
    {
        roadCells.Clear();
        blockedCells.Clear();
        Vector2Int[] routeA = { C(0,8),C(1,8),C(2,8),C(3,8),C(4,8),C(5,8),C(5,7),C(5,6),C(6,6),C(7,6),C(8,6),C(9,6),C(10,6),C(11,6),C(12,6),C(13,6),C(14,6),C(15,6),C(16,6) };
        Vector2Int[] routeB = { C(0,3),C(1,3),C(2,3),C(3,3),C(4,3),C(5,3),C(5,4),C(5,5),C(6,5),C(7,5),C(8,5),C(9,5),C(10,5),C(11,5),C(12,5),C(13,5),C(13,6),C(14,6),C(15,6),C(16,6) };
        foreach (Vector2Int p in routeA) roadCells.Add(p);
        foreach (Vector2Int p in routeB) roadCells.Add(p);
        blockedCells.Add(C(17,6));
    }

    void CreateGrid()
    {
        GameObject root = new GameObject("Chapter01_Roads");
        Vector2Int[] routeA = { C(0,8),C(5,8),C(5,6),C(13,6),C(16,6) };
        Vector2Int[] routeB = { C(0,3),C(5,3),C(5,5),C(13,5),C(13,6),C(16,6) };
        CreateRoadRibbon(root.transform,routeA,"Upper Battle Track",11);
        CreateRoadRibbon(root.transform,routeB,"Lower Battle Track",23);

        CreateRoadJunction(root.transform,C(5,6),1.42f,-8f);
        CreateRoadJunction(root.transform,C(13,6),1.58f,7f);
        CreateRoadJunction(root.transform,C(16,6),1.46f,-3f);
    }

    void CreateRoadRibbon(Transform parent, Vector2Int[] cells, string name, int routeSeed)
    {
        GameObject routeRoot = new GameObject(name);
        routeRoot.transform.SetParent(parent,false);

        for (int i = 0; i < cells.Length - 1; i++)
        {
            Vector3 a = CellToWorld(cells[i],-.045f);
            Vector3 b = CellToWorld(cells[i+1],-.045f);
            int seed = routeSeed + i * 7;
            float width = CellSize * 1.13f;

            CreateIrregularGroundRibbon(routeRoot.transform,a,b,width,seed,PackedEarth,.015f,1.0f,"Trampled Route");
            CreateIrregularGroundRibbon(routeRoot.transform,a+Vector3.up*.024f,b+Vector3.up*.024f,width*.62f,seed+17,WornEarth,.011f,.86f,"Worn Route Center");
            CreateWheelRuts(routeRoot.transform,a,b,width,seed);
            CreateFootScuffs(routeRoot.transform,a,b,width,seed);

            if (i % 3 != 1)
            {
                Vector3 mid = Vector3.Lerp(a,b,.42f + Mathf.Sin(seed*.37f)*.08f);
                Vector3 dir = (b-a).normalized;
                Vector3 side = new Vector3(-dir.z,0f,dir.x);
                AddRoadStone(routeRoot.transform,mid+side*CellSize*(.55f+.06f*Mathf.Sin(seed)),seed*17f,.88f+(i%2)*.16f);
                if (i % 3 == 0)
                    AddRoadStone(routeRoot.transform,mid-side*CellSize*.60f,seed*23f+19f,.72f);
            }
        }
    }

    void CreateIrregularGroundRibbon(Transform parent, Vector3 a, Vector3 b, float width, int seed, Color color, float height, float density, string name)
    {
        Vector3 delta = b-a;
        if (delta.sqrMagnitude < .001f) return;

        Vector3 dir = delta.normalized;
        Vector3 side = new Vector3(-dir.z,0f,dir.x);
        float length = delta.magnitude;
        int count = Mathf.Max(3,Mathf.CeilToInt(length/(1.55f/Mathf.Max(.45f,density))));
        Quaternion rotation = Quaternion.LookRotation(dir,Vector3.up);

        for (int i = 0; i < count; i++)
        {
            float t = (i+.5f)/count;
            float wave = Mathf.Sin(seed*.83f+i*1.91f);
            float secondWave = Mathf.Sin(seed*.29f+i*2.63f);
            float offset = wave*width*.11f;
            float patchWidth = width*(.70f+.10f*secondWave);
            float patchLength = (length/count)*(1.22f+.12f*Mathf.Sin(i*1.43f+seed));
            GameObject patch = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            patch.name = name;
            patch.transform.SetParent(parent,false);
            patch.transform.position = Vector3.Lerp(a,b,t)+side*offset+Vector3.up*height;
            patch.transform.rotation = rotation * Quaternion.Euler(0f,wave*5f,0f);
            patch.transform.localScale = new Vector3(patchWidth,.028f,patchLength);
            Object.Destroy(patch.GetComponent<Collider>());
            TowerFactory.SetColor(patch,color*(.94f+(i%3)*.025f));
        }
    }

    void CreateWheelRuts(Transform parent, Vector3 a, Vector3 b, float roadWidth, int seed)
    {
        Vector3 delta = b-a;
        if (delta.sqrMagnitude < .001f) return;

        Vector3 dir = delta.normalized;
        Vector3 side = new Vector3(-dir.z,0f,dir.x);
        float drift = Mathf.Sin(seed*.71f)*roadWidth*.035f;
        float separation = roadWidth*.19f;
        CreateBrokenRoadMark(parent,a+side*(separation+drift),b+side*(separation+drift),seed,"Cart Rut");
        CreateBrokenRoadMark(parent,a-side*(separation-drift),b-side*(separation-drift),seed+31,"Cart Rut");
    }

    void CreateBrokenRoadMark(Transform parent, Vector3 a, Vector3 b, int seed, string name)
    {
        Vector3 delta = b-a;
        if (delta.sqrMagnitude < .001f) return;
        Vector3 dir = delta.normalized;
        Vector3 side = new Vector3(-dir.z,0f,dir.x);
        float length = delta.magnitude;
        int count = Mathf.Max(2,Mathf.CeilToInt(length/1.45f));
        Quaternion rotation = Quaternion.LookRotation(dir,Vector3.up);

        for (int i = 0; i < count; i++)
        {
            if (((i+seed)&3)==2) continue;
            float t=(i+.5f)/count;
            float drift=Mathf.Sin(seed*.47f+i*1.61f)*.035f;
            GameObject mark=GameObject.CreatePrimitive(PrimitiveType.Sphere);
            mark.name=name;
            mark.transform.SetParent(parent,false);
            mark.transform.position=Vector3.Lerp(a,b,t)+side*drift+Vector3.up*.047f;
            mark.transform.rotation=rotation;
            mark.transform.localScale=new Vector3(.075f,.008f,(length/count)*.62f);
            Object.Destroy(mark.GetComponent<Collider>());
            TowerFactory.SetColor(mark,RutEarth*(.94f+(i%2)*.04f));
        }
    }

    void CreateFootScuffs(Transform parent, Vector3 a, Vector3 b, float width, int seed)
    {
        Vector3 delta=b-a;
        if(delta.sqrMagnitude<.001f) return;
        Vector3 dir=delta.normalized;
        Vector3 side=new Vector3(-dir.z,0f,dir.x);
        int count=Mathf.Clamp(Mathf.CeilToInt(delta.magnitude*.58f),3,8);
        float yaw=Mathf.Atan2(dir.x,dir.z)*Mathf.Rad2Deg;
        for(int i=0;i<count;i++)
        {
            float t=(i+.35f)/count;
            float lane=Mathf.Sin(seed*.71f+i*2.17f)*width*.29f;
            GameObject scuff=GameObject.CreatePrimitive(PrimitiveType.Sphere);
            scuff.name="Foot Traffic Scuff";
            scuff.transform.SetParent(parent,false);
            scuff.transform.position=Vector3.Lerp(a,b,t)+side*lane+Vector3.up*.044f;
            scuff.transform.rotation=Quaternion.Euler(0f,yaw+(i%2==0?-10f:9f),0f);
            scuff.transform.localScale=new Vector3(.11f,.006f,.22f+(i%3)*.035f);
            Object.Destroy(scuff.GetComponent<Collider>());
            TowerFactory.SetColor(scuff,RutEarth*.91f);
        }
    }

    void CreateRoadJunction(Transform parent, Vector2Int cell, float scale, float yaw)
    {
        Vector3 p = CellToWorld(cell,-.020f);
        GameObject outer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        outer.name = "Trampled Road Junction";
        outer.transform.SetParent(parent,false);
        outer.transform.position = p;
        outer.transform.rotation = Quaternion.Euler(0f,yaw,0f);
        outer.transform.localScale = new Vector3(CellSize*1.55f*scale,.050f,CellSize*1.12f*scale);
        Object.Destroy(outer.GetComponent<Collider>());
        TowerFactory.SetColor(outer,PackedEarth*.94f);

        GameObject center = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        center.name = "Worn Junction Center";
        center.transform.SetParent(parent,false);
        center.transform.position = p+Vector3.up*.028f;
        center.transform.rotation = Quaternion.Euler(0f,yaw+11f,0f);
        center.transform.localScale = new Vector3(CellSize*.88f*scale,.028f,CellSize*.69f*scale);
        Object.Destroy(center.GetComponent<Collider>());
        TowerFactory.SetColor(center,WornEarth);
    }

    void AddRoadStone(Transform parent, Vector3 p, float yaw, float scale)
    {
        GameObject stone = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        stone.name = "Roadside Stone";
        stone.transform.SetParent(parent,false);
        stone.transform.position = p+Vector3.up*.03f;
        stone.transform.localScale = new Vector3(.28f,.11f,.21f)*scale;
        stone.transform.rotation = Quaternion.Euler(0f,yaw,0f);
        Object.Destroy(stone.GetComponent<Collider>());
        TowerFactory.SetColor(stone,RoadStone*(.94f+.04f*Mathf.Abs(Mathf.Sin(yaw))));
    }

    void CreateGameplayAnchors()
    {
        GameObject root = new GameObject("Chapter01_GameplayAnchors");
        GameObject gateAnchor = new GameObject("TroyGateGameplayAnchor");
        gateAnchor.transform.SetParent(root.transform,false);
        gateAnchor.transform.position = CellToWorld(C(17,6),.35f);

        GameObject impactAnchor = new GameObject("GateImpactAnchor");
        impactAnchor.transform.SetParent(gateAnchor.transform,false);
        impactAnchor.transform.localPosition = new Vector3(-.45f,.55f,0f);
    }

    void CreateBuildPoints()
    {
        GameObject root = new GameObject("BuildableCells");
        for (int y = 0; y < GridHeight; y++)
            for (int x = 0; x < GridWidth; x++)
            {
                Vector2Int cell = C(x,y);
                if (roadCells.Contains(cell) || blockedCells.Contains(cell)) continue;
                GameObject build = new GameObject($"Build_{x}_{y}");
                build.transform.SetParent(root.transform);
                build.transform.position = CellToWorld(cell,0f);
                build.AddComponent<BuildPoint>().Initialize();
            }
    }

    Transform[][] CreatePaths()
    {
        Vector2Int[] a = { C(0,8),C(2,8),C(5,8),C(5,6),C(9,6),C(13,6),C(16,6),C(17,6) };
        Vector2Int[] b = { C(0,3),C(2,3),C(5,3),C(5,5),C(9,5),C(13,5),C(13,6),C(16,6),C(17,6) };
        return new[] { MakePath("Route_A",a), MakePath("Route_B",b) };
    }

    Transform[] MakePath(string name, Vector2Int[] cells)
    {
        GameObject root = new GameObject(name);
        List<Transform> points = new List<Transform>();
        foreach (Vector2Int cell in cells)
        {
            GameObject waypoint = new GameObject($"WP_{cell.x}_{cell.y}");
            waypoint.transform.SetParent(root.transform);
            waypoint.transform.position = CellToWorld(cell,.55f);
            points.Add(waypoint.transform);
        }
        return points.ToArray();
    }

    static Vector2Int C(int x, int y) => new Vector2Int(x,y);

    public static Vector3 CellToWorld(Vector2Int cell, float height = 0f)
    {
        float originX = -(GridWidth-1)*CellSize*.5f;
        float originZ = -(GridHeight-1)*CellSize*.5f;
        return new Vector3(originX+cell.x*CellSize,height,originZ+cell.y*CellSize);
    }

    public static Vector3 ClampToPlayableArea(Vector3 worldPosition, float margin = .65f)
    {
        Vector3 min = CellToWorld(new Vector2Int(0,0));
        Vector3 max = CellToWorld(new Vector2Int(GridWidth-1,GridHeight-1));
        worldPosition.x = Mathf.Clamp(worldPosition.x,min.x+margin,max.x-margin);
        worldPosition.z = Mathf.Clamp(worldPosition.z,min.z+margin,max.z-margin);
        return worldPosition;
    }
}
