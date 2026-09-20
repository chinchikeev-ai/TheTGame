using UnityEngine;

public sealed class GameRuntimeContext
{
    public Transform Root { get; }
    public Transform CoreRoot { get; }
    public Transform ChapterRoot { get; }
    public Transform UiRoot { get; }

    public Camera Camera { get; }
    public Light Sun { get; }

    public CampaignController Campaign { get; internal set; }
    public ChapterController Chapters { get; internal set; }
    public GameManager Game { get; internal set; }
    public GameStateController State { get; internal set; }
    public RuntimeInputBootstrap Input { get; internal set; }
    public RuntimeEffects Effects { get; internal set; }
    public AncientMusicController Music { get; internal set; }
    public EnemySpawner Spawner { get; internal set; }
    public ModernCombatHud CombatHud { get; internal set; }
    public GameMenuController Menu { get; internal set; }

    public ChapterRuntimeContext Chapter { get; internal set; }

    public GameRuntimeContext(Transform owner, Camera camera, Light sun)
    {
        Camera = camera;
        Sun = sun;

        Root = CreateRoot(owner, "RuntimeGraph");
        CoreRoot = CreateRoot(Root, "Core");
        ChapterRoot = CreateRoot(Root, "Chapter");
        UiRoot = CreateRoot(Root, "UI");
    }

    public T CreateCore<T>(string objectName) where T : Component =>
        CreateOwned<T>(CoreRoot, objectName);

    public T CreateChapter<T>(string objectName) where T : Component =>
        CreateOwned<T>(ChapterRoot, objectName);

    public T CreateUi<T>(string objectName) where T : Component =>
        CreateOwned<T>(UiRoot, objectName);

    static T CreateOwned<T>(Transform parent, string objectName) where T : Component
    {
        GameObject host = new GameObject(objectName);
        host.transform.SetParent(parent, false);
        return host.AddComponent<T>();
    }

    static Transform CreateRoot(Transform parent, string objectName)
    {
        GameObject root = new GameObject(objectName);
        root.transform.SetParent(parent, false);
        return root.transform;
    }
}
