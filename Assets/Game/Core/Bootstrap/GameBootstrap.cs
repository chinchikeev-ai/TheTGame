using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    public static GameBootstrap Instance { get; private set; }
    public GameRuntimeContext Runtime { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoStart()
    {
        if (Instance == null)
            new GameObject("GameBootstrap").AddComponent<GameBootstrap>();
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    void Start()
    {
        Camera camera = EnsureMainCamera();
        Light sun = EnsureSun();
        Runtime = new GameRuntimeContext(transform, camera, sun);

        CampaignController campaign = Runtime.CreateCore<CampaignController>("CampaignController");
        ChapterController chapters = Runtime.CreateCore<ChapterController>("ChapterController");
        Runtime.Campaign = campaign;
        Runtime.Chapters = chapters;
        if (chapters.ActiveChapter == null) chapters.LoadChapter(1);

        Runtime.Game = Runtime.CreateCore<GameManager>("GameManager");
        Runtime.State = Runtime.CreateCore<GameStateController>("GameState");
        Runtime.Input = Runtime.CreateCore<RuntimeInputBootstrap>("RuntimeInput");
        Runtime.Effects = Runtime.CreateCore<RuntimeEffects>("RuntimeEffects");
        Runtime.Music = Runtime.CreateCore<AncientMusicController>("AncientMusic");

        Runtime.Chapter = ChapterRuntimeInstaller.Install(chapters.ActiveChapter, Runtime);

        EnemySpawner spawner = Runtime.CreateCore<EnemySpawner>("EnemySpawner");
        Runtime.Spawner = spawner;
        if (Runtime.Chapter.Paths != null && Runtime.Chapter.Paths.Length > 0)
            spawner.Initialize(Runtime.Chapter.Paths);

        Runtime.CombatHud = Runtime.CreateUi<ModernCombatHud>("ModernCombatHUD");
        Runtime.Menu = Runtime.CreateUi<GameMenuController>("GameMenu");
        Runtime.CreateUi<GameMenuUxEnhancer>("GameMenuUX");
        Runtime.CreateUi<MenuProgressPresentation>("GameMenuProgress");
        Runtime.CreateUi<CampaignMapPresentation>("CampaignMapPresentation");
        Runtime.CreateUi<MainMenuAmbientPresentation>("MainMenuAmbientPresentation");
        Runtime.CreateUi<ModernSettingsPresentation>("ModernSettingsPresentation");
        Runtime.CreateUi<GameScreenshotController>("GameScreenshotController");

        RuntimeFileLogger.Event(
            "BOOT",
            $"Runtime graph ready. unlockedChapter={campaign.UnlockedChapter}, activeChapter={(chapters.ActiveChapter != null ? chapters.ActiveChapter.chapterId : "none")}, runtimeProfile={(chapters.ActiveChapter != null ? chapters.ActiveChapter.runtimeProfile : "none")}");
    }

    static Camera EnsureMainCamera()
    {
        Camera camera = Camera.main;
        if (camera != null) return camera;

        GameObject cameraObject = new GameObject("Main Camera");
        cameraObject.tag = "MainCamera";
        return cameraObject.AddComponent<Camera>();
    }

    static Light EnsureSun()
    {
        Light sun = RenderSettings.sun;
        if (sun != null) return sun;

        GameObject lightObject = new GameObject("Directional Light");
        sun = lightObject.AddComponent<Light>();
        sun.type = LightType.Directional;
        RenderSettings.sun = sun;
        return sun;
    }
}
