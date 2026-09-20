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
    public IEnumerator HectorHud_IsHiddenUntilHectorIsSelected_AndHasMouseCommands()
    {
        yield return null;
        yield return null;

        HectorController hector = Object.FindFirstObjectByType<HectorController>();
        HectorHUD hud = Object.FindFirstObjectByType<HectorHUD>();
        Assert.NotNull(hector);
        Assert.NotNull(hud);

        hector.SetSelected(false);
        yield return null;
        Transform panel = hud.transform.Find("HectorHUDCanvas/HectorPanel");
        Assert.NotNull(panel);
        Assert.IsFalse(panel.gameObject.activeSelf, "Hector command menu must stay hidden until Hector is selected.");

        hector.SetSelected(true);
        yield return null;
        Assert.IsTrue(panel.gameObject.activeSelf, "Selecting Hector must open the Hector command menu.");

        Button[] buttons = hud.GetComponentsInChildren<Button>(true);
        Assert.GreaterOrEqual(buttons.Length, 5, "Hector HUD should expose the Hector card plus four clickable ability buttons.");
        Button cardButton = buttons.FirstOrDefault(b => b.name == "HectorPanel");
        Assert.NotNull(cardButton, "The full Hector card must be the click target.");
        Assert.IsTrue(buttons.Any(b => b.name == "Ability_Q"));
        Assert.IsTrue(buttons.Any(b => b.name == "Ability_E"));
        Assert.IsTrue(buttons.Any(b => b.name == "Ability_R"));
        Assert.IsTrue(buttons.Any(b => b.name == "Ability_F"));

        hector.SetSelected(false);
        cardButton.onClick.Invoke();
        yield return null;
        Assert.IsTrue(hector.Selected, "Clicking the Hector HUD card must select Hector.");
    }

    [UnityTest]
    public IEnumerator Hector_MoveCommand_ChangesDestinationAndMoves()
    {
        yield return null;
        yield return null;

        GameBootstrap bootstrap = Object.FindFirstObjectByType<GameBootstrap>();
        Assert.NotNull(bootstrap);
        Assert.NotNull(bootstrap.Runtime);
        Assert.NotNull(bootstrap.Runtime.Chapter);

        HectorController hector = bootstrap.Runtime.Chapter.Hector;
        HectorInputDriver input = hector != null ? hector.GetComponent<HectorInputDriver>() : null;
        Camera camera = bootstrap.Runtime.Camera;
        Transform[][] routes = bootstrap.Runtime.Chapter.Paths;

        Assert.NotNull(hector);
        Assert.NotNull(input);
        Assert.NotNull(camera);
        Assert.NotNull(routes);
        Assert.Greater(routes.Length, 0);
        Assert.NotNull(routes[0]);
        Assert.Greater(routes[0].Length, 2);

        hector.SetSelected(true);
        Vector3 start = hector.transform.position;
        Transform targetNode = routes[0][Mathf.Max(0, routes[0].Length / 2)];
        Vector2 screenPoint = camera.WorldToScreenPoint(targetNode.position);

        Assert.IsTrue(input.TryMoveAtScreenPoint(screenPoint), "A selected Hector must accept a battlefield move command.");
        Assert.Greater((hector.MoveDestination - start).sqrMagnitude, .01f, "Move command must change Hector's active destination.");

        float previousScale = Time.timeScale;
        Time.timeScale = 1f;
        yield return null;
        yield return null;
        yield return null;
        Time.timeScale = previousScale;

        Assert.Greater((hector.transform.position - start).sqrMagnitude, .0001f, "Hector must physically move after accepting the command.");
    }

    [UnityTest]
    public IEnumerator Hector_AlwaysHasAnAnimationPath()
    {
        HectorController hector = Object.FindFirstObjectByType<HectorController>();
        Assert.NotNull(hector);
        yield return null;

        Animator animator = hector.GetComponentInChildren<Animator>(true);
        bool authoredAnimatorActive = animator != null && animator.enabled && animator.runtimeAnimatorController != null;
        HectorMotionFallbackAnimator fallback = hector.GetComponent<HectorMotionFallbackAnimator>();
        Assert.NotNull(fallback, "Hector must have a runtime animation fallback component.");

        if (!authoredAnimatorActive)
        {
            Assert.IsTrue(fallback.UsingProceduralFallback, "Fallback must activate when no authored Animator Controller is available.");
            Assert.IsTrue(fallback.HasAnimationTargets, "Fallback must resolve Hector body/bone targets so the hero cannot remain static.");
        }
    }
}
