using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    static readonly Dictionary<TowerType, Material> TrailMaterials = new Dictionary<TowerType, Material>();
    static readonly Stack<Projectile> Pool = new Stack<Projectile>();
    static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    static readonly int ColorId = Shader.PropertyToID("_Color");

    Enemy target;
    float damage;
    float speed;
    float splashRadius;
    float slowMultiplier = 1f;
    float slowDuration;
    float expiresAt;
    TowerType sourceType;
    Renderer coreRenderer;
    TrailRenderer trail;
    Light pointLight;
    MaterialPropertyBlock propertyBlock;
    bool inPool;

    public static void Spawn(Vector3 start, Enemy newTarget, float newDamage, float newSpeed, float newSplashRadius,
        float newSlowMultiplier, float newSlowDuration, TowerType newSourceType, string displayName)
    {
        Projectile projectile = Acquire();
        projectile.gameObject.name = displayName + " Projectile";
        projectile.transform.position = start;
        projectile.transform.rotation = Quaternion.identity;
        projectile.gameObject.SetActive(true);
        projectile.Init(newTarget,newDamage,newSpeed,newSplashRadius,newSlowMultiplier,newSlowDuration,newSourceType);
    }

    static Projectile Acquire()
    {
        while (Pool.Count > 0)
        {
            Projectile cached = Pool.Pop();
            if (cached == null) continue;
            cached.inPool = false;
            return cached;
        }

        GameObject projectileObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Collider collider = projectileObject.GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = false;
            Object.Destroy(collider);
        }
        Projectile projectile = projectileObject.AddComponent<Projectile>();
        projectile.EnsureVisualComponents();
        return projectile;
    }

    public void Init(Enemy newTarget, float newDamage, float newSpeed, float newSplashRadius = 0f,
        float newSlowMultiplier = 1f, float newSlowDuration = 0f, TowerType newSourceType = TowerType.MachineGun)
    {
        target = newTarget;
        damage = newDamage;
        speed = newSpeed;
        splashRadius = newSplashRadius;
        slowMultiplier = newSlowMultiplier;
        slowDuration = newSlowDuration;
        sourceType = newSourceType;
        expiresAt = Time.time + 4f;
        ConfigureVisuals();
    }

    void Update()
    {
        if (inPool) return;
        if (target == null || Time.time >= expiresAt)
        {
            Release();
            return;
        }

        Vector3 aim = target.transform.position + Vector3.up * .7f;
        Vector3 direction = aim - transform.position;
        float move = speed * Time.deltaTime;
        if (direction.magnitude <= move + .12f)
        {
            Impact(target.transform.position);
            Release();
            return;
        }

        Vector3 normalized = direction.normalized;
        transform.position += normalized * move;
        transform.rotation = Quaternion.LookRotation(normalized);
    }

    void Impact(Vector3 point)
    {
        // One visual owner for projectile impacts. RuntimeEffects contributes audio only.
        RuntimeEffects.Instance?.PlayHitSound(splashRadius > .01f);
        CombatImpactPresentation.ProjectileHit(point + Vector3.up * .15f, sourceType);

        if (splashRadius > .01f)
        {
            float radiusSq = splashRadius * splashRadius;
            foreach (Enemy enemy in EnemyRegistry.All)
            {
                if (enemy == null) continue;
                if ((enemy.transform.position - point).sqrMagnitude <= radiusSq) Apply(enemy);
            }
        }
        else if (target != null)
        {
            Apply(target);
        }
    }

    void Apply(Enemy enemy)
    {
        enemy.ReceiveDamage(new DamagePacket(damage, DamageRules.ForTower(sourceType), sourceType));
        if (slowMultiplier < .999f && slowDuration > 0f) enemy.ApplySlow(slowMultiplier, slowDuration);
        if (sourceType == TowerType.FireTower) enemy.ApplyBurn(Mathf.Max(6f, damage * .35f), 4f);
        if (sourceType == TowerType.Slow) enemy.ApplyArmorBreak(.18f, 4f);
    }

    void ConfigureVisuals()
    {
        EnsureVisualComponents();
        Color core = ProjectileColor(sourceType);
        transform.localScale = ProjectileScale(sourceType);
        ApplyCoreColor(core);

        trail.Clear();
        trail.enabled = true;
        trail.time = sourceType == TowerType.FireTower ? .42f : .26f;
        trail.startWidth = sourceType == TowerType.Cannon ? .26f : sourceType == TowerType.SpearThrower ? .10f : .15f;
        trail.endWidth = 0f;
        trail.numCornerVertices = 3;
        trail.numCapVertices = 4;
        trail.minVertexDistance = .04f;
        trail.sharedMaterial = GetTrailMaterial(sourceType, core);
        trail.startColor = new Color(core.r, core.g, core.b, .78f);
        trail.endColor = new Color(core.r, core.g, core.b, 0f);

        bool illuminated = sourceType == TowerType.FireTower || sourceType == TowerType.Cannon;
        pointLight.enabled = illuminated;
        if (illuminated)
        {
            pointLight.color = core;
            pointLight.range = sourceType == TowerType.FireTower ? 2.3f : 1.6f;
            pointLight.intensity = sourceType == TowerType.FireTower ? 2.2f : 1.3f;
        }
    }

    void EnsureVisualComponents()
    {
        if (coreRenderer == null) coreRenderer = GetComponent<Renderer>();
        if (propertyBlock == null) propertyBlock = new MaterialPropertyBlock();
        if (trail == null)
        {
            trail = GetComponent<TrailRenderer>();
            if (trail == null) trail = gameObject.AddComponent<TrailRenderer>();
        }
        if (pointLight == null)
        {
            pointLight = GetComponent<Light>();
            if (pointLight == null) pointLight = gameObject.AddComponent<Light>();
            pointLight.type = LightType.Point;
            pointLight.enabled = false;
        }
    }

    void ApplyCoreColor(Color color)
    {
        if (coreRenderer == null) return;
        coreRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor(BaseColorId, color);
        propertyBlock.SetColor(ColorId, color);
        coreRenderer.SetPropertyBlock(propertyBlock);
    }

    void Release()
    {
        if (inPool) return;
        inPool = true;
        target = null;
        if (trail != null)
        {
            trail.Clear();
            trail.enabled = false;
        }
        if (pointLight != null) pointLight.enabled = false;
        gameObject.SetActive(false);
        Pool.Push(this);
    }

    static Vector3 ProjectileScale(TowerType type)
    {
        switch (type)
        {
            case TowerType.Cannon: return new Vector3(.30f,.30f,.42f);
            case TowerType.SpearThrower: return new Vector3(.10f,.10f,.46f);
            case TowerType.FireTower: return new Vector3(.24f,.24f,.34f);
            case TowerType.Slow: return new Vector3(.20f,.20f,.20f);
            default: return new Vector3(.16f,.16f,.22f);
        }
    }

    static Color ProjectileColor(TowerType type)
    {
        switch (type)
        {
            case TowerType.Cannon: return new Color(1f,.36f,.07f);
            case TowerType.Slow: return new Color(.20f,.74f,1f);
            case TowerType.SpearThrower: return new Color(.92f,.78f,.36f);
            case TowerType.FireTower: return new Color(1f,.22f,.02f);
            case TowerType.TrojanGuard: return new Color(.86f,.42f,.13f);
            default: return new Color(1f,.76f,.16f);
        }
    }

    static Material GetTrailMaterial(TowerType type, Color color)
    {
        if (TrailMaterials.TryGetValue(type, out Material cached) && cached != null) return cached;
        Shader shader = Shader.Find("Universal Render Pipeline/Particles/Unlit")
            ?? Shader.Find("Universal Render Pipeline/Unlit")
            ?? Shader.Find("Sprites/Default")
            ?? Shader.Find("Standard");
        Material material = new Material(shader);
        material.name = $"ProjectileTrail_{type}";
        if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
        if (material.HasProperty("_Color")) material.SetColor("_Color", color);
        if (material.HasProperty("_TintColor")) material.SetColor("_TintColor", color);
        TrailMaterials[type] = material;
        return material;
    }
}
