using UnityEngine;
using UnityEngine.UI;

// Decorative skin for ModernCombatHud. It may replace sprites/colors and add unique icons,
// but ModernCombatHud remains the sole owner of panel layout and content geometry.
public sealed class TroyCombatHudSkin : MonoBehaviour
{
    static readonly Color Stone = new Color(.12f,.105f,.09f,.96f);
    static readonly Color StoneSoft = new Color(.15f,.125f,.095f,.94f);
    static readonly Color Bronze = new Color(.55f,.34f,.12f,1f);
    static readonly Color BronzeHot = new Color(.76f,.48f,.14f,1f);
    static readonly Color TrojanRed = new Color(.46f,.055f,.035f,1f);
    static readonly Color TrojanRedHot = new Color(.62f,.085f,.045f,1f);
    static readonly Color Disabled = new Color(.16f,.13f,.11f,.68f);

    bool applied;
    TowerPlacement placement;
    Image selectedTowerIcon;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoCreate()
    {
        if (FindFirstObjectByType<TroyCombatHudSkin>() == null)
            new GameObject("TroyCombatHudSkin").AddComponent<TroyCombatHudSkin>();
    }

    void Update()
    {
        if (!applied)
        {
            GameObject hud = GameObject.Find("ModernCombatHUD");
            if (hud == null) return;
            placement = FindFirstObjectByType<TowerPlacement>();
            Apply(hud.transform);
            applied = true;
        }

        if (placement == null) placement = FindFirstObjectByType<TowerPlacement>();
        if (selectedTowerIcon != null && placement != null)
        {
            Tower tower = placement.SelectedTower;
            TowerType type = tower != null ? tower.Type : placement.SelectedBuildType;
            selectedTowerIcon.sprite = TroyHudArt.Tower(type);
        }
    }

    void Apply(Transform hud)
    {
        SkinPanel(hud.Find("TopResources"),StoneSoft);
        SkinPanel(hud.Find("WaveStatus"),Stone);
        SkinPanel(hud.Find("CombatActions"),Stone);
        SkinPanel(hud.Find("BuildDock"),StoneSoft);
        SkinPanel(hud.Find("SelectedTowerCard"),Stone);
        SkinPanel(hud.Find("PcHints"),new Color(.10f,.09f,.08f,.84f));
        SkinPanel(hud.Find("BuildHoverTooltip"),Stone);

        Transform top = hud.Find("TopResources");
        if (top != null)
        {
            Transform coin = top.Find("CoinIcon");
            if (coin != null)
            {
                Image coinImage = coin.GetComponent<Image>();
                if (coinImage != null) coinImage.sprite = TroyHudArt.Icon("gold");
            }
        }

        Transform wave = hud.Find("WaveStatus");
        if (wave != null) EnsureIcon(wave,"WaveCrest",new Vector2(-390,0),48,TroyHudArt.Icon("sword"));

        Transform actions = hud.Find("CombatActions");
        if (actions != null)
        {
            EnsureIcon(actions,"MagicIcon",new Vector2(54,28),38,TroyHudArt.Ability("magic"));
            EnsureIcon(actions,"GiftIcon",new Vector2(54,-42),36,TroyHudArt.Ability("gift"));
        }

        Transform dock = hud.Find("BuildDock");
        if (dock != null)
        {
            Button[] buttons = dock.GetComponentsInChildren<Button>(true);
            int towerIndex = 0;
            TowerType[] types = { TowerType.SpearThrower,TowerType.MachineGun,TowerType.Cannon,TowerType.Slow,TowerType.FireTower,TowerType.TrojanGuard };
            for (int i=0; i<buttons.Length && towerIndex<types.Length; i++)
            {
                if (buttons[i].transform.parent != dock) continue;
                Image img = buttons[i].GetComponent<Image>();
                if (img != null)
                {
                    img.sprite=TroyHudArt.Panel();
                    img.type=Image.Type.Sliced;
                    img.color=StoneSoft;
                }
                Transform authoredIcon = buttons[i].transform.Find("TowerIcon");
                if (authoredIcon != null)
                {
                    Image authoredImage = authoredIcon.GetComponent<Image>();
                    if (authoredImage != null) authoredImage.sprite = TroyHudArt.Tower(types[towerIndex]);
                }
                else
                {
                    EnsureIcon(buttons[i].transform,"TowerPortrait",new Vector2(0,15),40,TroyHudArt.Tower(types[towerIndex]));
                    Text txt = buttons[i].GetComponentInChildren<Text>();
                    if (txt != null) { txt.rectTransform.anchoredPosition=new Vector2(0,-22); txt.fontSize=11; }
                }
                towerIndex++;
            }
        }

        Transform selected = hud.Find("SelectedTowerCard");
        if (selected != null)
        {
            selectedTowerIcon = EnsureIcon(selected,"SelectedTowerCrest",new Vector2(-178,166),54,TroyHudArt.Tower(TowerType.TrojanGuard));
            Text[] texts = selected.GetComponentsInChildren<Text>(true);
            for (int i=0; i<texts.Length; i++)
                if (texts[i].fontSize > 20) texts[i].fontSize = 20;
        }

        StyleAllButtons(hud);
        StyleTextHierarchy(hud);
    }

    void SkinPanel(Transform panel, Color tint)
    {
        if (panel == null) return;
        Image img = panel.GetComponent<Image>();
        if (img != null)
        {
            img.sprite = TroyHudArt.Panel();
            img.type = Image.Type.Sliced;
            img.color = tint;
        }
    }

    void StyleAllButtons(Transform root)
    {
        Button[] buttons = root.GetComponentsInChildren<Button>(true);
        for (int i=0; i<buttons.Length; i++)
        {
            Image img = buttons[i].GetComponent<Image>();
            if (img == null) continue;
            img.sprite = TroyHudArt.Panel();
            img.type = Image.Type.Sliced;
            img.color = StoneSoft;
            ColorBlock cb = buttons[i].colors;
            cb.normalColor = StoneSoft;
            cb.highlightedColor = BronzeHot;
            cb.pressedColor = TrojanRedHot;
            cb.selectedColor = Bronze;
            cb.disabledColor = Disabled;
            cb.colorMultiplier = 1f;
            buttons[i].colors = cb;
        }
    }

    void StyleTextHierarchy(Transform root)
    {
        Text[] texts = root.GetComponentsInChildren<Text>(true);
        for (int i=0; i<texts.Length; i++)
        {
            Text text = texts[i];
            if (text == null) continue;
            text.color = text.fontSize >= 20
                ? new Color(.96f,.84f,.62f,1f)
                : new Color(.89f,.83f,.72f,1f);
        }
    }

    Image EnsureIcon(Transform parent,string name,Vector2 pos,float size,Sprite sprite)
    {
        Transform existing = parent.Find(name);
        if (existing != null)
        {
            Image existingImage = existing.GetComponent<Image>();
            if (existingImage != null) existingImage.sprite = sprite;
            return existingImage;
        }

        GameObject go = new GameObject(name);
        go.transform.SetParent(parent,false);
        Image image = go.AddComponent<Image>();
        image.sprite = sprite;
        image.raycastTarget = false;
        RectTransform rt = image.rectTransform;
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(.5f,.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(size,size);
        return image;
    }
}
