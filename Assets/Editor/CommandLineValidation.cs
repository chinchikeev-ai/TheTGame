#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class CommandLineValidation
{
    static int errors;

    public static void RunArchitectureChecks()
    {
        errors = 0;
        Application.logMessageReceived += Capture;
        try
        {
            ArchitectureSmokeValidator.Run();
        }
        finally
        {
            Application.logMessageReceived -= Capture;
        }

        if (errors == 0) Debug.Log("[VALIDATION] Architecture command-line checks passed.");
        else Debug.LogError($"[VALIDATION] Architecture command-line checks failed with {errors} error log(s).");

        EditorApplication.Exit(errors == 0 ? 0 : 1);
    }

    static void Capture(string condition, string stackTrace, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert)
            errors++;
    }
}
#endif
