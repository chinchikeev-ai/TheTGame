using System;
using System.IO;
using System.Text;
using UnityEngine;

public static class RuntimeFileLogger
{
    static readonly object Sync = new object();
    static StreamWriter writer;
    static bool initialized;
    static string logPath;

    public static string LogPath => logPath;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Initialize()
    {
        if (initialized) return;
        initialized = true;

        try
        {
            string directory = Path.Combine(Application.persistentDataPath, "Logs");
            Directory.CreateDirectory(directory);

            string stamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff");
            logPath = Path.Combine(directory, $"TheTroyGame_{stamp}.log");

            writer = new StreamWriter(logPath, false, new UTF8Encoding(false));
            writer.AutoFlush = true;

            WriteDirect("============================================================");
            WriteDirect("TheTroyGame runtime log");
            WriteDirect($"Started: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
            WriteDirect($"Unity: {Application.unityVersion}");
            WriteDirect($"Version: {Application.version}");
            WriteDirect($"Platform: {Application.platform}");
            WriteDirect($"OS: {SystemInfo.operatingSystem}");
            WriteDirect($"CPU: {SystemInfo.processorType} ({SystemInfo.processorCount} cores)");
            WriteDirect($"RAM: {SystemInfo.systemMemorySize} MB");
            WriteDirect($"GPU: {SystemInfo.graphicsDeviceName}");
            WriteDirect($"Graphics API: {SystemInfo.graphicsDeviceType}");
            WriteDirect($"Resolution: {Screen.width}x{Screen.height}, fullscreen={Screen.fullScreen}");
            WriteDirect($"PersistentDataPath: {Application.persistentDataPath}");
            WriteDirect($"LogFile: {logPath}");
            WriteDirect("============================================================");

            Application.logMessageReceivedThreaded += HandleLog;
            Application.quitting += Shutdown;
        }
        catch (Exception ex)
        {
            // Do not throw during bootstrap if logging itself cannot be initialized.
            Debug.LogError($"RuntimeFileLogger initialization failed: {ex.Message}");
        }
    }

    static void HandleLog(string condition, string stackTrace, LogType type)
    {
        lock (Sync)
        {
            if (writer == null) return;

            string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            writer.WriteLine($"[{timestamp}] [{type}] {condition}");

            if ((type == LogType.Error || type == LogType.Exception || type == LogType.Assert) &&
                !string.IsNullOrWhiteSpace(stackTrace))
            {
                writer.WriteLine(stackTrace);
            }
        }
    }

    public static void Event(string category, string message)
    {
        lock (Sync)
        {
            if (writer == null) return;
            writer.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [EVENT] [{category}] {message}");
        }
    }

    static void WriteDirect(string message)
    {
        lock (Sync)
        {
            writer?.WriteLine(message);
        }
    }

    static void Shutdown()
    {
        lock (Sync)
        {
            if (writer == null) return;
            writer.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [SESSION] Application quitting");
            writer.Flush();
            writer.Dispose();
            writer = null;
        }

        Application.logMessageReceivedThreaded -= HandleLog;
        Application.quitting -= Shutdown;
    }
}
