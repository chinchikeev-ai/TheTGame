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
        Camera cam = SetupLightingAndCamera();

        CampaignController campaign = EnsureComponent<CampaignController>("CampaignController");
        ChapterController chapters = EnsureComponent<ChapterController>("ChapterController");
        if (chapters.ActiveChapter == null) chapters.LoadChapter(1);

        EnsureComponent<GameManager>("GameManager");
        EnsureComponent<GameStateController>("GameState");
        EnsureComponent<RuntimeEffects>("RuntimeEffects");
        EnsureComponent<AncientMusicController>("AncientMusic");

        MapBuilder mapBuilder = EnsureComponent<MapBuilder>("MapBuilder");
        bool worldNeedsBuild = mapBuilder.Paths == null || mapBuilder.Paths.Length == 0;
        if (worldNeedsBuild)
        {
            bool hadRuntimeWorld = GameObject.Find("Route_A") != null || GameObject.Find("Chapter01_Roads") != null;
            mapBuilder.BuildMap();
            if (!hadRuntimeWorld)
            {
                ChapterOneVisualEnhancer.Enhance();
                TroyGateHeroBuilder.Build();
                ChapterOneWallLife.Build();
            }
        }

        EnsureComponent<ChapterOneAtmosphereController>("ChapterOneAtmosphere");

        EnemySpawner spawner = EnsureComponent<EnemySpawner>("EnemySpawner");
        if ((spawner.paths == null || spawner.paths.Length == 0) && mapBuilder.Paths != null && mapBuilder.Paths.Length > 0)
            spawner.Initialize(mapBuilder.Paths);

        EnsureComponent<ChapterOnePlaythroughReporter>("ChapterOnePlaythroughReporter");

        TowerPlacement placement = EnsureComponent<TowerPlacement>("TowerPlacement");
        if (placement.gameCamera == null) placement.gameCamera = cam;

        EnsureHector(cam);
        EnsureComponent<LandingPresentation>("LandingPresentation");

        ChapterOneCinematicCamera cinematic = FindFirstObjectByType<ChapterOneCinematicCamera>();
        if (cinematic == null)
        {
            cinematic = new GameObject("ChapterOneCinematicCamera").AddComponent<ChapterOneCinematicCamera>();
            cinematic.Initialize(cam);
        }

        EnsureComponent<ModernCombatHud>("ModernCombatHUD");
        EnsureComponent<ChapterOneGuidancePresentation>("ChapterOneGuidance");
        EnsureComponent<GameMenuController>("GameMenu");
        EnsureComponent<GameMenuUxEnhancer>("GameMenuUX");
        EnsureComponent<MenuProgressPresentation>("GameMenuProgress");
        EnsureComponent<CampaignMapPresentation>("CampaignMapPresentation");
        EnsureComponent<MainMenuAmbientPresentation>("MainMenuAmbientPresentation");
        EnsureComponent<ResultScreenPresentation>("ResultScreenPresentation");
        EnsureComponent<ModernSettingsPresentation>("ModernSettingsPresentation");

        RuntimeFileLogger.Event("BOOT", $"Runtime graph ready. unlockedChapter={campaign.UnlockedChapter}, activeChapter={(chapters.ActiveChapter != null ? chapters.ActiveChapter.chapterId : "none")}");
    }

    static T EnsureComponent<T>(string objectName) where T : Component
    {
        T existing = FindFirstObjectByType<T>();
        return existing != null ? existing : new GameObject(objectName).AddComponent<T>();
    }

    void EnsureHector(Camera cam)
    {
        HectorController controller = FindFirstObjectByType<HectorController>();
        GameObject hector;

        if (controller == null)
        {
            hector = HeroVisualFactory.Create(TroyHeroId.Hector);
            hector.name = "Hector";
            hector.transform.position = MapBuilder.CellToWorld(new Vector2Int(15, 4), .6f);
            controller = hector.AddComponent<HectorController>();
        }
        else
        {
            hector = controller.gameObject;
        }

        if (hector.GetComponentInChildren<Collider>() == null)
        {
            CapsuleCollider collider = hector.AddComponent<CapsuleCollider>();
            collider.center = new Vector3(0f, .9f, 0f);
            collider.height = 1.8f;
            collider.radius = .35f;
        }

        HectorPresentationBridge presentation = hector.GetComponent<HectorPresentationBridge>();
        if (presentation == null) presentation = hector.AddComponent<HectorPresentationBridge>();
        presentation.Initialize(controller);

        HectorInputDriver input = hector.GetComponent<HectorInputDriver>();
        if (input == null) input = hector.AddComponent<HectorInputDriver>();
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
        cam.backgroundColor = new Color(.22f, .31f, .34f);

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
            lightComp.color = new Color(1f, .84f, .60f);
            lightComp.shadows = LightShadows.Soft;
            lightComp.shadowStrength = .72f;
            l.transform.rotation = Quaternion.Euler(52f, -38f, 0f);
        }
        return cam;
    }
}
