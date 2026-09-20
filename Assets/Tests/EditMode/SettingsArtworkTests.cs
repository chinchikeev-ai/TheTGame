using System.IO;
using System.Reflection;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public sealed class SettingsArtworkTests
{
    [TestCase(1920,1080,0)]
    [TestCase(1448,1086,0)]
    [TestCase(1920,1080,1)]
    [TestCase(1920,1080,2)]
    [TestCase(1920,1080,3)]
    public void SettingsRenderWithLiveControls(int width, int height, int tab)
    {
        var scene = EditorSceneManager.NewPreviewScene();
        var pipeline = GraphicsSettings.defaultRenderPipeline;
        var quality = QualitySettings.renderPipeline;
        var target = new RenderTexture(width,height,24);
        Texture2D pixels = null;
        try
        {
            GraphicsSettings.defaultRenderPipeline = null;
            QualitySettings.renderPipeline = null;
            var host = new GameObject("SettingsPreview",typeof(Canvas));
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(host,scene);
            var root = new GameObject("Settings",typeof(RectTransform),typeof(Image));
            root.transform.SetParent(host.transform,false);
            var rect = root.GetComponent<RectTransform>();
            rect.anchorMin=Vector2.zero; rect.anchorMax=Vector2.one; rect.sizeDelta=Vector2.zero;
            var presenter=host.AddComponent<ModernSettingsPresentation>();
            var flags=BindingFlags.Instance|BindingFlags.NonPublic;
            typeof(ModernSettingsPresentation).GetField("settingsRoot",flags).SetValue(presenter,root);
            var tabField = typeof(ModernSettingsPresentation).GetField("activeTab",flags);
            tabField.SetValue(presenter, System.Enum.ToObject(tabField.FieldType,tab));
            typeof(ModernSettingsPresentation).GetMethod("Build",flags).Invoke(presenter,null);
            Assert.AreEqual(tab == 0 ? 2 : 0,root.GetComponentsInChildren<Slider>().Length);
            foreach(var slider in root.GetComponentsInChildren<Slider>())
            {
                Assert.IsNotNull(slider.fillRect);
                Assert.IsNotNull(slider.handleRect);
                Assert.IsTrue(slider.interactable);
            }
            var cameraObject=new GameObject("Camera",typeof(Camera));
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(cameraObject,scene);
            var camera=cameraObject.GetComponent<Camera>();camera.scene=scene;camera.targetTexture=target;
            var canvas=host.GetComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=camera;canvas.planeDistance=1;
            Canvas.ForceUpdateCanvases();
            var layout=root.GetComponentInChildren<SettingsArtworkLayout>();
            typeof(SettingsArtworkLayout).GetMethod("LateUpdate",flags).Invoke(layout,null);
            Canvas.ForceUpdateCanvases();
            camera.Render();
            var previous=RenderTexture.active;
            try
            {
                RenderTexture.active=target;
                pixels=new Texture2D(width,height,TextureFormat.RGB24,false);
                pixels.ReadPixels(new Rect(0,0,width,height),0,0);pixels.Apply();
            }
            finally { RenderTexture.active=previous; }
            Directory.CreateDirectory("Logs/Validation/SettingsArtwork");
            File.WriteAllBytes($"Logs/Validation/SettingsArtwork/{width}-{tab}.png",pixels.EncodeToPNG());
        }
        finally
        {
            GraphicsSettings.defaultRenderPipeline=pipeline;QualitySettings.renderPipeline=quality;
            EditorSceneManager.ClosePreviewScene(scene);Object.DestroyImmediate(target);
            if(pixels!=null)Object.DestroyImmediate(pixels);
        }
    }
}
