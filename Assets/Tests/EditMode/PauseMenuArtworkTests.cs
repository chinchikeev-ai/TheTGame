using System.IO;
using System.Reflection;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public sealed class PauseMenuArtworkTests
{
    [TestCase(1600, 900, true)]
    [TestCase(1024, 768, true)]
    [TestCase(1600, 900, false)]
    public void ButtonsKeepCallbacksAndFitScreen(int width, int height, bool russian)
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
            var host = new GameObject("PausePreview", typeof(Canvas));
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(host, scene);
            var root = new GameObject("PauseMenu", typeof(RectTransform), typeof(Image));
            root.transform.SetParent(host.transform, false);
            var rect = root.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
            var card = new GameObject("PauseCard", typeof(RectTransform), typeof(Image));
            card.transform.SetParent(root.transform, false);
            int clicks = 0;
            for (int i = 0; i < 5; i++)
            {
                var button = new GameObject("Button", typeof(RectTransform), typeof(Image), typeof(Button));
                button.transform.SetParent(card.transform, false);
                button.GetComponent<Button>().onClick.AddListener(() => clicks++);
            }
            var art = root.AddComponent<PauseMenuArtwork>();
            art.Apply(card.GetComponent<RectTransform>(), russian);
            var cameraObject = new GameObject("Camera", typeof(Camera));
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(cameraObject, scene);
            var camera = cameraObject.GetComponent<Camera>();
            camera.scene = scene;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(.08f, .16f, .19f);
            camera.targetTexture = target;
            var canvas = host.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = camera;
            canvas.planeDistance = 1;
            Canvas.ForceUpdateCanvases();
            typeof(PauseMenuArtwork).GetMethod("LateUpdate", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(art, null);
            Canvas.ForceUpdateCanvases();
            foreach (var button in root.GetComponentsInChildren<Button>())
            {
                Assert.IsNotNull(button.GetComponent<Image>().sprite);
                button.onClick.Invoke();
                var corners = new Vector3[4];
                button.GetComponent<RectTransform>().GetWorldCorners(corners);
                foreach (var corner in corners)
                {
                    var point = camera.WorldToViewportPoint(corner);
                    Assert.That(point.x, Is.InRange(0f, 1f));
                    Assert.That(point.y, Is.InRange(0f, 1f));
                }
            }
            Assert.AreEqual(5, clicks);
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
            Directory.CreateDirectory("Logs/Validation/PauseMenu");
            File.WriteAllBytes($"Logs/Validation/PauseMenu/{width}-{height}-{russian}.png", pixels.EncodeToPNG());
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
