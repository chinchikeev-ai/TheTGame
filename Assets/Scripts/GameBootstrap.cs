using System.Collections.Generic;
using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoStart()
    {
        if (FindFirstObjectByType<GameBootstrap>() == null)
            new GameObject("GameBootstrap").AddComponent<GameBootstrap>();
    }

    void Start()
    {
        if (FindFirstObjectByType<GameManager>() != null) return;

        SetupLightingAndCamera();
        new GameObject("GameManager").AddComponent<GameManager>();
        new GameObject("HUD").AddComponent<GameHUD>();

        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Ground";
        ground.transform.position = Vector3.zero;
        ground.transform.localScale = new Vector3(2.2f, 1f, 1.4f);

        CreateRoad();
        CreateBase();
        Transform[] path = CreateWaypoints();

        GameObject spawnerObj = new GameObject("EnemySpawner");
        EnemySpawner spawner = spawnerObj.AddComponent<EnemySpawner>();
        spawner.spawnPoint = path[0];
        spawner.waypoints = path;

        TowerPlacement placement = new GameObject("TowerPlacement").AddComponent<TowerPlacement>();
        placement.gameCamera = Camera.main;

        // One free starter tower.
        CreateStarterTower(new Vector3(-2f, 0.5f, -3f));
        spawner.Begin();
    }

    void SetupLightingAndCamera()
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            GameObject c = new GameObject("Main Camera");
            cam = c.AddComponent<Camera>();
            c.tag = "MainCamera";
        }
        cam.transform.position = new Vector3(0f, 17f, -15f);
        cam.transform.rotation = Quaternion.Euler(48f, 0f, 0f);
        cam.fieldOfView = 55f;

        if (FindFirstObjectByType<Light>() == null)
        {
            GameObject l = new GameObject("Directional Light");
            Light lightComp = l.AddComponent<Light>();
            lightComp.type = LightType.Directional;
            lightComp.intensity = 1.2f;
            l.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        }
    }

    void CreateRoad()
    {
        Vector3[] centers = {
            new Vector3(-7f, 0.06f, 2f),
            new Vector3(-2.5f, 0.06f, 2f),
            new Vector3(1f, 0.06f, 0f),
            new Vector3(4.5f, 0.06f, -2f),
            new Vector3(7f, 0.06f, -2f)
        };
        Vector3[] scales = {
            new Vector3(5f, 0.12f, 1.6f),
            new Vector3(4f, 0.12f, 1.6f),
            new Vector3(4.5f, 0.12f, 1.6f),
            new Vector3(4f, 0.12f, 1.6f),
            new Vector3(3f, 0.12f, 1.6f)
        };

        for (int i = 0; i < centers.Length; i++)
        {
            GameObject r = GameObject.CreatePrimitive(PrimitiveType.Cube);
            r.name = "Road";
            r.transform.position = centers[i];
            r.transform.localScale = scales[i];
        }
    }

    Transform[] CreateWaypoints()
    {
        List<Transform> list = new List<Transform>();
        Vector3[] points = {
            new Vector3(-9f, 0.8f, 2f),
            new Vector3(-4f, 0.8f, 2f),
            new Vector3(-1f, 0.8f, 2f),
            new Vector3(1f, 0.8f, 0f),
            new Vector3(4f, 0.8f, -2f),
            new Vector3(8f, 0.8f, -2f)
        };
        foreach (Vector3 p in points)
        {
            GameObject w = new GameObject("Waypoint");
            w.transform.position = p;
            list.Add(w.transform);
        }
        return list.ToArray();
    }

    void CreateBase()
    {
        GameObject b = GameObject.CreatePrimitive(PrimitiveType.Cube);
        b.name = "Base";
        b.transform.position = new Vector3(8.5f, 1f, -2f);
        b.transform.localScale = new Vector3(2f, 2f, 3f);
    }

    void CreateStarterTower(Vector3 position)
    {
        GameObject root = new GameObject("StarterTower");
        root.transform.position = position;

        GameObject baseObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        baseObj.transform.SetParent(root.transform);
        baseObj.transform.localScale = new Vector3(0.75f, 0.35f, 0.75f);

        GameObject head = GameObject.CreatePrimitive(PrimitiveType.Cube);
        head.transform.SetParent(root.transform);
        head.transform.localPosition = new Vector3(0f, 0.75f, 0f);
        head.transform.localScale = new Vector3(0.6f, 0.35f, 1.25f);

        Tower tower = root.AddComponent<Tower>();
        tower.head = head.transform;
    }
}
