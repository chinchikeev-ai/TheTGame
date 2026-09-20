using System.IO;
using System.Reflection;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public sealed class WaveHudArtworkTests
{
    [TestCase("Ares")]
    [TestCase("Athena")]
    [TestCase("Apollo")]
    [TestCase("Poseidon")]
    public void BannerSurvivesSkinAndPortraitContainsColor(string patron)
    {
        var scene = EditorSceneManager.NewPreviewScene();
        var pipeline = GraphicsSettings.defaultRenderPipeline;
        var quality = QualitySettings.renderPipeline;
        var target = new RenderTexture(1000,420,24);
        Texture2D pixels = null;
        Sprite portraitSprite = null;
        try
        {
            GraphicsSettings.defaultRenderPipeline=null; QualitySettings.renderPipeline=null;
            var host=new GameObject("HudArtPreview",typeof(Canvas));
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(host,scene);
            var hud=host.AddComponent<ModernCombatHud>();
            var flags=BindingFlags.Instance|BindingFlags.NonPublic;
            typeof(ModernCombatHud).GetMethod("BuildEncounterBar",flags).Invoke(hud,new object[]{host.transform});
            var wave=host.transform.Find("WaveStatus").GetComponent<RectTransform>();
            var original=wave.GetComponent<Image>().sprite;
            typeof(TroyCombatHudSkin).GetMethod("Apply",flags).Invoke(host.AddComponent<TroyCombatHudSkin>(),new object[]{host.transform});
            Assert.IsNotNull(original); Assert.AreSame(original,wave.GetComponent<Image>().sprite);
            Assert.IsNull(wave.Find("WaveCrest"));
            wave.anchorMin=wave.anchorMax=wave.pivot=new Vector2(.5f,.5f);
            wave.anchoredPosition=new Vector2(0,120);
            ((Text)typeof(ModernCombatHud).GetField("encounterText",flags).GetValue(hud)).text="БОЙ 1/5   •   00:18";
            ((Text)typeof(ModernCombatHud).GetField("threatText",flags).GetValue(hud)).text="1 ВРАГ В СТРОЮ • ОБЫЧНАЯ ВРАЖЕСКАЯ ФОРМАЦИЯ";
            ((Text)typeof(ModernCombatHud).GetField("encounterProgressText",flags).GetValue(hud)).text="3 / 11 • 27%";
            ((Button)typeof(ModernCombatHud).GetField("startEncounterButton",flags).GetValue(hud)).gameObject.SetActive(false);
            var portrait=new GameObject("Portrait",typeof(RectTransform),typeof(Image)).GetComponent<Image>();
            portrait.transform.SetParent(host.transform,false);
            portrait.rectTransform.sizeDelta=new Vector2(210,210);
            portrait.rectTransform.anchoredPosition=new Vector2(0,-90);
            var texture=Resources.Load<Texture2D>("UI/Patrons/"+patron);
            Assert.IsNotNull(texture);
            Assert.AreNotEqual(TextureFormat.Alpha8,texture.format,"Portrait must retain RGB, not only alpha.");
            portraitSprite=Sprite.Create(texture,new Rect(0,0,texture.width,texture.height),new Vector2(.5f,.5f));
            portrait.sprite=portraitSprite; portrait.preserveAspect=true;
            var cameraObject=new GameObject("Camera",typeof(Camera));
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(cameraObject,scene);
            var camera=cameraObject.GetComponent<Camera>();camera.scene=scene;camera.targetTexture=target;
            camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=new Color(.025f,.07f,.08f);
            var canvas=host.GetComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=camera;canvas.planeDistance=1;
            Canvas.ForceUpdateCanvases();camera.Render();
            var previous=RenderTexture.active;
            try
            {
                RenderTexture.active=target;pixels=new Texture2D(1000,420,TextureFormat.RGB24,false);
                pixels.ReadPixels(new Rect(0,0,1000,420),0,0);pixels.Apply();
            }
            finally {RenderTexture.active=previous;}
            int colored=0;
            for(int y=40;y<210;y+=2) for(int x=410;x<590;x+=2)
            {
                Color c=pixels.GetPixel(x,y);
                if(c.maxColorComponent>.3f && Mathf.Max(c.r,c.g,c.b)-Mathf.Min(c.r,c.g,c.b)>.15f) colored++;
            }
            Assert.Greater(colored,100,"Portrait render must contain colored detail.");
            Directory.CreateDirectory("Logs/Validation/WaveHud");
            File.WriteAllBytes("Logs/Validation/WaveHud/"+patron+".png",pixels.EncodeToPNG());
        }
        finally
        {
            GraphicsSettings.defaultRenderPipeline=pipeline;QualitySettings.renderPipeline=quality;
            EditorSceneManager.ClosePreviewScene(scene);Object.DestroyImmediate(target);
            if(pixels!=null)Object.DestroyImmediate(pixels);
            if(portraitSprite!=null)Object.DestroyImmediate(portraitSprite);
        }
    }
}
