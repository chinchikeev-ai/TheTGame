using UnityEngine;

public class ChapterOneAtmosphereController : MonoBehaviour
{
    static readonly Color FogColor = new Color(.36f, .43f, .45f);
    static readonly Color SkyAmbient = new Color(.34f, .40f, .41f);
    static readonly Color EquatorAmbient = new Color(.43f, .38f, .30f);
    static readonly Color GroundAmbient = new Color(.23f, .19f, .14f);
    static readonly Color SunWarm = new Color(1f, .77f, .54f);
    static readonly Color SunLift = new Color(1f, .84f, .64f);

    Light sun;
    float phase;

    void Start()
    {
        RenderSettings.fog = true;
        RenderSettings.fogColor = FogColor;
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogStartDistance = 24f;
        RenderSettings.fogEndDistance = 57f;
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
        RenderSettings.ambientSkyColor = SkyAmbient;
        RenderSettings.ambientEquatorColor = EquatorAmbient;
        RenderSettings.ambientGroundColor = GroundAmbient;
        RenderSettings.reflectionIntensity = .78f;

        Camera cam = Camera.main;
        if (cam != null)
        {
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(.18f, .27f, .30f);
            cam.nearClipPlane = .25f;
            cam.farClipPlane = 120f;
        }

        GameObject sunObject = GameObject.Find("Directional Light");
        sun = sunObject != null ? sunObject.GetComponent<Light>() : FindFirstObjectByType<Light>();
        if (sun != null && sun.type != LightType.Directional) sun = null;
        if (sun != null)
        {
            sun.color = SunWarm;
            sun.intensity = 1.28f;
            sun.shadows = LightShadows.Soft;
            sun.shadowStrength = .76f;
            sun.shadowBias = .035f;
            sun.shadowNormalBias = .28f;
            sun.transform.rotation = Quaternion.Euler(51f, -34f, 0f);
        }

        phase = Random.value * 10f;
    }

    void Update()
    {
        if (sun == null) return;
        float drift = (Mathf.Sin(Time.time * .13f + phase) + 1f) * .5f;
        sun.intensity = Mathf.Lerp(1.25f, 1.31f, drift);
        sun.color = Color.Lerp(SunWarm, SunLift, drift * .34f);
    }
}
