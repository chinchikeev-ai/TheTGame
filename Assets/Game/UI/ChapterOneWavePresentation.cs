using UnityEngine;
using UnityEngine.UI;

public sealed class ChapterOneWavePresentation : MonoBehaviour
{
    CanvasGroup group;
    Image portrait;
    Text kicker;
    Text title;
    Text subtitle;
    EnemySpawner spawner;
    int shownWave;
    bool bossWarningShown;
    bool bossArrivalShown;
    float hideAt;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoCreate()
    {
        if (FindFirstObjectByType<ChapterOneWavePresentation>() == null)
            new GameObject("ChapterOneWavePresentation").AddComponent<ChapterOneWavePresentation>();
    }

    void Start()
    {
        spawner = FindFirstObjectByType<EnemySpawner>();
        Build();
    }

    void Build()
    {
        GameObject canvasObj = new GameObject("ChapterOneWavePresentationCanvas");
        canvasObj.transform.SetParent(transform, false);
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 87;
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920,1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = .5f;

        GameObject card = new GameObject("WaveIntroCard");
        card.transform.SetParent(canvasObj.transform,false);
        Image bg = card.AddComponent<Image>();
        bg.sprite = TroyHudArt.Panel();
        bg.type = Image.Type.Sliced;
        bg.color = Color.white;
        RectTransform cr = bg.rectTransform;
        cr.anchorMin = cr.anchorMax = cr.pivot = new Vector2(.5f,.5f);
        cr.anchoredPosition = new Vector2(0,160);
        cr.sizeDelta = new Vector2(700,146);

        group = card.AddComponent<CanvasGroup>();
        group.alpha = 0f;
        group.interactable = false;
        group.blocksRaycasts = false;

        portrait = AddImage(card.transform,"Portrait",new Vector2(-292,0),new Vector2(104,104),TroyHudArt.Enemy("infantry"));
        kicker = AddText(card.transform,"",new Vector2(-215,38),new Vector2(450,24),12,new Color(1f,.61f,.18f,1f),TextAnchor.MiddleLeft,FontStyle.Bold);
        title = AddText(card.transform,"",new Vector2(-215,6),new Vector2(450,38),25,new Color(1f,.88f,.68f,1f),TextAnchor.MiddleLeft,FontStyle.Bold);
        subtitle = AddText(card.transform,"",new Vector2(-215,-34),new Vector2(450,42),13,new Color(.80f,.71f,.61f,1f),TextAnchor.MiddleLeft,FontStyle.Normal);
    }

    void Update()
    {
        GameManager gm = GameManager.Instance;
        if (gm == null || gm.MapNumber != 1 || gm.GameEnded)
        {
            if (group != null) group.alpha = 0f;
            return;
        }
        if (spawner == null) spawner = FindFirstObjectByType<EnemySpawner>();
        if (spawner == null || group == null) return;

        if (!bossWarningShown && spawner.NextWaveHasBoss && !spawner.WaveActive)
        {
            bossWarningShown = true;
            ShowBossWarning();
        }

        if (spawner.WaveActive && spawner.CurrentWave > shownWave)
        {
            shownWave = spawner.CurrentWave;
            if (spawner.CurrentWave == gm.MaxWaves) ShowFinalWave();
            else ShowWave(spawner.CurrentWave, gm.MaxWaves);
        }

        if (!bossArrivalShown && HasActiveBoss())
        {
            bossArrivalShown = true;
            ShowBossArrival();
        }

        float target = Time.unscaledTime < hideAt ? 1f : 0f;
        group.alpha = Mathf.MoveTowards(group.alpha, target, Time.unscaledDeltaTime * 3.8f);
    }

    void ShowWave(int wave, int max)
    {
        portrait.sprite = WavePortrait();
        kicker.text = GameLanguage.T($"WAVE {wave} OF {max}",$"ВОЛНА {wave} ИЗ {max}");
        title.text = WaveTitle(wave);
        subtitle.text = WaveSubtitle();
        hideAt = Time.unscaledTime + 2.35f;
        RuntimeFileLogger.Event("PRESENTATION", $"Wave intro shown wave={wave}");
    }

    void ShowBossWarning()
    {
        portrait.sprite = TroyHudArt.Portrait("menelaus");
        kicker.text = GameLanguage.T("FINAL ASSAULT INCOMING","ПРИБЛИЖАЕТСЯ ФИНАЛЬНЫЙ ШТУРМ");
        title.text = GameLanguage.T("MENELAUS APPROACHES","МЕНЕЛАЙ ПРИБЛИЖАЕТСЯ");
        subtitle.text = GameLanguage.T("Reinforce the gate • prepare anti-heavy defenses","Укрепите ворота • подготовьте оборону против тяжёлых целей");
        hideAt = Time.unscaledTime + 3.8f;
    }

