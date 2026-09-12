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

        // Reserved/special cells can be extended later for Hector nodes,
        // scenery, objectives, walls or scripted chapter objects.
        blockedCells.Add(C(17,6)); // Troy Gate / base.
    }

    void CreateGrid()
    {
        GameObject root = new GameObject("BalanceGrid");
        for (int y = 0; y < GridHeight; y++)
        {
            for (int x = 0; x < GridWidth; x++)
            {
                Vector2Int cell = C(x,y);
                GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
                tile.name = $"Cell_{x}_{y}";
                tile.transform.SetParent(root.transform);
                tile.transform.position = CellToWorld(cell, -0.08f);
                tile.transform.localScale = new Vector3(CellSize * 0.94f, 0.12f, CellSize * 0.94f);
                Object.Destroy(tile.GetComponent<Collider>());

                Color color;
                if (roadCells.Contains(cell))
                    color = ((x + y) & 1) == 0 ? new Color(0.48f,0.39f,0.27f) : new Color(0.43f,0.35f,0.24f);
                else if (blockedCells.Contains(cell))
                    color = new Color(0.48f,0.34f,0.16f);
                else
                    color = ((x + y) & 1) == 0 ? new Color(0.19f,0.31f,0.18f) : new Color(0.16f,0.27f,0.16f);
                TowerFactory.SetColor(tile, color);
            }
        }
    }

    void CreateBase()
    {
        Vector2Int baseCell = C(17,6);
        GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
        tile.name = "TroyGate";
        tile.transform.position = CellToWorld(baseCell, 0.15f);
        tile.transform.localScale = new Vector3(CellSize * 0.9f, 0.35f, CellSize * 0.9f);
        TowerFactory.SetColor(tile, new Color(0.78f,0.62f,0.25f));
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
        Vector2Int[] a = {
            C(0,8), C(2,8), C(5,8), C(5,6), C(9,6), C(13,6), C(16,6), C(17,6)
        };
        Vector2Int[] b = {
            C(0,3), C(2,3), C(5,3), C(5,5), C(9,5), C(13,5), C(13,6), C(16,6), C(17,6)
        };
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
            waypoint.transform.position = CellToWorld(cell, 0.55f);
            points.Add(waypoint.transform);
        }
        return points.ToArray();
    }

    static Vector2Int C(int x, int y) => new Vector2Int(x,y);

    public static Vector3 CellToWorld(Vector2Int cell, float height = 0f)
    {
        float originX = -(GridWidth - 1) * CellSize * 0.5f;
        float originZ = -(GridHeight - 1) * CellSize * 0.5f;
        return new Vector3(originX + cell.x * CellSize, height, originZ + cell.y * CellSize);
    }
}
