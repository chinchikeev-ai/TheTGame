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

    public void PlayShotSound(TowerType type)
    {
        float freq = 820f;
        float dur = .055f;
        float volume = .12f;

        switch(type)
        {
            case TowerType.Cannon: freq=120f; dur=.14f; volume=.25f; break;
            case TowerType.Slow: freq=520f; break;
            case TowerType.SpearThrower: freq=680f; break;
            case TowerType.FireTower: freq=180f; dur=.10f; volume=.18f; break;
            case TowerType.TrojanGuard: freq=260f; dur=.08f; break;
        }

        source.PlayOneShot(GetTone(freq,dur,volume));
    }

    public void PlayHitSound(bool heavy = false)
    {
        source.PlayOneShot(GetTone(heavy ? 90f : 240f, .06f, .10f));
    }

    public void PlayShieldBlockSound(bool heavy = false)
    {
        source.PlayOneShot(GetTone(heavy ? 155f : 205f, heavy ? .11f : .075f, heavy ? .20f : .14f));
        source.PlayOneShot(GetTone(heavy ? 460f : 540f, .045f, heavy ? .10f : .07f));
    }

    public void PlayDeathSound(bool boss = false)
    {
        source.PlayOneShot(GetTone(boss ? 70f : 150f, boss ? .35f : .16f, boss ? .32f : .16f));
    }

    public void PlayHeroAbilitySound()
    {
        source.PlayOneShot(GetTone(330f, .12f, .16f));
    }

    public void PlayBuildSuccessSound(bool upgrade)
    {
        source.PlayOneShot(GetTone(upgrade ? 620f : 510f, upgrade ? .16f : .12f, .18f));
        source.PlayOneShot(GetTone(upgrade ? 880f : 720f, .08f, .10f));
    }

    public void PlayBuildDeniedSound()
    {
        source.PlayOneShot(GetTone(115f, .12f, .18f));
    }

    [System.Obsolete("Use PlayShotSound and CombatImpactPresentation.ShotFlash separately.")]
    public void PlayShot(TowerType type, Vector3 position)
    {
        PlayShotSound(type);
        CombatImpactPresentation.ShotFlash(position, type);
    }

    [System.Obsolete("Use PlayHitSound and CombatImpactPresentation.GenericHit separately.")]
    public void PlayHit(Vector3 position, bool heavy = false)
    {
        PlayHitSound(heavy);
        CombatImpactPresentation.GenericHit(position, heavy);
    }

    [System.Obsolete("Use PlayDeathSound and CombatImpactPresentation.GenericDeath separately.")]
    public void PlayDeath(Vector3 position, bool boss = false)
    {
        PlayDeathSound(boss);
        CombatImpactPresentation.GenericDeath(position, boss);
    }

    [System.Obsolete("Use PlayHeroAbilitySound and CombatImpactPresentation.Pulse separately.")]
    public void PlayHeroPulse(Vector3 position, Color color, float radius = 2.8f, float duration = .42f)
    {
        PlayHeroAbilitySound();
        CombatImpactPresentation.Pulse(position, color, radius, duration);
    }

    [System.Obsolete("Use PlayBuildSuccessSound and BuildFeedbackPresentation.Success separately.")]
    public void PlayBuildSuccess(Vector3 position, bool upgrade)
    {
        PlayBuildSuccessSound(upgrade);
        BuildFeedbackPresentation.Success(position, upgrade);
    }

    [System.Obsolete("Use PlayBuildDeniedSound and BuildFeedbackPresentation.Denied separately.")]
    public void PlayBuildDenied(Vector3 position)
    {
        PlayBuildDeniedSound();
        BuildFeedbackPresentation.Denied(position);
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
}
