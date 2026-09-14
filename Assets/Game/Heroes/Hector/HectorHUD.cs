using UnityEngine;
using UnityEngine.UI;

public class HectorHUD : MonoBehaviour
{
    static readonly string[] BlockingMenuNames =
    {
        "MainMenu", "LevelSelect", "Settings", "PauseMenu", "EndMenu"
    };

    GameObject root;
    Text nameText;
    Text hpText;
    Image hpFill;
    Text[] abilityTexts = new Text[4];
    Image[] cooldownFills = new Image[4];
    Canvas menuCanvas;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoCreate()
    {
        if (FindFirstObjectByType<HectorHUD>() == null)
            new GameObject("HectorHUD").AddComponent<HectorHUD>();
    }

    void Start()
    {
        GameObject canvasObj = new GameObject("HectorHUDCanvas");
        canvasObj.transform.SetParent(transform, false);
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 83;
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = .5f;

        root = new GameObject("HectorPanel");
        root.transform.SetParent(canvasObj.transform, false);
        Image panel = root.AddComponent<Image>();
        panel.sprite = TroyHudArt.Panel();
        panel.type = Image.Type.Sliced;
        panel.color = Color.white;
        RectTransform pr = panel.rectTransform;
        pr.anchorMin = pr.anchorMax = pr.pivot = new Vector2(1f,1f);
        pr.anchoredPosition = new Vector2(-24f,-164f);
        pr.sizeDelta = new Vector2(330f,204f);

        AddImage(root.transform,"HectorPortrait",new Vector2(-124,50),new Vector2(74,74),TroyHudArt.Portrait("hector"));
        nameText = AddText(root.transform,"HECTOR",new Vector2(-34,65),new Vector2(174,28),17,new Color(1f,.72f,.28f,1f),TextAnchor.MiddleLeft,FontStyle.Bold);
        hpText = AddText(root.transform,"",new Vector2(-34,37),new Vector2(174,22),13,new Color(.94f,.86f,.72f,1f),TextAnchor.MiddleLeft,FontStyle.Bold);

        GameObject track = new GameObject("HealthTrack");
        track.transform.SetParent(root.transform,false);
        Image trackImage = track.AddComponent<Image>();
        trackImage.color = new Color(.15f,.06f,.035f,.95f);
        RectTransform tr = trackImage.rectTransform;
        tr.anchorMin = tr.anchorMax = tr.pivot = new Vector2(.5f,.5f);
        tr.anchoredPosition = new Vector2(30,11);
        tr.sizeDelta = new Vector2(232,13);
        GameObject fillObj = new GameObject("HealthFill");
        fillObj.transform.SetParent(track.transform,false);
        hpFill = fillObj.AddComponent<Image>();
        hpFill.color = new Color(.72f,.16f,.06f,1f);
        hpFill.type = Image.Type.Filled;
        hpFill.fillMethod = Image.FillMethod.Horizontal;
        RectTransform fr = hpFill.rectTransform;
        fr.anchorMin = Vector2.zero; fr.anchorMax = Vector2.one; fr.offsetMin = new Vector2(2,2); fr.offsetMax = new Vector2(-2,-2);

        string[] keys = { "Q", "E", "R", "F" };
        string[] names = { GameLanguage.T("WAR CRY","КЛИЧ"), GameLanguage.T("SHIELD","ЩИТЫ"), GameLanguage.T("SPEAR","КОПЬЁ"), GameLanguage.T("FOR TROY!","ЗА ТРОЮ!") };
        string[] art = { "warcry", "shieldwall", "spear", "ultimate" };
        for (int i=0;i<4;i++)
        {
            float x = -111 + i*74;
            GameObject slot = new GameObject("Ability_"+keys[i]);
            slot.transform.SetParent(root.transform,false);
            Image bg = slot.AddComponent<Image>();
            bg.sprite = TroyHudArt.Panel(); bg.type = Image.Type.Sliced; bg.color = new Color(.42f,.22f,.08f,1f);
            RectTransform sr = bg.rectTransform;
            sr.anchorMin = sr.anchorMax = sr.pivot = new Vector2(.5f,.5f);
            sr.anchoredPosition = new Vector2(x,-59);
            sr.sizeDelta = new Vector2(68,76);
            AddImage(slot.transform,"Icon",new Vector2(0,11),new Vector2(40,40),TroyHudArt.Ability(art[i]));
            abilityTexts[i] = AddText(slot.transform,keys[i]+"  "+names[i],new Vector2(0,-25),new Vector2(64,20),8,new Color(.96f,.86f,.70f,1f),TextAnchor.MiddleCenter,FontStyle.Bold);
            GameObject cd = new GameObject("Cooldown");
            cd.transform.SetParent(slot.transform,false);
            cooldownFills[i] = cd.AddComponent<Image>();
            cooldownFills[i].color = new Color(.02f,.015f,.01f,.72f);
            cooldownFills[i].type = Image.Type.Filled;
            cooldownFills[i].fillMethod = Image.FillMethod.Radial360;
            cooldownFills[i].fillOrigin = 2;
            cooldownFills[i].fillClockwise = false;
            RectTransform cr = cooldownFills[i].rectTransform;
            cr.anchorMin = Vector2.zero; cr.anchorMax = Vector2.one; cr.offsetMin = Vector2.zero; cr.offsetMax = Vector2.zero;
            cooldownFills[i].transform.SetAsFirstSibling();
        }
    }

