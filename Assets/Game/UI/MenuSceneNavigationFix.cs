using UnityEngine;

// Compatibility shell kept so older scene/script references do not break.
// Restart and Main Menu navigation are owned exclusively by GameMenuController.
//
// The previous implementation rebound buttons after GameMenuController had built
// the runtime UI, stored a second static post-reload state, then searched button
// labels and invoked them after SceneManager.LoadScene. That created two competing
// navigation state machines and could leave the freshly loaded scene with every
// menu screen hidden (blank screen).
public sealed class MenuSceneNavigationFix : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void RemoveLegacyInstance()
    {
        MenuSceneNavigationFix legacy = FindFirstObjectByType<MenuSceneNavigationFix>();
        if (legacy != null)
            Object.Destroy(legacy.gameObject);
    }
}
