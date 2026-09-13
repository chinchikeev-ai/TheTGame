using UnityEngine;
using UnityEngine.UI;

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
        RestylePanel(hud.Find("TopResources"), new Vector2(24,-24), new Vector2(500,96), new Vector2(0,1), new Vector2(0,1));
        RestylePanel(hud.Find("WaveStatus"), new Vector2(0,-24), new Vector2(760,146), new Vector2(.5f,1), new Vector2(.5f,1));
        RestylePanel(hud.Find("CombatActions"), new Vector2(-24,-24), new Vector2(430,190), new Vector2(1,1), new Vector2(1,1));
        RestylePanel(hud.Find("BuildDock"), new Vector2(0,24), new Vector2(1120,154), new Vector2(.5f,0), new Vector2(.5f,0));
        RestylePanel(hud.Find("SelectedTowerCard"), new Vector2(-24,24), new Vector2(450,430), new Vector2(1,0), new Vector2(1,0));
        RestylePanel(hud.Find("PcHints"), new Vector2(24,24), new Vector2(650,48), new Vector2(0,0), new Vector2(0,0));
        RestylePanel(hud.Find("BuildHoverTooltip"), new Vector2(0,344), new Vector2(520,174), new Vector2(.5f,0), new Vector2(.5f,0));

        Transform top = hud.Find("TopResources");
        if (top != null)
        {
            EnsureIcon(top,"ArtGold",new Vector2(36,0),42,TroyHudArt.Icon("gold"));
            EnsureIcon(top,"ArtGate",new Vector2(178,0),42,TroyHudArt.Icon("gate"));
            EnsureIcon(top,"ArtEnemy",new Vector2(340,0),42,TroyHudArt.Icon("enemy"));
            Text[] texts = top.GetComponentsInChildren<Text>(true);
            for (int i=0;i<texts.Length;i++)
            {
                texts[i].fontSize = Mathf.Min(texts[i].fontSize,18);
                RectTransform rt = texts[i].rectTransform;
                if (i == 0) { rt.anchoredPosition = new Vector2(72,0); rt.sizeDelta = new Vector2(86,60); }
                else if (i == 1) { rt.anchoredPosition = new Vector2(226,0); rt.sizeDelta = new Vector2(116,60); }
                else if (i == 2) { rt.anchoredPosition = new Vector2(405,0); rt.sizeDelta = new Vector2(150,60); }
            }
        }

        Transform wave = hud.Find("WaveStatus");
        if (wave != null) EnsureIcon(wave,"WaveCrest",new Vector2(-334,0),56,TroyHudArt.Icon("sword"));

        Transform actions = hud.Find("CombatActions");
        if (actions != null)
        {
            EnsureIcon(actions,"MagicIcon",new Vector2(54,28),42,TroyHudArt.Ability("magic"));
            EnsureIcon(actions,"GiftIcon",new Vector2(54,-42),40,TroyHudArt.Ability("gift"));
        }

        Transform dock = hud.Find("BuildDock");
        if (dock != null)
        {
            Button[] buttons = dock.GetComponentsInChildren<Button>(true);
            int towerIndex = 0;
            TowerType[] types = { TowerType.SpearThrower,TowerType.MachineGun,TowerType.Cannon,TowerType.Slow,TowerType.FireTower,TowerType.TrojanGuard };
            for(int i=0;i<buttons.Length && towerIndex<types.Length;i++)
            {
                if (buttons[i].transform.parent != dock) continue;
                Image img = buttons[i].GetComponent<Image>();
                if(img!=null){img.sprite=TroyHudArt.Panel();img.type=Image.Type.Sliced;}
                EnsureIcon(buttons[i].transform,"TowerPortrait",new Vector2(0,15),42,TroyHudArt.Tower(types[towerIndex]));
                Text txt = buttons[i].GetComponentInChildren<Text>();
                if(txt!=null){txt.rectTransform.anchoredPosition=new Vector2(0,-22);txt.fontSize=11;}
                towerIndex++;
            }
        }

        Transform selected = hud.Find("SelectedTowerCard");
        if(selected!=null)
        {
            selectedTowerIcon = EnsureIcon(selected,"SelectedTowerCrest",new Vector2(-178,166),60,TroyHudArt.Tower(TowerType.TrojanGuard));
            Text[] texts=selected.GetComponentsInChildren<Text>(true);
            for(int i=0;i<texts.Length;i++) if(texts[i].fontSize>20) texts[i].fontSize=20;
        }

        StyleAllButtons(hud);
    }

    void RestylePanel(Transform panel, Vector2 pos, Vector2 size, Vector2 anchor, Vector2 pivot)
    {
        if(panel==null) return;
        Image img=panel.GetComponent<Image>();
        if(img!=null){img.sprite=TroyHudArt.Panel();img.type=Image.Type.Sliced;img.color=Color.white;}
        RectTransform rt=panel.GetComponent<RectTransform>();
        if(rt!=null){rt.anchorMin=rt.anchorMax=anchor;rt.pivot=pivot;rt.anchoredPosition=pos;rt.sizeDelta=size;}
    }

    void StyleAllButtons(Transform root)
    {
        Button[] buttons=root.GetComponentsInChildren<Button>(true);
        for(int i=0;i<buttons.Length;i++)
        {
            Image img=buttons[i].GetComponent<Image>();
            if(img==null) continue;
            img.sprite=TroyHudArt.Panel();
            img.type=Image.Type.Sliced;
            ColorBlock cb=buttons[i].colors;
            cb.normalColor=new Color(.42f,.22f,.08f,1f);
            cb.highlightedColor=new Color(.70f,.39f,.10f,1f);
            cb.pressedColor=new Color(.52f,.12f,.045f,1f);
            cb.disabledColor=new Color(.18f,.13f,.10f,.72f);
            buttons[i].colors=cb;
        }
    }

    Image EnsureIcon(Transform parent,string name,Vector2 pos,float size,Sprite sprite)
    {
        Transform existing = parent.Find(name);
        if(existing != null)
        {
            Image existingImage = existing.GetComponent<Image>();
            if (existingImage != null) existingImage.sprite = sprite;
            return existingImage;
        }
        GameObject go=new GameObject(name);
        go.transform.SetParent(parent,false);
        Image image=go.AddComponent<Image>();
        image.sprite=sprite; image.raycastTarget=false;
        RectTransform rt=image.rectTransform;
        rt.anchorMin=rt.anchorMax=rt.pivot=new Vector2(.5f,.5f);
        rt.anchoredPosition=pos; rt.sizeDelta=new Vector2(size,size);
        return image;
    }
}
