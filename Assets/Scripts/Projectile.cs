using UnityEngine;

public class Projectile : MonoBehaviour
{
    Enemy target;
    float damage;
    float speed;

    public void Init(Enemy newTarget, float newDamage, float newSpeed)
    {
        target = newTarget;
        damage = newDamage;
        speed = newSpeed;
        Destroy(gameObject, 4f);
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 aim = target.transform.position + Vector3.up * 0.7f;
        Vector3 direction = aim - transform.position;
        float move = speed * Time.deltaTime;

        if (direction.magnitude <= move + 0.12f)
        {
            target.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        transform.position += direction.normalized * move;
        transform.rotation = Quaternion.LookRotation(direction.normalized);
    }
}
