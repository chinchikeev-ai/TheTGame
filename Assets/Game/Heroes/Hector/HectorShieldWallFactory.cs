using UnityEngine;

public static class HectorShieldWallFactory
{
    public static ShieldWallZone Create(Vector3 center, Quaternion rotation)
    {
        return Create(center, rotation, 4f);
    }

    public static ShieldWallZone Create(Vector3 center, Quaternion rotation, float duration)
    {
        GameObject root = new GameObject("Hector Shield Wall");
        root.transform.position = center;
        root.transform.rotation = rotation;

        ShieldWallZone zone = root.AddComponent<ShieldWallZone>();
        zone.duration = Mathf.Max(.2f, duration);
        Object.Destroy(root, zone.duration + .1f);
        return zone;
    }
}
