using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

public class CombatHudLayoutUxTests
{
    [UnityTest]
    public IEnumerator SpeedAndSettings_LiveWithGoldInTopLeftUtilityCluster()
    {
        yield return null;
        yield return null;

        Transform resources = FindSceneTransform("TopResources");
        Assert.NotNull(resources, "Modern combat HUD must create TopResources.");
        Transform speedDown = resources.Find("SpeedPrevious");
        Transform speedUp = resources.Find("SpeedNext");
        Transform speedValue = resources.Find("SpeedValue");
        Transform settings = resources.Find("CombatSettingsButton");
        Assert.NotNull(speedDown, "Left speed arrow must live beside Gold.");
        Assert.NotNull(speedUp, "Right speed arrow must live beside Gold.");
        Assert.NotNull(speedValue, "Current combat speed must be visible beside Gold.");
        Assert.NotNull(settings, "Combat settings must be a compact top-left utility control.");
        Assert.IsNull(FindSceneTransform("WaveStatus")?.Find("SpeedControlPanel"), "Encounter center must not own speed controls.");

        RectTransform downRect = speedDown as RectTransform;
        RectTransform upRect = speedUp as RectTransform;
        RectTransform speedRect = speedValue as RectTransform;
        Assert.Less(downRect.anchoredPosition.x, speedRect.anchoredPosition.x);
        Assert.Greater(upRect.anchoredPosition.x, speedRect.anchoredPosition.x);
        Assert.Greater((settings as RectTransform).anchoredPosition.x, upRect.anchoredPosition.x);
    }

    [UnityTest]
    public IEnumerator WaveStatus_HasCenteredDynamicProgressBar()
    {
        yield return null;
        yield return null;

        Transform waveStatus = FindSceneTransform("WaveStatus");
        Assert.NotNull(waveStatus);
        Transform progress = waveStatus.Find("WaveProgress");
        Assert.NotNull(progress, "WaveStatus must expose a live encounter progress bar.");
        RectTransform progressRect = progress as RectTransform;
        Assert.AreEqual(0f, progressRect.anchoredPosition.x, .01f, "Encounter progress must stay centered on screen.");
        Image fill = progress.Find("Fill")?.GetComponent<Image>();
        Assert.NotNull(fill);
        Assert.AreEqual(Image.Type.Filled, fill.type);
        Assert.AreEqual(Image.FillMethod.Horizontal, fill.fillMethod);
    }

    [UnityTest]
    public IEnumerator TopLeftResources_SeparateGoldUtilityAndGateHealth()
    {
        yield return null;
        yield return null;

        Transform resources = FindSceneTransform("TopResources");
        Assert.NotNull(resources);
        Assert.NotNull(resources.Find("CoinIcon"));
        Assert.NotNull(resources.Find("GoldResourcePanel")?.GetComponent<Image>()?.sprite);
        Assert.NotNull(resources.Find("SpeedControlPanel")?.GetComponent<Image>()?.sprite);
        Assert.NotNull(resources.Find("CombatSettingsButton")?.GetComponent<Button>());
        Transform gateProgress = resources.Find("GateHealthProgress");
        Assert.NotNull(gateProgress, "Gate health must remain the heavier second row below utility controls.");
        Assert.NotNull(gateProgress.Find("Fill")?.GetComponent<Image>());
    }

    [UnityTest]
    public IEnumerator DivinePower_HasOneDirectPortraitLedAction()
    {
        yield return null;
        yield return null;

        Transform actions = FindSceneTransform("DivinePowerActions");
        Assert.NotNull(actions);
        Transform action = actions.Find("Magic_Primary");
        Assert.NotNull(action);
        Assert.NotNull(action.Find("MagicIcon")?.GetComponent<Image>());
        Assert.AreEqual(1, Resources.FindObjectsOfTypeAll<Transform>().Count(t => t != null && t.name == "Magic_Primary" && t.gameObject.scene.IsValid()));
        Assert.IsNull(FindSceneTransform("MagicToggle"));
        Assert.IsNull(FindSceneTransform("MagicFlyout"));
        Assert.IsNull(FindSceneTransform("CombatActions"));
        Assert.IsNull(FindSceneTransform("DivineGiftChoiceOverlay"));
    }

