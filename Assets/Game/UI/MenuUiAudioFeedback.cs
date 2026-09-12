using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public sealed class MenuUiAudioFeedback : MonoBehaviour, IPointerEnterHandler, ISelectHandler, IPointerClickHandler, ISubmitHandler
{
    static AudioSource source;
    static AudioClip hoverClip;
    static AudioClip clickClip;
    static float lastHoverTime;

    void Awake()
    {
        EnsureAudio();
    }

    public void OnPointerEnter(PointerEventData eventData) => PlayHover();
    public void OnSelect(BaseEventData eventData) => PlayHover();
    public void OnPointerClick(PointerEventData eventData) => PlayClick();
    public void OnSubmit(BaseEventData eventData) => PlayClick();

    void PlayHover()
    {
        Button button = GetComponent<Button>();
        if (button != null && !button.interactable) return;
        if (Time.unscaledTime - lastHoverTime < .055f) return;
        lastHoverTime = Time.unscaledTime;
        EnsureAudio();
        source.PlayOneShot(hoverClip, .28f);
    }

    void PlayClick()
    {
        Button button = GetComponent<Button>();
        if (button != null && !button.interactable) return;
        EnsureAudio();
        source.PlayOneShot(clickClip, .42f);
    }

    static void EnsureAudio()
    {
        if (source != null) return;
        GameObject go = new GameObject("MenuUiAudio");
        Object.DontDestroyOnLoad(go);
        source = go.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.loop = false;
        source.spatialBlend = 0f;
        source.ignoreListenerPause = true;
        hoverClip = BuildTone("UI_Hover", 720f, 0.045f, .34f, 0.018f);
        clickClip = BuildTone("UI_Click", 470f, 0.075f, .48f, 0.012f);
    }

    static AudioClip BuildTone(string name, float frequency, float duration, float amplitude, float attack)
    {
        const int sampleRate = 44100;
        int samples = Mathf.Max(32, Mathf.CeilToInt(duration * sampleRate));
        float[] data = new float[samples];
        for (int i = 0; i < samples; i++)
        {
            float t = i / (float)sampleRate;
            float a = Mathf.Clamp01(t / Mathf.Max(.001f, attack));
            float r = Mathf.Clamp01((duration - t) / Mathf.Max(.001f, duration * .55f));
            float envelope = a * r;
            float fundamental = Mathf.Sin(2f * Mathf.PI * frequency * t);
            float harmonic = .22f * Mathf.Sin(2f * Mathf.PI * frequency * 2f * t);
            data[i] = (fundamental + harmonic) * envelope * amplitude;
        }
        AudioClip clip = AudioClip.Create(name, samples, 1, sampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }
}
