using UnityEngine;

// Compatibility shell. Tower selection actions are owned by ModernCombatHud.
// The old implementation created a second floating HUD and hid the canonical
// selected-tower card every frame, producing overlapping and moving UI blocks.
public sealed class TowerContextActionHud : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void RemoveLegacyInstance()
    {
        TowerContextActionHud legacy = FindFirstObjectByType<TowerContextActionHud>();
        if (legacy != null)
            Object.Destroy(legacy.gameObject);
    }
}
