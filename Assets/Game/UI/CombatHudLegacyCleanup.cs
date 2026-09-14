using UnityEngine;
using UnityEngine.UI;

public sealed class CombatHudLegacyCleanup : MonoBehaviour
{
    Transform root;

    void LateUpdate()
    {
        if (root == null)
        {
            GameObject go = GameObject.Find("ModernCombatHUD");
            if (go == null) return;
            root = go.transform;
        }

        Transform actions = root.Find("CombatActions");
        if (actions != null) actions.gameObject.SetActive(false);

        Transform gift = root.Find("DivineGiftChoiceOverlay");
        if (gift != null) gift.gameObject.SetActive(false);

        Transform status = root.Find("WaveStatus");
        if (status == null) return;

        Text[] texts = status.GetComponentsInChildren<Text>(true);
        for (int i = 0; i < texts.Length; i++)
        {
            Text t = texts[i];
            if (t.text.StartsWith("WAVE ")) t.text = "ENCOUNTER " + t.text.Substring(5);
            else if (t.text.StartsWith("ВОЛНА ")) t.text = "БОЙ " + t.text.Substring(6);
            else if (t.text.Contains("NEXT WAVE IN")) t.text = t.text.Replace("NEXT WAVE IN", "NEXT ENCOUNTER IN");
            else if (t.text.Contains("СЛЕДУЮЩАЯ ВОЛНА ЧЕРЕЗ")) t.text = t.text.Replace("СЛЕДУЮЩАЯ ВОЛНА ЧЕРЕЗ", "СЛЕДУЮЩИЙ БОЙ ЧЕРЕЗ");
            else if (t.text == "START WAVE") t.text = "START ENCOUNTER";
            else if (t.text == "НАЧАТЬ ВОЛНУ") t.text = "НАЧАТЬ БОЙ";
        }
    }
}