    void ShowFinalWave()
    {
        portrait.sprite = TroyHudArt.Portrait("menelaus");
        kicker.text = GameLanguage.T("FINAL WAVE","ФИНАЛЬНАЯ ВОЛНА");
        title.text = GameLanguage.T("THE KING OF SPARTA LANDS","ЦАРЬ СПАРТЫ ВЫСАДИЛСЯ");
        subtitle.text = GameLanguage.T("Break his escort before he reaches the Trojan gate","Разбейте сопровождение до того, как он достигнет ворот Трои");
        hideAt = Time.unscaledTime + 3.2f;
    }

    void ShowBossArrival()
    {
        portrait.sprite = TroyHudArt.Portrait("menelaus");
        kicker.text = GameLanguage.T("BOSS ARRIVAL","ПОЯВЛЕНИЕ БОССА");
        title.text = "MENELAUS";
        subtitle.text = GameLanguage.T("Commander Aura active • reinforcements will follow","Аура командира активна • последуют подкрепления");
        hideAt = Time.unscaledTime + 3.4f;
        RuntimeEffects.Instance?.PlayHeroPulse(new Vector3(-13f,.2f,0f), new Color(.82f,.12f,.04f), 7f, .65f);
    }

    Sprite WavePortrait()
    {
        if (spawner.NextWaveHeavyCount > 0 || spawner.NextWaveShieldCount > 0) return TroyHudArt.Enemy("heavy");
        if (spawner.NextWaveArcherCount > 0) return TroyHudArt.Enemy("archer");
        if (spawner.NextWaveRunnerCount > 0) return TroyHudArt.Enemy("runner");
        return TroyHudArt.Enemy("infantry");
    }

    string WaveTitle(int wave)
    {
        if (wave == 1) return GameLanguage.T("THE FIRST BOATS HIT THE SHORE","ПЕРВЫЕ ЛОДКИ У БЕРЕГА");
        if (wave == 2) return GameLanguage.T("THE BEACHHEAD EXPANDS","ПЛАЦДАРМ РАСШИРЯЕТСЯ");
        if (wave == 3) return GameLanguage.T("HEAVY TROOPS ADVANCE","ТЯЖЁЛЫЕ ВОЙСКА НАСТУПАЮТ");
        if (wave == 4) return GameLanguage.T("THE GREEKS PRESS FORWARD","ГРЕКИ УСИЛИВАЮТ НАТИСК");
        return GameLanguage.T("HOLD THE LINE","УДЕРЖИВАЙТЕ ЛИНИЮ");
    }

    string WaveSubtitle()
    {
        if (spawner.NextWaveHeavyCount > 0) return GameLanguage.T("Heavy armor detected • Spears and Ballista recommended","Обнаружена тяжёлая броня • рекомендуются копья и баллисты");
        if (spawner.NextWaveRunnerCount > 0) return GameLanguage.T("Fast units detected • slow and area control recommended","Обнаружены быстрые враги • рекомендуется замедление и контроль зоны");
        if (spawner.NextWaveArcherCount > 0) return GameLanguage.T("Ranged support detected • protect exposed lanes","Обнаружена дальняя поддержка • прикройте открытые линии");
        return GameLanguage.T("Greek infantry is advancing from the beach","Греческая пехота наступает с берега");
    }

    bool HasActiveBoss()
    {
        foreach (Enemy enemy in EnemyRegistry.All)
            if (enemy != null && enemy.Archetype == EnemyArchetype.Boss && enemy.Health > 0f) return true;
        return false;
    }

    Image AddImage(Transform parent,string name,Vector2 pos,Vector2 size,Sprite sprite)
    {
        GameObject go=new GameObject(name); go.transform.SetParent(parent,false); Image image=go.AddComponent<Image>(); image.sprite=sprite; image.raycastTarget=false;
        RectTransform rt=image.rectTransform; rt.anchorMin=rt.anchorMax=rt.pivot=new Vector2(.5f,.5f); rt.anchoredPosition=pos; rt.sizeDelta=size; return image;
    }

    Text AddText(Transform parent,string value,Vector2 pos,Vector2 size,int fontSize,Color color,TextAnchor alignment,FontStyle style)
    {
        GameObject go=new GameObject("Text"); go.transform.SetParent(parent,false); Text t=go.AddComponent<Text>(); t.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); t.text=value; t.fontSize=fontSize; t.fontStyle=style; t.color=color; t.alignment=alignment; t.raycastTarget=false; t.horizontalOverflow=HorizontalWrapMode.Wrap; t.verticalOverflow=VerticalWrapMode.Truncate;
        RectTransform rt=t.rectTransform; rt.anchorMin=rt.anchorMax=rt.pivot=new Vector2(.5f,.5f); rt.anchoredPosition=pos; rt.sizeDelta=size; return t;
    }
}
