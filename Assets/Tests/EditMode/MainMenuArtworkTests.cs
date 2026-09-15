using System.IO;
using System.Reflection;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public sealed class MainMenuArtworkTests
{
    GameObject root;
    MainMenuArtwork view;
    readonly int[] calls = new int[4];

    [SetUp]
    public void SetUp()
    {
        System.Array.Clear(calls, 0, calls.Length);
        root = new GameObject("MainMenuTest", typeof(RectTransform));
        root.GetComponent<RectTransform>().sizeDelta = new Vector2(1600, 900);
        view = root.AddComponent<MainMenuArtwork>();
        view.Build(() => calls[0]++, () => calls[1]++, () => calls[2]++, () => calls[3]++);
    }

    [TearDown]
    public void TearDown() { if (root != null) Object.DestroyImmediate(root); }

    [Test]
    public void AllArtworkLoadsAndCommandsRouteIndependently()
    {
        string[] names = { "PLAY_Button", "HEROES_Button", "SETTINGS_Button", "EXIT_Button" };
        Assert.AreEqual(4, root.GetComponentsInChildren<Button>(true).Length);
        foreach (var art in root.GetComponentsInChildren<RawImage>(true))
            Assert.IsNotNull(art.texture, art.name);
        for (int i = 0; i < names.Length; i++)
        {
            view.Content.Find(names[i]).GetComponent<Button>().onClick.Invoke();
            for (int j = 0; j < calls.Length; j++) Assert.AreEqual(j <= i ? 1 : 0, calls[j]);
        }
    }

    [Test]
    public void ProductionPresenterRoutesMenusAndRebindsAfterCanvasReplacement()
    {
        float previousTimeScale = Time.timeScale;
        GameObject owner = new GameObject("MenuControllerTest");
        GameObject presenterObject = new GameObject("MenuPresenterTest");
        Canvas canvas = null;
        try
        {
            var controller = owner.AddComponent<GameMenuController>();
            var presenter = presenterObject.AddComponent<MainMenuBackgroundOverride>();
            var flags = BindingFlags.Instance | BindingFlags.NonPublic;
            typeof(MainMenuBackgroundOverride).GetField("controller", flags).SetValue(presenter, controller);
            for (int pass = 0; pass < 2; pass++)
            {
                typeof(GameMenuController).GetMethod("BuildUI", flags).Invoke(controller, null);
                canvas = (Canvas)typeof(GameMenuController).GetField("canvas", flags).GetValue(controller);
                var main = canvas.transform.Find("MainMenu");
                typeof(MainMenuBackgroundOverride).GetMethod("BuildProductionMenu", flags).Invoke(presenter, new object[] { main });
                var content = main.Find("ProductionMainMenu/MainScreenLayout");
                Assert.IsFalse(main.Find("MainPanel").gameObject.activeSelf);
                content.Find("PLAY_Button").GetComponent<Button>().onClick.Invoke();
                Assert.IsFalse(main.gameObject.activeSelf);
                Assert.IsTrue(canvas.transform.Find("LevelSelect").gameObject.activeSelf);
                typeof(GameMenuController).GetMethod("ShowMainMenu", flags).Invoke(controller, null);
                content.Find("SETTINGS_Button").GetComponent<Button>().onClick.Invoke();
                Assert.IsTrue(canvas.transform.Find("Settings").gameObject.activeSelf);
                typeof(GameMenuController).GetMethod("BackFromSettings", flags).Invoke(controller, null);
                Assert.IsTrue(main.gameObject.activeSelf);
                content.Find("HEROES_Button").GetComponent<Button>().onClick.Invoke();
                var overlay = content.Find("ArmyOverlay");
                Assert.IsTrue(overlay.gameObject.activeSelf);
                Assert.IsFalse(content.Find("PLAY_Button").GetComponent<Button>().interactable);
                overlay.Find("ArmyCard/BackButton").GetComponent<Button>().onClick.Invoke();
                Assert.IsFalse(overlay.gameObject.activeSelf);
                Assert.IsTrue(content.Find("PLAY_Button").GetComponent<Button>().interactable);
                Object.DestroyImmediate(canvas.gameObject);
                canvas = null;
            }
        }
        finally
        {
            if (canvas != null) Object.DestroyImmediate(canvas.gameObject);
            Object.DestroyImmediate(presenterObject);
            Object.DestroyImmediate(owner);
            Time.timeScale = previousTimeScale;
        }
    }

    [TestCase(1600, 900, false)]
    [TestCase(1366, 768, false)]
    [TestCase(1024, 768, false)]
    [TestCase(2560, 1080, false)]
    [TestCase(1600, 900, true)]
    [TestCase(1366, 768, true)]
    [TestCase(1024, 768, true)]
    [TestCase(2560, 1080, true)]
    public void CaptureAndCheckLayout(int width, int height, bool russian)
    {
        var scene = EditorSceneManager.NewPreviewScene();
        var pipeline = GraphicsSettings.defaultRenderPipeline;
        var quality = QualitySettings.renderPipeline;
        var target = new RenderTexture(width, height, 24);
        Texture2D pixels = null;
        try
        {
            GraphicsSettings.defaultRenderPipeline = null;
            QualitySettings.renderPipeline = null;
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(root, scene);
            var canvas = root.AddComponent<Canvas>();
            root.AddComponent<GraphicRaycaster>();
            var cameraObject = new GameObject("MainMenuPreviewCamera", typeof(Camera));
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(cameraObject, scene);
            var camera = cameraObject.GetComponent<Camera>();
            camera.scene = scene;
            camera.orthographic = true;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.targetTexture = target;
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = camera;
            canvas.planeDistance = 1;
            Canvas.ForceUpdateCanvases();
            Invoke("RefreshLayout");
            Invoke("ApplyLanguage", russian);
            Canvas.ForceUpdateCanvases();
            var corners = new Vector3[4];
            foreach (var button in root.GetComponentsInChildren<Button>())
            {
                var rect = (RectTransform)button.transform;
                rect.GetWorldCorners(corners);
                foreach (var corner in corners)
                {
                    var point = camera.WorldToViewportPoint(corner);
                    Assert.That(point.x, Is.InRange(0f, 1f), button.name);
                    Assert.That(point.y, Is.InRange(0f, 1f), button.name);
                }
            }
            foreach (var text in root.GetComponentsInChildren<Text>())
                Assert.LessOrEqual(text.preferredHeight, text.rectTransform.rect.height + 1, text.name);
            camera.Render();
            var previous = RenderTexture.active;
            try
            {
                RenderTexture.active = target;
                pixels = new Texture2D(width, height, TextureFormat.RGB24, false);
                pixels.ReadPixels(new Rect(0, 0, width, height), 0, 0);
                pixels.Apply();
            }
            finally { RenderTexture.active = previous; }
            var samples = pixels.GetPixels32();
            int bright = 0;
            foreach (var pixel in samples) if (pixel.r > 80 || pixel.g > 80 || pixel.b > 80) bright++;
            Assert.Greater(bright, samples.Length / 2, "Canvas render must not be blank.");
            Directory.CreateDirectory("Logs/Validation/MainScreen");
            File.WriteAllBytes($"Logs/Validation/MainScreen/{(russian ? "RU" : "EN")}-{width}.png", pixels.EncodeToPNG());
        }
        finally
        {
            GraphicsSettings.defaultRenderPipeline = pipeline;
            QualitySettings.renderPipeline = quality;
            EditorSceneManager.ClosePreviewScene(scene);
            Object.DestroyImmediate(target);
            if (pixels != null) Object.DestroyImmediate(pixels);
        }
    }

    void Invoke(string method, params object[] args) => typeof(MainMenuArtwork)
        .GetMethod(method, BindingFlags.Instance | BindingFlags.NonPublic).Invoke(view, args);
}
