using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class CombatFlightPresentation : MonoBehaviour
{
    enum FlightStyle
    {
        Arrow,
        Spear
    }

    static readonly Stack<CombatFlightPresentation> Pool = new Stack<CombatFlightPresentation>();
    static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    static readonly int ColorId = Shader.PropertyToID("_Color");
    static Material trailMaterial;

    Transform target;
    Vector3 targetOffset;
    Vector3 startPoint;
    Vector3 lastTargetPoint;
    Vector3 previousPoint;
    Action<Vector3> onArrive;
    float elapsed;
    float duration;
    float arcHeight;
    bool inPool;

    Transform shaft;
    Transform tip;
    Transform featherA;
    Transform featherB;
    TrailRenderer trail;
    MaterialPropertyBlock propertyBlock;

    public static void SpawnArrow(Vector3 start, Transform target, Vector3 offset, Action<Vector3> onArrive)
    {
        Spawn(start, target, offset, 18f, FlightStyle.Arrow, onArrive);
    }

    public static void SpawnSpear(Vector3 start, Transform target, Vector3 offset, Action<Vector3> onArrive)
    {
        Spawn(start, target, offset, 14f, FlightStyle.Spear, onArrive);
    }

    static void Spawn(Vector3 start, Transform target, Vector3 offset, float speed, FlightStyle style, Action<Vector3> onArrive)
    {
        if (target == null) return;

        CombatFlightPresentation flight = Acquire();
        flight.gameObject.name = style == FlightStyle.Arrow ? "Character Arrow Flight" : "Hector Spear Flight";
        flight.gameObject.SetActive(true);
        flight.Begin(start, target, offset, speed, style, onArrive);
    }

    static CombatFlightPresentation Acquire()
    {
        while (Pool.Count > 0)
        {
            CombatFlightPresentation cached = Pool.Pop();
            if (cached == null) continue;
            cached.inPool = false;
            return cached;
        }

        GameObject root = new GameObject("Combat Flight");
        CombatFlightPresentation flight = root.AddComponent<CombatFlightPresentation>();
        flight.BuildVisuals();
        return flight;
    }

    void Begin(Vector3 start, Transform newTarget, Vector3 offset, float speed, FlightStyle style, Action<Vector3> callback)
    {
        if (shaft == null) BuildVisuals();

        target = newTarget;
        targetOffset = offset;
        startPoint = start;
        lastTargetPoint = target.position + targetOffset;
        previousPoint = start;
        elapsed = 0f;
        float distance = Vector3.Distance(startPoint, lastTargetPoint);
        duration = Mathf.Clamp(distance / Mathf.Max(4f, speed), .12f, 1.25f);
        arcHeight = style == FlightStyle.Arrow
            ? Mathf.Clamp(distance * .055f, .18f, .90f)
            : Mathf.Clamp(distance * .035f, .12f, .55f);
        onArrive = callback;

        ConfigureVisuals(style);
        transform.position = startPoint;
        Vector3 initialDirection = lastTargetPoint - startPoint;
        if (initialDirection.sqrMagnitude > .0001f)
            transform.rotation = Quaternion.LookRotation(initialDirection.normalized);
    }

    void Update()
    {
        if (inPool) return;

        if (target != null)
            lastTargetPoint = target.position + targetOffset;

        elapsed += Time.deltaTime;
        float t = duration <= .001f ? 1f : Mathf.Clamp01(elapsed / duration);
        Vector3 basePoint = Vector3.Lerp(startPoint, lastTargetPoint, t);
        Vector3 position = basePoint + Vector3.up * (Mathf.Sin(t * Mathf.PI) * arcHeight);
        Vector3 direction = position - previousPoint;
        if (direction.sqrMagnitude > .000001f)
            transform.rotation = Quaternion.LookRotation(direction.normalized);
        transform.position = position;
        previousPoint = position;

        if (t >= 1f) Arrive();
    }

    void Arrive()
    {
        Vector3 point = lastTargetPoint;
        Action<Vector3> callback = onArrive;
        Release();
        callback?.Invoke(point);
    }

    void Release()
    {
        if (inPool) return;
        inPool = true;
        target = null;
        onArrive = null;
        if (trail != null)
        {
            trail.Clear();
            trail.enabled = false;
        }
        gameObject.SetActive(false);
        Pool.Push(this);
    }

    void BuildVisuals()
    {
        if (shaft != null) return;

        shaft = CreatePart("Shaft", PrimitiveType.Cylinder, transform);
        tip = CreatePart("Tip", PrimitiveType.Cube, transform);
        featherA = CreatePart("Fletching A", PrimitiveType.Cube, transform);
        featherB = CreatePart("Fletching B", PrimitiveType.Cube, transform);

        trail = gameObject.GetComponent<TrailRenderer>();
        if (trail == null) trail = gameObject.AddComponent<TrailRenderer>();
        trail.enabled = false;
        trail.numCornerVertices = 2;
        trail.numCapVertices = 2;
        trail.minVertexDistance = .03f;
        trail.sharedMaterial = GetTrailMaterial();
        propertyBlock = new MaterialPropertyBlock();
    }

    void ConfigureVisuals(FlightStyle style)
    {
        bool arrow = style == FlightStyle.Arrow;

        shaft.localPosition = Vector3.zero;
        shaft.localRotation = Quaternion.Euler(90f, 0f, 0f);
        shaft.localScale = arrow ? new Vector3(.018f, .42f, .018f) : new Vector3(.030f, .85f, .030f);

        tip.localPosition = arrow ? new Vector3(0f, 0f, .48f) : new Vector3(0f, 0f, .99f);
        tip.localRotation = Quaternion.Euler(0f, 0f, 45f);
        tip.localScale = arrow ? new Vector3(.060f, .060f, .12f) : new Vector3(.105f, .105f, .28f);

        featherA.gameObject.SetActive(arrow);
        featherB.gameObject.SetActive(arrow);
        if (arrow)
        {
            featherA.localPosition = new Vector3(.045f, 0f, -.35f);
            featherA.localRotation = Quaternion.Euler(0f, 0f, 25f);
            featherA.localScale = new Vector3(.055f, .015f, .17f);
            featherB.localPosition = new Vector3(-.045f, 0f, -.35f);
            featherB.localRotation = Quaternion.Euler(0f, 0f, -25f);
            featherB.localScale = new Vector3(.055f, .015f, .17f);
        }

        SetPartColor(shaft, arrow ? new Color(.34f, .18f, .07f) : new Color(.30f, .15f, .055f));
        SetPartColor(tip, arrow ? new Color(.72f, .70f, .64f) : new Color(.78f, .55f, .18f));
        if (arrow)
        {
            SetPartColor(featherA, new Color(.42f, .10f, .055f));
            SetPartColor(featherB, new Color(.42f, .10f, .055f));
        }

        trail.Clear();
        trail.enabled = true;
        trail.time = arrow ? .10f : .14f;
        trail.startWidth = arrow ? .025f : .035f;
        trail.endWidth = 0f;
        Color trailColor = arrow ? new Color(.86f, .74f, .48f, .45f) : new Color(.92f, .66f, .20f, .50f);
        trail.startColor = trailColor;
        trail.endColor = new Color(trailColor.r, trailColor.g, trailColor.b, 0f);
    }

    static Transform CreatePart(string name, PrimitiveType primitive, Transform parent)
    {
        GameObject part = GameObject.CreatePrimitive(primitive);
        part.name = name;
        part.transform.SetParent(parent, false);
        Collider collider = part.GetComponent<Collider>();
        if (collider != null) Destroy(collider);
        return part.transform;
    }

    void SetPartColor(Transform part, Color color)
    {
        if (part == null) return;
        Renderer renderer = part.GetComponent<Renderer>();
        if (renderer == null) return;
        if (propertyBlock == null) propertyBlock = new MaterialPropertyBlock();
        renderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor(BaseColorId, color);
        propertyBlock.SetColor(ColorId, color);
        renderer.SetPropertyBlock(propertyBlock);
        propertyBlock.Clear();
    }

    static Material GetTrailMaterial()
    {
        if (trailMaterial != null) return trailMaterial;
        Shader shader = Shader.Find("Universal Render Pipeline/Particles/Unlit")
            ?? Shader.Find("Universal Render Pipeline/Unlit")
            ?? Shader.Find("Sprites/Default")
            ?? Shader.Find("Standard");
        trailMaterial = new Material(shader) { name = "CharacterProjectileTrail" };
        return trailMaterial;
    }
}
