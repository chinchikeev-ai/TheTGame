using UnityEngine;

public sealed class HectorAbilityPresentation : MonoBehaviour
{
    const float ShieldVisualLifetime = 6.1f;

    static readonly Color WarCryColor = new Color(1f, .52f, .08f);
    static readonly Color ShieldWallColor = new Color(.95f, .72f, .18f);
    static readonly Color SpearColor = new Color(1f, .78f, .20f);
    static readonly Color UltimateColor = new Color(1f, .18f, .04f);

    public void PlayWarCry(float radius)
    {
        RuntimeEffects.Instance?.PlayHeroAbilitySound();
        CombatImpactPresentation.Pulse(transform.position, WarCryColor, radius * 1.35f, .55f);
    }

    public void PlayShieldWall(Vector3 center, Quaternion rotation)
    {
        PlayShieldWall(center, rotation, ShieldVisualLifetime);
    }

    public void PlayShieldWall(Vector3 center, Quaternion rotation, float duration)
    {
        GameObject root = new GameObject("Hector Shield Wall Visual");
        root.transform.position = center;
        root.transform.rotation = rotation;

        for (int i = -1; i <= 1; i++)
            CreateShield(root.transform, i * 1.05f);

        RuntimeEffects.Instance?.PlayHeroAbilitySound();
        CombatImpactPresentation.Pulse(center, ShieldWallColor, 3.8f, .48f);
        Destroy(root, Mathf.Max(.2f, duration));
    }

    public void PlaySpearImpact(Vector3 point)
    {
        RuntimeEffects.Instance?.PlayHeroAbilitySound();
        CombatImpactPresentation.Pulse(point, SpearColor, 2.2f, .32f);
    }

    public void PlayUltimate(float radius)
    {
        RuntimeEffects.Instance?.PlayHeroAbilitySound();
        CombatImpactPresentation.Pulse(transform.position, UltimateColor, radius * 1.15f, .72f);
    }

    static void CreateShield(Transform parent, float localX)
    {
        GameObject shield = GameObject.CreatePrimitive(PrimitiveType.Cube);
        shield.name = "Shield";
        shield.transform.SetParent(parent, false);
        shield.transform.localPosition = new Vector3(localX, .55f, 0f);
        shield.transform.localScale = new Vector3(.9f, 1.1f, .28f);
        TowerFactory.SetColor(shield, new Color(.70f, .50f, .18f));

        Collider collider = shield.GetComponent<Collider>();
        if (collider != null) Object.Destroy(collider);
    }
}
