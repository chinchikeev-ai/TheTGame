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

        Vector2Int[] routeA = {
            C(0,8), C(1,8), C(2,8), C(3,8), C(4,8), C(5,8),
            C(5,7), C(5,6), C(6,6), C(7,6), C(8,6), C(9,6),
            C(10,6), C(11,6), C(12,6), C(13,6), C(14,6), C(15,6), C(16,6)
        };
        Vector2Int[] routeB = {
            C(0,3), C(1,3), C(2,3), C(3,3), C(4,3), C(5,3),
            C(5,4), C(5,5), C(6,5), C(7,5), C(8,5), C(9,5),
            C(10,5), C(11,5), C(12,5), C(13,5), C(13,6), C(14,6), C(15,6), C(16,6)
        };
        foreach (Vector2Int p in routeA) roadCells.Add(p);
        foreach (Vector2Int p in routeB) roadCells.Add(p);
        blockedCells.Add(C(17,6));
    }

    void CreateGrid()
    {
        GameObject root = new GameObject("Chapter01_Roads");
        foreach (Vector2Int cell in roadCells)
        {
            GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tile.name = $"Road_{cell.x}_{cell.y}";
            tile.transform.SetParent(root.transform);
            tile.transform.position = CellToWorld(cell, -0.045f);
            tile.transform.localScale = new Vector3(CellSize * .96f, .06f, CellSize * .96f);
            Object.Destroy(tile.GetComponent<Collider>());
            bool alt = ((cell.x + cell.y) & 1) == 0;
            TowerFactory.SetColor(tile, alt ? new Color(.40f,.34f,.25f) : new Color(.36f,.30f,.22f));
        }
    }

    void CreateBase()
    {
        Vector2Int baseCell = C(17,6);
        Vector3 p = CellToWorld(baseCell, .35f);

        GameObject gate = GameObject.CreatePrimitive(PrimitiveType.Cube);
        gate.name = "TroyGate";
        gate.transform.position = p;
        gate.transform.localScale = new Vector3(CellSize * .80f, 1.25f, CellSize * 1.8f);
        TowerFactory.SetColor(gate, new Color(.46f,.28f,.14f));

        for (int i = -1; i <= 1; i += 2)
        {
            GameObject tower = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            tower.name = "Troy Gate Tower";
            tower.transform.position = p + new Vector3(0f, .55f, i * CellSize * 1.20f);
            tower.transform.localScale = new Vector3(.75f, 1.1f, .75f);
            Object.Destroy(tower.GetComponent<Collider>());
            TowerFactory.SetColor(tower, new Color(.66f,.53f,.31f));
        }
    }

    void CreateBuildPoints()
    {
        GameObject root = new GameObject("BuildableCells");
        for (int y = 0; y < GridHeight; y++)
        {
            for (int x = 0; x < GridWidth; x++)
            {
                Vector2Int cell = C(x,y);
                if (roadCells.Contains(cell) || blockedCells.Contains(cell)) continue;
                GameObject build = new GameObject($"Build_{x}_{y}");
                build.transform.SetParent(root.transform);
                build.transform.position = CellToWorld(cell, 0f);
                build.AddComponent<BuildPoint>().Initialize();
            }
        }
    }

    Transform[][] CreatePaths()
    {
        Vector2Int[] a = { C(0,8), C(2,8), C(5,8), C(5,6), C(9,6), C(13,6), C(16,6), C(17,6) };
        Vector2Int[] b = { C(0,3), C(2,3), C(5,3), C(5,5), C(9,5), C(13,5), C(13,6), C(16,6), C(17,6) };
        return new[] { MakePath("Route_A", a), MakePath("Route_B", b) };
    }

    Transform[] MakePath(string name, Vector2Int[] cells)
    {
        GameObject root = new GameObject(name);
        List<Transform> points = new List<Transform>();
        foreach (Vector2Int cell in cells)
        {
            GameObject waypoint = new GameObject($"WP_{cell.x}_{cell.y}");
            waypoint.transform.SetParent(root.transform);
            waypoint.transform.position = CellToWorld(cell, .55f);
            points.Add(waypoint.transform);
        }
        return points.ToArray();
    }

    static Vector2Int C(int x, int y) => new Vector2Int(x,y);

    public static Vector3 CellToWorld(Vector2Int cell, float height = 0f)
    {
        float originX = -(GridWidth - 1) * CellSize * .5f;
        float originZ = -(GridHeight - 1) * CellSize * .5f;
        return new Vector3(originX + cell.x * CellSize, height, originZ + cell.y * CellSize);
    }

    public static Vector3 ClampToPlayableArea(Vector3 worldPosition, float margin = 0.65f)
    {
        Vector3 min = CellToWorld(new Vector2Int(0, 0));
        Vector3 max = CellToWorld(new Vector2Int(GridWidth - 1, GridHeight - 1));
        worldPosition.x = Mathf.Clamp(worldPosition.x, min.x + margin, max.x - margin);
        worldPosition.z = Mathf.Clamp(worldPosition.z, min.z + margin, max.z - margin);
        return worldPosition;
    }
}
