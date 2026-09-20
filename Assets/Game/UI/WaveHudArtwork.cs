using UnityEngine;
using UnityEngine.UI;

// Artwork only; ModernCombatHud owns geometry and live encounter text.
public sealed class WaveHudArtwork : MonoBehaviour
{
    Sprite banner;
    public void Apply()
    {
        var texture = Resources.Load<Texture2D>("WaveHud/Banner");
        if (texture == null) { Debug.LogError("Missing WaveHud/Banner"); return; }
        banner = Sprite.Create(texture, new Rect(0, texture.height * .24f, texture.width, texture.height * .63f), new Vector2(.5f,.5f));
        var image = GetComponent<Image>();
        image.sprite = banner;
        image.type = Image.Type.Simple;
        image.color = Color.white;
        image.raycastTarget = false;
        foreach (var outline in GetComponents<Outline>()) outline.enabled = false;
    }
    void OnDestroy()
    {
        if (banner == null) return;
        if (Application.isPlaying) Destroy(banner); else DestroyImmediate(banner);
    }
}