    [UnityTest]
    public IEnumerator MainCombatCards_UseTrojanPanelArtAndSemanticIcons()
    {
        yield return null;
        yield return null;

        Transform resources = FindSceneTransform("TopResources");
        Transform wave = FindSceneTransform("WaveStatus");
        Assert.NotNull(resources);
        Assert.NotNull(wave);
        Assert.NotNull(resources.Find("GoldResourcePanel")?.GetComponent<Image>()?.sprite);
        Assert.NotNull(wave.GetComponent<Image>()?.sprite);
        Assert.NotNull(resources.Find("GateIcon")?.GetComponent<Image>()?.sprite);
        Assert.NotNull(wave.Find("WaveCrest")?.GetComponent<Image>()?.sprite);
    }

    [UnityTest]
    public IEnumerator DefenderCards_ArePortraitLedAndReadable()
    {
        yield return null;
        yield return null;

        Transform dock = FindSceneTransform("BuildDock");
        Assert.NotNull(dock);
        Transform[] cards = dock.GetComponentsInChildren<Transform>(true).Where(t => t.name.StartsWith("BuildCard_")).ToArray();
        Assert.AreEqual(6, cards.Length);
        foreach (Transform card in cards)
        {
            Image portrait = card.Find("TowerIcon")?.GetComponent<Image>();
            Assert.NotNull(portrait);
            Assert.NotNull(portrait.sprite);
            Assert.GreaterOrEqual(portrait.rectTransform.sizeDelta.x, 60f, "Defender portrait must be visually dominant.");
            Assert.GreaterOrEqual((card as RectTransform).sizeDelta.y, 130f, "Defender card must have room for portrait, name and price.");
        }
    }

    [UnityTest]
    public IEnumerator SelectedDefenseAndTooltip_ArePortraitLed()
    {
        yield return null;
        yield return null;

        Image selected = FindSceneTransform("SelectedTowerCrest")?.GetComponent<Image>();
        Image tooltip = FindSceneTransform("TooltipTowerPortrait")?.GetComponent<Image>();
        Assert.NotNull(selected);
        Assert.NotNull(tooltip);
        Assert.GreaterOrEqual(selected.rectTransform.sizeDelta.x, 80f);
        Assert.GreaterOrEqual(tooltip.rectTransform.sizeDelta.x, 80f);
    }

    [UnityTest]
    public IEnumerator GuidanceNotificationsAndPreview_ReserveNonCompetingZones()
    {
        yield return null;
        yield return null;

        RectTransform objective = FindSceneTransform("ChapterObjective") as RectTransform;
        RectTransform tutorial = FindSceneTransform("ContextTutorial") as RectTransform;
        RectTransform notification = FindSceneTransform("NotificationPanel") as RectTransform;
        RectTransform preview = FindSceneTransform("EnemyEncounterCardRow") as RectTransform;
        Assert.NotNull(objective);
        Assert.NotNull(tutorial);
        Assert.NotNull(notification);
        Assert.NotNull(preview);
        Assert.LessOrEqual(objective.anchoredPosition.y, -170f);
        Assert.LessOrEqual(tutorial.anchoredPosition.y, -295f);
        Assert.GreaterOrEqual(notification.anchoredPosition.y, 325f);
        Assert.LessOrEqual(preview.anchoredPosition.x, -260f);
        Assert.LessOrEqual(preview.anchoredPosition.y, -60f);
    }

    [UnityTest]
    public IEnumerator TutorialAdvice_UsesParchmentSurface()
    {
        yield return null;
        yield return null;

        Image tutorial = FindSceneTransform("ContextTutorial")?.GetComponent<Image>();
        Assert.NotNull(tutorial);
        Assert.NotNull(tutorial.sprite);
        Assert.AreEqual("Parchment", tutorial.sprite.name, "Contextual advice should use the Hector parchment art rather than another stone system panel.");
    }

