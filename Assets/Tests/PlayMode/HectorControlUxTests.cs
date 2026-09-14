using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class HectorControlUxTests
{
    [UnityTest]
    public IEnumerator Hector_IsReliablyClickable_AndShowsSelectedState()
    {
        HectorController hector = Object.FindFirstObjectByType<HectorController>();
        Assert.NotNull(hector, "Chapter I must create Hector.");

        HectorInputDriver input = hector.GetComponent<HectorInputDriver>();
        Assert.NotNull(input, "Hector must have the runtime input driver.");

        Transform target = hector.transform.Find("HectorSelectionTarget");
        Assert.NotNull(target, "Hector must have a dedicated click target independent from decorative art colliders.");
        CapsuleCollider collider = target.GetComponent<CapsuleCollider>();
        Assert.NotNull(collider);
        Assert.IsTrue(collider.enabled);
        Assert.IsTrue(collider.isTrigger);

        Camera camera = Camera.main;
        Assert.NotNull(camera);
        Physics.SyncTransforms();
        Vector2 screenPoint = camera.WorldToScreenPoint(collider.bounds.center);
        Assert.IsTrue(input.TrySelectAtScreenPoint(screenPoint), "Clicking Hector's visible selection target must select him.");
        Assert.IsTrue(hector.Selected);

        yield return null;

        HectorSelectionPresentation selection = hector.GetComponent<HectorSelectionPresentation>();
        Assert.NotNull(selection);
        Transform ringTransform = hector.transform.Find("HectorSelectionRing");
        Assert.NotNull(ringTransform);
        LineRenderer ring = ringTransform.GetComponent<LineRenderer>();
        Assert.NotNull(ring);
        Assert.IsTrue(ring.enabled, "Selected Hector must have an obvious persistent selection ring.");
    }

    [UnityTest]
    public IEnumerator HectorHud_HasMouseCommands()
    {
        yield return null;
        yield return null;

        HectorHUD hud = Object.FindFirstObjectByType<HectorHUD>();
        Assert.NotNull(hud);

        Button[] buttons = hud.GetComponentsInChildren<Button>(true);
        Assert.GreaterOrEqual(buttons.Length, 5, "Hector HUD should expose portrait selection plus four clickable ability buttons.");
        Assert.IsTrue(buttons.Any(b => b.name == "HectorPortrait"), "Hector portrait must be clickable to select the hero from HUD.");
        Assert.IsTrue(buttons.Any(b => b.name == "Ability_Q"));
        Assert.IsTrue(buttons.Any(b => b.name == "Ability_E"));
        Assert.IsTrue(buttons.Any(b => b.name == "Ability_R"));
        Assert.IsTrue(buttons.Any(b => b.name == "Ability_F"));
    }
}
