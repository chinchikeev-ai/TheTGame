using UnityEngine;
using UnityEngine.UI;

public sealed class VisualEncounterPreviewPresentation : MonoBehaviour
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
    static void AutoCreate(){if(FindFirstObjectByType<VisualEncounterPreviewPresentation>()==null)new GameObject("VisualEncounterPreviewPresentation").AddComponent<VisualEncounterPreviewPresentation>();}

    void Update()
    {
        if(!built)
        {
            Transform hud=ModernCombatHud.Instance!=null?ModernCombatHud.Instance.HudRoot:null;if(hud==null)return;
            Transform encounter=hud.Find("WaveStatus");if(encounter==null)return;
            spawner=EnemySpawner.Instance;Build(encounter);built=true;
        }
        if(spawner==null)spawner=EnemySpawner.Instance;if(spawner==null||row==null)return;
        int[] values={EncounterRuntime.NextEncounterInfantryCount(spawner),EncounterRuntime.NextEncounterRunnerCount(spawner),EncounterRuntime.NextEncounterHeavyCount(spawner),EncounterRuntime.NextEncounterShieldCount(spawner),EncounterRuntime.NextEncounterArcherCount(spawner),EncounterRuntime.NextEncounterBossCount(spawner)};
        int visible=0;for(int i=0;i<cards.Length;i++)if(values[i]>0)visible++;
        if(visible==0){row.SetActive(false);return;}
        row.SetActive(true);float start=-((visible-1)*74f)*.5f;int shown=0;
        for(int i=0;i<cards.Length;i++)
        {
            bool active=values[i]>0;cards[i].SetActive(active);if(!active)continue;
            RectTransform rt=cards[i].GetComponent<RectTransform>();rt.anchoredPosition=new Vector2(start+shown*74f,0f);counts[i].text="×"+values[i];shown++;
        }
    }

    void Build(Transform encounter)
    {
        row=new GameObject("EnemyEncounterCardRow");row.transform.SetParent(encounter,false);
        RectTransform rowRt=row.AddComponent<RectTransform>();rowRt.anchorMin=rowRt.anchorMax=rowRt.pivot=new Vector2(.5f,.5f);rowRt.anchoredPosition=new Vector2(-270f,-68f);rowRt.sizeDelta=new Vector2(440f,66f);
        for(int i=0;i<cards.Length;i++)
        {
            GameObject card=new GameObject("EncounterCard_"+keys[i]);card.transform.SetParent(row.transform,false);
            Image bg=card.AddComponent<Image>();bg.sprite=TroyHudArt.Panel(i==5);bg.type=Image.Type.Sliced;bg.color=i==5?new Color(.55f,.12f,.045f,1f):new Color(.31f,.18f,.08f,1f);
            RectTransform rt=bg.rectTransform;rt.anchorMin=rt.anchorMax=rt.pivot=new Vector2(.5f,.5f);rt.sizeDelta=new Vector2(i==5?88f:66f,62f);
            AddImage(card.transform,"Portrait",new Vector2(0,8),new Vector2(38,38),TroyHudArt.Enemy(keys[i]));
            AddText(card.transform,GameLanguage.T(en[i],ru[i]),new Vector2(0,-22),new Vector2(i==5?82:62,14),i==5?8:9,new Color(1f,.82f,.52f,1f));
            counts[i]=AddText(card.transform,"×0",new Vector2(23,20),new Vector2(28,16),10,Color.white);counts[i].alignment=TextAnchor.MiddleCenter;counts[i].fontStyle=FontStyle.Bold;cards[i]=card;
        }
    }

    Image AddImage(Transform parent,string name,Vector2 pos,Vector2 size,Sprite sprite){GameObject go=new GameObject(name);go.transform.SetParent(parent,false);Image image=go.AddComponent<Image>();image.sprite=sprite;image.raycastTarget=false;RectTransform rt=image.rectTransform;rt.anchorMin=rt.anchorMax=rt.pivot=new Vector2(.5f,.5f);rt.anchoredPosition=pos;rt.sizeDelta=size;return image;}
    Text AddText(Transform parent,string value,Vector2 pos,Vector2 size,int fontSize,Color color){GameObject go=new GameObject("Text");go.transform.SetParent(parent,false);Text t=go.AddComponent<Text>();t.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");t.text=value;t.fontSize=fontSize;t.fontStyle=FontStyle.Bold;t.color=color;t.alignment=TextAnchor.MiddleCenter;t.horizontalOverflow=HorizontalWrapMode.Wrap;t.verticalOverflow=VerticalWrapMode.Truncate;t.raycastTarget=false;RectTransform rt=t.rectTransform;rt.anchorMin=rt.anchorMax=rt.pivot=new Vector2(.5f,.5f);rt.anchoredPosition=pos;rt.sizeDelta=size;return t;}
}
