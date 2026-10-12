using System.Security.Cryptography;

namespace VRCNext.Services.Helpers;

public static class PathSafety
{
    public static readonly string[] StartupAppExtensions = { ".exe" };

    private static readonly char[] Seps = { '\\', '/' };

    public static bool IsNetworkPath(string? path)
    {
        var p = (path ?? "").Trim().Trim('"');
        if (p.Length == 0) return false;
        if (p.StartsWith(@"\\") || p.StartsWith("//")) return true;
        if (!OperatingSystem.IsWindows()) return false;
        try
        {
            var root = Path.GetPathRoot(Path.GetFullPath(p));
            if (string.IsNullOrEmpty(root)) return false;
            return new DriveInfo(root).DriveType == DriveType.Network;
        }
        catch { return true; }
    }

    private static bool IsLocalAbsolute(string p)
    {
        if (p.Length == 0) return false;
        if (p.IndexOfAny(Path.GetInvalidPathChars()) >= 0) return false;
        if (!Path.IsPathFullyQualified(p)) return false;
        return !IsNetworkPath(p);
    }

    public static bool IsSafeStartupApp(string? path)
    {
        var p = (path ?? "").Trim().Trim('"');
        if (!IsLocalAbsolute(p)) return false;
        if (!OperatingSystem.IsWindows()) return true;
        return StartupAppExtensions.Contains(Path.GetExtension(p), StringComparer.OrdinalIgnoreCase);
    }

    public static bool IsSafeVrcPath(string? path)
    {
        var p = (path ?? "").Trim().Trim('"');
        if (p.Length == 0) return true;
        if (!IsLocalAbsolute(p)) return false;
        var name = Path.GetFileName(p);
        return name.Equals("launch.exe", StringComparison.OrdinalIgnoreCase)
            || name.Equals("VRChat.exe", StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsSafeWatchFolder(string? path)
    {
        var p = (path ?? "").Trim().Trim('"');
        if (!IsLocalAbsolute(p)) return false;
        string full;
        try { full = Path.GetFullPath(p).TrimEnd(Seps); }
        catch { return false; }
        var root = (Path.GetPathRoot(full) ?? "").TrimEnd(Seps);
        return full.Length > root.Length;
    }

    public static List<string> FilterStartupApps(IEnumerable<string>? paths)
        => (paths ?? Enumerable.Empty<string>()).Where(IsSafeStartupApp).ToList();

    public static List<string> FilterWatchFolders(IEnumerable<string>? paths)
        => (paths ?? Enumerable.Empty<string>()).Where(IsSafeWatchFolder).ToList();

    public static bool Sha256Matches(string filePath, string expectedHex)
    {
        using var fs = File.OpenRead(filePath);
        var hash = Convert.ToHexString(SHA256.HashData(fs));
        return hash.Equals(expectedHex.Trim(), StringComparison.OrdinalIgnoreCase);
    }
}
