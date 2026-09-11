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
        new GameObject("RuntimeEffects").AddComponent<RuntimeEffects>();

        MapBuilder mapBuilder = new GameObject("MapBuilder").AddComponent<MapBuilder>();
        mapBuilder.BuildMap();

        EnemySpawner spawner = new GameObject("EnemySpawner").AddComponent<EnemySpawner>();
        spawner.Initialize(mapBuilder.Paths);

        TowerPlacement placement = new GameObject("TowerPlacement").AddComponent<TowerPlacement>();
        placement.gameCamera = cam;

        new GameObject("GameUI").AddComponent<GameUIController>();
        new GameObject("GameMenu").AddComponent<GameMenuController>();
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

        cam.transform.position = new Vector3(0f, 19f, -17f);
        cam.transform.rotation = Quaternion.Euler(50f, 0f, 0f);
        cam.fieldOfView = 56f;

        CameraController controller = cam.GetComponent<CameraController>();
        if (controller == null) controller = cam.gameObject.AddComponent<CameraController>();
        controller.xBounds = new Vector2(-11f, 11f);
        controller.zBounds = new Vector2(-11f, 5f);
        controller.minHeight = 12f;
        controller.maxHeight = 26f;

        if (FindFirstObjectByType<Light>() == null)
        {
            GameObject l = new GameObject("Directional Light");
            Light lightComp = l.AddComponent<Light>();
            lightComp.type = LightType.Directional;
            lightComp.intensity = 1.25f;
            l.transform.rotation = Quaternion.Euler(48f, -35f, 0f);
        }
        return cam;
    }
}
