using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class SettingsArtworkLayout : MonoBehaviour
{
    readonly Dictionary<RectTransform, Vector2> positions = new Dictionary<RectTransform, Vector2>();
    Sprite backgroundSprite;
    Sprite knobSprite;
    public Sprite KnobSprite
    {
        get
        {
            if (knobSprite != null) return knobSprite;
            var texture = Resources.Load<Texture2D>("Menu/Settings_Reference");
            float x = texture.width / 1448f, y = texture.height / 1086f;
            knobSprite = Sprite.Create(texture, new Rect(993*x, (1086-395)*y, 48*x, 50*y), new Vector2(.5f,.5f));
            return knobSprite;
        }
    }
    public void Initialize()
    {
        var texture = Resources.Load<Texture2D>("Menu/Settings_Illustrated");
        backgroundSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(.5f, .5f));
        var image = GetComponent<Image>();
        image.sprite = backgroundSprite;
        image.color = Color.white;
        image.raycastTarget = true;
        foreach (RectTransform child in transform) positions.Add(child, child.anchoredPosition);
        Fit();
    }
    void LateUpdate() => Fit();
    void Fit()
    {
        var parent = transform.parent as RectTransform;
        if (parent == null) return;
        var rect = (RectTransform)transform;
        float scale = Mathf.Min(parent.rect.width / 1448f, parent.rect.height / 1086f);
        if (scale <= 0) return;
        rect.localScale = Vector3.one * scale;
        rect.sizeDelta = parent.rect.size / scale;
        foreach (var item in positions)
            if (item.Key != null) item.Key.anchoredPosition = new Vector2(item.Value.x * rect.sizeDelta.x / 1448f, item.Value.y * rect.sizeDelta.y / 1086f);
    }
    void OnDestroy()
    {
        if (backgroundSprite == null) return;
        if (Application.isPlaying) Destroy(backgroundSprite); else DestroyImmediate(backgroundSprite);
        if (knobSprite != null)
            if (Application.isPlaying) Destroy(knobSprite); else DestroyImmediate(knobSprite);
    }
}
