using UnityEngine;
using UnityEngine.UI;

// Owns only the gate artwork; ModernCombatHud retains layout and health updates.
public sealed class GateHudArtwork : MonoBehaviour
{
    Sprite panelSprite, gateSprite, fillSprite;

    public void Apply(Image panel, Image gate, Image fill)
    {
        var panelTexture = Resources.Load<Texture2D>("GateHud/Panel");
        var gateTexture = Resources.Load<Texture2D>("GateHud/Gate");
        if (panelTexture == null || gateTexture == null)
        {
            Debug.LogError("Missing GateHud artwork.");
            return;
        }
        // Exclude the transparent export margins without altering the source bitmap.
        panelSprite = Sprite.Create(panelTexture,
            new Rect(0, panelTexture.height * .22f, panelTexture.width, panelTexture.height * .58f),
            new Vector2(.5f, .5f));
        gateSprite = Sprite.Create(gateTexture, new Rect(0, 0, gateTexture.width, gateTexture.height), new Vector2(.5f, .5f));
        fillSprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 2, 2), new Vector2(.5f, .5f));
        panel.sprite = panelSprite;
        panel.type = Image.Type.Simple;
        panel.color = Color.white;
        panel.raycastTarget = false;
        var outline = panel.GetComponent<Outline>();
        if (outline != null) outline.enabled = false;
        gate.sprite = gateSprite;
        gate.preserveAspect = true;
        gate.color = Color.white;
        fill.sprite = fillSprite;
        fill.type = Image.Type.Filled;
        fill.fillMethod = Image.FillMethod.Horizontal;
        fill.fillOrigin = 0;
        fill.color = new Color(.92f, .035f, .07f, 1f);
    }

    void OnDestroy()
    {
        Release(panelSprite);
        Release(gateSprite);
        Release(fillSprite);
    }

    static void Release(Sprite sprite)
    {
        if (sprite == null) return;
        if (Application.isPlaying) Destroy(sprite);
        else DestroyImmediate(sprite);
    }
}
