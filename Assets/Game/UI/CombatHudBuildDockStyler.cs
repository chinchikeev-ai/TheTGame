using UnityEngine;
using UnityEngine.UI;

public static class CombatHudBuildDockStyler
{
    public static void Apply(Button button, bool selected, bool recommended, bool affordable)
    {
        if (button == null) return;
        Image image = button.GetComponent<Image>();
        if (image == null) return;

        if (selected) image.color = new Color(.58f, .11f, .045f, .98f);
        else if (recommended && affordable) image.color = new Color(.48f, .30f, .07f, .98f);
        else if (recommended) image.color = new Color(.29f, .20f, .08f, .92f);
        else image.color = affordable ? new Color(.24f, .14f, .08f, .96f) : new Color(.11f, .085f, .07f, .88f);
    }
}
