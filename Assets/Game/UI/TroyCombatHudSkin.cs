using UnityEngine;
using UnityEngine.UI;

// Decorative skin for ModernCombatHud. It may replace sprites/colors and add unique icons,
// but ModernCombatHud remains the sole owner of panel layout and content geometry.
public sealed class TroyCombatHudSkin : MonoBehaviour
{
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
        SkinPanel(hud.Find("TopResources"));
        SkinPanel(hud.Find("WaveStatus"));
        SkinPanel(hud.Find("CombatActions"));
        SkinPanel(hud.Find("BuildDock"));
        SkinPanel(hud.Find("SelectedTowerCard"));
        SkinPanel(hud.Find("PcHints"));
        SkinPanel(hud.Find("BuildHoverTooltip"));

        Transform top = hud.Find("TopResources");
        if (top != null)
        {
            Transform coin = top.Find("CoinIcon");
            if (coin != null)
            {
                Image coinImage = coin.GetComponent<Image>();
                if (coinImage != null) coinImage.sprite = TroyHudArt.Icon("gold");
            }
            Transform gate = top.Find("GateIcon");
            if (gate != null && gate.TryGetComponent(out Image gateImage)) gateImage.sprite = TroyHudArt.Icon("gate");
        }

        Transform wave = hud.Find("WaveStatus");
        if (wave != null) EnsureIcon(wave,"WaveCrest",new Vector2(-338,28),58,TroyHudArt.Icon("sword"));

        Transform actions = hud.Find("CombatActions");
        if (actions != null)
        {
            Transform magic = actions.Find("Magic_Primary/MagicIcon");
            if (magic != null && magic.TryGetComponent(out Image magicImage)) magicImage.sprite = TroyHudArt.Ability("magic");
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
                if (img != null) { img.sprite=TroyHudArt.Panel(); img.type=Image.Type.Sliced; }
                EnsureIcon(buttons[i].transform,"TowerPortrait",new Vector2(0,15),42,TroyHudArt.Tower(types[towerIndex]));
                Text txt = buttons[i].GetComponentInChildren<Text>();
                if (txt != null) { txt.rectTransform.anchoredPosition=new Vector2(0,-22); txt.fontSize=11; }
                towerIndex++;
            }
        }

        Transform selected = hud.Find("SelectedTowerCard");
        if (selected != null)
        {
            selectedTowerIcon = EnsureIcon(selected,"SelectedTowerCrest",new Vector2(-178,166),60,TroyHudArt.Tower(TowerType.TrojanGuard));
            Text[] texts = selected.GetComponentsInChildren<Text>(true);
            for (int i=0; i<texts.Length; i++)
                if (texts[i].fontSize > 20) texts[i].fontSize = 20;
        }

        StyleAllButtons(hud);
    }

    void SkinPanel(Transform panel)
    {
        if (panel == null) return;
        Image img = panel.GetComponent<Image>();
        if (img != null)
        {
            img.sprite = TroyHudArt.Panel();
            img.type = Image.Type.Sliced;
            img.color = Color.white;
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
            ColorBlock cb = buttons[i].colors;
            cb.normalColor = new Color(.42f,.22f,.08f,1f);
            cb.highlightedColor = new Color(.70f,.39f,.10f,1f);
            cb.pressedColor = new Color(.52f,.12f,.045f,1f);
            cb.disabledColor = new Color(.18f,.13f,.10f,.72f);
            buttons[i].colors = cb;
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
