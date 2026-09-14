using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ChapterOneAtmosphereController : MonoBehaviour
{
    static readonly Color FogColor = new Color(.43f, .50f, .50f);
    static readonly Color FogLift = new Color(.49f, .54f, .51f);
    static readonly Color SkyAmbient = new Color(.42f, .49f, .49f);
    static readonly Color EquatorAmbient = new Color(.55f, .46f, .33f);
    static readonly Color GroundAmbient = new Color(.29f, .23f, .16f);
    static readonly Color SunWarm = new Color(1f, .79f, .55f);
    static readonly Color SunLift = new Color(1f, .88f, .69f);
    static readonly Color Dust = new Color(.58f,.47f,.31f);

    Light sun;
    float phase;
    VolumeProfile runtimeProfile;

    void Start()
    {
        RenderSettings.fog = true;
        RenderSettings.fogColor = FogColor;
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogStartDistance = 27f;
        RenderSettings.fogEndDistance = 64f;
        RenderSettings.ambientMode = AmbientMode.Trilight;
        RenderSettings.ambientSkyColor = SkyAmbient;
        RenderSettings.ambientEquatorColor = EquatorAmbient;
        RenderSettings.ambientGroundColor = GroundAmbient;
        RenderSettings.reflectionIntensity = .90f;

        Camera cam = Camera.main;
        if (cam != null)
        {
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(.24f, .38f, .45f);
            cam.nearClipPlane = .25f;
            cam.farClipPlane = 140f;

            UniversalAdditionalCameraData cameraData = cam.GetComponent<UniversalAdditionalCameraData>();
            if (cameraData == null) cameraData = cam.gameObject.AddComponent<UniversalAdditionalCameraData>();
            cameraData.renderPostProcessing = true;
        }

        GameObject sunObject = GameObject.Find("Directional Light");
        sun = sunObject != null ? sunObject.GetComponent<Light>() : FindFirstObjectByType<Light>();
        if (sun != null && sun.type != LightType.Directional) sun = null;
        if (sun != null)
        {
            sun.color = SunWarm;
            sun.intensity = 1.36f;
            sun.shadows = LightShadows.Soft;
            sun.shadowStrength = .74f;
            sun.shadowBias = .035f;
            sun.shadowNormalBias = .26f;
            sun.transform.rotation = Quaternion.Euler(50f, -35f, 0f);
        }

        BuildPostProcessing();
        BuildBattlefieldAir();
        phase = Random.value * 10f;
    }

    void BuildPostProcessing()
    {
        if (GameObject.Find("Chapter01_GlobalVolume") != null) return;

        GameObject volumeObject = new GameObject("Chapter01_GlobalVolume");
        Volume volume = volumeObject.AddComponent<Volume>();
        volume.isGlobal = true;
        volume.priority = 40f;
        volume.weight = 1f;

        runtimeProfile = ScriptableObject.CreateInstance<VolumeProfile>();
        runtimeProfile.name = "Chapter01_StylizedRuntimeProfile";
        volume.sharedProfile = runtimeProfile;

        Tonemapping tonemapping = runtimeProfile.Add<Tonemapping>(true);
        tonemapping.mode.Override(TonemappingMode.ACES);

        ColorAdjustments color = runtimeProfile.Add<ColorAdjustments>(true);
        color.postExposure.Override(.08f);
        color.contrast.Override(11f);
        color.saturation.Override(9f);
        color.colorFilter.Override(new Color(1f,.97f,.91f,1f));

        WhiteBalance whiteBalance = runtimeProfile.Add<WhiteBalance>(true);
        whiteBalance.temperature.Override(5f);
        whiteBalance.tint.Override(1f);

        Bloom bloom = runtimeProfile.Add<Bloom>(true);
        bloom.threshold.Override(1.04f);
        bloom.intensity.Override(.26f);
        bloom.scatter.Override(.62f);

        Vignette vignette = runtimeProfile.Add<Vignette>(true);
        vignette.color.Override(new Color(.12f,.065f,.035f,1f));
        vignette.intensity.Override(.115f);
        vignette.smoothness.Override(.55f);
        vignette.rounded.Override(false);
    }

    void BuildBattlefieldAir()
    {
        if (GameObject.Find("Chapter01_BattlefieldAir") != null) return;
        GameObject root=new GameObject("Chapter01_BattlefieldAir");
        Vector3[] wisps=
        {
            new Vector3(-8.0f,.17f,5.25f), new Vector3(-6.4f,.15f,-5.1f),
            new Vector3(-2.4f,.16f,4.65f), new Vector3(.8f,.14f,-4.45f),
            new Vector3(4.2f,.16f,4.35f), new Vector3(7.0f,.15f,-3.95f),
            new Vector3(9.0f,.18f,3.25f)
        };
        for(int i=0;i<wisps.Length;i++)
        {
            GameObject wisp=GameObject.CreatePrimitive(PrimitiveType.Sphere);
            wisp.name="Ground Dust Wisp";
            wisp.transform.SetParent(root.transform,false);
            wisp.transform.position=wisps[i];
            wisp.transform.rotation=Quaternion.Euler(0f,-22f+i*17f,0f);
            wisp.transform.localScale=new Vector3(.62f+(i%3)*.16f,.025f,.28f+(i%2)*.09f);
            Collider c=wisp.GetComponent<Collider>();
            if(c!=null) Destroy(c);
            TowerFactory.SetColor(wisp,Dust*(.86f+(i%3)*.04f));
            ChapterOneAmbientMotion motion=wisp.AddComponent<ChapterOneAmbientMotion>();
            motion.kind=ChapterOneAmbientMotion.MotionKind.Dust;
            motion.phase=.35f+i*.77f;
        }

        for(int i=0;i<9;i++)
        {
            GameObject mote=GameObject.CreatePrimitive(PrimitiveType.Sphere);
            mote.name="Sunlit Dust Mote";
            mote.transform.SetParent(root.transform,false);
            mote.transform.position=new Vector3(-5.5f+i*1.75f,.55f+(i%3)*.24f,(i%2==0?3.0f:-3.1f)+(i%4)*.17f);
            mote.transform.localScale=Vector3.one*(.025f+(i%3)*.008f);
            Collider c=mote.GetComponent<Collider>();
            if(c!=null) Destroy(c);
            TowerFactory.SetColor(mote,new Color(.83f,.65f,.34f));
            ChapterOneAmbientMotion motion=mote.AddComponent<ChapterOneAmbientMotion>();
            motion.kind=ChapterOneAmbientMotion.MotionKind.Dust;
            motion.phase=1.1f+i*.51f;
        }
    }

    void Update()
    {
        float drift = (Mathf.Sin(Time.time * .13f + phase) + 1f) * .5f;
        RenderSettings.fogColor = Color.Lerp(FogColor,FogLift,drift*.12f);
        if (sun == null) return;
        sun.intensity = Mathf.Lerp(1.34f, 1.42f, drift);
        sun.color = Color.Lerp(SunWarm, SunLift, drift * .38f);
    }

    void OnDestroy()
    {
        if(runtimeProfile != null) Destroy(runtimeProfile);
    }
}
