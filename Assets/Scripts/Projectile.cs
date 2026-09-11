using UnityEngine;

public class Projectile : MonoBehaviour
{
    Enemy target;
    float damage;
    float speed;
    float splashRadius;
    float slowMultiplier = 1f;
    float slowDuration;
    TowerType sourceType;

    public void Init(Enemy newTarget, float newDamage, float newSpeed, float newSplashRadius = 0f, float newSlowMultiplier = 1f, float newSlowDuration = 0f, TowerType newSourceType = TowerType.MachineGun)
    {
        target = newTarget;
        damage = newDamage;
        speed = newSpeed;
        splashRadius = newSplashRadius;
        slowMultiplier = newSlowMultiplier;
        slowDuration = newSlowDuration;
        sourceType = newSourceType;
        Destroy(gameObject, 4f);
    }

    void Update()
    {
        if (target == null) { Destroy(gameObject); return; }
        Vector3 aim = target.transform.position + Vector3.up * .7f;
        Vector3 direction = aim - transform.position;
        float move = speed * Time.deltaTime;
        if (direction.magnitude <= move + .12f)
        {
            Impact(target.transform.position);
            Destroy(gameObject);
            return;
        }
        transform.position += direction.normalized * move;
        transform.rotation = Quaternion.LookRotation(direction.normalized);
    }

    void Impact(Vector3 point)
    {
        if (RuntimeEffects.Instance != null) RuntimeEffects.Instance.PlayHit(point + Vector3.up * .4f, splashRadius > .01f);
        if (splashRadius > .01f)
        {
            foreach (Enemy enemy in EnemyRegistry.All)
                if (enemy != null && Vector3.Distance(enemy.transform.position, point) <= splashRadius) Apply(enemy);
        }
        else if (target != null) Apply(target);
    }

    void Apply(Enemy enemy)
    {
        enemy.TakeDamage(damage, sourceType);
        if (slowMultiplier < .999f && slowDuration > 0f) enemy.ApplySlow(slowMultiplier, slowDuration);
    }
}
