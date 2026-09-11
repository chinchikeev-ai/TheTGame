using UnityEngine;

public class Projectile : MonoBehaviour
{
    Enemy target;
    float damage;
    float speed;
    float splashRadius;
    float slowMultiplier = 1f;
    float slowDuration;

    public void Init(Enemy newTarget, float newDamage, float newSpeed, float newSplashRadius = 0f, float newSlowMultiplier = 1f, float newSlowDuration = 0f)
    {
        target = newTarget;
        damage = newDamage;
        speed = newSpeed;
        splashRadius = newSplashRadius;
        slowMultiplier = newSlowMultiplier;
        slowDuration = newSlowDuration;
        Destroy(gameObject, 4f);
    }

    void Update()
    {
        if (target == null) { Destroy(gameObject); return; }

        Vector3 aim = target.transform.position + Vector3.up * 0.7f;
        Vector3 direction = aim - transform.position;
        float move = speed * Time.deltaTime;

        if (direction.magnitude <= move + 0.12f)
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
        if (RuntimeEffects.Instance != null) RuntimeEffects.Instance.PlayHit(point + Vector3.up * 0.4f, splashRadius > 0.01f);

        if (splashRadius > 0.01f)
        {
            foreach (Enemy enemy in EnemyRegistry.All)
                if (enemy != null && Vector3.Distance(enemy.transform.position, point) <= splashRadius) Apply(enemy);
        }
        else if (target != null) Apply(target);
    }

    void Apply(Enemy enemy)
    {
        enemy.TakeDamage(damage);
        if (slowMultiplier < 0.999f && slowDuration > 0f)
            enemy.ApplySlow(slowMultiplier, slowDuration);
    }
}
