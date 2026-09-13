using UnityEngine;
using UnityEngine.UI;

public class BossHUD : MonoBehaviour
{
    GameObject root;
    Text label;
    Text hpText;
    Image fill;
    Image dangerGlow;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoCreate()
    {
        if (FindFirstObjectByType<BossHUD>() == null)
            new GameObject("BossHUD").AddComponent<BossHUD>();
    }

    void Start()
    {
        GameObject canvasObj = new GameObject("BossHUDCanvas");
        canvasObj.transform.SetParent(transform, false);
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 84;
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920,1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = .5f;

        root = new GameObject("BossBar");
        root.transform.SetParent(canvasObj.transform,false);
        Image bg = root.AddComponent<Image>();
        bg.sprite = TroyHudArt.Panel(true);
        bg.type = Image.Type.Sliced;
        bg.color = Color.white;
        RectTransform rr = bg.rectTransform;
        rr.anchorMin = rr.anchorMax = rr.pivot = new Vector2(.5f,1f);
        rr.anchoredPosition = new Vector2(0,-182);
        rr.sizeDelta = new Vector2(820,132);

        GameObject glowObj = new GameObject("DangerGlow");
        glowObj.transform.SetParent(root.transform,false);
        dangerGlow = glowObj.AddComponent<Image>();
        dangerGlow.color = new Color(.55f,.04f,.02f,.12f);
        RectTransform gr = dangerGlow.rectTransform;
        gr.anchorMin = Vector2.zero; gr.anchorMax = Vector2.one;
        gr.offsetMin = new Vector2(8,8); gr.offsetMax = new Vector2(-8,-8);
        gr.SetAsFirstSibling();

        AddImage(root.transform,"MenelausPortrait",new Vector2(-354,4),new Vector2(84,84),TroyHudArt.Icon("boss"));
        label = MakeText(root.transform,"Label",22,TextAnchor.MiddleLeft);
        label.color = new Color(1f,.72f,.30f,1f);
        SetRect(label.rectTransform,new Vector2(-298,35),new Vector2(430,34));
        hpText = MakeText(root.transform,"HpText",16,TextAnchor.MiddleRight);
        hpText.color = new Color(1f,.88f,.72f,1f);
        SetRect(hpText.rectTransform,new Vector2(275,35),new Vector2(210,30));

        GameObject trackObj = new GameObject("Track");
        trackObj.transform.SetParent(root.transform,false);
        Image track = trackObj.AddComponent<Image>();
        track.color = new Color(.15f,.055f,.035f,1f);
        RectTransform tr = track.rectTransform;
        tr.anchorMin = tr.anchorMax = tr.pivot = new Vector2(.5f,.5f);
        tr.anchoredPosition = new Vector2(36,0);
        tr.sizeDelta = new Vector2(650,24);
        GameObject fillObj = new GameObject("Fill");
        fillObj.transform.SetParent(trackObj.transform,false);
        fill = fillObj.AddComponent<Image>();
        fill.color = new Color(.78f,.12f,.045f,1f);
        fill.type = Image.Type.Filled;
        fill.fillMethod = Image.FillMethod.Horizontal;
        RectTransform fr = fill.rectTransform;
        fr.anchorMin = Vector2.zero; fr.anchorMax = Vector2.one; fr.offsetMin = new Vector2(3,3); fr.offsetMax = new Vector2(-3,-3);

        MakeBadge(root.transform,"AURA",new Vector2(-160,-43),new Color(.78f,.18f,.06f,1f));
        MakeBadge(root.transform,"REINFORCEMENTS",new Vector2(5,-43),new Color(.62f,.34f,.10f,1f));
        MakeBadge(root.transform,"GATE THREAT",new Vector2(190,-43),new Color(.72f,.10f,.035f,1f));
        root.SetActive(false);
    }

    void Update()
    {
        Enemy boss = null;
        foreach (Enemy enemy in EnemyRegistry.All)
        {
            if (enemy != null && enemy.Archetype == EnemyArchetype.Boss)
            {
                boss = enemy;
                break;
            }
        }

        bool visible = boss != null && boss.Health > 0f;
        root.SetActive(visible);
        if (!visible) return;

        float health01 = boss.Health01;
        fill.fillAmount = health01;
        fill.color = health01 < .30f ? new Color(.98f,.18f,.035f,1f) : new Color(.78f,.12f,.045f,1f);
        dangerGlow.color = health01 < .30f
            ? new Color(.78f,.04f,.01f,.22f + Mathf.PingPong(Time.unscaledTime*.18f,.16f))
            : new Color(.55f,.04f,.02f,.10f);

        label.text = GameLanguage.T("MENELAUS • COMMANDER OF THE ASSAULT","МЕНЕЛАЙ • КОМАНДУЮЩИЙ ШТУРМОМ");
        hpText.text = $"{Mathf.CeilToInt(boss.Health)} / {Mathf.CeilToInt(boss.maxHealth)}   •   {Mathf.RoundToInt(health01*100f)}%";
    }

    void MakeBadge(Transform parent,string textValue,Vector2 pos,Color accent)
    {
        GameObject badge=new GameObject("Badge_"+textValue); badge.transform.SetParent(parent,false);
        Image bg=badge.AddComponent<Image>(); bg.sprite=TroyHudArt.Panel(true); bg.type=Image.Type.Sliced; bg.color=new Color(.45f,.18f,.07f,1f);
        RectTransform rt=bg.rectTransform; rt.anchorMin=rt.anchorMax=rt.pivot=new Vector2(.5f,.5f); rt.anchoredPosition=pos; rt.sizeDelta=new Vector2(textValue.Length>8?150:100,26);
        Text t=MakeText(badge.transform,"Text",10,TextAnchor.MiddleCenter); t.text=textValue; t.color=new Color(1f,.82f,.54f,1f); RectTransform tr=t.rectTransform; tr.anchorMin=Vector2.zero; tr.anchorMax=Vector2.one; tr.offsetMin=Vector2.zero; tr.offsetMax=Vector2.zero;
        Outline o=badge.AddComponent<Outline>(); o.effectColor=accent; o.effectDistance=new Vector2(1,-1);
    }

    Image AddImage(Transform parent,string name,Vector2 pos,Vector2 size,Sprite sprite)
    {
        GameObject go=new GameObject(name); go.transform.SetParent(parent,false); Image image=go.AddComponent<Image>(); image.sprite=sprite; image.raycastTarget=false; RectTransform rt=image.rectTransform; rt.anchorMin=rt.anchorMax=rt.pivot=new Vector2(.5f,.5f); rt.anchoredPosition=pos; rt.sizeDelta=size; return image;
    }

    Text MakeText(Transform parent,string name,int fontSize,TextAnchor alignment)
    {
        GameObject textObj=new GameObject(name); textObj.transform.SetParent(parent,false); Text text=textObj.AddComponent<Text>(); text.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); text.fontSize=fontSize; text.fontStyle=FontStyle.Bold; text.color=Color.white; text.alignment=alignment; text.horizontalOverflow=HorizontalWrapMode.Wrap; text.verticalOverflow=VerticalWrapMode.Truncate; text.raycastTarget=false; return text;
    }

    void SetRect(RectTransform rt,Vector2 pos,Vector2 size){rt.anchorMin=rt.anchorMax=rt.pivot=new Vector2(.5f,.5f);rt.anchoredPosition=pos;rt.sizeDelta=size;}
}
