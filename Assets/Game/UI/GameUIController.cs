using UnityEngine;

// Legacy compatibility component. The only runtime combat HUD is ModernCombatHud.
// Kept so older scenes do not lose a serialized script reference.
public class GameUIController : MonoBehaviour
{
    void Awake()
    {
        enabled = false;
    }
}
