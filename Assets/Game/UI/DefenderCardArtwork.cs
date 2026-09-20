using UnityEngine;
using UnityEngine.UI;

// Presentation only: selection and prices still come from the existing HUD owner.
public sealed class DefenderCardArtwork : MonoBehaviour
{
    Sprite portraitSprite;
    Sprite frameSprite;
    Image portrait;
    Text price;
    Outline selection;

    public void Apply(TowerType type, Image icon, Text priceLabel)
    {
        portrait = icon;
        price = priceLabel;
        var frame = Resources.Load<Texture2D>("HectorHud/AbilityButton");
        if (frame != null)
        {
            frameSprite = Sprite.Create(frame, new Rect(0, 0, frame.width, frame.height), new Vector2(.5f, .5f));
            var image = GetComponent<Image>();
            image.sprite = frameSprite;
            image.type = Image.Type.Simple;
            image.color = Color.white;
        }

        string path = null;
        switch (type)
        {
            case TowerType.SpearThrower: path = "DefenderHud/Spearman"; break;
            case TowerType.MachineGun: path = "DefenderHud/Archer"; break;
            case TowerType.Cannon: path = "DefenderHud/Ballista"; break;
            case TowerType.TrojanGuard: path = "DefenderHud/Guard"; break;
            case TowerType.Slow: path = "DefenderHud/Priest"; break;
            case TowerType.FireTower: path = "DefenderHud/Fire"; break;
        }
        if (path != null)
        {
            var texture = Resources.Load<Texture2D>(path);
            if (texture != null)
            {
                portraitSprite = Sprite.Create(texture,
                    new Rect(0, 0, texture.width, texture.height), new Vector2(.5f, .5f));
                portrait.sprite = portraitSprite;
                portrait.preserveAspect = true;
            }
            else Debug.LogError("Missing defender artwork: " + path);
        }
        portrait.raycastTarget = false;
        selection = gameObject.AddComponent<Outline>();
        selection.effectDistance = new Vector2(3, -3);
        selection.useGraphicAlpha = true;
        var feedback = GetComponent<MenuButtonFeedback>();
        if (feedback != null) feedback.enabled = false;
        var button = GetComponent<Button>();
        var colors = ColorBlock.defaultColorBlock;
        colors.highlightedColor = new Color(1f, .93f, .72f);
        colors.pressedColor = new Color(.8f, .7f, .5f);
        colors.selectedColor = Color.white;
        button.colors = colors;
        SetState(false, false, true);
    }

    public void SetState(bool selected, bool recommended, bool affordable)
    {
        selection.enabled = selected || recommended;
        selection.effectColor = selected ? new Color(1f, .35f, .04f) : new Color(1f, .85f, .3f);
        portrait.color = affordable ? Color.white : new Color(.58f, .58f, .58f);
        price.color = affordable ? new Color(1f, .9f, .65f) : new Color(1f, .36f, .28f);
    }

    void OnDestroy()
    {
        Release(portraitSprite);
        Release(frameSprite);
    }

    static void Release(Sprite sprite)
    {
        if (sprite == null) return;
        if (Application.isPlaying) Destroy(sprite);
        else DestroyImmediate(sprite);
    }
}
