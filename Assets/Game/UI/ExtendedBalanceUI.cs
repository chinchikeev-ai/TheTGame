using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ExtendedBalanceUI : MonoBehaviour
{
    EnemySpawner spawner;
    TowerPlacement placement;
    Text hoverText;
    GameObject hoverPanel;

    string L(string en, string ru) => GameLanguage.T(en, ru);

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoCreate()
    {
        if (FindFirstObjectByType<ExtendedBalanceUI>() == null)
            new GameObject("ExtendedBalanceUI").AddComponent<ExtendedBalanceUI>();
    }

    void Start()
    {
        spawner = FindFirstObjectByType<EnemySpawner>();
        placement = FindFirstObjectByType<TowerPlacement>();
        BuildUI();
    }

    void Update()
    {
        if (spawner == null) spawner = FindFirstObjectByType<EnemySpawner>();
        if (placement == null) placement = FindFirstObjectByType<TowerPlacement>();

        UpdateEnemyHover();
    }

    void UpdateEnemyHover()
    {
        if (Camera.main == null || EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            hoverPanel.SetActive(false);
            return;
        }

        Vector2 pointer = GameInput.PointerPosition;
        Ray ray = Camera.main.ScreenPointToRay(pointer);
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
        string resistance = ResistanceText(d);
        string offense = OffenseText(d);
        hoverText.text = $"{LocalizedEnemyName(d)}\n" +
                         $"HP  {enemy.Health:0}/{enemy.maxHealth:0}\n" +
                         $"{L("SPEED", "СКОРОСТЬ")}  {enemy.speed:0.00}\n" +
                         $"{L("ARMOR", "БРОНЯ")}  {d.armor * 100f:0}%\n" +
                         $"{L("RESIST", "СОПРОТИВЛЕНИЕ")}  {resistance}\n" +
                         $"{L("DAMAGE", "УРОН")}  {offense}\n" +
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
            return $"{L("Gate", "Ворота")} {d.baseDamage}  •  {L("SIEGE", "ОСАДНЫЙ")}";
        if (d.archetype == EnemyArchetype.Archer)
            return $"{L("Gate", "Ворота")} {d.baseDamage}  •  {L("RANGED", "ДАЛЬНИЙ БОЙ")}";
        if (d.archetype == EnemyArchetype.Boss)
            return $"{L("Gate", "Ворота")} {d.baseDamage}  •  {L("BOSS", "БОСС")}";
        return $"{L("Gate", "Ворота")} {d.baseDamage}";
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
            case EnemyArchetype.Boss: return "Menelaus";
            default: return d.displayName;
        }
    }

    void BuildUI()
    {
        GameObject canvasObj = new GameObject("BalanceExtraCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 20;
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920,1080);
        canvasObj.AddComponent<GraphicRaycaster>();

        hoverPanel = Panel(canvas.transform, "EnemyHover", new Vector2(-24,0), new Vector2(330,205));
        RectTransform hrt = hoverPanel.GetComponent<RectTransform>();
        Anchor(hrt, new Vector2(1,.5f), new Vector2(1,.5f), new Vector2(1,.5f));
        hoverText = Text(hoverPanel.transform, "EnemyInfo", new Vector2(16,-14), new Vector2(298,180), 18, TextAnchor.UpperLeft);
        hoverPanel.SetActive(false);
    }

    GameObject Panel(Transform parent, string name, Vector2 pos, Vector2 size)
    {
        GameObject go = new GameObject(name); go.transform.SetParent(parent,false);
        Image img = go.AddComponent<Image>(); img.color = new Color(.05f,.07f,.10f,.9f);
        RectTransform rt = img.rectTransform; rt.anchoredPosition=pos; rt.sizeDelta=size; return go;
    }

    Text Text(Transform parent,string name,Vector2 pos,Vector2 size,int fontSize,TextAnchor align)
    {
        GameObject go=new GameObject(name); go.transform.SetParent(parent,false);
        Text t=go.AddComponent<Text>(); t.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); t.fontSize=fontSize; t.fontStyle=FontStyle.Bold; t.color=Color.white; t.alignment=align;
        RectTransform rt=t.rectTransform; rt.anchorMin=rt.anchorMax=rt.pivot=new Vector2(0,1); rt.anchoredPosition=pos; rt.sizeDelta=size; return t;
    }

    void Anchor(RectTransform rt,Vector2 min,Vector2 max,Vector2 pivot){rt.anchorMin=min;rt.anchorMax=max;rt.pivot=pivot;}
}
