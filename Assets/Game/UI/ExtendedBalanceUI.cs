using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ExtendedBalanceUI : MonoBehaviour
{
    Text hoverText;
    GameObject hoverPanel;

    string L(string en, string ru) => GameLanguage.T(en, ru);

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoCreate()
    {
        if (FindFirstObjectByType<ExtendedBalanceUI>() == null)
            new GameObject("ExtendedBalanceUI").AddComponent<ExtendedBalanceUI>();
    }

    void Start() => BuildUI();

    void Update() => UpdateEnemyHover();

    void UpdateEnemyHover()
    {
        if (hoverPanel == null) return;
        if (Camera.main == null || (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()))
        {
            hoverPanel.SetActive(false);
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(GameInput.PointerPosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, 250f))
        {
            hoverPanel.SetActive(false);
            return;
        }

        Enemy enemy = hit.collider.GetComponentInParent<Enemy>();
        if (enemy == null)
        {
            hoverPanel.SetActive(false);
            return;
        }

        EnemyData d = BalanceCatalog.GetEnemy(enemy.Archetype);
        if (d == null)
        {
            hoverPanel.SetActive(false);
            return;
        }

        hoverText.text =
            $"<b>{LocalizedEnemyName(d)}</b>\n" +
            $"HP  {enemy.Health:0}/{enemy.maxHealth:0}\n" +
            $"{L("ARMOR", "БРОНЯ")}  {d.armor * 100f:0}%    {L("SPEED", "СКОРОСТЬ")}  {enemy.speed:0.0}\n" +
            $"{L("RESIST", "СОПРОТИВЛЕНИЕ")}  {ResistanceText(d)}\n" +
            $"{OffenseText(d)}\n" +
            $"{L("REWARD", "НАГРАДА")}  {d.reward} {L("gold", "золота")}";
        hoverPanel.SetActive(true);
    }

    string ResistanceText(EnemyData d)
    {
        if (d.arrowResistance > .01f)
            return $"{L("Arrows", "Стрелы")} {d.arrowResistance * 100f:0}%";
        if (d.armor > .01f)
            return $"{L("Physical", "Физический")} {d.armor * 100f:0}%";
        return L("None", "Нет");
    }

    string OffenseText(EnemyData d)
    {
        if (d.archetype == EnemyArchetype.BatteringRam)
            return $"{L("THREAT", "УГРОЗА")}: {L("Siege damage to the gate", "Осадный урон воротам")}";
        if (d.archetype == EnemyArchetype.Archer)
            return $"{L("THREAT", "УГРОЗА")}: {L("Ranged pressure", "Дальний бой")}";
        if (d.archetype == EnemyArchetype.Boss)
            return $"{L("THREAT", "УГРОЗА")}: {L("Commander aura + reinforcements", "Аура командира + подкрепления")}";
        return $"{L("GATE DAMAGE", "УРОН ВОРОТАМ")}: {d.baseDamage}";
    }

    string LocalizedEnemyName(EnemyData d)
    {
        switch (d.archetype)
        {
            case EnemyArchetype.Infantry: return L("Greek Infantry", "Греческая пехота");
            case EnemyArchetype.Runner: return L("Greek Runner", "Греческий бегун");
            case EnemyArchetype.HeavyHoplite: return L("Heavy Hoplite", "Тяжёлый гоплит");
            case EnemyArchetype.ShieldBearer: return L("Shield Bearer", "Щитоносец");
            case EnemyArchetype.Archer: return L("Greek Archer", "Греческий лучник");
            case EnemyArchetype.BatteringRam: return L("Battering Ram", "Таран");
            case EnemyArchetype.Boss: return L("MENELAUS", "МЕНЕЛАЙ");
            default: return d.displayName;
        }
    }

    void BuildUI()
    {
        GameObject canvasObj = new GameObject("EnemyInspectionCanvas");
        canvasObj.transform.SetParent(transform, false);
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 22;
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        hoverPanel = Panel(canvas.transform, "EnemyHover", new Vector2(-20, 0), new Vector2(350, 190));
        RectTransform hrt = hoverPanel.GetComponent<RectTransform>();
        Anchor(hrt, new Vector2(1, .5f), new Vector2(1, .5f), new Vector2(1, .5f));
        hoverText = Text(hoverPanel.transform, "EnemyInfo", new Vector2(16, -14), new Vector2(318, 165), 17, TextAnchor.UpperLeft);
        hoverPanel.SetActive(false);
    }

    GameObject Panel(Transform parent, string name, Vector2 pos, Vector2 size)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        Image img = go.AddComponent<Image>();
        img.color = new Color(.035f, .045f, .060f, .95f);
        RectTransform rt = img.rectTransform;
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        return go;
    }

    Text Text(Transform parent, string name, Vector2 pos, Vector2 size, int fontSize, TextAnchor align)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        Text t = go.AddComponent<Text>();
        t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        t.fontSize = fontSize;
        t.fontStyle = FontStyle.Bold;
        t.color = Color.white;
        t.alignment = align;
        t.supportRichText = true;
        RectTransform rt = t.rectTransform;
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0, 1);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        return t;
    }

    void Anchor(RectTransform rt, Vector2 min, Vector2 max, Vector2 pivot)
    {
        rt.anchorMin = min;
        rt.anchorMax = max;
        rt.pivot = pivot;
    }
}
