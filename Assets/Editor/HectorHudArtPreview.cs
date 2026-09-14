using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public static class HectorHudArtPreview
{
    const string ArtPath = "Assets/Game/Art/Resources/HectorHud";

    // Explicit preview command; never changes the user's active scene or save.
    public static void ValidateAndCapture()
    {
        try
        {
            foreach (string path in Directory.GetFiles(ArtPath, "*.png"))
            {
                var importer = (TextureImporter)AssetImporter.GetAtPath(path.Replace('\\', '/'));
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.mipmapEnabled = false;
                importer.alphaIsTransparency = true;
                string assetName = Path.GetFileNameWithoutExtension(path);
                importer.maxTextureSize = assetName == "Panel" || assetName == "HealthTrack" || assetName == "Nameplate" || assetName == "Parchment" ? 1024 : 512;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.SaveAndReimport();
            }
            Directory.CreateDirectory("Logs/Validation/HectorHud");
            Capture(1600, 900, false);
            Capture(1280, 720, true);
            Capture(1000, 720, true, true);
            Debug.Log("HECTOR_HUD_PREVIEW_PASS: imported 11 sprites, rendered RU/EN, verified text fit and five buttons.");
            EditorApplication.Exit(0);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            EditorApplication.Exit(1);
        }
    }

    static void Capture(int width, int height, bool russian, bool detail = false)
    {
        var scene = EditorSceneManager.NewPreviewScene();
        var oldPipeline = GraphicsSettings.defaultRenderPipeline;
        var oldQualityPipeline = QualitySettings.renderPipeline;
        RenderTexture target = null;
        Texture2D result = null;
        try
        {
            GraphicsSettings.defaultRenderPipeline = null;
            QualitySettings.renderPipeline = null;
            var host = new GameObject("HectorHudPreview");
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(host, scene);
            var hud = host.AddComponent<HectorHUD>();
            typeof(HectorHUD).GetMethod("Start", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(hud, null);
            var canvas = host.GetComponentInChildren<Canvas>();
            var panel = canvas.transform.Find("HectorPanel").GetComponent<RectTransform>();
            panel.gameObject.SetActive(true);
            if (detail)
            {
                var scaler = canvas.GetComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
                scaler.scaleFactor = 2;
                canvas.scaleFactor = 2;
                panel.anchorMin = panel.anchorMax = panel.pivot = new Vector2(.5f, .5f);
                panel.anchoredPosition = Vector2.zero;
            }
            SetText(panel, "HectorName", russian ? "ГЕКТОР" : "HECTOR");
            SetText(panel, "HectorHealthText", "HP 375 / 500");
            SetText(panel, "HectorStatus", russian ? "ПРИНЦ ТРОИ" : "PRINCE OF TROY");
            SetText(panel, "HectorCommentary", russian ? "Троя стоит. Покажи мне, где строй слабее всего." : "Troy stands. Tell me where the line is weakest.");
            panel.Find("HealthTrack/HealthFill").GetComponent<Image>().fillAmount = .75f;
            string[] keys = { "Q", "E", "R", "F" };
            string[] names = russian ? new[] { "КЛИЧ", "ЩИТЫ", "КОПЬЁ", "ЗА ТРОЮ!" } : new[] { "WAR CRY", "SHIELD", "SPEAR", "FOR TROY!" };
            for (int i = 0; i < 4; i++)
            {
                var slot = panel.Find("Ability_" + keys[i]);
                foreach (Text label in slot.GetComponentsInChildren<Text>())
                    if (label.gameObject.name != "CooldownSeconds" && label.text != keys[i]) label.text = names[i];
                slot.Find("Cooldown").GetComponent<Image>().fillAmount = i == 1 ? .55f : 0;
                slot.Find("CooldownSeconds").GetComponent<Text>().text = i == 1 ? "8" : "";
            }
            if (canvas.GetComponent<GraphicRaycaster>() == null || panel.GetComponentsInChildren<Button>().Length != 5)
                throw new InvalidOperationException("Hector HUD mouse controls missing.");

            var cameraObject = new GameObject("PreviewCamera");
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(cameraObject, scene);
            var camera = cameraObject.AddComponent<Camera>();
            camera.scene = scene;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(.035f, .09f, .10f);
            camera.orthographic = true;
            target = new RenderTexture(width, height, 24);
            camera.targetTexture = target;
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = camera;
            canvas.planeDistance = 1;
            Canvas.ForceUpdateCanvases();
            foreach (Text label in panel.GetComponentsInChildren<Text>())
                if (label.preferredHeight > label.rectTransform.rect.height + 1)
                    throw new InvalidOperationException("Text overflow: " + label.text);
            camera.Render();
            var previous = RenderTexture.active;
            RenderTexture.active = target;
            result = new Texture2D(width, height, TextureFormat.RGB24, false);
            result.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            result.Apply();
            RenderTexture.active = previous;
            File.WriteAllBytes("Logs/Validation/HectorHud/" + (detail ? "Detail" : russian ? "RU" : "EN") + "-" + width + ".png", result.EncodeToPNG());
        }
        finally
        {
            GraphicsSettings.defaultRenderPipeline = oldPipeline;
            QualitySettings.renderPipeline = oldQualityPipeline;
            if (target != null) UnityEngine.Object.DestroyImmediate(target);
            if (result != null) UnityEngine.Object.DestroyImmediate(result);
            EditorSceneManager.ClosePreviewScene(scene);
        }
    }

    static void SetText(Transform panel, string name, string value)
    {
        panel.Find(name).GetComponent<Text>().text = value;
    }
}
