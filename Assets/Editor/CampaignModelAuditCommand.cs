#if UNITY_EDITOR
using UnityEditor;

public static class CampaignModelAuditCommand
{
    public static void RunCurrentBatchmode()
    {
        CampaignModelAuditValidator.AuditReport report = CampaignModelAuditValidator.Run(true);
        EditorApplication.Exit(report.missing == 0 && report.broken == 0 ? 0 : 1);
    }
}
#endif
