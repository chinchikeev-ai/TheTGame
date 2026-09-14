using UnityEngine;

public sealed class ResultScreenPresentation : MonoBehaviour
{
    // Compatibility shell only.
    // Result layout is owned by GameMenuController so the menu stack has one visual owner.
    void Start()
    {
        GameObject menu = GameObject.Find("MenuCanvas");
        if (menu == null) return;

        Transform endMenu = menu.transform.Find("EndMenu");
        Transform card = endMenu != null ? endMenu.Find("ResultCard") : null;
        Transform legacy = card != null ? card.Find("ResultCards") : null;
        if (legacy != null) Destroy(legacy.gameObject);
    }
}
