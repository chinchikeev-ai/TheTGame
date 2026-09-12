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
        DefineLayout();
        CoastEnvironmentBuilder.Build();
        CreateGrid();
        CreateBase();
        CreateBuildPoints();
        Paths = CreatePaths();
        return Paths[0];
    }

    void DefineLayout()
    {
        roadCells.Clear();
        blockedCells.Clear();
        Vector2Int[] routeA = { C(0,8),C(1,8),C(2,8),C(3,8),C(4,8),C(5,8),C(5,7),C(5,6),C(6,6),C(7,6),C(8,6),C(9,6),C(10,6),C(11,6),C(12,6),C(13,6),C(14,6),C(15,6),C(16,6) };
        Vector2Int[] routeB = { C(0,3),C(1,3),C(2,3),C(3,3),C(4,3),C(5,3),C(5,4),C(5,5),C(6,5),C(7,5),C(8,5),C(9,5),C(10,5),C(11,5),C(12,5),C(13,5),C(13,6),C(14,6),C(15,6),C(16,6) };
        foreach(Vector2Int p in routeA) roadCells.Add(p);
        foreach(Vector2Int p in routeB) roadCells.Add(p);
        blockedCells.Add(C(17,6));
    }

    void CreateGrid()
    {
        GameObject root=new GameObject("Chapter01_Roads");
        foreach(Vector2Int cell in roadCells)
        {
            Vector3 p=CellToWorld(cell,-.045f);
            bool alt=((cell.x+cell.y)&1)==0;

            GameObject bed=GameObject.CreatePrimitive(PrimitiveType.Cube);
            bed.name=$"RoadBed_{cell.x}_{cell.y}";
            bed.transform.SetParent(root.transform);
            bed.transform.position=p;
            bed.transform.localScale=new Vector3(CellSize*.96f,.055f,CellSize*.96f);
            Object.Destroy(bed.GetComponent<Collider>());
            TowerFactory.SetColor(bed,alt?new Color(.38f,.31f,.22f):new Color(.34f,.28f,.20f));

            GameObject worn=GameObject.CreatePrimitive(PrimitiveType.Cube);
            worn.name=$"RoadWear_{cell.x}_{cell.y}";
            worn.transform.SetParent(root.transform);
            worn.transform.position=p+new Vector3(0f,.035f,0f);
            worn.transform.localScale=new Vector3(CellSize*.68f,.018f,CellSize*.78f);
            Object.Destroy(worn.GetComponent<Collider>());
            TowerFactory.SetColor(worn,alt?new Color(.50f,.42f,.30f):new Color(.46f,.38f,.27f));
        }
    }

    void CreateBase()
    {
        Vector3 p=CellToWorld(C(17,6),.35f);
        GameObject root=new GameObject("TroyGateComplex");

        GameObject lintel=GameObject.CreatePrimitive(PrimitiveType.Cube);
        lintel.name="Troy Gate Lintel";
        lintel.transform.SetParent(root.transform);
        lintel.transform.position=p+new Vector3(0f,1.15f,0f);
        lintel.transform.localScale=new Vector3(CellSize*.85f,.38f,CellSize*1.72f);
        Object.Destroy(lintel.GetComponent<Collider>());
        TowerFactory.SetColor(lintel,new Color(.62f,.48f,.28f));

        GameObject door=GameObject.CreatePrimitive(PrimitiveType.Cube);
        door.name="TroyGate";
        door.transform.SetParent(root.transform);
        door.transform.position=p+new Vector3(0f,.15f,0f);
        door.transform.localScale=new Vector3(CellSize*.36f,1.45f,CellSize*.95f);
        TowerFactory.SetColor(door,new Color(.30f,.16f,.08f));

        for(int i=-1;i<=1;i+=2)
        {
            GameObject tower=GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            tower.name="Troy Gate Tower";
            tower.transform.SetParent(root.transform);
            tower.transform.position=p+new Vector3(0f,.72f,i*CellSize*1.18f);
            tower.transform.localScale=new Vector3(.82f,1.35f,.82f);
            Object.Destroy(tower.GetComponent<Collider>());
            TowerFactory.SetColor(tower,new Color(.67f,.54f,.32f));

            GameObject cap=GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            cap.name="Tower Crown";
            cap.transform.SetParent(root.transform);
            cap.transform.position=tower.transform.position+Vector3.up*1.48f;
            cap.transform.localScale=new Vector3(.96f,.12f,.96f);
            Object.Destroy(cap.GetComponent<Collider>());
            TowerFactory.SetColor(cap,new Color(.76f,.62f,.37f));
        }
    }

    void CreateBuildPoints()
    {
        GameObject root=new GameObject("BuildableCells");
        for(int y=0;y<GridHeight;y++)
            for(int x=0;x<GridWidth;x++)
            {
                Vector2Int cell=C(x,y);
                if(roadCells.Contains(cell)||blockedCells.Contains(cell)) continue;
                GameObject build=new GameObject($"Build_{x}_{y}");
                build.transform.SetParent(root.transform);
                build.transform.position=CellToWorld(cell,0f);
                build.AddComponent<BuildPoint>().Initialize();
            }
    }

    Transform[][] CreatePaths()
    {
        Vector2Int[] a={C(0,8),C(2,8),C(5,8),C(5,6),C(9,6),C(13,6),C(16,6),C(17,6)};
        Vector2Int[] b={C(0,3),C(2,3),C(5,3),C(5,5),C(9,5),C(13,5),C(13,6),C(16,6),C(17,6)};
        return new[]{MakePath("Route_A",a),MakePath("Route_B",b)};
    }

    Transform[] MakePath(string name,Vector2Int[] cells)
    {
        GameObject root=new GameObject(name);
        List<Transform> points=new List<Transform>();
        foreach(Vector2Int cell in cells)
        {
            GameObject waypoint=new GameObject($"WP_{cell.x}_{cell.y}");
            waypoint.transform.SetParent(root.transform);
            waypoint.transform.position=CellToWorld(cell,.55f);
            points.Add(waypoint.transform);
        }
        return points.ToArray();
    }

    static Vector2Int C(int x,int y)=>new Vector2Int(x,y);

    public static Vector3 CellToWorld(Vector2Int cell,float height=0f)
    {
        float originX=-(GridWidth-1)*CellSize*.5f;
        float originZ=-(GridHeight-1)*CellSize*.5f;
        return new Vector3(originX+cell.x*CellSize,height,originZ+cell.y*CellSize);
    }

    public static Vector3 ClampToPlayableArea(Vector3 worldPosition,float margin=.65f)
    {
        Vector3 min=CellToWorld(new Vector2Int(0,0));
        Vector3 max=CellToWorld(new Vector2Int(GridWidth-1,GridHeight-1));
        worldPosition.x=Mathf.Clamp(worldPosition.x,min.x+margin,max.x-margin);
        worldPosition.z=Mathf.Clamp(worldPosition.z,min.z+margin,max.z-margin);
        return worldPosition;
    }
}
