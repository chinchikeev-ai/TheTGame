using System.IO;
using System.Reflection;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public sealed class DefenderCardArtworkTests
{
    [TestCase(1600, 900, false)]
    [TestCase(1600, 900, true)]
    [TestCase(1024, 768, false)]
    [TestCase(1024, 768, true)]
    public void CardsPreserveArtCallbacksAndLivePrices(int width, int height, bool russian)
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
            var host = new GameObject("DefenderPreview", typeof(Canvas));
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(host, scene);
            var hud = host.AddComponent<ModernCombatHud>();
            var placement = host.AddComponent<TowerPlacement>();
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            typeof(ModernCombatHud).GetField("placement", flags).SetValue(hud, placement);
            typeof(ModernCombatHud).GetMethod("BuildDock", flags).Invoke(hud, new object[] { host.transform });
            var dock = host.transform.Find("BuildDock").GetComponent<RectTransform>();
            dock.gameObject.SetActive(true);
            dock.anchorMin = dock.anchorMax = dock.pivot = new Vector2(.5f, .5f);
            dock.anchoredPosition = Vector2.zero;
            dock.localScale = Vector3.one * .78f;
            var cards = dock.GetComponentsInChildren<DefenderCardArtwork>();
            Assert.AreEqual(6, cards.Length);
            var frames = new Sprite[6];
            for (int i = 0; i < cards.Length; i++) frames[i] = cards[i].GetComponent<Image>().sprite;
            typeof(TroyCombatHudSkin).GetMethod("Apply", flags).Invoke(host.AddComponent<TroyCombatHudSkin>(), new object[] { host.transform });
            var types = (TowerType[])typeof(ModernCombatHud).GetField("buildTypes", flags).GetValue(hud);
            for (int i = 0; i < cards.Length; i++)
            {
                Assert.AreSame(frames[i], cards[i].GetComponent<Image>().sprite);
                cards[i].GetComponent<Button>().onClick.Invoke();
                Assert.AreEqual(types[i], placement.SelectedBuildType);
                Assert.AreEqual(TowerFactory.GetCost(types[i]).ToString(), cards[i].transform.Find("Price").GetComponent<Text>().text);
                var portrait = cards[i].transform.Find("TowerIcon").GetComponent<Image>();
                Assert.IsNotNull(portrait.sprite);
                cards[i].SetState(true, false, false);
                Assert.IsTrue(cards[i].GetComponent<Outline>().enabled);
                Assert.Less(portrait.color.r, .7f);
                cards[i].SetState(i == 0, i == 1, i != 4);
                foreach (Graphic graphic in cards[i].GetComponentsInChildren<Graphic>())
                    if (graphic.gameObject != cards[i].gameObject) Assert.IsFalse(graphic.raycastTarget);
                Assert.AreNotEqual(TextureFormat.Alpha8, portrait.sprite.texture.format);
                Assert.IsTrue(portrait.sprite.texture.name == "Spearman" || portrait.sprite.texture.name == "Archer" ||
                    portrait.sprite.texture.name == "Ballista" || portrait.sprite.texture.name == "Priest" ||
                    portrait.sprite.texture.name == "Fire" || portrait.sprite.texture.name == "Guard");
            }

            var cameraObject = new GameObject("Camera", typeof(Camera));
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(cameraObject, scene);
            var camera = cameraObject.GetComponent<Camera>();
            camera.scene = scene; camera.targetTexture = target;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(.025f, .07f, .08f);
            var canvas = host.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera; canvas.worldCamera = camera; canvas.planeDistance = 1;
            Canvas.ForceUpdateCanvases(); camera.Render();
            var previous = RenderTexture.active;
            try
            {
                RenderTexture.active = target;
                pixels = new Texture2D(width, height, TextureFormat.RGB24, false);
                pixels.ReadPixels(new Rect(0, 0, width, height), 0, 0); pixels.Apply();
            }
            finally { RenderTexture.active = previous; }
            int colorful = 0;
            for (int y = height / 2 - 60; y < height / 2 + 60; y += 2)
                for (int x = width / 2 - 340; x < width / 2 + 340; x += 2)
                {
                    Color c = pixels.GetPixel(x, y);
                    if (c.maxColorComponent > .3f && Mathf.Max(c.r, c.g, c.b) - Mathf.Min(c.r, c.g, c.b) > .15f) colorful++;
                }
            Assert.Greater(colorful, 1000);
            Directory.CreateDirectory("Logs/Validation/DefenderHud");
            File.WriteAllBytes($"Logs/Validation/DefenderHud/{width}-{(russian ? "RU" : "EN")}.png", pixels.EncodeToPNG());
        }
        finally
        {
            if (hadLanguage) PlayerPrefs.SetInt("TheTroyGame.Language", language);
            else PlayerPrefs.DeleteKey("TheTroyGame.Language");
            GraphicsSettings.defaultRenderPipeline = pipeline;
            QualitySettings.renderPipeline = quality;
            EditorSceneManager.ClosePreviewScene(scene);
            Object.DestroyImmediate(target);
            if (pixels != null) Object.DestroyImmediate(pixels);
        }
    }
}
