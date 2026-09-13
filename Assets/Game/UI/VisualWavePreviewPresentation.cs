using UnityEngine;
using UnityEngine.UI;

public sealed class VisualWavePreviewPresentation : MonoBehaviour
{
    EnemySpawner spawner;
    GameObject row;
    readonly GameObject[] cards = new GameObject[6];
    readonly Text[] counts = new Text[6];
    readonly string[] keys = { "infantry", "runner", "heavy", "shield", "archer", "boss" };
    readonly string[] en = { "INF", "RUN", "HEAVY", "SHIELD", "ARCHER", "MENELAUS" };
    readonly string[] ru = { "ПЕХ", "БЕГ", "ТЯЖ", "ЩИТ", "ЛУК", "МЕНЕЛАЙ" };
    bool built;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoCreate()
    {
        if (FindFirstObjectByType<VisualWavePreviewPresentation>() == null)
            new GameObject("VisualWavePreviewPresentation").AddComponent<VisualWavePreviewPresentation>();
    }

    void Update()
    {
        if (!built)
        {
            GameObject hud = GameObject.Find("ModernCombatHUD");
            if (hud == null) return;
            Transform wave = hud.transform.Find("WaveStatus");
            if (wave == null) return;
            spawner = FindFirstObjectByType<EnemySpawner>();
            Build(wave);
            built = true;
        }

        if (spawner == null) spawner = FindFirstObjectByType<EnemySpawner>();
        if (spawner == null || row == null) return;

        int[] values =
        {
            spawner.NextWaveInfantryCount,
            spawner.NextWaveRunnerCount,
            spawner.NextWaveHeavyCount,
            spawner.NextWaveShieldCount,
            spawner.NextWaveArcherCount,
            spawner.NextWaveBossCount
        };

        int visible = 0;
        for (int i=0;i<cards.Length;i++) if (values[i] > 0) visible++;
        if (visible == 0)
        {
            row.SetActive(false);
            return;
        }

        row.SetActive(true);
        float start = -((visible - 1) * 78f) * .5f;
        int shown = 0;
        for (int i=0;i<cards.Length;i++)
        {
            bool active = values[i] > 0;
            cards[i].SetActive(active);
            if (!active) continue;
            RectTransform rt = cards[i].GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(start + shown * 78f, -28f);
            counts[i].text = "×" + values[i];
            shown++;
        }
    }

    void Build(Transform wave)
    {
        HideLegacyPreview(wave);

        row = new GameObject("EnemyCardRow");
        row.transform.SetParent(wave,false);
        RectTransform rowRt = row.AddComponent<RectTransform>();
        rowRt.anchorMin = rowRt.anchorMax = rowRt.pivot = new Vector2(.5f,.5f);
        rowRt.anchoredPosition = new Vector2(-75f,0f);
        rowRt.sizeDelta = new Vector2(470f,76f);

        for (int i=0;i<cards.Length;i++)
        {
            GameObject card = new GameObject("WaveCard_" + keys[i]);
            card.transform.SetParent(row.transform,false);
            Image bg = card.AddComponent<Image>();
            bg.sprite = TroyHudArt.Panel(i == 5);
            bg.type = Image.Type.Sliced;
            bg.color = i == 5 ? new Color(.55f,.12f,.045f,1f) : new Color(.31f,.18f,.08f,1f);
            RectTransform rt = bg.rectTransform;
            rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(.5f,.5f);
            rt.sizeDelta = new Vector2(i == 5 ? 92f : 70f,68f);

            AddImage(card.transform,"Portrait",new Vector2(0,9),new Vector2(38,38),TroyHudArt.Enemy(keys[i]));
            AddText(card.transform,GameLanguage.T(en[i],ru[i]),new Vector2(0,-18),new Vector2(i==5?88:66,16),i==5?8:9,new Color(1f,.82f,.52f,1f));
            counts[i] = AddText(card.transform,"×0",new Vector2(21,21),new Vector2(28,18),11,Color.white);
            counts[i].alignment = TextAnchor.MiddleCenter;
            counts[i].fontStyle = FontStyle.Bold;
            cards[i] = card;
        }
    }

    void HideLegacyPreview(Transform wave)
    {
        Text[] texts = wave.GetComponentsInChildren<Text>(true);
        for (int i=0;i<texts.Length;i++)
        {
            RectTransform rt = texts[i].rectTransform;
            if (Vector2.Distance(rt.anchoredPosition,new Vector2(-90f,-28f)) < 4f)
            {
                texts[i].enabled = false;
                break;
            }
        }
    }

    Image AddImage(Transform parent,string name,Vector2 pos,Vector2 size,Sprite sprite)
    {
        GameObject go=new GameObject(name); go.transform.SetParent(parent,false);
        Image image=go.AddComponent<Image>(); image.sprite=sprite; image.raycastTarget=false;
        RectTransform rt=image.rectTransform; rt.anchorMin=rt.anchorMax=rt.pivot=new Vector2(.5f,.5f); rt.anchoredPosition=pos; rt.sizeDelta=size;
        return image;
    }

    Text AddText(Transform parent,string value,Vector2 pos,Vector2 size,int fontSize,Color color)
    {
        GameObject go=new GameObject("Text"); go.transform.SetParent(parent,false);
        Text t=go.AddComponent<Text>(); t.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); t.text=value; t.fontSize=fontSize; t.fontStyle=FontStyle.Bold; t.color=color; t.alignment=TextAnchor.MiddleCenter; t.horizontalOverflow=HorizontalWrapMode.Wrap; t.verticalOverflow=VerticalWrapMode.Truncate; t.raycastTarget=false;
        RectTransform rt=t.rectTransform; rt.anchorMin=rt.anchorMax=rt.pivot=new Vector2(.5f,.5f); rt.anchoredPosition=pos; rt.sizeDelta=size;
        return t;
    }
}
