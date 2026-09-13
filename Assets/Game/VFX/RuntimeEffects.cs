using System.Collections.Generic;
using UnityEngine;

public class RuntimeEffects : MonoBehaviour
{
    public static RuntimeEffects Instance { get; private set; }

    readonly Dictionary<Vector3Int, AudioClip> toneCache = new Dictionary<Vector3Int, AudioClip>();
    AudioSource source;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0f;
    }

    void OnDestroy()
    {
        if (Instance != this) return;
        foreach (AudioClip clip in toneCache.Values)
            if (clip != null) Destroy(clip);
        toneCache.Clear();
        Instance = null;
    }

    public void PlayShot(TowerType type, Vector3 position)
    {
        float freq = 820f;
        float dur = .055f;
        float volume = .12f;
        Color flash = new Color(1f,.68f,.18f);
        float size = .28f;

        switch(type)
        {
            case TowerType.Cannon: freq=120f; dur=.14f; volume=.25f; flash=new Color(1f,.42f,.08f); size=.48f; break;
            case TowerType.Slow: freq=520f; flash=new Color(.30f,.70f,1f); size=.34f; break;
            case TowerType.SpearThrower: freq=680f; flash=new Color(.92f,.78f,.36f); size=.24f; break;
            case TowerType.FireTower: freq=180f; dur=.10f; volume=.18f; flash=new Color(1f,.18f,.02f); size=.52f; break;
            case TowerType.TrojanGuard: freq=260f; dur=.08f; flash=new Color(.76f,.38f,.12f); size=.26f; break;
        }

        source.PlayOneShot(GetTone(freq,dur,volume));
        Flash(position, flash, size);
    }

    public void PlayHit(Vector3 position, bool heavy = false)
    {
        source.PlayOneShot(GetTone(heavy ? 90f : 240f, .06f, .10f));
        Burst(position, heavy ? .75f : .34f, heavy ? new Color(1f,.24f,.05f) : new Color(1f,.62f,.18f));
    }

    public void PlayDeath(Vector3 position, bool boss = false)
    {
        source.PlayOneShot(GetTone(boss ? 70f : 150f, boss ? .35f : .16f, boss ? .32f : .16f));
        Burst(position, boss ? 2.2f : .88f, boss ? new Color(.88f,.06f,.06f) : new Color(1f,.72f,.18f));
        if (boss) BossShockwave(position);
    }

    public void PlayHeroPulse(Vector3 position, Color color, float radius = 2.8f, float duration = .42f)
    {
        source.PlayOneShot(GetTone(330f, .12f, .16f));
        GroundPulse(position, color, radius, duration);
    }

    public void PlayBuildSuccess(Vector3 position, bool upgrade)
    {
        source.PlayOneShot(GetTone(upgrade ? 620f : 510f, upgrade ? .16f : .12f, .18f));
        source.PlayOneShot(GetTone(upgrade ? 880f : 720f, .08f, .10f));
        GroundPulse(position, upgrade ? new Color(1f,.68f,.18f) : new Color(.24f,.88f,.34f), upgrade ? 1.45f : 1.15f, .32f);
        BuildBurst(position, upgrade);
    }

    public void PlayBuildDenied(Vector3 position)
    {
        source.PlayOneShot(GetTone(115f, .12f, .18f));
        GroundPulse(position, new Color(.95f,.12f,.08f), .8f, .22f);
    }

    AudioClip GetTone(float frequency, float duration, float volume)
    {
        Vector3Int key = new Vector3Int(
            Mathf.RoundToInt(frequency * 10f),
            Mathf.RoundToInt(duration * 1000f),
            Mathf.RoundToInt(volume * 1000f));

        if (toneCache.TryGetValue(key, out AudioClip cached) && cached != null)
            return cached;

        const int rate = 44100;
        int samples = Mathf.Max(1, Mathf.RoundToInt(rate * duration));
        float[] data = new float[samples];
        for (int i = 0; i < samples; i++)
        {
            float t = i / (float)rate;
            float envelope = 1f - i / (float)samples;
            data[i] = Mathf.Sin(2f * Mathf.PI * frequency * t) * envelope * volume;
        }

        AudioClip clip = AudioClip.Create($"RuntimeTone_{key.x}_{key.y}_{key.z}", samples, 1, rate, false);
        clip.SetData(data, 0);
        toneCache[key] = clip;
        return clip;
    }

    static void Flash(Vector3 position, Color color, float size)
    {
        CombatVfxPool.Spawn(
            PrimitiveType.Sphere,
            position,
            Vector3.one * size,
            Vector3.one * (size * .20f),
            color,
            .07f,
            Vector3.zero);
    }

    static void Burst(Vector3 position, float size, Color color)
    {
        CombatVfxPool.Spawn(
            PrimitiveType.Sphere,
            position,
            Vector3.one * .10f,
            Vector3.one * size,
            color,
            .18f,
            Vector3.up * .06f);
    }

    static void BuildBurst(Vector3 position, bool upgrade)
    {
        Color color = upgrade ? new Color(1f,.65f,.16f) : new Color(.30f,.92f,.38f);
        for (int i = 0; i < 5; i++)
        {
            Vector3 start = position + new Vector3((i - 2) * .18f, .12f + (i % 2) * .08f, ((i * 3) % 5 - 2) * .12f);
            CombatVfxPool.Spawn(
                PrimitiveType.Sphere,
                start,
                Vector3.one * .08f,
                Vector3.one * .015f,
                color,
                .28f + i * .025f,
                Vector3.up * .8f);
        }
    }

    static void BossShockwave(Vector3 position)
    {
        CombatVfxPool.Spawn(
            PrimitiveType.Cylinder,
            position + Vector3.up * .04f,
            new Vector3(.2f,.02f,.2f),
            new Vector3(3.8f,.02f,3.8f),
            new Color(.95f,.62f,.12f),
            .5f,
            Vector3.zero);
    }

    static void GroundPulse(Vector3 position, Color color, float radius, float duration)
    {
        CombatVfxPool.Spawn(
            PrimitiveType.Cylinder,
            position + Vector3.up * .045f,
            new Vector3(.22f,.018f,.22f),
            new Vector3(radius,.018f,radius),
            color,
            duration,
            Vector3.zero);
    }
}
