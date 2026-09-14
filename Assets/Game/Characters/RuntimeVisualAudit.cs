using System.Collections.Generic;

public static class RuntimeVisualAudit
{
    static readonly HashSet<string> Reported = new HashSet<string>();

    public static void Report(string role, string source, string detail)
    {
        string key = role + "|" + source + "|" + detail;
        if (!Reported.Add(key)) return;
        RuntimeFileLogger.Event("ART_SOURCE", role + " source=" + source + " detail=" + detail);
    }
}
