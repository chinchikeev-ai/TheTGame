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
        new GameObject("GameState").AddComponent<GameStateController>();
        new GameObject("RuntimeEffects").AddComponent<RuntimeEffects>();
        new GameObject("AncientMusic").AddComponent<AncientMusicController>();

        MapBuilder mapBuilder = new GameObject("MapBuilder").AddComponent<MapBuilder>();
        mapBuilder.BuildMap();

        EnemySpawner spawner = new GameObject("EnemySpawner").AddComponent<EnemySpawner>();
        spawner.Initialize(mapBuilder.Paths);

        TowerPlacement placement = new GameObject("TowerPlacement").AddComponent<TowerPlacement>();
        placement.gameCamera = cam;

        CreateHector();
        new GameObject("LandingPresentation").AddComponent<LandingPresentation>();

        new GameObject("GameUI").AddComponent<GameUIController>();
        new GameObject("GameMenu").AddComponent<GameMenuController>();
    }

    void CreateHector()
    {
        GameObject hector = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        hector.name = "Hector";
        hector.transform.position = MapBuilder.CellToWorld(new Vector2Int(15, 4), .6f);
        hector.transform.localScale = new Vector3(.75f, .75f, .75f);
        TowerFactory.SetColor(hector, new Color(.72f,.48f,.12f));
        hector.AddComponent<HectorController>();
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

        cam.orthographic = true;
        cam.orthographicSize = 10.5f;
        cam.transform.position = new Vector3(0f, 30f, 0f);
        cam.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(.32f,.43f,.48f);

        CameraController controller = cam.GetComponent<CameraController>();
        if (controller == null) controller = cam.gameObject.AddComponent<CameraController>();
        controller.xBounds = new Vector2(-4f, 4f);
        controller.zBounds = new Vector2(-2.5f, 2.5f);
        controller.minOrthoSize = 7f;
        controller.maxOrthoSize = 13f;

        if (FindFirstObjectByType<Light>() == null)
        {
            GameObject l = new GameObject("Directional Light");
            Light lightComp = l.AddComponent<Light>();
            lightComp.type = LightType.Directional;
            lightComp.intensity = 1.15f;
            lightComp.color = new Color(1f,.90f,.72f);
            l.transform.rotation = Quaternion.Euler(58f, -28f, 0f);
        }
        return cam;
    }
}
