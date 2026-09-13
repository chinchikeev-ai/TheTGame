using System.Collections.Generic;
using UnityEngine;

public sealed class CombatVfxPool : MonoBehaviour
{
    sealed class Entry
    {
        public GameObject gameObject;
        public Renderer renderer;
        public PrimitiveType shape;
        public bool active;
        public float startedAt;
        public float endsAt;
        public Vector3 startPosition;
        public Vector3 startScale;
        public Vector3 endScale;
        public Vector3 drift;
    }

    const int SphereCount = 18;
    const int CubeCount = 8;

    static CombatVfxPool instance;
    static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    static readonly int ColorId = Shader.PropertyToID("_Color");

    readonly List<Entry> entries = new List<Entry>(SphereCount + CubeCount);
    readonly MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
    Material sharedMaterial;

    public static void Spawn(
        PrimitiveType shape,
        Vector3 position,
        Vector3 startScale,
        Vector3 endScale,
        Color color,
        float lifetime,
        Vector3 drift)
    {
        EnsureInstance().SpawnInternal(shape, position, startScale, endScale, color, lifetime, drift);
    }

    static CombatVfxPool EnsureInstance()
    {
        if (instance != null) return instance;
        GameObject go = new GameObject("CombatVfxPool");
        instance = go.AddComponent<CombatVfxPool>();
        return instance;
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        sharedMaterial = Resources.Load<Material>("RuntimeColorMaterial");
        WarmPool(PrimitiveType.Sphere, SphereCount);
        WarmPool(PrimitiveType.Cube, CubeCount);
    }

    void OnDestroy()
    {
        if (instance == this) instance = null;
    }

    void WarmPool(PrimitiveType shape, int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject go = GameObject.CreatePrimitive(shape);
            go.name = shape == PrimitiveType.Cube ? "Pooled Combat Shard" : "Pooled Combat Particle";
            go.transform.SetParent(transform, false);
            Collider collider = go.GetComponent<Collider>();
            if (collider != null) Destroy(collider);

            Renderer renderer = go.GetComponent<Renderer>();
            if (renderer != null && sharedMaterial != null)
                renderer.sharedMaterial = sharedMaterial;

            go.SetActive(false);
            entries.Add(new Entry
            {
                gameObject = go,
                renderer = renderer,
                shape = shape
            });
        }
    }

    void SpawnInternal(
        PrimitiveType shape,
        Vector3 position,
        Vector3 startScale,
        Vector3 endScale,
        Color color,
        float lifetime,
        Vector3 drift)
    {
        Entry entry = Acquire(shape);
        if (entry == null) return;

        float now = Time.time;
        entry.active = true;
        entry.startedAt = now;
        entry.endsAt = now + Mathf.Max(.04f, lifetime);
        entry.startPosition = position;
        entry.startScale = startScale;
        entry.endScale = endScale;
        entry.drift = drift;

        Transform item = entry.gameObject.transform;
        item.position = position;
        item.rotation = shape == PrimitiveType.Cube ? Random.rotation : Quaternion.identity;
        item.localScale = startScale;
        ApplyColor(entry.renderer, color);
        entry.gameObject.SetActive(true);
    }

    Entry Acquire(PrimitiveType shape)
    {
        Entry oldest = null;
        for (int i = 0; i < entries.Count; i++)
        {
            Entry entry = entries[i];
            if (entry.shape != shape) continue;
            if (!entry.active || !entry.gameObject.activeSelf) return entry;
            if (oldest == null || entry.endsAt < oldest.endsAt) oldest = entry;
        }
        return oldest;
    }

    void ApplyColor(Renderer renderer, Color color)
    {
        if (renderer == null) return;
        renderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor(BaseColorId, color);
        propertyBlock.SetColor(ColorId, color);
        renderer.SetPropertyBlock(propertyBlock);
    }

    void Update()
    {
        float now = Time.time;
        for (int i = 0; i < entries.Count; i++)
        {
            Entry entry = entries[i];
            if (!entry.active || entry.gameObject == null || !entry.gameObject.activeSelf) continue;

            if (now >= entry.endsAt)
            {
                entry.active = false;
                entry.gameObject.SetActive(false);
                continue;
            }

            float duration = Mathf.Max(.001f, entry.endsAt - entry.startedAt);
            float t = Mathf.Clamp01((now - entry.startedAt) / duration);
            Transform item = entry.gameObject.transform;
            item.position = entry.startPosition + entry.drift * t;
            item.localScale = Vector3.Lerp(entry.startScale, entry.endScale, t);
            if (entry.shape == PrimitiveType.Cube)
                item.Rotate(95f * Time.deltaTime, 145f * Time.deltaTime, 70f * Time.deltaTime, Space.Self);
        }
    }
}
