using System;
using System.IO;
using UnityEngine;

public static class BuildVersionInfo
{
    public const string ProductVersion = "0.6";
    const int ShortShaLength = 8;

    public static string MenuBadge
    {
        get
        {
#if UNITY_EDITOR
            if (TryGetRepositoryIdentity(out string branch, out string sha))
                return FormatBadge(branch, sha);
#endif
            return BadgeFromStampedVersion(Application.version);
        }
    }

    public static bool TryGetRepositoryIdentity(out string branch, out string sha)
    {
        branch = Environment.GetEnvironmentVariable("GITHUB_REF_NAME");
        sha = Environment.GetEnvironmentVariable("GITHUB_SHA");

#if UNITY_EDITOR
        if (string.IsNullOrWhiteSpace(branch)) branch = RunGit("rev-parse --abbrev-ref HEAD");
        if (string.IsNullOrWhiteSpace(sha)) sha = RunGit("rev-parse HEAD");
#endif

        branch = NormalizeToken(branch);
        sha = ShortSha(sha);
        return !string.IsNullOrWhiteSpace(branch) && !string.IsNullOrWhiteSpace(sha);
    }

    public static string ComposeStampedVersion(string branch, string sha)
    {
        string normalizedBranch = NormalizeToken(branch);
        string shortSha = ShortSha(sha).ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(normalizedBranch)) normalizedBranch = "unknown";
        if (string.IsNullOrWhiteSpace(shortSha)) shortSha = "unknown";
        return $"{ProductVersion}-{normalizedBranch}-{shortSha}";
    }

    public static string BadgeFromStampedVersion(string version)
    {
        string prefix = ProductVersion + "-";
        if (!string.IsNullOrWhiteSpace(version) && version.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        {
            string payload = version.Substring(prefix.Length);
            int separator = payload.LastIndexOf('-');
            if (separator > 0 && separator < payload.Length - 1)
            {
                string branch = payload.Substring(0, separator);
                string sha = payload.Substring(separator + 1);
                return FormatBadge(branch, sha);
            }
        }

        return $"BUILD v{ProductVersion} • UNSTAMPED";
    }

    static string FormatBadge(string branch, string sha)
    {
        string normalizedBranch = NormalizeToken(branch).ToUpperInvariant();
        string shortSha = ShortSha(sha).ToUpperInvariant();
        if (string.IsNullOrWhiteSpace(normalizedBranch)) normalizedBranch = "UNKNOWN";
        if (string.IsNullOrWhiteSpace(shortSha)) shortSha = "UNKNOWN";
        return $"BUILD v{ProductVersion} • {normalizedBranch} • {shortSha}";
    }

    static string NormalizeToken(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return string.Empty;

        char[] buffer = value.Trim().ToCharArray();
        for (int i = 0; i < buffer.Length; i++)
        {
            char c = buffer[i];
            if (char.IsLetterOrDigit(c) || c == '.' || c == '_') continue;
            buffer[i] = '_';
        }
        return new string(buffer);
    }

    static string ShortSha(string sha)
    {
        if (string.IsNullOrWhiteSpace(sha)) return string.Empty;
        string value = sha.Trim();
        return value.Length <= ShortShaLength ? value : value.Substring(0, ShortShaLength);
    }

#if UNITY_EDITOR
    static string RunGit(string arguments)
    {
        try
        {
            string projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
            var startInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "git",
                Arguments = arguments,
                WorkingDirectory = projectRoot,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using (var process = new System.Diagnostics.Process { StartInfo = startInfo })
            {
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                if (!process.WaitForExit(1200) || process.ExitCode != 0) return string.Empty;
                return output.Trim();
            }
        }
        catch
        {
            return string.Empty;
        }
    }
#endif
}
