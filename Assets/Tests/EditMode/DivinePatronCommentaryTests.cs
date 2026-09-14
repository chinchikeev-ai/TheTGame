using NUnit.Framework;

public sealed class DivinePatronCommentaryTests
{
    [TestCase(DivineGiftType.Ares)]
    [TestCase(DivineGiftType.Athena)]
    [TestCase(DivineGiftType.Apollo)]
    [TestCase(DivineGiftType.Poseidon)]
    public void EveryPatron_HasLocalizedCommentaryForEveryEvent(DivineGiftType patron)
    {
        foreach (PatronCommentaryEvent eventType in System.Enum.GetValues(typeof(PatronCommentaryEvent)))
        {
            string line = DivinePatronCommentaryCatalog.Line(patron, eventType, 10);
            Assert.IsFalse(string.IsNullOrWhiteSpace(line), $"Missing {patron} commentary for {eventType}.");
        }
    }
}
