using System;
using System.IO;
using UnityEngine;

public static class BuildVersionInfo
{
    public const string ProductVersion = "0.6";
    const int ShortShaLength = 8;

#if UNITY_EDITOR
    static string editorRemoteSha;
    static bool editorRemoteKnown;
    static bool editorOutdated;
    static bool editorDirty;
#endif

    public static string MenuBadge
    {
        get
        {
#if UNITY_EDITOR
            if (TryGetRepositoryIdentity(out string branch, out string sha))
            {
                string badge = FormatBadge(branch, sha);
                if (editorRemoteKnown)
                {
                    badge += editorOutdated
                        ? " • OUTDATED→" + editorRemoteSha.ToUpperInvariant()
                        : " • SYNC";
                    if (editorDirty) badge += " • DIRTY";
                }
                return badge;
            }
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

#if UNITY_EDITOR
    public static bool RefreshEditorMainSyncState(out string branch, out string localSha, out string remoteSha, out bool dirty)
    {
        branch = NormalizeToken(RunGit("rev-parse --abbrev-ref HEAD"));
        localSha = ShortSha(RunGit("rev-parse HEAD"));
        dirty = !string.IsNullOrWhiteSpace(RunGit("status --porcelain --untracked-files=no"));

        bool fetchSucceeded = RunGitCommand("fetch origin main --quiet", out _);
        remoteSha = ShortSha(RunGit("rev-parse origin/main"));

        editorRemoteKnown = !string.IsNullOrWhiteSpace(remoteSha);
        editorRemoteSha = remoteSha;
        editorOutdated = editorRemoteKnown && !string.Equals(localSha, remoteSha, StringComparison.OrdinalIgnoreCase);
        editorDirty = dirty;

        return fetchSucceeded
            && !string.IsNullOrWhiteSpace(branch)
            && !string.IsNullOrWhiteSpace(localSha)
            && editorRemoteKnown;
    }
#endif

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
        return RunGitCommand(arguments, out string output) ? output : string.Empty;
    }

    static bool RunGitCommand(string arguments, out string output)
    {
        output = string.Empty;
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
                string stdout = process.StandardOutput.ReadToEnd();
                process.StandardError.ReadToEnd();
                if (!process.WaitForExit(5000) || process.ExitCode != 0) return false;
                output = stdout.Trim();
                return true;
            }
        }
        catch
        {
            return false;
        }
    }
#endif
}
