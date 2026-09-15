using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering;
using System.IO;
using System.Reflection;

public sealed class PatronSelectionArtworkTests
{
    GameObject root;
    PatronSelectionArtwork view;
    int confirmations, backs;
    DivineGiftType chosen;

    [SetUp]
    public void SetUp()
    {
        confirmations = backs = 0;
        root = new GameObject("PatronTest", typeof(RectTransform));
        root.GetComponent<RectTransform>().sizeDelta = new Vector2(1600, 900);
        view = root.AddComponent<PatronSelectionArtwork>();
        view.Build(() => backs++, gift => { chosen = gift; confirmations++; });
    }

    [TearDown]
    public void TearDown() => Object.DestroyImmediate(root);

    [TestCase(DivineGiftType.Athena)]
    [TestCase(DivineGiftType.Ares)]
    [TestCase(DivineGiftType.Apollo)]
    [TestCase(DivineGiftType.Poseidon)]
    public void CardOnlySelectsAndConfirmAppliesCorrectGift(DivineGiftType gift)
    {
        var confirm = root.transform.Find("GodsLayout/ConfirmPatron").GetComponent<Button>();
        Assert.IsFalse(confirm.interactable);
        confirm.onClick.Invoke();
        Assert.AreEqual(0, confirmations);
        root.transform.Find("GodsLayout/Patron_" + gift).GetComponent<Button>().onClick.Invoke();
        Assert.AreEqual(0, confirmations);
        Assert.IsTrue(confirm.interactable);
        confirm.onClick.Invoke();
        Assert.AreEqual(1, confirmations);
        Assert.AreEqual(gift, chosen);
        view.ResetSelection();
        Assert.IsFalse(confirm.interactable);
    }

    [Test]
    public void BackDoesNotApplyGiftAndAllFiveImagesLoad()
    {
        root.transform.Find("GodsLayout/Back").GetComponent<Button>().onClick.Invoke();
        Assert.AreEqual(1, backs);
        Assert.AreEqual(0, confirmations);
        var images = root.GetComponentsInChildren<RawImage>(true);
        Assert.AreEqual(5, images.Length);
        foreach (var image in images) Assert.IsNotNull(image.texture, image.name);
    }

    [TestCase(1600, 900, true)]
    [TestCase(1366, 768, true)]
    [TestCase(1024, 768, true)]
    [TestCase(1600, 900, false)]
    [TestCase(1366, 768, false)]
    [TestCase(1024, 768, false)]
    public void CaptureLayoutWithoutChangingPlayerSave(int width, int height, bool russian)
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
            var cameraObject = new GameObject("PatronPreviewCamera", typeof(Camera));
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
            typeof(PatronSelectionArtwork).GetMethod("LateUpdate", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(view, null);
            typeof(PatronSelectionArtwork).GetMethod("ApplyLanguage", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(view, new object[] { russian });
            root.transform.Find("GodsLayout/Patron_Athena").GetComponent<Button>().onClick.Invoke();
            Canvas.ForceUpdateCanvases();
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
            Directory.CreateDirectory("Logs/Validation/PatronSelection");
            File.WriteAllBytes($"Logs/Validation/PatronSelection/{(russian ? "RU" : "EN")}-{width}.png", pixels.EncodeToPNG());
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
}
