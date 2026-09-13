using UnityEngine;

public static class HectorShieldWallFactory
{
    public static ShieldWallZone Create(Vector3 center, Quaternion rotation)
    {
        GameObject root = new GameObject("Hector Shield Wall");
        root.transform.position = center;
        root.transform.rotation = rotation;

        ShieldWallZone zone = root.AddComponent<ShieldWallZone>();
        Object.Destroy(root, zone.duration + .1f);
        return zone;
    }
}
