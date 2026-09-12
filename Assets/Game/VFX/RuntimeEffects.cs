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

        source.PlayOneShot(MakeTone(freq,dur,volume));
        StartCoroutine(Flash(position,flash,size));
    }

    public void PlayHit(Vector3 position, bool heavy = false)
    {
        source.PlayOneShot(MakeTone(heavy ? 90f : 240f, 0.06f, 0.10f));
        StartCoroutine(Burst(position, heavy ? .75f : .34f, heavy ? new Color(1f,.24f,.05f) : new Color(1f,.62f,.18f)));
    }

    public void PlayDeath(Vector3 position, bool boss = false)
    {
        source.PlayOneShot(MakeTone(boss ? 70f : 150f, boss ? 0.35f : 0.16f, boss ? 0.32f : 0.16f));
        StartCoroutine(Burst(position, boss ? 2.2f : .88f, boss ? new Color(.88f,.06f,.06f) : new Color(1f,.72f,.18f)));
        if(boss) StartCoroutine(BossShockwave(position));
    }

    public void PlayHeroPulse(Vector3 position, Color color, float radius = 2.8f, float duration = .42f)
    {
        source.PlayOneShot(MakeTone(330f, .12f, .16f));
        StartCoroutine(GroundPulse(position, color, radius, duration));
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

    IEnumerator Flash(Vector3 position, Color color, float size)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.name = "MuzzleFlash";
        go.transform.position = position;
        go.transform.localScale = Vector3.one * size;
        Destroy(go.GetComponent<Collider>());
        TowerFactory.SetColor(go, color);
        yield return new WaitForSeconds(.05f);
        if (go != null) Destroy(go);
    }

    IEnumerator Burst(Vector3 position, float size, Color color)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.name = "ImpactFX";
        go.transform.position = position;
        go.transform.localScale = Vector3.one * .1f;
        Destroy(go.GetComponent<Collider>());
        TowerFactory.SetColor(go, color);
        float t = 0f;
        while (t < .18f && go != null)
        {
            t += Time.deltaTime;
            go.transform.localScale = Vector3.one * Mathf.Lerp(.1f, size, t / .18f);
            yield return null;
        }
        if (go != null) Destroy(go);
    }

    IEnumerator BossShockwave(Vector3 position)
    {
        GameObject ring=GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        ring.name="BossDeathShockwave";
        ring.transform.position=position+Vector3.up*.04f;
        ring.transform.localScale=new Vector3(.2f,.02f,.2f);
        Destroy(ring.GetComponent<Collider>());
        TowerFactory.SetColor(ring,new Color(.95f,.62f,.12f));
        float t=0f;
        while(t<.5f && ring!=null)
        {
            t+=Time.deltaTime;
            float s=Mathf.Lerp(.2f,3.8f,t/.5f);
            ring.transform.localScale=new Vector3(s,.02f,s);
            yield return null;
        }
        if(ring!=null) Destroy(ring);
    }

    IEnumerator GroundPulse(Vector3 position, Color color, float radius, float duration)
    {
        GameObject ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        ring.name = "GroundPulse";
        ring.transform.position = position + Vector3.up * .045f;
        ring.transform.localScale = new Vector3(.22f,.018f,.22f);
        Destroy(ring.GetComponent<Collider>());
        TowerFactory.SetColor(ring,color);

        float t = 0f;
        while (t < duration && ring != null)
        {
            t += Time.deltaTime;
            float u = Mathf.Clamp01(t / duration);
            float s = Mathf.Lerp(.22f, radius, u);
            ring.transform.localScale = new Vector3(s,.018f,s);
            ring.transform.Rotate(0f, 120f * Time.deltaTime, 0f, Space.Self);
            yield return null;
        }
        if (ring != null) Destroy(ring);
    }
}
