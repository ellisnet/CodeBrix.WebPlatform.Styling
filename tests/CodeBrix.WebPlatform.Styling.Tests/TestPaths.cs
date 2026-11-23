using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace CodeBrix.WebPlatform.Styling.Tests;

internal static class TestPaths
{
#if DEBUG
    public const string Configuration = "Debug";
#else
    public const string Configuration = "Release";
#endif

    public static string RepositoryRoot { get; } = FindRepositoryRoot();

    public static string DownloadDirectory { get; } = Path.Combine(RepositoryRoot, "Downloaded", "v4.3.5");

    public static string InfoDirectory { get; } = Path.Combine(RepositoryRoot, "info", "v4.3.5");

    public static string OutputDirectory { get; } = AppContext.BaseDirectory;

    public static string ProjectDirectory(string projectName) => Path.Combine(RepositoryRoot, "src", projectName);

    /// <summary>The newest package the build wrote for a project (the highest version in bin/{Configuration}/).</summary>
    public static string NewestPackage(string projectName)
    {
        var folder = Path.Combine(ProjectDirectory(projectName), "bin", Configuration);
        var newest = Directory.GetFiles(folder, "*.nupkg")
            .Select(path => (Path: path, Version: PackageVersion(path)))
            .OrderByDescending(p => p.Version)
            .FirstOrDefault();
        if (newest.Path == null)
        {
            throw new InvalidOperationException($"No package in {folder}; build the solution first.");
        }

        return newest.Path;
    }

    /// <summary>The version in a package file name (NuGet drops a revision of 0: 1.0.327.0 is written 1.0.327).</summary>
    public static Version PackageVersion(string packagePath)
        => Version.Parse(Regex.Match(Path.GetFileNameWithoutExtension(packagePath), @"\.(\d+\.\d+\.\d+(\.\d+)?)$").Groups[1].Value);

    /// <summary>A version the way NuGet writes it in a package file name.</summary>
    public static string FileVersion(string version)
    {
        var parts = version.Split('.');
        return parts.Length == 4 && parts[3] == "0" ? string.Join(".", parts.Take(3)) : version;
    }

    private static string FindRepositoryRoot()
    {
        var folder = new DirectoryInfo(AppContext.BaseDirectory);
        while (folder != null && !File.Exists(Path.Combine(folder.FullName, "CodeBrix.WebPlatform.Styling.slnx")))
        {
            folder = folder.Parent;
        }

        return folder?.FullName ?? throw new InvalidOperationException("The repository root (CodeBrix.WebPlatform.Styling.slnx) was not found above " + AppContext.BaseDirectory);
    }
}
