using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class CombatHudLayoutUxTests
{
    [UnityTest]
    public IEnumerator SpeedControls_AreCenteredInsideWaveStatus()
    {
        yield return null;
        yield return null;

        Transform waveStatus = FindSceneTransform("WaveStatus");
        Assert.NotNull(waveStatus, "Modern combat HUD must create WaveStatus.");

        Transform speedDown = waveStatus.Find("SpeedPrevious");
        Transform speedUp = waveStatus.Find("SpeedNext");
        Transform speedValue = waveStatus.Find("SpeedValue");
        Assert.NotNull(speedDown, "Left speed arrow must live inside WaveStatus.");
        Assert.NotNull(speedUp, "Right speed arrow must live inside WaveStatus.");
        Assert.NotNull(speedValue, "Current combat speed must be displayed inside WaveStatus.");

        RectTransform downRect = speedDown as RectTransform;
        RectTransform upRect = speedUp as RectTransform;
        RectTransform speedRect = speedValue as RectTransform;
        Assert.NotNull(downRect);
        Assert.NotNull(upRect);
        Assert.NotNull(speedRect);
        Assert.Less(downRect.anchoredPosition.x, 0f);
        Assert.AreEqual(0f, speedRect.anchoredPosition.x, .01f, "Speed value should be centered between arrows.");
        Assert.Greater(upRect.anchoredPosition.x, 0f);
        Assert.AreEqual(Mathf.Abs(downRect.anchoredPosition.x), Mathf.Abs(upRect.anchoredPosition.x), .01f, "Speed arrows should be symmetric around center.");
    }

    [UnityTest]
    public IEnumerator WaveStatus_HasCenteredDynamicProgressBar()
    {
        yield return null;
        yield return null;

        Transform waveStatus = FindSceneTransform("WaveStatus");
        Assert.NotNull(waveStatus);
        Transform progress = waveStatus.Find("WaveProgress");
        Assert.NotNull(progress, "WaveStatus must expose a live wave progress bar.");

        RectTransform progressRect = progress as RectTransform;
        Assert.NotNull(progressRect);
        Assert.AreEqual(0f, progressRect.anchoredPosition.x, .01f, "Wave progress must stay centered on screen.");

        Image fill = progress.Find("Fill")?.GetComponent<Image>();
        Assert.NotNull(fill);
        Assert.AreEqual(Image.Type.Filled, fill.type);
        Assert.AreEqual(Image.FillMethod.Horizontal, fill.fillMethod);
    }

    [UnityTest]
    public IEnumerator TopLeftResources_SeparateGoldAndGateHealthProgress()
    {
        yield return null;
        yield return null;

        Transform resources = FindSceneTransform("TopResources");
        Assert.NotNull(resources, "Top-left resources block must exist.");
        Assert.NotNull(resources.Find("CoinIcon"), "Gold must remain the primary top-left resource.");

        Transform gateProgress = resources.Find("GateHealthProgress");
        Assert.NotNull(gateProgress, "Gate health progress bar must be directly under gold.");
        Assert.NotNull(gateProgress.Find("Fill")?.GetComponent<Image>());
    }

    [UnityTest]
    public IEnumerator DivinePower_HasOneDirectPortraitLedAction()
    {
        yield return null;
        yield return null;

        Transform actions = FindSceneTransform("DivinePowerActions");
        Assert.NotNull(actions, "Combat HUD must expose one Divine Power block.");
        Transform action = actions.Find("Magic_Primary");
        Assert.NotNull(action);
        Assert.NotNull(action.Find("MagicIcon")?.GetComponent<Image>(), "Divine Power must use the same portrait/icon hierarchy as Hector abilities.");
        Assert.AreEqual(1, Resources.FindObjectsOfTypeAll<Transform>().Count(t => t != null && t.name == "Magic_Primary" && t.gameObject.scene.IsValid()), "Only one Divine Power action may exist in combat.");
        Assert.IsNull(FindSceneTransform("MagicToggle"), "A second corner magic control must not compete with the primary action.");
        Assert.IsNull(FindSceneTransform("MagicFlyout"), "Legacy magic flyout must stay removed.");
        Assert.IsNull(FindSceneTransform("CombatActions"), "Legacy combat action panel must stay removed.");
        Assert.IsNull(FindSceneTransform("DivineGiftChoiceOverlay"), "Patron selection belongs to the pre-map flow, not the combat HUD.");
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
        Assert.NotNull(wave?.GetComponent<Image>()?.sprite);
        Assert.NotNull(resources.Find("GateIcon")?.GetComponent<Image>()?.sprite);
        Assert.NotNull(wave.Find("WaveCrest")?.GetComponent<Image>()?.sprite);
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

        Assert.LessOrEqual(objective.anchoredPosition.y, -170f, "Objective card must stay below the top-left resource cluster.");
        Assert.LessOrEqual(tutorial.anchoredPosition.y, -290f, "Tutorial card must stay below the objective card.");
        Assert.GreaterOrEqual(notification.anchoredPosition.y, 320f, "Notifications must reserve the lower-left Hector region.");
        Assert.LessOrEqual(preview.anchoredPosition.x, -280f, "Next-encounter cards must not occupy the centered speed controls.");
        Assert.LessOrEqual(preview.anchoredPosition.y, -60f, "Next-encounter cards must sit below the progress bar.");
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
        Assert.AreEqual(1f, divine.anchorMin.y, .01f);
        Assert.AreEqual(1f, buildDock.anchorMin.x, .01f);
        Assert.AreEqual(0f, buildDock.anchorMin.y, .01f);
        Assert.AreEqual(0f, hector.anchorMin.x, .01f);
        Assert.AreEqual(0f, hector.anchorMin.y, .01f);
    }

    [UnityTest]
    public IEnumerator CharacterAndCombatInspectors_HaveSemanticPortraits()
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
        Assert.NotNull(enemy);
        Assert.NotNull(enemy.sprite);
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
        Assert.NotNull(inspector.Find("EnemyPortrait")?.GetComponent<Image>()?.sprite);
    }

    [UnityTest]
    public IEnumerator SelectedPatron_WatchesBattlefieldFromRightCommentaryPanel()
    {
        yield return null;
        yield return null;

        Assert.NotNull(GameManager.Instance);
        if (GameManager.Instance.GiftAvailable)
            Assert.IsTrue(GameManager.Instance.UseGift(DivineGiftType.Athena));

        yield return null;
        yield return null;

        Transform panel = FindSceneTransform("PatronCommentaryCard");
        Assert.NotNull(panel, "Selected patron must own a dedicated observer panel.");
        RectTransform panelRect = panel as RectTransform;
        Assert.NotNull(panelRect);
        Assert.AreEqual(1f, panelRect.anchorMin.x, .01f, "Patron observer must be anchored on the right side.");
        Assert.AreEqual(1f, panelRect.anchorMin.y, .01f, "Patron observer must be anchored near the top edge.");

        Image portrait = FindChildRecursive(panel, "PatronPortrait")?.GetComponent<Image>();
        Assert.NotNull(portrait);
        Assert.NotNull(portrait.sprite, "Selected patron portrait must load from project Resources.");
        Assert.NotNull(FindChildRecursive(panel, "PatronSpeech"), "Patron panel must expose the current event commentary.");
        Assert.IsNull(FindSceneTransform("PatronObserverPanel"), "The imported observer must not duplicate the canonical patron panel.");
        Assert.NotNull(FindSceneTransform("HectorCommentary"), "Preserve the approved illustrated Hector block.");
    }

    static Transform FindSceneTransform(string name)
    {
        return Resources.FindObjectsOfTypeAll<Transform>()
            .FirstOrDefault(t => t != null && t.name == name && t.gameObject.scene.IsValid());
    }

    static Transform FindChildRecursive(Transform root, string name)
    {
        if (root == null) return null;
        Transform[] children = root.GetComponentsInChildren<Transform>(true);
        return children.FirstOrDefault(t => t != null && t.name == name);
    }
}
