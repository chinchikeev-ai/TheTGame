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
        CampaignController campaign = EnsureComponent<CampaignController>("CampaignController");
        ChapterController chapters = EnsureComponent<ChapterController>("ChapterController");
        if (chapters.ActiveChapter == null) chapters.LoadChapter(1);

        EnsureComponent<GameManager>("GameManager");
        EnsureComponent<GameStateController>("GameState");
        EnsureComponent<RuntimeEffects>("RuntimeEffects");
        EnsureComponent<AncientMusicController>("AncientMusic");

        Camera camera = EnsureMainCamera();
        ChapterRuntimeContext runtime = ChapterRuntimeInstaller.Install(chapters.ActiveChapter, camera);

        EnemySpawner spawner = EnsureComponent<EnemySpawner>("EnemySpawner");
        if ((spawner.paths == null || spawner.paths.Length == 0) && runtime.Paths != null && runtime.Paths.Length > 0)
            spawner.Initialize(runtime.Paths);

        EnsureComponent<ModernCombatHud>("ModernCombatHUD");
        EnsureComponent<GameMenuController>("GameMenu");
        EnsureComponent<GameMenuUxEnhancer>("GameMenuUX");
        EnsureComponent<MenuProgressPresentation>("GameMenuProgress");
        EnsureComponent<CampaignMapPresentation>("CampaignMapPresentation");
        EnsureComponent<MainMenuAmbientPresentation>("MainMenuAmbientPresentation");
        EnsureComponent<ModernSettingsPresentation>("ModernSettingsPresentation");
        EnsureComponent<GameScreenshotController>("GameScreenshotController");

        RuntimeFileLogger.Event(
            "BOOT",
            $"Runtime graph ready. unlockedChapter={campaign.UnlockedChapter}, activeChapter={(chapters.ActiveChapter != null ? chapters.ActiveChapter.chapterId : "none")}, runtimeProfile={(chapters.ActiveChapter != null ? chapters.ActiveChapter.runtimeProfile : "none")}");
    }

    static T EnsureComponent<T>(string objectName) where T : Component
    {
        T existing = FindFirstObjectByType<T>();
        return existing != null ? existing : new GameObject(objectName).AddComponent<T>();
    }

    static Camera EnsureMainCamera()
    {
        Camera camera = Camera.main;
        if (camera != null) return camera;

        GameObject cameraObject = new GameObject("Main Camera");
        cameraObject.tag = "MainCamera";
        return cameraObject.AddComponent<Camera>();
    }
}