    [UnityTest]
    public IEnumerator PrimaryHudAnchors_StayOnPerimeterZones()
    {
        yield return null;
        yield return null;

        RectTransform resources = FindSceneTransform("TopResources") as RectTransform;
        RectTransform encounter = FindSceneTransform("WaveStatus") as RectTransform;
        RectTransform divine = FindSceneTransform("DivinePowerActions") as RectTransform;
        RectTransform buildDock = FindSceneTransform("BuildDock") as RectTransform;
        RectTransform hector = FindSceneTransform("HectorPanel") as RectTransform;
        Assert.NotNull(resources);
        Assert.NotNull(encounter);
        Assert.NotNull(divine);
        Assert.NotNull(buildDock);
        Assert.NotNull(hector);
        Assert.AreEqual(0f, resources.anchorMin.x, .01f);
        Assert.AreEqual(1f, resources.anchorMin.y, .01f);
        Assert.AreEqual(.5f, encounter.anchorMin.x, .01f);
        Assert.AreEqual(1f, encounter.anchorMin.y, .01f);
        Assert.AreEqual(1f, divine.anchorMin.x, .01f);
        Assert.AreEqual(0f, divine.anchorMin.y, .01f);
        Assert.AreEqual(.5f, buildDock.anchorMin.x, .01f);
        Assert.AreEqual(0f, buildDock.anchorMin.y, .01f);
        Assert.AreEqual(0f, hector.anchorMin.x, .01f);
        Assert.AreEqual(0f, hector.anchorMin.y, .01f);
    }

    [UnityTest]
    public IEnumerator RuntimeInput_HasOneBootstrapAndOneEventSystem()
    {
        yield return null;
        yield return null;

        RuntimeInputBootstrap[] bootstraps = Resources.FindObjectsOfTypeAll<RuntimeInputBootstrap>()
            .Where(x => x != null && x.gameObject.scene.IsValid()).ToArray();
        EventSystem[] eventSystems = Resources.FindObjectsOfTypeAll<EventSystem>()
            .Where(x => x != null && x.gameObject.scene.IsValid()).ToArray();

        Assert.AreEqual(1, bootstraps.Length, "Runtime input must have one project-wide owner.");
        Assert.AreEqual(1, eventSystems.Length, "Only one runtime EventSystem may exist.");
#if ENABLE_INPUT_SYSTEM
        InputSystemUIInputModule module = eventSystems[0].GetComponent<InputSystemUIInputModule>();
        Assert.NotNull(module);
        Assert.IsTrue(module.enabled);
#endif
    }

    [UnityTest]
    public IEnumerator CombatHud_RaycastTargetsBelongOnlyToInteractiveControls()
    {
        yield return null;
        yield return null;

        Transform[] roots =
        {
            FindSceneTransform("ModernCombatHUD"),
            FindSceneTransform("HectorHUDCanvas")
        };

        foreach (Transform root in roots)
        {
            Assert.NotNull(root);
            foreach (Graphic graphic in root.GetComponentsInChildren<Graphic>(true))
            {
                if (!graphic.raycastTarget) continue;
                Selectable selectable = graphic.GetComponentInParent<Selectable>();
                Assert.NotNull(selectable, $"{graphic.name} blocks pointer input without an interactive Selectable owner.");
            }
        }
    }

    [UnityTest]
    public IEnumerator MagicAndDefenders_AreEqualBottomRightActions()
    {
        yield return null;
        yield return null;

        RectTransform magic = FindSceneTransform("Magic_Primary") as RectTransform;
        RectTransform defenders = FindSceneTransform("DefendersToggle") as RectTransform;
        RectTransform actions = FindSceneTransform("DivinePowerActions") as RectTransform;
        Assert.NotNull(magic);
        Assert.NotNull(defenders);
        Assert.NotNull(actions);

        Assert.AreEqual(1f, actions.anchorMin.x, .01f);
        Assert.AreEqual(0f, actions.anchorMin.y, .01f);
        Assert.AreEqual(1f, defenders.anchorMin.x, .01f);
        Assert.AreEqual(0f, defenders.anchorMin.y, .01f);
        Assert.AreEqual(magic.sizeDelta.x, defenders.sizeDelta.x, .01f);
        Assert.AreEqual(magic.sizeDelta.y, defenders.sizeDelta.y, .01f);
    }

