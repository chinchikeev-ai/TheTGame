using System.Collections.Generic;
using UnityEngine;

public class MapBuilder : MonoBehaviour
{
    public Transform[][] Paths { get; private set; }

    public Transform[] BuildMap()
    {
        CreateGround();
        CreateRoadAndDecor();
        CreateBase();
        CreateBuildPoints();
        Paths = CreatePaths();
        return Paths[0];
    }

    void CreateGround()
    {
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Ground";
        ground.transform.localScale = new Vector3(3.2f, 1f, 2.2f);
        TowerFactory.SetColor(ground, new Color(0.11f, 0.24f, 0.14f));
        for (int i = 0; i < 14; i++)
        {
            GameObject decor = GameObject.CreatePrimitive(i % 3 == 0 ? PrimitiveType.Cylinder : PrimitiveType.Cube);
            decor.name = "Decor";
            float x = -13f + (i * 2.1f) % 26f;
            float z = i % 2 == 0 ? 7.3f : -7.2f;
            decor.transform.position = new Vector3(x, 0.35f, z);
            decor.transform.localScale = new Vector3(0.7f, 0.7f + (i % 3) * 0.3f, 0.7f);
            Object.Destroy(decor.GetComponent<Collider>());
            TowerFactory.SetColor(decor, new Color(0.16f, 0.31f, 0.19f));
        }
    }

    void CreateRoadAndDecor()
    {
        Road(new Vector3(-9.5f,0.06f,3.2f), new Vector3(7f,0.12f,1.5f));
        Road(new Vector3(-4.2f,0.06f,2.3f), new Vector3(5f,0.12f,1.5f), -18f);
        Road(new Vector3(-9.5f,0.06f,-3.2f), new Vector3(7f,0.12f,1.5f));
        Road(new Vector3(-4.2f,0.06f,-2.3f), new Vector3(5f,0.12f,1.5f), 18f);
        Road(new Vector3(0.8f,0.06f,0f), new Vector3(6.5f,0.12f,1.7f));
        Road(new Vector3(6.0f,0.06f,-1.2f), new Vector3(6f,0.12f,1.7f), -14f);
        Road(new Vector3(10f,0.06f,-2f), new Vector3(3f,0.12f,1.7f));
    }

    void Road(Vector3 pos, Vector3 scale, float yRot = 0f)
    {
        GameObject r = GameObject.CreatePrimitive(PrimitiveType.Cube);
        r.name = "Road";
        r.transform.position = pos;
        r.transform.localScale = scale;
        r.transform.rotation = Quaternion.Euler(0f, yRot, 0f);
        TowerFactory.SetColor(r, new Color(0.25f, 0.27f, 0.30f));
    }

    void CreateBase()
    {
        GameObject root = new GameObject("Base");
        root.transform.position = new Vector3(11.5f, 0f, -2f);
        GameObject core = GameObject.CreatePrimitive(PrimitiveType.Cube);
        core.transform.SetParent(root.transform);
        core.transform.localPosition = new Vector3(0f, 1.2f, 0f);
        core.transform.localScale = new Vector3(2.8f, 2.4f, 3.6f);
        TowerFactory.SetColor(core, new Color(0.16f, 0.34f, 0.60f));
        GameObject top = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        top.transform.SetParent(root.transform);
        top.transform.localPosition = new Vector3(0f, 2.8f, 0f);
        top.transform.localScale = new Vector3(1.1f, 0.35f, 1.1f);
        TowerFactory.SetColor(top, new Color(0.10f, 0.65f, 0.95f));
    }

    Transform[][] CreatePaths()
    {
        Vector3[] a = { new Vector3(-13f,0.8f,3.2f), new Vector3(-7f,0.8f,3.2f), new Vector3(-3.4f,0.8f,1.5f), new Vector3(0f,0.8f,0f), new Vector3(5f,0.8f,0f), new Vector3(8f,0.8f,-1.7f), new Vector3(11.5f,0.8f,-2f) };
        Vector3[] b = { new Vector3(-13f,0.8f,-3.2f), new Vector3(-7f,0.8f,-3.2f), new Vector3(-3.4f,0.8f,-1.5f), new Vector3(0f,0.8f,0f), new Vector3(5f,0.8f,0f), new Vector3(8f,0.8f,-1.7f), new Vector3(11.5f,0.8f,-2f) };
        return new[] { MakePath("Path_A", a), MakePath("Path_B", b) };
    }

    Transform[] MakePath(string name, Vector3[] points)
    {
        GameObject root = new GameObject(name);
        List<Transform> list = new List<Transform>();
        foreach (Vector3 p in points)
        {
            GameObject w = new GameObject("Waypoint");
            w.transform.SetParent(root.transform);
            w.transform.position = p;
            list.Add(w.transform);
        }
        return list.ToArray();
    }

    void CreateBuildPoints()
    {
        Vector3[] pts = { new Vector3(-10f,0f,0f), new Vector3(-7f,0f,0f), new Vector3(-4f,0f,4.8f), new Vector3(-4f,0f,-4.8f), new Vector3(-1.5f,0f,3.5f), new Vector3(-1.5f,0f,-3.5f), new Vector3(2f,0f,3.2f), new Vector3(2f,0f,-3.2f), new Vector3(5f,0f,3.0f), new Vector3(5f,0f,-3.8f), new Vector3(8f,0f,2.0f), new Vector3(8f,0f,-4.6f) };
        foreach (Vector3 p in pts)
        {
            GameObject root = new GameObject("BuildPoint");
            root.transform.position = p;
            root.AddComponent<BuildPoint>().Initialize();
        }
    }
}
