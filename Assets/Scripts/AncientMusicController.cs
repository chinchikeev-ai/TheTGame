using UnityEngine;

public class AncientMusicController : MonoBehaviour
{
    public static AncientMusicController Instance { get; private set; }

    AudioSource source;
    AudioClip musicClip;

    const int SampleRate = 44100;
    const float Bpm = 78f;
    const int Bars = 16;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.loop = true;
        source.spatialBlend = 0f;
        source.volume = 0.22f;

        musicClip = BuildAncientLoop();
        source.clip = musicClip;
        source.Play();
    }

    public void SetVolume(float value)
    {
        source.volume = Mathf.Clamp01(value);
    }

    AudioClip BuildAncientLoop()
    {
        float beat = 60f / Bpm;
        float bar = beat * 4f;
        float duration = Bars * bar;
        int samples = Mathf.CeilToInt(duration * SampleRate);
        float[] data = new float[samples];

        // D Dorian / ancient modal palette: D E F G A B C D.
        float[] scale = { 293.66f, 329.63f, 349.23f, 392.00f, 440.00f, 493.88f, 523.25f, 587.33f };
        int[] melody = {
            0,2,3,4, 3,2,0,1,
            0,3,4,5, 4,3,2,0,
            0,2,4,3, 2,1,0,2,
            3,4,5,4, 3,2,1,0
        };

        for (int i = 0; i < melody.Length; i++)
        {
            float start = i * beat * 0.5f;
            AddPluck(data, scale[melody[i]], start, beat * 0.42f, 0.22f);
        }

        // Drone on D/A for a restrained antique atmosphere.
        AddDrone(data, 146.83f, 0f, duration, 0.055f);
        AddDrone(data, 220.00f, 0f, duration, 0.035f);

        // Slow flute-like answer every two bars.
        for (int barIndex = 0; barIndex < Bars; barIndex += 2)
        {
            float start = barIndex * bar + beat;
            AddFlute(data, scale[(barIndex / 2) % 2 == 0 ? 4 : 3], start, beat * 2.4f, 0.075f);
        }

        AudioClip clip = AudioClip.Create("AncientGreekLoop", samples, 1, SampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }

    void AddPluck(float[] data, float frequency, float startTime, float duration, float volume)
    {
        int start = Mathf.RoundToInt(startTime * SampleRate);
        int length = Mathf.RoundToInt(duration * SampleRate);
        for (int i = 0; i < length && start + i < data.Length; i++)
        {
            float t = i / (float)SampleRate;
            float envelope = Mathf.Exp(-5.2f * t / Mathf.Max(duration, 0.001f));
            float fundamental = Mathf.Sin(2f * Mathf.PI * frequency * t);
            float harmonic = 0.34f * Mathf.Sin(2f * Mathf.PI * frequency * 2f * t);
            float harmonic2 = 0.12f * Mathf.Sin(2f * Mathf.PI * frequency * 3f * t);
            data[start + i] += (fundamental + harmonic + harmonic2) * envelope * volume;
        }
    }

    void AddFlute(float[] data, float frequency, float startTime, float duration, float volume)
    {
        int start = Mathf.RoundToInt(startTime * SampleRate);
        int length = Mathf.RoundToInt(duration * SampleRate);
        for (int i = 0; i < length && start + i < data.Length; i++)
        {
            float t = i / (float)SampleRate;
            float attack = Mathf.Clamp01(t / 0.18f);
            float release = Mathf.Clamp01((duration - t) / 0.35f);
            float vibrato = 1f + 0.0035f * Mathf.Sin(2f * Mathf.PI * 5.2f * t);
            float wave = Mathf.Sin(2f * Mathf.PI * frequency * vibrato * t) + 0.15f * Mathf.Sin(2f * Mathf.PI * frequency * 2f * t);
            data[start + i] += wave * attack * release * volume;
        }
    }

    void AddDrone(float[] data, float frequency, float startTime, float duration, float volume)
    {
        int start = Mathf.RoundToInt(startTime * SampleRate);
        int length = Mathf.RoundToInt(duration * SampleRate);
        for (int i = 0; i < length && start + i < data.Length; i++)
        {
            float t = i / (float)SampleRate;
            float slowPulse = 0.75f + 0.25f * Mathf.Sin(2f * Mathf.PI * 0.16f * t);
            data[start + i] += Mathf.Sin(2f * Mathf.PI * frequency * t) * slowPulse * volume;
        }
    }
}