    [UnityTest]
    public IEnumerator HectorHud_StaysCompactAtReferenceResolution()
    {
        yield return null;
        yield return null;

        RectTransform hector = FindSceneTransform("HectorPanel") as RectTransform;
        Assert.NotNull(hector);
        Assert.LessOrEqual(hector.localScale.x, .85f);
        Assert.LessOrEqual(hector.sizeDelta.x * hector.localScale.x, 1920f * .18f + 20f);
        Assert.LessOrEqual(hector.sizeDelta.y * hector.localScale.y, 1080f * .22f + 10f);
    }

    [UnityTest]
    public IEnumerator CharacterAndCombatInspectors_HaveLargeSemanticPortraits()
    {
        yield return null;
        yield return null;

        Image hector = FindSceneTransform("HectorPortrait")?.GetComponent<Image>();
        Image menelaus = FindSceneTransform("MenelausPortrait")?.GetComponent<Image>();
        Image enemy = FindSceneTransform("EnemyPortrait")?.GetComponent<Image>();
        Image patron = FindSceneTransform("PatronPortrait")?.GetComponent<Image>();
        Assert.NotNull(hector);
        Assert.NotNull(hector.sprite);
        Assert.NotNull(menelaus);
        Assert.NotNull(menelaus.sprite);
        Assert.GreaterOrEqual(menelaus.rectTransform.sizeDelta.x, 96f);
        Assert.NotNull(enemy);
        Assert.NotNull(enemy.sprite);
        Assert.GreaterOrEqual(enemy.rectTransform.sizeDelta.x, 84f);
        Assert.NotNull(patron);
    }

    [UnityTest]
    public IEnumerator EnemyInspector_UsesTrojanPanelArt()
    {
        yield return null;
        yield return null;

        Transform inspector = FindSceneTransform("EnemyInspectorPanel");
        Assert.NotNull(inspector);
        Image panel = inspector.GetComponent<Image>();
        Assert.NotNull(panel);
        Assert.NotNull(panel.sprite);
        Assert.AreEqual(Image.Type.Sliced, panel.type);
        Assert.NotNull(inspector.Find("EnemyPortraitFrame")?.GetComponent<Image>()?.sprite);
        Assert.NotNull(inspector.Find("EnemyPortrait")?.GetComponent<Image>()?.sprite);
    }

    [UnityTest]
    public IEnumerator SelectedPatron_WatchesBattlefieldFromRightCommentaryPanel()
    {
        yield return null;
        yield return null;

        Assert.NotNull(GameManager.Instance);
        if (GameManager.Instance.GiftAvailable) Assert.IsTrue(GameManager.Instance.UseGift(DivineGiftType.Athena));
        yield return null;
        yield return null;

        Transform panel = FindSceneTransform("PatronCommentaryCard");
        Assert.NotNull(panel);
        RectTransform panelRect = panel as RectTransform;
        Assert.AreEqual(1f, panelRect.anchorMin.x, .01f);
        Assert.AreEqual(1f, panelRect.anchorMin.y, .01f);
        Image portrait = FindChildRecursive(panel, "PatronPortrait")?.GetComponent<Image>();
        Assert.NotNull(portrait);
        Assert.NotNull(portrait.sprite);
        Assert.NotNull(FindChildRecursive(panel, "PatronSpeech"));
        Assert.IsNull(FindSceneTransform("PatronObserverPanel"));
        Assert.NotNull(FindSceneTransform("HectorCommentary"));
    }

    static Transform FindSceneTransform(string name)
    {
        return Resources.FindObjectsOfTypeAll<Transform>().FirstOrDefault(t => t != null && t.name == name && t.gameObject.scene.IsValid());
    }

    static Transform FindChildRecursive(Transform root, string name)
    {
        if (root == null) return null;
        Transform[] children = root.GetComponentsInChildren<Transform>(true);
        return children.FirstOrDefault(t => t != null && t.name == name);
    }
}
