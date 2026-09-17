using System.IO;
using System.Reflection;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public sealed class GateHudArtworkTests
{
    [TestCase(24, true)]
    [TestCase(12, true)]
    [TestCase(0, false)]
    public void GateArtSurvivesSkinAndRendersHealth(int health, bool russian)
    {
        var scene = EditorSceneManager.NewPreviewScene();
        var pipeline = GraphicsSettings.defaultRenderPipeline;
        var quality = QualitySettings.renderPipeline;
        var target = new RenderTexture(800, 280, 24);
        Texture2D pixels = null;
        try
        {
            GraphicsSettings.defaultRenderPipeline = null;
            QualitySettings.renderPipeline = null;
            var host = new GameObject("GatePreview", typeof(Canvas));
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(host, scene);
            var hud = host.AddComponent<ModernCombatHud>();
            var flags = BindingFlags.Instance | BindingFlags.NonPublic;
            typeof(ModernCombatHud).GetMethod("BuildTopBar", flags).Invoke(hud, new object[] { host.transform });
            var bar = host.transform.Find("TopResources").GetComponent<RectTransform>();
            var icon = bar.Find("GateIcon").GetComponent<Image>();
            var original = icon.sprite;
            var skin = host.AddComponent<TroyCombatHudSkin>();
            typeof(TroyCombatHudSkin).GetMethod("Apply", flags).Invoke(skin, new object[] { host.transform });
            Assert.AreSame(original, icon.sprite);
            Assert.IsNotNull(original);
            var label = bar.Find("GateHealthLabel").GetComponent<Text>();
            label.text = (russian ? "ВОРОТА" : "GATE") + "   " + health + " / 24";
            var fill = bar.Find("GateHealthProgress/Fill").GetComponent<Image>();
            fill.fillAmount = health / 24f;
            Assert.IsNotNull(fill.sprite);
            Assert.AreEqual(Image.Type.Filled, fill.type);
            Assert.IsFalse(fill.raycastTarget);
            bar.anchorMin = bar.anchorMax = bar.pivot = new Vector2(.5f, .5f);
            bar.anchoredPosition = new Vector2(42, 0);
            bar.localScale = Vector3.one * 1.25f;
            var cameraObject = new GameObject("Camera", typeof(Camera));
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(cameraObject, scene);
            var camera = cameraObject.GetComponent<Camera>();
            camera.scene = scene;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(.045f, .10f, .12f);
            camera.targetTexture = target;
            var canvas = host.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = camera;
            canvas.planeDistance = 1;
            Canvas.ForceUpdateCanvases();
            Assert.LessOrEqual(label.preferredWidth, label.rectTransform.rect.width);
            Assert.LessOrEqual(label.preferredHeight, label.rectTransform.rect.height);
            camera.Render();
            var previous = RenderTexture.active;
            try
            {
                RenderTexture.active = target;
                pixels = new Texture2D(800, 280, TextureFormat.RGB24, false);
                pixels.ReadPixels(new Rect(0, 0, 800, 280), 0, 0);
                pixels.Apply();
            }
            finally { RenderTexture.active = previous; }
            Directory.CreateDirectory("Logs/Validation/GateHud");
            File.WriteAllBytes($"Logs/Validation/GateHud/{health}.png", pixels.EncodeToPNG());
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
