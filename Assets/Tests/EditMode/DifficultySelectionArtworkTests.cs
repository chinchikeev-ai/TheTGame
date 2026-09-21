using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public sealed class DifficultySelectionArtworkTests
{
    GameObject root;
    DifficultySelectionArtwork view;
    int backs;
    int confirmations;
    CampaignDifficulty confirmed;

    [SetUp]
    public void SetUp()
    {
        backs = 0;
        confirmations = 0;
        confirmed = CampaignDifficulty.Story;

        root = new GameObject("DifficultyTest", typeof(RectTransform));
        root.GetComponent<RectTransform>().sizeDelta = new Vector2(1600f, 900f);
        view = root.AddComponent<DifficultySelectionArtwork>();
        view.Build(
            () => backs++,
            difficulty =>
            {
                confirmed = difficulty;
                confirmations++;
            },
            CampaignDifficulty.Story);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(root);
    }

    [Test]
    public void DedicatedDifficultyPortraits_AreAvailableAsResources()
    {
        Assert.NotNull(Resources.Load<Texture2D>("DifficultySelection/Story"));
        Assert.NotNull(Resources.Load<Texture2D>("DifficultySelection/Strategos"));
        Assert.NotNull(Resources.Load<Texture2D>("DifficultySelection/Legendary"));
    }

    [Test]
    public void BuildsThreeLargeDifficultyCardsWithCorrectStartingGold()
    {
        Transform layout = root.transform.Find("DifficultyLayout");
        Assert.NotNull(layout);

        AssertCard(layout, CampaignDifficulty.Story, 190);
        AssertCard(layout, CampaignDifficulty.Strategos, 150);
        AssertCard(layout, CampaignDifficulty.Legendary, 120);

        Assert.NotNull(layout.Find("Back")?.GetComponent<Button>());
        Assert.NotNull(layout.Find("Next")?.GetComponent<Button>());
    }

    [Test]
    public void ReferenceDecorAndLargeNavigationButtonsExist()
    {
        Transform layout = root.transform.Find("DifficultyLayout");
        Assert.NotNull(layout.Find("DifficultyTitlePlate"));
        Assert.NotNull(layout.Find("DifficultySubtitlePlate"));
        Assert.NotNull(layout.Find("LeftSignGods"));
        Assert.NotNull(layout.Find("LeftSignHeroes"));
        Assert.NotNull(layout.Find("RightTroyBanner"));
        Assert.NotNull(layout.Find("RightChoiceStone"));
        Assert.NotNull(layout.Find("BottomHelmet"));
        Assert.NotNull(layout.Find("Back")?.GetComponent<Button>());
        Assert.NotNull(layout.Find("Next")?.GetComponent<Button>());

        RectTransform back = layout.Find("Back") as RectTransform;
        RectTransform next = layout.Find("Next") as RectTransform;
        Assert.GreaterOrEqual(back.sizeDelta.x, 300f);
        Assert.GreaterOrEqual(next.sizeDelta.x, 420f);
    }

    [Test]
    public void CardSelectsButNextConfirmsDifficulty()
    {
        Transform layout = root.transform.Find("DifficultyLayout");
        Button strategos = layout.Find("Difficulty_Strategos").GetComponent<Button>();
        Button next = layout.Find("Next").GetComponent<Button>();

        strategos.onClick.Invoke();

        Assert.AreEqual(CampaignDifficulty.Strategos, view.SelectedDifficulty);
        Assert.AreEqual(0, confirmations, "Selecting a card must not immediately leave the screen.");
        Assert.IsTrue(next.interactable);

        next.onClick.Invoke();

        Assert.AreEqual(1, confirmations);
        Assert.AreEqual(CampaignDifficulty.Strategos, confirmed);
    }

    [Test]
    public void BackDoesNotConfirmDifficulty()
    {
        Transform layout = root.transform.Find("DifficultyLayout");
        layout.Find("Back").GetComponent<Button>().onClick.Invoke();

        Assert.AreEqual(1, backs);
        Assert.AreEqual(0, confirmations);
    }

    [TestCase(1920, 1080)]
    [TestCase(1366, 768)]
    [TestCase(1376, 768)]
    [TestCase(1024, 768)]
    public void LayoutScalesInsideReferenceViewport(int width, int height)
    {
        RectTransform rect = root.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(width, height);

        typeof(DifficultySelectionArtwork)
            .GetMethod("LateUpdate", BindingFlags.Instance | BindingFlags.NonPublic)
            .Invoke(view, null);

        RectTransform layout = root.transform.Find("DifficultyLayout") as RectTransform;
        Assert.NotNull(layout);

        float expected = Mathf.Min(width / 1600f, height / 900f);
        Assert.AreEqual(expected, layout.localScale.x, .001f);
        Assert.AreEqual(expected, layout.localScale.y, .001f);

        Assert.AreEqual(new Vector2(418f, 594f), ((RectTransform)layout.Find("Difficulty_Story")).sizeDelta);
        Assert.AreEqual(new Vector2(418f, 594f), ((RectTransform)layout.Find("Difficulty_Strategos")).sizeDelta);
        Assert.AreEqual(new Vector2(418f, 594f), ((RectTransform)layout.Find("Difficulty_Legendary")).sizeDelta);
    }

    static void AssertCard(Transform layout, CampaignDifficulty difficulty, int expectedGold)
    {
        Transform card = layout.Find("Difficulty_" + difficulty);
        Assert.NotNull(card, difficulty.ToString());

        Button button = card.GetComponent<Button>();
        Assert.NotNull(button);
        Assert.GreaterOrEqual(((RectTransform)card).sizeDelta.x, 418f);
        Assert.GreaterOrEqual(((RectTransform)card).sizeDelta.y, 594f);

        Assert.NotNull(card.Find("Header/Title")?.GetComponent<Text>());
        Assert.NotNull(card.Find("PortraitFrame/Portrait")?.GetComponent<RawImage>());

        Text gold = card.Find("StartingGold")?.GetComponent<Text>();
        Assert.NotNull(gold);
        Assert.AreEqual(expectedGold.ToString(), gold.text);
    }
}
