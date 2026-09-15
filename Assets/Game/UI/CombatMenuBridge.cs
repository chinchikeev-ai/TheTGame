using UnityEngine;

public static class CombatMenuBridge
{
    public static void OpenCombatSettings(this GameMenuController menu)
    {
        if (menu == null || GameManager.Instance == null || GameManager.Instance.GameEnded) return;
        menu.SendMessage("Pause", SendMessageOptions.DontRequireReceiver);
        menu.SendMessage("ShowSettingsFromPause", SendMessageOptions.DontRequireReceiver);
    }
}
