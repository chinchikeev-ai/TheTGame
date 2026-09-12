using System.Collections;
using UnityEngine;

public class RuntimeEffects : MonoBehaviour
{
    public static RuntimeEffects Instance { get; private set; }
    AudioSource source;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0f;
    }

    public void PlayShot(TowerType type, Vector3 position)
    {
        float freq = type == TowerType.Cannon ? 120f : type == TowerType.Slow ? 520f : 820f;
        float dur = type == TowerType.Cannon ? 0.14f : 0.055f;
        source.PlayOneShot(MakeTone(freq, dur, type == TowerType.Cannon ? 0.25f : 0.12f));
        StartCoroutine(Flash(position, type == TowerType.Slow ? new Color(0.2f,0.8f,1f) : new Color(1f,0.65f,0.1f)));
    }

    public void PlayHit(Vector3 position, bool heavy = false)
    {
        source.PlayOneShot(MakeTone(heavy ? 90f : 240f, 0.06f, 0.10f));
        StartCoroutine(Burst(position, heavy ? 0.7f : 0.35f, new Color(1f,0.45f,0.12f)));
    }

    public void PlayDeath(Vector3 position, bool boss = false)
    {
        source.PlayOneShot(MakeTone(boss ? 70f : 150f, boss ? 0.35f : 0.16f, boss ? 0.32f : 0.16f));
        StartCoroutine(Burst(position, boss ? 2.0f : 0.85f, boss ? new Color(0.9f,0.1f,0.12f) : new Color(1f,0.72f,0.18f)));
    }

    AudioClip MakeTone(float frequency, float duration, float volume)
    {
        int rate = 44100;
        int samples = Mathf.Max(1, Mathf.RoundToInt(rate * duration));
        float[] data = new float[samples];
        for (int i = 0; i < samples; i++)
        {
            float t = i / (float)rate;
            float envelope = 1f - i / (float)samples;
            data[i] = Mathf.Sin(2f * Mathf.PI * frequency * t) * envelope * volume;
        }
        AudioClip clip = AudioClip.Create("RuntimeTone", samples, 1, rate, false);
        clip.SetData(data, 0);
        return clip;
    }

    IEnumerator Flash(Vector3 position, Color color)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.name = "MuzzleFlash";
        go.transform.position = position;
        go.transform.localScale = Vector3.one * 0.28f;
        Destroy(go.GetComponent<Collider>());
        TowerFactory.SetColor(go, color);
        yield return new WaitForSeconds(0.045f);
        if (go != null) Destroy(go);
    }

    IEnumerator Burst(Vector3 position, float size, Color color)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.name = "ImpactFX";
        go.transform.position = position;
        go.transform.localScale = Vector3.one * 0.1f;
        Destroy(go.GetComponent<Collider>());
        TowerFactory.SetColor(go, color);
        float t = 0f;
        while (t < 0.18f && go != null)
        {
            t += Time.deltaTime;
            go.transform.localScale = Vector3.one * Mathf.Lerp(0.1f, size, t / 0.18f);
            yield return null;
        }
        if (go != null) Destroy(go);
    }
}
