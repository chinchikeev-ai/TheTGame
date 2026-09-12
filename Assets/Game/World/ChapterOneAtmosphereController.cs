using UnityEngine;

public class ChapterOneAtmosphereController : MonoBehaviour
{
    Light sun;
    float phase;

    void Start()
    {
        RenderSettings.fog = true;
        RenderSettings.fogColor = new Color(.48f, .52f, .50f);
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogStartDistance = 22f;
        RenderSettings.fogEndDistance = 58f;
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(.42f, .39f, .31f);

        sun = FindFirstObjectByType<Light>();
        if (sun != null && sun.type == LightType.Directional)
        {
            sun.shadows = LightShadows.Soft;
            sun.shadowStrength = .72f;
        }
        phase = Random.value * 10f;
    }

    void Update()
    {
        if (sun == null) return;
        float pulse = Mathf.Sin(Time.time * .16f + phase) * .035f;
        sun.intensity = 1.34f + pulse;
    }
}
