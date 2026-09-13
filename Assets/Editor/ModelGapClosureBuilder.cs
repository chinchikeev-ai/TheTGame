using UnityEditor;
using UnityEngine;

public static class ModelGapClosureBuilder
{
    [MenuItem("The Troy Game/Characters/Build Full Campaign Model Candidate Set")]
    public static void BuildAll()
    {
        CampaignArtCandidateBuilder.BuildAll();
        MythicAndSupportArtCandidateBuilder.BuildAll();
        CampaignEnvironmentCandidateBuilder.BuildAll();
        CampaignCivilianVariantBuilder.BuildAll();
        CampaignChariotHorseBuilder.Build();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Full campaign model candidate set rebuilt. Final authored art and Play Mode QA are still required before DONE.");
    }
}
