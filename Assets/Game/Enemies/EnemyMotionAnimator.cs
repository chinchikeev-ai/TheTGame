using System.Collections.Generic;
using UnityEngine;

public class EnemyMotionAnimator : MonoBehaviour
{
    readonly List<Transform> animatedParts = new List<Transform>();
    readonly List<Vector3> baseLocalPositions = new List<Vector3>();
    readonly List<Vector3> baseLocalScales = new List<Vector3>();

    EnemyArchetype archetype;
    Transform aura;
    float phase;
    bool initialized;

    public static void Attach(GameObject target, EnemyArchetype enemyArchetype)
    {
        if (target == null || target.GetComponent<EnemyMotionAnimator>() != null) return;
        EnemyMotionAnimator animator = target.AddComponent<EnemyMotionAnimator>();
        animator.archetype = enemyArchetype;
    }

    void Start()
    {
        phase = Random.value * Mathf.PI * 2f;
        CaptureParts();
        if (archetype == EnemyArchetype.Boss) CreateBossAura();
        initialized = true;
    }

    void Update()
    {
        if (!initialized) return;

        float speed = archetype == EnemyArchetype.Runner ? 9.5f : archetype == EnemyArchetype.BatteringRam ? 4.5f : 6.8f;
        float bob = Mathf.Sin(Time.time * speed + phase);
        float sway = Mathf.Cos(Time.time * speed * .5f + phase);

        for (int i = 0; i < animatedParts.Count; i++)
        {
            Transform part = animatedParts[i];
            if (part == null) continue;

            float vertical = archetype == EnemyArchetype.BatteringRam ? .015f : .035f;
            float lateral = archetype == EnemyArchetype.Boss ? .025f : .015f;
            part.localPosition = baseLocalPositions[i] + new Vector3(sway * lateral, Mathf.Abs(bob) * vertical, 0f);

            float squash = archetype == EnemyArchetype.BatteringRam ? .012f : .022f;
            Vector3 scale = baseLocalScales[i];
            part.localScale = new Vector3(scale.x * (1f + bob * squash), scale.y * (1f - bob * squash * .55f), scale.z);
        }

        if (aura != null)
        {
            float pulse = 1f + Mathf.Sin(Time.time * 2.7f + phase) * .08f;
            aura.localScale = new Vector3(2.8f * pulse, .018f, 2.8f * pulse);
            aura.Rotate(0f, 28f * Time.deltaTime, 0f, Space.Self);
        }
    }

    void CaptureParts()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            if (renderer == null) continue;
            Transform part = renderer.transform;
            if (part == transform) continue;
            animatedParts.Add(part);
            baseLocalPositions.Add(part.localPosition);
            baseLocalScales.Add(part.localScale);
        }

        // Never animate the root transform: Enemy owns root movement along the route.
    }

    void CreateBossAura()
    {
        GameObject ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        ring.name = "Menelaus Aura Pulse";
        ring.transform.SetParent(transform, false);
        ring.transform.localPosition = new Vector3(0f, .04f, 0f);
        ring.transform.localScale = new Vector3(2.8f, .018f, 2.8f);
        Destroy(ring.GetComponent<Collider>());
        TowerFactory.SetColor(ring, new Color(.82f,.10f,.06f));
        aura = ring.transform;
    }
}
