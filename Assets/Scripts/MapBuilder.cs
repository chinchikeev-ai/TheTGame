using System.Collections.Generic;
using UnityEngine;

public class MapBuilder : MonoBehaviour
{
    public Transform[] BuildMap()
    {
        CreateGround();
        CreateRoad();
        CreateBase();
        CreateBuildPoints();
        return CreateWaypoints();
    }

    void CreateGround()
    {
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Ground";
        ground.transform.localScale = new Vector3(2.6f, 1f, 1.7f);
        TowerFactory.SetColor(ground, new Color(0.16f, 0.29f, 0.17f));
    }

    void CreateRoad()
    {
        Vector3[] centers = {
            new Vector3(-8f, 0.06f, 2f), new Vector3(-3.5f, 0.06f, 2f),
            new Vector3(0f, 0.06f, 0.5f), new Vector3(4f, 0.06f, -2f), new Vector3(7.5f, 0.06f, -2f)
        };
        Vector3[] scales = {
            new Vector3(4f,0.12f,1.6f), new Vector3(5f,0.12f,1.6f),
            new Vector3(4.5f,0.12f,1.6f), new Vector3(4.5f,0.12f,1.6f), new Vector3(3f,0.12f,1.6f)
        };
        for (int i = 0; i < centers.Length; i++)
        {
            GameObject r = GameObject.CreatePrimitive(PrimitiveType.Cube);
            r.name = "Road";
            r.transform.position = centers[i];
            r.transform.localScale = scales[i];
            TowerFactory.SetColor(r, new Color(0.27f, 0.28f, 0.30f));
        }
    }

    void CreateBase()
    {
        GameObject b = GameObject.CreatePrimitive(PrimitiveType.Cube);
        b.name = "Base";
        b.transform.position = new Vector3(9f, 1f, -2f);
        b.transform.localScale = new Vector3(2f, 2f, 3f);
        TowerFactory.SetColor(b, new Color(0.22f, 0.34f, 0.56f));
    }

    Transform[] CreateWaypoints()
    {
        Vector3[] points = {
            new Vector3(-10f,0.8f,2f), new Vector3(-5f,0.8f,2f), new Vector3(-1f,0.8f,2f),
            new Vector3(1f,0.8f,0f), new Vector3(5f,0.8f,-2f), new Vector3(9f,0.8f,-2f)
        };
        List<Transform> list = new List<Transform>();
        foreach (Vector3 p in points)
        {
            GameObject w = new GameObject("Waypoint");
            w.transform.position = p;
            list.Add(w.transform);
        }
        return list.ToArray();
    }

    void CreateBuildPoints()
    {
        Vector3[] pts = {
            new Vector3(-7f,0f,-1.5f), new Vector3(-4f,0f,-1.5f), new Vector3(-1f,0f,-2.5f),
            new Vector3(2f,0f,3f), new Vector3(4f,0f,1.2f), new Vector3(6.5f,0f,1.2f),
            new Vector3(1f,0f,-4f), new Vector3(5.5f,0f,-4.5f)
        };
        foreach (Vector3 p in pts)
        {
            GameObject root = new GameObject("BuildPoint");
            root.transform.position = p;
            root.AddComponent<BuildPoint>().Initialize();
        }
    }
}