    void Update()
    {
        HectorController h = HectorController.Instance;
        GameManager gm = GameManager.Instance;
        bool hidden = h == null || gm == null || gm.GameEnded || IsMenuBlockingCombat();
        if (hidden || root == null)
        {
            if (root != null) root.SetActive(false);
            return;
        }

        root.SetActive(true);
        if (h.IsDowned)
        {
            nameText.text = GameLanguage.T("HECTOR DOWNED","ГЕКТОР ПОВЕРЖЕН");
            hpText.text = GameLanguage.T("REVIVE ","ВОЗВРАЩЕНИЕ ") + Mathf.CeilToInt(h.DownedRemaining) + GameLanguage.T("s","с");
            hpFill.fillAmount = 0f;
        }
        else
        {
            nameText.text = GameLanguage.T("HECTOR • PRINCE OF TROY","ГЕКТОР • ПРИНЦ ТРОИ");
            hpText.text = $"HP {Mathf.CeilToInt(h.Health)} / {Mathf.CeilToInt(h.maxHealth)}";
            hpFill.fillAmount = h.maxHealth > 0f ? Mathf.Clamp01(h.Health / h.maxHealth) : 0f;
        }

        float[] remain = { h.WarCryCooldownRemaining, h.ShieldWallCooldownRemaining, h.SpearThrowCooldownRemaining, h.UltimateCooldownRemaining };
        float[] total = { h.warCryCooldown, h.shieldWallCooldown, h.spearThrowCooldown, h.ultimateCooldown };
        for(int i=0;i<4;i++)
        {
            float ratio = total[i] > 0f ? Mathf.Clamp01(remain[i]/total[i]) : 0f;
            cooldownFills[i].fillAmount = ratio;
            abilityTexts[i].color = remain[i] <= .01f ? new Color(1f,.86f,.42f,1f) : new Color(.66f,.60f,.52f,1f);
        }
    }

    bool IsMenuBlockingCombat()
    {
        if (menuCanvas == null)
        {
            GameObject menu = GameObject.Find("MenuCanvas");
            menuCanvas = menu != null ? menu.GetComponent<Canvas>() : null;
        }
        if (menuCanvas == null) return false;

        for (int i = 0; i < BlockingMenuNames.Length; i++)
        {
            Transform screen = menuCanvas.transform.Find(BlockingMenuNames[i]);
            if (screen != null && screen.gameObject.activeInHierarchy) return true;
        }
        return false;
    }

    Image AddImage(Transform parent,string name,Vector2 pos,Vector2 size,Sprite sprite)
    {
        GameObject go = new GameObject(name); go.transform.SetParent(parent,false);
        Image image = go.AddComponent<Image>(); image.sprite = sprite; image.raycastTarget = false;
        RectTransform rt=image.rectTransform; rt.anchorMin=rt.anchorMax=rt.pivot=new Vector2(.5f,.5f); rt.anchoredPosition=pos; rt.sizeDelta=size; return image;
    }

    Text AddText(Transform parent,string name,Vector2 pos,Vector2 size,int fontSize,Color color,TextAnchor alignment,FontStyle style)
    {
        GameObject go=new GameObject(name); go.transform.SetParent(parent,false);
        Text t=go.AddComponent<Text>(); t.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); t.text=name; t.fontSize=fontSize; t.fontStyle=style; t.color=color; t.alignment=alignment; t.horizontalOverflow=HorizontalWrapMode.Wrap; t.verticalOverflow=VerticalWrapMode.Truncate; t.raycastTarget=false;
        RectTransform rt=t.rectTransform; rt.anchorMin=rt.anchorMax=rt.pivot=new Vector2(.5f,.5f); rt.anchoredPosition=pos; rt.sizeDelta=size; return t;
    }
}
