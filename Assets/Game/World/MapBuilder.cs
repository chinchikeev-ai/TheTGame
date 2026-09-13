using System.Collections.Generic;
using UnityEngine;

public class MapBuilder : MonoBehaviour
{
    public const int GridWidth = 18;
    public const int GridHeight = 12;
    public const float CellSize = 1.5f;

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
        CreateRoadRibbon(root.transform,routeA,"Upper Track",11);
        CreateRoadRibbon(root.transform,routeB,"Lower Track",23);

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

            CreateRoadSegment(routeRoot.transform,a,b,CellSize*1.08f,new Color(.35f,.285f,.20f),"Packed Earth");
            CreateIrregularShoulder(routeRoot.transform,a,b,CellSize*1.08f,seed);
            CreateRoadSegment(routeRoot.transform,a+Vector3.up*.026f,b+Vector3.up*.026f,CellSize*.57f,new Color(.48f,.39f,.275f),"Worn Center");
            CreateWheelRuts(routeRoot.transform,a,b,CellSize*1.08f,seed);

            if (i % 3 != 1)
            {
                Vector3 mid = Vector3.Lerp(a,b,.42f + Mathf.Sin(seed*.37f)*.08f);
                Vector3 dir = (b-a).normalized;
                Vector3 side = new Vector3(-dir.z,0f,dir.x);
                float sign = ((i + routeSeed) & 1) == 0 ? 1f : -1f;
                AddRoadStone(routeRoot.transform,mid+side*CellSize*(.53f+.05f*Mathf.Sin(seed)),seed*17f);
                if (i % 3 == 0)
                    AddRoadStone(routeRoot.transform,mid-side*CellSize*.58f,seed*23f+19f);
            }
        }
    }

    void CreateIrregularShoulder(Transform parent, Vector3 a, Vector3 b, float width, int seed)
    {
        Vector3 delta = b-a;
        if (delta.sqrMagnitude < .001f) return;

        Vector3 dir = delta.normalized;
        Vector3 side = new Vector3(-dir.z,0f,dir.x);
        float length = delta.magnitude;
        int count = Mathf.Max(2,Mathf.CeilToInt(length/2.25f));
        Quaternion rotation = Quaternion.LookRotation(dir,Vector3.up);
        Color shoulder = new Color(.405f,.335f,.235f);

        for (int i = 0; i < count; i++)
        {
            float t = (i+.5f)/count;
            float wave = Mathf.Sin(seed*.83f+i*1.91f);
            float offset = wave*width*.13f;
            float patchWidth = width*(.78f+.08f*Mathf.Sin(seed*.21f+i*2.7f));
            float patchLength = (length/count)*(1.05f+.12f*Mathf.Sin(i*1.43f+seed));
            GameObject patch = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            patch.name = "Irregular Road Shoulder";
            patch.transform.SetParent(parent,false);
            patch.transform.position = Vector3.Lerp(a,b,t)+side*offset+Vector3.up*.018f;
            patch.transform.rotation = rotation;
            patch.transform.localScale = new Vector3(patchWidth,.035f,patchLength);
            Object.Destroy(patch.GetComponent<Collider>());
            TowerFactory.SetColor(patch,shoulder*(.94f+(i%3)*.025f));
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
        CreateRoadMark(parent,a+side*(separation+drift),b+side*(separation+drift),"Cart Rut");
        CreateRoadMark(parent,a-side*(separation-drift),b-side*(separation-drift),"Cart Rut");
    }

    void CreateRoadMark(Transform parent, Vector3 a, Vector3 b, string name)
    {
        Vector3 delta = b-a;
        float length = delta.magnitude;
        GameObject mark = GameObject.CreatePrimitive(PrimitiveType.Cube);
        mark.name = name;
        mark.transform.SetParent(parent,false);
        mark.transform.position = (a+b)*.5f+Vector3.up*.046f;
        mark.transform.localScale = new Vector3(.075f,.012f,length+.08f);
        mark.transform.rotation = Quaternion.LookRotation(delta.normalized,Vector3.up);
        Object.Destroy(mark.GetComponent<Collider>());
        TowerFactory.SetColor(mark,new Color(.285f,.235f,.175f));
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
        TowerFactory.SetColor(outer,new Color(.355f,.285f,.20f));

        GameObject center = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        center.name = "Worn Junction Center";
        center.transform.SetParent(parent,false);
        center.transform.position = p+Vector3.up*.028f;
        center.transform.rotation = Quaternion.Euler(0f,yaw+11f,0f);
        center.transform.localScale = new Vector3(CellSize*.88f*scale,.028f,CellSize*.69f*scale);
        Object.Destroy(center.GetComponent<Collider>());
        TowerFactory.SetColor(center,new Color(.49f,.40f,.285f));
    }

    void CreateRoadSegment(Transform parent, Vector3 a, Vector3 b, float width, Color color, string name)
    {
        Vector3 delta = b-a;
        float length = delta.magnitude;
        GameObject segment = GameObject.CreatePrimitive(PrimitiveType.Cube);
        segment.name = name;
        segment.transform.SetParent(parent,false);
        segment.transform.position = (a+b)*.5f;
        segment.transform.localScale = new Vector3(width,.055f,length+width*.35f);
        if (delta.sqrMagnitude > .001f) segment.transform.rotation = Quaternion.LookRotation(delta.normalized,Vector3.up);
        Object.Destroy(segment.GetComponent<Collider>());
        TowerFactory.SetColor(segment,color);
    }

    void AddRoadStone(Transform parent, Vector3 p, float yaw)
    {
        GameObject stone = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        stone.name = "Roadside Stone";
        stone.transform.SetParent(parent,false);
        stone.transform.position = p+Vector3.up*.03f;
        stone.transform.localScale = new Vector3(.26f,.10f,.20f);
        stone.transform.rotation = Quaternion.Euler(0f,yaw,0f);
        Object.Destroy(stone.GetComponent<Collider>());
        TowerFactory.SetColor(stone,new Color(.33f,.31f,.27f));
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
