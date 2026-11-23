using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Linq;
using SilverAssertions;
using Xunit;

namespace CodeBrix.WebPlatform.Styling.Tests;

/// <summary>The newest .nupkg each platform project built (bin/{Configuration}/).</summary>
public class PlatformPackageContents
{
    [Theory]
    [MemberData(nameof(PlatformCase.Platforms), MemberType = typeof(PlatformCase))]
    public void package_holds_exactly_the_agreed_files(string platform)
    {
        //Arrange
        var tool = PlatformCase.For(platform);
        var folder = $"tools/tailwind/{tool.RuntimeIdentifier}/";

        //Act
        var entries = PackageEntries(tool);

        //Assert
        entries.Should().BeEquivalentTo(
            folder + tool.ExecutableFileName,
            folder + "LICENSE-Tailwind.txt",
            folder + "THIRD-PARTY-NOTICES-Tailwind.txt",
            folder + "version.txt",
            $"buildTransitive/{tool.PackageId}.targets",
            $"lib/net10.0/{tool.ProjectName}.dll",
            $"lib/net10.0/{tool.ProjectName}.xml",
            "README.md",
            "AGENT-README.txt",
            "THIRD-PARTY-NOTICES.txt",
            "icon-codebrix-128.png");
    }

    [Theory]
    [MemberData(nameof(PlatformCase.Platforms), MemberType = typeof(PlatformCase))]
    public void packaged_program_matches_its_hash(string platform)
    {
        //Arrange
        var tool = PlatformCase.For(platform);
        using var package = ZipFile.OpenRead(TestPaths.NewestPackage(tool.ProjectName));
        using var stream = package.GetEntry($"tools/tailwind/{tool.RuntimeIdentifier}/{tool.ExecutableFileName}").Open();

        //Act
        var hash = FileHash.Sha256Of(stream);

        //Assert
        hash.Should().Be(tool.Sha256);
    }

    [Theory]
    [MemberData(nameof(PlatformCase.Platforms), MemberType = typeof(PlatformCase))]
    public void package_depends_on_one_exact_built_sources_version(string platform)
    {
        //Arrange
        var tool = PlatformCase.For(platform);
        var nuspec = Nuspec(tool);
        var ns = nuspec.Root.Name.Namespace;

        //Act
        var dependencies = nuspec.Descendants(ns + "dependency").ToList();

        //Assert
        dependencies.Count.Should().Be(1);
        dependencies[0].Attribute("id").Value.Should().Be("CodeBrix.WebPlatform.Styling.Tailwind4.Sources");
        var range = dependencies[0].Attribute("version").Value;
        range.Should().MatchRegex(@"^\[\d+\.\d+\.\d+(\.\d+)?\]$");
        var sourcesFolder = Path.Combine(TestPaths.ProjectDirectory("CodeBrix.WebPlatform.Styling.Tailwind4.Sources"), "bin", TestPaths.Configuration);
        File.Exists(Path.Combine(sourcesFolder, $"CodeBrix.WebPlatform.Styling.Tailwind4.Sources.{TestPaths.FileVersion(range.Trim('[', ']'))}.nupkg")).Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(PlatformCase.Platforms), MemberType = typeof(PlatformCase))]
    public void package_declares_the_mit_licence(string platform)
    {
        //Arrange
        var nuspec = Nuspec(PlatformCase.For(platform));
        var license = nuspec.Descendants(nuspec.Root.Name.Namespace + "license").Single();

        //Assert
        license.Attribute("type").Value.Should().Be("expression");
        license.Value.Should().Be("MIT");
    }

    [Theory]
    [MemberData(nameof(PlatformCase.Platforms), MemberType = typeof(PlatformCase))]
    public void buildTransitive_targets_copy_the_tool_folder(string platform)
    {
        //Arrange
        var tool = PlatformCase.For(platform);
        using var package = ZipFile.OpenRead(TestPaths.NewestPackage(tool.ProjectName));
        using var reader = new StreamReader(package.GetEntry($"buildTransitive/{tool.PackageId}.targets").Open());

        //Act
        var targets = reader.ReadToEnd();

        //Assert
        targets.Should().Contain(@"$(MSBuildThisFileDirectory)..\tools\tailwind\**\*");
        targets.Should().Contain(@"Link=""tailwind\%(RecursiveDir)%(Filename)%(Extension)""");
    }

    private static string[] PackageEntries(PlatformCase tool)
    {
        using var package = ZipFile.OpenRead(TestPaths.NewestPackage(tool.ProjectName));
        return package.Entries
            .Select(e => e.FullName)
            .Where(n => !n.StartsWith("_rels/") && !n.StartsWith("package/") && n != "[Content_Types].xml" && !n.EndsWith(".nuspec"))
            .ToArray();
    }

    private static XDocument Nuspec(PlatformCase tool)
    {
        using var package = ZipFile.OpenRead(TestPaths.NewestPackage(tool.ProjectName));
        using var stream = package.Entries.Single(e => e.FullName.EndsWith(".nuspec")).Open();
        return XDocument.Load(stream);
    }
}
