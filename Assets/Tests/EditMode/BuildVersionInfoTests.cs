using NUnit.Framework;

public class BuildVersionInfoTests
{
    [Test]
    public void StampedVersion_RoundTripsToReadableBadge()
    {
        string version = BuildVersionInfo.ComposeStampedVersion("main", "0123456789abcdef");

        Assert.AreEqual("0.6-main-01234567", version);
        Assert.AreEqual("BUILD v0.6 • MAIN • 01234567", BuildVersionInfo.BadgeFromStampedVersion(version));
        Assert.AreEqual("v0.6 · 01234567", BuildVersionInfo.CompactBadgeFromStampedVersion(version));
    }

    [Test]
    public void MissingStamp_IsExplicitInsteadOfPretendingToBeCurrentMain()
    {
        Assert.AreEqual("BUILD v0.6 • UNSTAMPED", BuildVersionInfo.BadgeFromStampedVersion("0.1.0"));
        Assert.AreEqual("v0.6 · UNSTAMPED", BuildVersionInfo.CompactBadgeFromStampedVersion("0.1.0"));
    }

    [Test]
    public void ComposeStampedVersion_NormalizesBranchAndShortsSha()
    {
        Assert.AreEqual("0.6-feature_foo-12345678", BuildVersionInfo.ComposeStampedVersion("feature/foo", "1234567890abcdef"));
        Assert.AreEqual("0.6-unknown-unknown", BuildVersionInfo.ComposeStampedVersion(null, null));
    }

    [Test]
    public void BadgeFromStampedVersion_RoundTripsNormalizedBranches()
    {
        string version = BuildVersionInfo.ComposeStampedVersion("feature/foo", "0123456789abcdef");
        Assert.AreEqual("BUILD v0.6 • FEATURE_FOO • 01234567", BuildVersionInfo.BadgeFromStampedVersion(version));
    }
}
