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

        CampaignController campaign = new GameObject("CampaignController").AddComponent<CampaignController>();
        ChapterController chapters = new GameObject("ChapterController").AddComponent<ChapterController>();
        chapters.LoadChapter(1);

        new GameObject("GameManager").AddComponent<GameManager>();
        new GameObject("GameState").AddComponent<GameStateController>();
        new GameObject("RuntimeEffects").AddComponent<RuntimeEffects>();
        new GameObject("AncientMusic").AddComponent<AncientMusicController>();

        MapBuilder mapBuilder = new GameObject("MapBuilder").AddComponent<MapBuilder>();
        mapBuilder.BuildMap();
        ChapterOneVisualEnhancer.Enhance();
        TroyGateHeroBuilder.Build();
        ChapterOneWallLife.Build();
        new GameObject("ChapterOneAtmosphere").AddComponent<ChapterOneAtmosphereController>();

        EnemySpawner spawner = new GameObject("EnemySpawner").AddComponent<EnemySpawner>();
        spawner.Initialize(mapBuilder.Paths);
        new GameObject("ChapterOnePlaythroughReporter").AddComponent<ChapterOnePlaythroughReporter>();

        TowerPlacement placement = new GameObject("TowerPlacement").AddComponent<TowerPlacement>();
        placement.gameCamera = cam;

        CreateHector(cam);
        new GameObject("LandingPresentation").AddComponent<LandingPresentation>();
        ChapterOneCinematicCamera cinematic = new GameObject("ChapterOneCinematicCamera").AddComponent<ChapterOneCinematicCamera>();
        cinematic.Initialize(cam);

        new GameObject("ModernCombatHUD").AddComponent<ModernCombatHud>();
        new GameObject("ChapterOneGuidance").AddComponent<ChapterOneGuidancePresentation>();
        new GameObject("GameMenu").AddComponent<GameMenuController>();
        new GameObject("GameMenuUX").AddComponent<GameMenuUxEnhancer>();
        new GameObject("GameMenuProgress").AddComponent<MenuProgressPresentation>();
        new GameObject("CampaignMapPresentation").AddComponent<CampaignMapPresentation>();
        new GameObject("MainMenuAmbientPresentation").AddComponent<MainMenuAmbientPresentation>();
        new GameObject("ResultScreenPresentation").AddComponent<ResultScreenPresentation>();
        new GameObject("ModernSettingsPresentation").AddComponent<ModernSettingsPresentation>();

        RuntimeFileLogger.Event("BOOT", $"Runtime graph ready. unlockedChapter={campaign.UnlockedChapter}, activeChapter={(chapters.ActiveChapter != null ? chapters.ActiveChapter.chapterId : "none")}");
    }

    void CreateHector(Camera cam)
    {
        GameObject hector = HeroVisualFactory.Create(TroyHeroId.Hector);
        hector.name = "Hector";
        hector.transform.position = MapBuilder.CellToWorld(new Vector2Int(15, 4), .6f);
        if (hector.GetComponentInChildren<Collider>() == null)
        {
            CapsuleCollider collider = hector.AddComponent<CapsuleCollider>();
            collider.center = new Vector3(0f, .9f, 0f);
            collider.height = 1.8f;
            collider.radius = .35f;
        }

        HectorController controller = hector.AddComponent<HectorController>();
        HectorPresentationBridge presentation = hector.AddComponent<HectorPresentationBridge>();
        presentation.Initialize(controller);
        HectorInputDriver input = hector.AddComponent<HectorInputDriver>();
        input.Initialize(controller, cam);
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
        cam.orthographicSize = 10.2f;
        cam.transform.position = new Vector3(-1.2f, 27.5f, -5.2f);
        cam.transform.rotation = Quaternion.Euler(74f, 0f, 0f);
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(.22f,.31f,.34f);

        CameraController controller = cam.GetComponent<CameraController>();
        if (controller == null) controller = cam.gameObject.AddComponent<CameraController>();
        controller.xBounds = new Vector2(-7.5f, 7.5f);
        controller.zBounds = new Vector2(-12f, 5f);
        controller.minOrthoSize = 7f;
        controller.maxOrthoSize = 13f;

        if (FindFirstObjectByType<Light>() == null)
        {
            GameObject l = new GameObject("Directional Light");
            Light lightComp = l.AddComponent<Light>();
            lightComp.type = LightType.Directional;
            lightComp.intensity = 1.35f;
            lightComp.color = new Color(1f,.84f,.60f);
            lightComp.shadows = LightShadows.Soft;
            lightComp.shadowStrength = .72f;
            l.transform.rotation = Quaternion.Euler(52f, -38f, 0f);
        }
        return cam;
    }
}
