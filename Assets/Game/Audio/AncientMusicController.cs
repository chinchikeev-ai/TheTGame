using UnityEngine;

public class AncientMusicController : MonoBehaviour
{
    public static AncientMusicController Instance { get; private set; }

    enum MusicState { Ambient, Battle, Boss }

    AudioSource sourceA;
    AudioSource sourceB;
    AudioClip ambientClip;
    AudioClip battleClip;
    AudioClip bossClip;
    EnemySpawner spawner;
    MusicState state = MusicState.Ambient;
    AudioSource activeSource;
    AudioSource fadeSource;
    float masterVolume = 0.24f;
    float fadeProgress = 1f;

    const int SampleRate = 44100;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        sourceA = CreateSource();
        sourceB = CreateSource();
        activeSource = sourceA;
        fadeSource = sourceB;

        ambientClip = BuildAmbientLoop();
        battleClip = BuildBattleLoop();
        bossClip = BuildBossLoop();

        activeSource.clip = ambientClip;
        activeSource.volume = masterVolume;
        activeSource.Play();
    }

    void Update()
    {
        if (spawner == null) spawner = FindFirstObjectByType<EnemySpawner>();

        MusicState wanted = MusicState.Ambient;
        if (spawner != null && spawner.WaveActive)
            wanted = spawner.CurrentWave >= spawner.maxWaves ? MusicState.Boss : MusicState.Battle;

        if (wanted != state) SwitchTo(wanted);
        UpdateCrossfade();
    }

    AudioSource CreateSource()
    {
        AudioSource s = gameObject.AddComponent<AudioSource>();
        s.playOnAwake = false;
        s.loop = true;
        s.spatialBlend = 0f;
        s.volume = 0f;
        return s;
    }

    void SwitchTo(MusicState next)
    {
        state = next;
        AudioClip nextClip = next == MusicState.Boss ? bossClip : next == MusicState.Battle ? battleClip : ambientClip;

        AudioSource old = activeSource;
        activeSource = fadeSource;
        fadeSource = old;

        activeSource.clip = nextClip;
        activeSource.time = 0f;
        activeSource.volume = 0f;
        activeSource.Play();
        fadeProgress = 0f;
    }

    void UpdateCrossfade()
    {
        if (fadeProgress >= 1f) return;
        fadeProgress = Mathf.Clamp01(fadeProgress + Time.unscaledDeltaTime / 1.8f);
        activeSource.volume = masterVolume * fadeProgress;
        fadeSource.volume = masterVolume * (1f - fadeProgress);
        if (fadeProgress >= 1f && fadeSource.isPlaying) fadeSource.Stop();
    }

    public void SetVolume(float value)
    {
        masterVolume = Mathf.Clamp01(value);
        activeSource.volume = masterVolume * Mathf.Max(fadeProgress, 0.01f);
        fadeSource.volume = masterVolume * (1f - fadeProgress);
    }

    AudioClip BuildAmbientLoop()
    {
        const float bpm = 76f;
        const int bars = 12;
        float beat = 60f / bpm;
        float duration = bars * beat * 4f;
        float[] data = NewBuffer(duration);
        float[] scale = { 293.66f, 329.63f, 349.23f, 392f, 440f, 493.88f, 523.25f, 587.33f };
        int[] melody = { 0,2,3,4, 3,2,0,1, 0,3,4,5, 4,3,2,0, 0,2,4,3, 2,1,0,2 };

        for (int i = 0; i < melody.Length; i++)
            AddPluck(data, scale[melody[i]], i * beat * 0.5f, beat * 0.42f, 0.20f);

        AddDrone(data, 146.83f, 0f, duration, 0.05f);
        AddDrone(data, 220f, 0f, duration, 0.03f);

        for (int b = 0; b < bars; b += 2)
            AddFlute(data, scale[(b / 2) % 2 == 0 ? 4 : 3], b * beat * 4f + beat, beat * 2.3f, 0.07f);

        return MakeClip("Ancient_Ambient", data);
    }

    AudioClip BuildBattleLoop()
    {
        const float bpm = 104f;
        const int bars = 12;
        float beat = 60f / bpm;
        float duration = bars * beat * 4f;
        float[] data = NewBuffer(duration);
        float[] scale = { 293.66f, 311.13f, 349.23f, 392f, 440f, 466.16f, 523.25f, 587.33f }; // D Phrygian color
        int[] motif = { 0,1,3,4, 3,1,0,3, 0,3,4,5, 4,3,1,0 };

        for (int b = 0; b < bars; b++)
        {
            float barStart = b * beat * 4f;
            for (int q = 0; q < 4; q++) AddDrum(data, barStart + q * beat, q == 0 ? 0.28f : 0.18f, q == 0 ? 85f : 115f);
            for (int e = 0; e < 8; e++)
            {
                int note = motif[(b * 8 + e) % motif.Length];
                AddPluck(data, scale[note], barStart + e * beat * 0.5f, beat * 0.34f, 0.15f);
            }
        }

        AddDrone(data, 146.83f, 0f, duration, 0.045f);
        AddDrone(data, 220f, 0f, duration, 0.025f);
        return MakeClip("Ancient_Battle", data);
    }

    AudioClip BuildBossLoop()
    {
        const float bpm = 92f;
        const int bars = 12;
        float beat = 60f / bpm;
        float duration = bars * beat * 4f;
        float[] data = NewBuffer(duration);
        float[] scale = { 220f, 233.08f, 261.63f, 293.66f, 329.63f, 349.23f, 392f, 440f };
        int[] motif = { 0,1,3,2, 0,3,4,3, 1,0,2,3, 4,3,1,0 };

        for (int b = 0; b < bars; b++)
        {
            float barStart = b * beat * 4f;
            AddDrum(data, barStart, 0.36f, 62f);
            AddDrum(data, barStart + beat, 0.23f, 78f);
            AddDrum(data, barStart + beat * 2f, 0.34f, 62f);
            AddDrum(data, barStart + beat * 3f, 0.25f, 78f);

            for (int e = 0; e < 8; e++)
                AddPluck(data, scale[motif[(b * 8 + e) % motif.Length]], barStart + e * beat * 0.5f, beat * 0.38f, 0.14f);

            AddChant(data, b % 2 == 0 ? 110f : 98f, barStart, beat * 4f, 0.055f);
        }

        AddDrone(data, 110f, 0f, duration, 0.08f);
        AddDrone(data, 164.81f, 0f, duration, 0.04f);
        return MakeClip("Ancient_Boss", data);
    }

    float[] NewBuffer(float duration) => new float[Mathf.CeilToInt(duration * SampleRate)];

    AudioClip MakeClip(string name, float[] data)
    {
        for (int i = 0; i < data.Length; i++) data[i] = Mathf.Clamp(data[i], -0.92f, 0.92f);
        AudioClip clip = AudioClip.Create(name, data.Length, 1, SampleRate, false);
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
            float env = Mathf.Exp(-5.4f * t / Mathf.Max(duration, 0.001f));
            float wave = Mathf.Sin(2f * Mathf.PI * frequency * t)
                + 0.32f * Mathf.Sin(4f * Mathf.PI * frequency * t)
                + 0.10f * Mathf.Sin(6f * Mathf.PI * frequency * t);
            data[start + i] += wave * env * volume;
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
            float vibrato = 1f + 0.003f * Mathf.Sin(2f * Mathf.PI * 5f * t);
            float wave = Mathf.Sin(2f * Mathf.PI * frequency * vibrato * t) + 0.12f * Mathf.Sin(4f * Mathf.PI * frequency * t);
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
            float pulse = 0.72f + 0.28f * Mathf.Sin(2f * Mathf.PI * 0.14f * t);
            data[start + i] += Mathf.Sin(2f * Mathf.PI * frequency * t) * pulse * volume;
        }
    }

    void AddDrum(float[] data, float startTime, float volume, float frequency)
    {
        int start = Mathf.RoundToInt(startTime * SampleRate);
        int length = Mathf.RoundToInt(0.28f * SampleRate);
        for (int i = 0; i < length && start + i < data.Length; i++)
        {
            float t = i / (float)SampleRate;
            float env = Mathf.Exp(-15f * t);
            float pitch = frequency * (1f - 0.28f * t);
            data[start + i] += Mathf.Sin(2f * Mathf.PI * pitch * t) * env * volume;
        }
    }

    void AddChant(float[] data, float frequency, float startTime, float duration, float volume)
    {
        int start = Mathf.RoundToInt(startTime * SampleRate);
        int length = Mathf.RoundToInt(duration * SampleRate);
        for (int i = 0; i < length && start + i < data.Length; i++)
        {
            float t = i / (float)SampleRate;
            float attack = Mathf.Clamp01(t / 0.45f);
            float release = Mathf.Clamp01((duration - t) / 0.65f);
            float wave = Mathf.Sin(2f * Mathf.PI * frequency * t)
                + 0.38f * Mathf.Sin(2f * Mathf.PI * frequency * 2f * t)
                + 0.16f * Mathf.Sin(2f * Mathf.PI * frequency * 3f * t);
            data[start + i] += wave * attack * release * volume;
        }
    }
}
