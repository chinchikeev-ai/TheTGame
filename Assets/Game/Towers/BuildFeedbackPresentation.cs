using UnityEngine;

public static class BuildFeedbackPresentation
{
    public static void Success(Vector3 position, bool upgrade)
    {
        Pulse(
            position,
            upgrade ? new Color(1f, .68f, .18f) : new Color(.24f, .88f, .34f),
            upgrade ? 1.45f : 1.15f,
            .32f);

        Color color = upgrade ? new Color(1f, .65f, .16f) : new Color(.30f, .92f, .38f);
        for (int i = 0; i < 5; i++)
        {
            Vector3 start = position + new Vector3(
                (i - 2) * .18f,
                .12f + (i % 2) * .08f,
                ((i * 3) % 5 - 2) * .12f);

            CombatVfxPool.Spawn(
                PrimitiveType.Sphere,
                start,
                Vector3.one * .08f,
                Vector3.one * .015f,
                color,
                .28f + i * .025f,
                Vector3.up * .8f);
        }
    }

    public static void Denied(Vector3 position)
    {
        Pulse(position, new Color(.95f, .12f, .08f), .8f, .22f);
    }

    static void Pulse(Vector3 position, Color color, float radius, float duration)
    {
        CombatVfxPool.Spawn(
            PrimitiveType.Cylinder,
            position + Vector3.up * .045f,
            new Vector3(.22f, .018f, .22f),
            new Vector3(radius, .018f, radius),
            color,
            duration,
            Vector3.zero);
    }
}
