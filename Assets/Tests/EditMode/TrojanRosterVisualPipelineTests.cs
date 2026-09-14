using System.IO;
using NUnit.Framework;

public class TrojanRosterVisualPipelineTests
{
    const string SupportBuilderPath = "Assets/Editor/MythicAndSupportArtCandidateBuilder.cs";
    const string RefinementPath = "Assets/Editor/TrojanRosterRefinementPass.cs";
    const string ShieldBuilderPath = "Assets/Editor/ChapterOneShieldCandidateBuilder.cs";

    [Test]
    public void SupportBuilder_RunsRosterRefinementAfterGeneration()
    {
        Assert.IsTrue(File.Exists(SupportBuilderPath), "Missing support candidate builder.");
        string source = File.ReadAllText(SupportBuilderPath);
        StringAssert.Contains("TrojanRosterRefinementPass.ApplyAll();", source);
    }

    [Test]
    public void RosterRefinement_BindsSignaturePropsToRigBones()
    {
        Assert.IsTrue(File.Exists(RefinementPath), "Missing Trojan roster refinement pass.");
        string source = File.ReadAllText(RefinementPath);

        StringAssert.Contains("Lit Fire Bottle", source);
        StringAssert.Contains("Sun Staff", source);
        StringAssert.Contains("Cyclops Throwing Boulder", source);
        StringAssert.Contains("Trojan_BallistaCrew", source);
        StringAssert.Contains("SetParent(target, true)", source);
    }

    [Test]
    public void Hector_ShieldCandidateUsesRoundShieldAndHorseEmblem()
    {
        Assert.IsTrue(File.Exists(ShieldBuilderPath), "Missing Chapter I shield candidate builder.");
        string source = File.ReadAllText(ShieldBuilderPath);

        StringAssert.Contains("Hero_Hector.prefab\", false, 1.10f, true", source);
        StringAssert.Contains("HectorHorseBody", source);
        StringAssert.Contains("AegeanRoundShield.obj", source);
    }
}
