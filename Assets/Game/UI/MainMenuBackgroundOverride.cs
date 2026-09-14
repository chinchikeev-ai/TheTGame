using UnityEngine;
using UnityEngine.UI;

public sealed class MainMenuBackgroundOverride : MonoBehaviour
{
    const string ResourcePath = "Menu/MainBackground";

    Texture2D texture;
    Sprite sprite;
    Image appliedImage;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoStart()
    {
        if (FindFirstObjectByType<MainMenuBackgroundOverride>() == null)
            new GameObject("MainMenuBackgroundOverride").AddComponent<MainMenuBackgroundOverride>();
    }

    void Awake()
    {
        texture = Resources.Load<Texture2D>(ResourcePath);
        if (texture != null)
            sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(.5f, .5f), 100f);
    }

    void LateUpdate()
    {
        if (sprite == null) return;

        GameObject canvasObject = GameObject.Find("MenuCanvas");
        if (canvasObject == null) return;

        Transform background = canvasObject.transform.Find("MainMenu/Background");
        if (background == null) return;

        Image image = background.GetComponent<Image>();
        if (image == null || image == appliedImage) return;

        image.sprite = sprite;
        image.type = Image.Type.Simple;
        image.preserveAspect = false;
        image.raycastTarget = false;

        AspectRatioFitter fitter = background.GetComponent<AspectRatioFitter>();
        if (fitter != null)
            fitter.aspectRatio = (float)texture.width / texture.height;

        appliedImage = image;
        RuntimeFileLogger.Event("MENU", "Applied clean main-menu background resource");
    }
}
