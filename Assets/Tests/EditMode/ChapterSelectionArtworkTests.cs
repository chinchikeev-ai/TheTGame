using System.IO;
using System.Reflection;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public sealed class ChapterSelectionArtworkTests
{
    [TestCase(1448, 1086, true)]
    [TestCase(1920, 1080, true)]
    [TestCase(2560, 1080, true)]
    [TestCase(1366, 768, false)]
    public void SelectionCallbacksAndLayout(int width, int height, bool ru)
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
            var host = new GameObject("Preview", typeof(Canvas));
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(host, scene);
            var root = new GameObject("LevelSelect", typeof(RectTransform), typeof(Image));
            root.transform.SetParent(host.transform, false);
            var rect = root.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
            int starts = 0, backs = 0;
            var art = root.AddComponent<ChapterSelectionArtwork>();
            art.Build(() => starts++, () => backs++, chapter => chapter <= 2, ru);
            var legacy = host.AddComponent<CampaignMapPresentation>();
            var privateInstance = BindingFlags.Instance | BindingFlags.NonPublic;
            typeof(CampaignMapPresentation).GetField("canvas", privateInstance).SetValue(legacy, host.GetComponent<Canvas>());
            typeof(CampaignMapPresentation).GetMethod("Build", privateInstance).Invoke(legacy, null);
            Assert.IsNull(root.transform.Find("CampaignMapLayer"));
            var composition = root.transform.Find("ChapterComposition");
            var start = composition.Find("StartChapter").GetComponent<Button>();
            Assert.IsTrue(start.interactable);
            start.onClick.Invoke();
            for (int i = 2; i <= 5; i++)
            {
                composition.Find("NodeClip" + i + "/Chapter" + i).GetComponent<Button>().onClick.Invoke();
                Assert.AreEqual(i, art.SelectedChapter);
                Assert.IsFalse(start.interactable);
                start.onClick.Invoke();
            }
            Assert.AreEqual(1, starts);
            int patronRequests = 0;
            art.SetStartAction(() => patronRequests++);
            art.SelectChapter(1);
            start.onClick.Invoke();
            Assert.AreEqual(1, patronRequests);
            Assert.AreEqual(1, starts, "Patron selection must replace direct run start.");
            composition.Find("Back").GetComponent<Button>().onClick.Invoke();
            Assert.AreEqual(1, backs);
            art.SelectChapter(1);
            var cameraObject = new GameObject("Camera", typeof(Camera));
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(cameraObject, scene);
            var camera = cameraObject.GetComponent<Camera>();
            camera.scene = scene;
            camera.targetTexture = target;
            var canvas = host.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = camera;
            canvas.planeDistance = 1;
            Canvas.ForceUpdateCanvases();
            typeof(ChapterSelectionArtwork).GetMethod("LateUpdate", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(art, null);
            Canvas.ForceUpdateCanvases();
            var mapCorners = new Vector3[4];
            composition.Find("AlignedMap").GetComponent<RectTransform>().GetWorldCorners(mapCorners);
            Assert.That(camera.WorldToViewportPoint(mapCorners[0]).x, Is.EqualTo(0).Within(.001f));
            Assert.That(camera.WorldToViewportPoint(mapCorners[0]).y, Is.EqualTo(0).Within(.001f));
            Assert.That(camera.WorldToViewportPoint(mapCorners[2]).x, Is.EqualTo(1).Within(.001f));
            Assert.That(camera.WorldToViewportPoint(mapCorners[2]).y, Is.EqualTo(1).Within(.001f));
            foreach (var button in root.GetComponentsInChildren<Button>())
            {
                var corners = new Vector3[4];
                button.GetComponent<RectTransform>().GetWorldCorners(corners);
                foreach (var corner in corners)
                {
                    var point = camera.WorldToViewportPoint(corner);
                    Assert.That(point.x, Is.InRange(0f, 1f));
                    Assert.That(point.y, Is.InRange(0f, 1f));
                }
            }
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
            Directory.CreateDirectory("Logs/Validation/ChapterSelection");
            File.WriteAllBytes($"Logs/Validation/ChapterSelection/{width}-{height}-{ru}.png", pixels.EncodeToPNG());
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
