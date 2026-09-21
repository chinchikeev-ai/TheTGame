using System.IO;
using System.Reflection;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public sealed class EndMenuArtworkTests
{
    [TestCase(1600, 900, true, true)]
    [TestCase(1600, 900, false, true)]
    [TestCase(1024, 768, true, true)]
    [TestCase(1024, 768, false, false)]
    [TestCase(2560, 1080, true, false)]
    [TestCase(2560, 1080, false, true)]
    public void LiveResultsAndCallbacksFitFullScreen(int width, int height, bool victory, bool russian)
    {
        var scene = EditorSceneManager.NewPreviewScene();
        var pipeline = GraphicsSettings.defaultRenderPipeline;
        var quality = QualitySettings.renderPipeline;
        bool hadLanguage = PlayerPrefs.HasKey("TheTroyGame.Language");
        int language = PlayerPrefs.GetInt("TheTroyGame.Language");
        var target = new RenderTexture(width, height, 24);
        Texture2D pixels = null;
        try
        {
            PlayerPrefs.SetInt("TheTroyGame.Language", russian ? 1 : 0);
            GraphicsSettings.defaultRenderPipeline = null;
            QualitySettings.renderPipeline = null;
            var host = new GameObject("EndPreview", typeof(Canvas));
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(host, scene);
            var root = new GameObject("EndMenu", typeof(RectTransform), typeof(Image));
            root.transform.SetParent(host.transform, false);
            var rect = root.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one; rect.sizeDelta = Vector2.zero;
            var panel = new GameObject("ResultCard", typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(root.transform, false);
            Text title = Caption(panel.transform, victory ? GameLanguage.T("VICTORY", "ПОБЕДА") : GameLanguage.T("DEFEAT", "ПОРАЖЕНИЕ"));
            Text chapter = Caption(panel.transform, GameLanguage.T("CHAPTER I - THE LANDING", "ГЛАВА I - ВЫСАДКА"));
            string summaryValue = russian
                ? "КАРТА 1    СЛОЖНОСТЬ: ЛЕГЕНДАРНАЯ\nБОИ: 5/5    ВРЕМЯ: 12:45\n\nСЧЁТ: 18954\nУБИТО: 759    ПРОПУЩЕНО: 0\nЗОЛОТО ПОЛУЧЕНО: 2942    ПОТРАЧЕНО: 1827\nПОСТРОЕНО БАШЕН: 5    ПРОДАНО: 0\nПРОЧНОСТЬ ВОРОТ: 26/26"
                : "MAP 1    DIFFICULTY: LEGENDARY\nENCOUNTERS: 5/5    TIME: 12:45\n\nSCORE: 18954\nKILLS: 759    LEAKS: 0\nGOLD EARNED: 2942    SPENT: 1827\nTOWERS BUILT: 5    SOLD: 0\nGATE HEALTH: 26/26";
            if (victory) summaryValue += GameLanguage.T("\nCHAPTER II UNLOCKED\nChapter II is in production. Return to Chapter Select.", "\nГЛАВА II ОТКРЫТА\nГлава II в разработке. Вернитесь к выбору главы.");
            Text summary = Caption(panel.transform, summaryValue);
            int clicks = 0;
            string[] labels = russian ? new[] { "ПОВТОРИТЬ", "ВЫБОР ГЛАВЫ", "ГЛАВНОЕ МЕНЮ" } : new[] { "RETRY", "CHAPTER SELECT", "MAIN MENU" };
            foreach (string label in labels)
                CombatHudUiFactory.Button(panel.transform, label, Vector2.zero, new Vector2(280, 58), () => clicks++, false);
            var art = root.AddComponent<EndMenuArtwork>();
            art.Apply(panel.GetComponent<RectTransform>(), title, chapter, summary);
            art.SetResult(victory);
            var sprite = root.transform.Find("ResultBackground").GetComponent<Image>().sprite;
            Assert.AreEqual(victory ? "Victory" : "Defeat", sprite.texture.name);
            typeof(MenuFlowStylePresentation).GetMethod("StyleEndMenu", BindingFlags.Instance | BindingFlags.NonPublic)
                .Invoke(host.AddComponent<MenuFlowStylePresentation>(), new object[] { root.transform });
            Assert.AreSame(sprite, root.transform.Find("ResultBackground").GetComponent<Image>().sprite);
            Assert.AreEqual(summaryValue, summary.text);

            var cameraObject = new GameObject("Camera", typeof(Camera));
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(cameraObject, scene);
            var camera = cameraObject.GetComponent<Camera>();
            camera.scene = scene; camera.targetTexture = target;
            camera.clearFlags = CameraClearFlags.SolidColor;
            var canvas = host.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera; canvas.worldCamera = camera; canvas.planeDistance = 1;
            Canvas.ForceUpdateCanvases(); art.Fit(); Canvas.ForceUpdateCanvases();
            foreach (var button in root.GetComponentsInChildren<Button>())
            {
                button.onClick.Invoke();
                Assert.IsNotNull(button.GetComponent<Image>().sprite);
                var corners = new Vector3[4]; button.GetComponent<RectTransform>().GetWorldCorners(corners);
                foreach (var corner in corners)
                {
                    var point = camera.WorldToViewportPoint(corner);
                    Assert.That(point.x, Is.InRange(0f, 1f)); Assert.That(point.y, Is.InRange(0f, 1f));
                }
            }
            Assert.AreEqual(3, clicks);
            var bgRect = root.transform.Find("ResultBackground").GetComponent<RectTransform>();
            Assert.GreaterOrEqual(bgRect.rect.width, rect.rect.width - .1f);
            Assert.GreaterOrEqual(bgRect.rect.height, rect.rect.height - .1f);
            camera.Render();
            var previous = RenderTexture.active;
            try
            {
                RenderTexture.active = target;
                pixels = new Texture2D(width, height, TextureFormat.RGB24, false);
                pixels.ReadPixels(new Rect(0, 0, width, height), 0, 0); pixels.Apply();
            }
            finally { RenderTexture.active = previous; }
            Directory.CreateDirectory("Logs/Validation/EndMenu");
            File.WriteAllBytes($"Logs/Validation/EndMenu/{width}-{victory}-{russian}.png", pixels.EncodeToPNG());
        }
        finally
        {
            if (hadLanguage) PlayerPrefs.SetInt("TheTroyGame.Language", language);
            else PlayerPrefs.DeleteKey("TheTroyGame.Language");
            GraphicsSettings.defaultRenderPipeline = pipeline; QualitySettings.renderPipeline = quality;
            EditorSceneManager.ClosePreviewScene(scene); Object.DestroyImmediate(target);
            if (pixels != null) Object.DestroyImmediate(pixels);
        }
    }

    static Text Caption(Transform parent, string value) => CombatHudUiFactory.Text(parent, value, Vector2.zero,
        new Vector2(900, 300), 18, Color.white, TextAnchor.MiddleCenter, FontStyle.Bold);
}
