using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class CombatNotificationPresentation : MonoBehaviour
{
    sealed class Entry { public string text; public float expires; public Sprite icon; }

    readonly List<Entry> entries = new List<Entry>();
    CanvasGroup group;
    GameObject panel;
    readonly Text[] lines = new Text[3];
    readonly Image[] icons = new Image[3];
    EnemySpawner spawner;
    Canvas menuCanvas;
    int lastWave = -1;
    int lastTowersBuilt = -1;
    int lastGate = -1;
    bool bossWasActive;
    bool bossDefeated;

    void Start(){spawner=EnemySpawner.Instance;menuCanvas=GameMenuController.Instance!=null?GameMenuController.Instance.MenuCanvas:null;Build();}

    void Build()
    {
        GameObject root=new GameObject("CombatNotifications");
        Canvas canvas=root.AddComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceOverlay;canvas.sortingOrder=81;
        CanvasScaler scaler=root.AddComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(1920,1080);scaler.screenMatchMode=CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;scaler.matchWidthOrHeight=.5f;
        group=root.AddComponent<CanvasGroup>();group.interactable=false;group.blocksRaycasts=false;
        panel=new GameObject("NotificationPanel");panel.transform.SetParent(root.transform,false);
        Image bg=panel.AddComponent<Image>();bg.sprite=TroyHudArt.Panel();bg.type=Image.Type.Sliced;bg.color=Color.white;bg.raycastTarget=false;
        RectTransform pr=bg.rectTransform;pr.anchorMin=pr.anchorMax=pr.pivot=new Vector2(0,0);pr.anchoredPosition=new Vector2(16,260);pr.sizeDelta=new Vector2(420,118);pr.localScale=Vector3.one*.8f;
        for(int i=0;i<3;i++)
        {
            float y=34-i*34;
            GameObject iconObj=new GameObject("Icon"+i);iconObj.transform.SetParent(panel.transform,false);icons[i]=iconObj.AddComponent<Image>();icons[i].raycastTarget=false;RectTransform ir=icons[i].rectTransform;ir.anchorMin=ir.anchorMax=ir.pivot=new Vector2(.5f,.5f);ir.anchoredPosition=new Vector2(-177,y);ir.sizeDelta=new Vector2(28,28);
            GameObject textObj=new GameObject("Line"+i);textObj.transform.SetParent(panel.transform,false);lines[i]=textObj.AddComponent<Text>();lines[i].font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");lines[i].fontSize=12;lines[i].fontStyle=FontStyle.Bold;lines[i].color=new Color(.94f,.86f,.75f,1f);lines[i].alignment=TextAnchor.MiddleLeft;lines[i].horizontalOverflow=HorizontalWrapMode.Wrap;lines[i].verticalOverflow=VerticalWrapMode.Truncate;lines[i].raycastTarget=false;RectTransform tr=lines[i].rectTransform;tr.anchorMin=tr.anchorMax=tr.pivot=new Vector2(.5f,.5f);tr.anchoredPosition=new Vector2(14,y);tr.sizeDelta=new Vector2(338,30);
        }
        panel.SetActive(false);
    }

    void Update()
    {
        GameManager gm=GameManager.Instance;if(gm==null){group.alpha=0f;return;}
        if(spawner==null)spawner=EnemySpawner.Instance;
        if(menuCanvas==null&&GameMenuController.Instance!=null)menuCanvas=GameMenuController.Instance.MenuCanvas;
        group.alpha=(gm.GameEnded||IsMenuBlocking())?0f:1f;if(group.alpha<=0f)return;
        if(lastWave<0){lastWave=gm.CurrentWave;lastTowersBuilt=gm.TowersBuilt;lastGate=gm.BaseHealth;bossDefeated=gm.BossDefeated;}
        if(gm.CurrentWave>lastWave)lastWave=gm.CurrentWave;
        if(gm.TowersBuilt>lastTowersBuilt){lastTowersBuilt=gm.TowersBuilt;Push(GameLanguage.T("Trojan defense deployed","Оборона Трои установлена"),TroyHudArt.Icon("shield"),4f);}
        if(lastGate>=0&&gm.BaseHealth<lastGate){int lost=lastGate-gm.BaseHealth;lastGate=gm.BaseHealth;Push(GameLanguage.T($"Gate damaged −{lost}",$"Ворота повреждены −{lost}"),TroyHudArt.Icon("gate"),5f);}
        bool bossActive=HasBoss();if(bossActive&&!bossWasActive)Push(GameLanguage.T("MENELAUS HAS ENTERED THE BATTLE","МЕНЕЛАЙ ВСТУПИЛ В БОЙ"),TroyHudArt.Icon("boss"),7f);bossWasActive=bossActive;
        if(gm.BossDefeated&&!bossDefeated){bossDefeated=true;Push(GameLanguage.T("Menelaus defeated • objective complete","Менелай повержен • цель выполнена"),TroyHudArt.Icon("hector"),7f);}
        for(int i=entries.Count-1;i>=0;i--)if(Time.unscaledTime>=entries[i].expires)entries.RemoveAt(i);
        Render();
    }

    void Push(string text,Sprite icon,float duration){entries.Insert(0,new Entry{text=text,icon=icon,expires=Time.unscaledTime+duration});while(entries.Count>3)entries.RemoveAt(entries.Count-1);}
    void Render()
    {
        panel.SetActive(entries.Count > 0);
        float height = 16 + entries.Count * 34;
        ((RectTransform)panel.transform).sizeDelta = new Vector2(420, height);
        for (int i = 0; i < 3; i++)
        {
            bool visible = i < entries.Count;
            lines[i].gameObject.SetActive(visible);
            icons[i].gameObject.SetActive(visible);
            if (!visible) continue;
            float y = height * .5f - 25 - i * 34;
            lines[i].rectTransform.anchoredPosition = new Vector2(14, y);
            icons[i].rectTransform.anchoredPosition = new Vector2(-177, y);
            lines[i].text = entries[i].text;
            icons[i].sprite = entries[i].icon;
            icons[i].color = Color.white;
        }
    }
    bool HasBoss(){foreach(Enemy e in EnemyRegistry.All)if(e!=null&&e.Archetype==EnemyArchetype.Boss&&e.Health>0f)return true;return false;}
    bool IsMenuBlocking(){if(menuCanvas==null)return false;string[] n={"MainMenu","LevelSelect","Settings","PauseMenu","EndMenu","ConfirmationModal"};for(int i=0;i<n.Length;i++){Transform t=menuCanvas.transform.Find(n[i]);if(t!=null&&t.gameObject.activeInHierarchy)return true;}return false;}
}
