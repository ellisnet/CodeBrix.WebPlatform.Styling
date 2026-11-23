using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace CodeBrix.WebPlatform.Styling.Tests;

/// <summary>One of the five platform packages, read through its TailwindTool type and its build metadata.</summary>
internal sealed class PlatformCase
{
    private static readonly Dictionary<string, Type> ToolTypes = new Dictionary<string, Type>(StringComparer.Ordinal)
    {
        ["LinuxX64"] = typeof(Tailwind4.LinuxX64.TailwindTool),
        ["LinuxArm64"] = typeof(Tailwind4.LinuxArm64.TailwindTool),
        ["MacOSX64"] = typeof(Tailwind4.MacOSX64.TailwindTool),
        ["MacOSArm64"] = typeof(Tailwind4.MacOSArm64.TailwindTool),
        ["WindowsX64"] = typeof(Tailwind4.WindowsX64.TailwindTool),
    };

    private PlatformCase(string platform)
    {
        Platform = platform;
        ToolType = ToolTypes[platform];
    }

    public static TheoryData<string> Platforms { get; } = new TheoryData<string>(ToolTypes.Keys);

    public static IReadOnlyCollection<string> Names => ToolTypes.Keys;

    public string Platform { get; }

    public Type ToolType { get; }

    public string ProjectName => "CodeBrix.WebPlatform.Styling.Tailwind4." + Platform;

    public string PackageId => ProjectName + ".MitLicenseForever";

    public string TailwindVersion => Constant("TailwindVersion");

    public string RuntimeIdentifier => Constant("RuntimeIdentifier");

    public string ToolsFolderName => Constant("ToolsFolderName");

    public string ExecutableFileName => Constant("ExecutableFileName");

    public string ReleaseFileName => Constant("ReleaseFileName");

    public string ReleaseUrl => Constant("ReleaseUrl");

    public string Sha256 => Constant("Sha256");

    public static PlatformCase For(string platform) => new PlatformCase(platform);

    public string Metadata(string key)
        => ToolType.Assembly.GetCustomAttributes<AssemblyMetadataAttribute>().Single(a => a.Key == key).Value;

    public string GetToolDirectory(string baseDirectory)
        => (string)ToolType.GetMethod("GetToolDirectory", new[] { typeof(string) }).Invoke(null, new object[] { baseDirectory });

    public string GetExecutablePath(string baseDirectory)
        => (string)ToolType.GetMethod("GetExecutablePath", new[] { typeof(string) }).Invoke(null, new object[] { baseDirectory });

    private string Constant(string name) => (string)ToolType.GetField(name).GetRawConstantValue();
}
