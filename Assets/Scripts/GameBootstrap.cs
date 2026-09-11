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

        Camera cam = SetupLightingAndCamera();
        new GameObject("GameManager").AddComponent<GameManager>();

        MapBuilder mapBuilder = new GameObject("MapBuilder").AddComponent<MapBuilder>();
        Transform[] path = mapBuilder.BuildMap();

        GameObject spawnerObj = new GameObject("EnemySpawner");
        EnemySpawner spawner = spawnerObj.AddComponent<EnemySpawner>();
        spawner.spawnPoint = path[0];
        spawner.waypoints = path;

        TowerPlacement placement = new GameObject("TowerPlacement").AddComponent<TowerPlacement>();
        placement.gameCamera = cam;

        new GameObject("GameUI").AddComponent<GameUIController>();
        spawner.Begin();
    }

    Camera SetupLightingAndCamera()
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

        CameraController controller = cam.GetComponent<CameraController>();
        if (controller == null) controller = cam.gameObject.AddComponent<CameraController>();
        controller.xBounds = new Vector2(-9f, 9f);
        controller.zBounds = new Vector2(-10f, 3f);
        controller.minHeight = 11f;
        controller.maxHeight = 24f;

        if (FindFirstObjectByType<Light>() == null)
        {
            GameObject l = new GameObject("Directional Light");
            Light lightComp = l.AddComponent<Light>();
            lightComp.type = LightType.Directional;
            lightComp.intensity = 1.2f;
            l.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        }

        return cam;
    }
}
