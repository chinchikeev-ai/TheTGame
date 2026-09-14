using NUnit.Framework;
using UnityEngine;

public class RuntimeVisualBindingTests
{
    [TestCase("Trojan_Archer", "Role_Bow")]
    [TestCase("Trojan_Infantry", "Role_Spear")]
    [TestCase("Trojan_Guard", "Role_GuardOversizedShield")]
    public void TrojanCrewFallback_IsRoleSpecificAndColliderFree(string role, string requiredMarker)
    {
        GameObject root = TrojanTowerCrewFallbackFactory.Create(role);
        try
        {
            Assert.NotNull(root);
            Assert.NotNull(FindByName(root.transform, requiredMarker), role + " should expose a readable role marker.");
            Assert.AreEqual(0, root.GetComponentsInChildren<Collider>(true).Length,
                role + " fallback is presentation-only and must not add gameplay colliders.");
        }
        finally
        {
            Object.DestroyImmediate(root);
        }
    }

    static Transform FindByName(Transform root, string objectName)
    {
        foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
            if (child != null && child.name == objectName) return child;
        return null;
    }
}
