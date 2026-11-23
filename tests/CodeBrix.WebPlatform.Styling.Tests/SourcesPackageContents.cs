using System.IO.Compression;
using System.Linq;
using System.Xml.Linq;
using CodeBrix.WebPlatform.Styling.Tailwind4.Sources;
using SilverAssertions;
using Xunit;

namespace CodeBrix.WebPlatform.Styling.Tests;

/// <summary>The newest .nupkg the .Sources project built (bin/{Configuration}/).</summary>
public class SourcesPackageContents
{
    private const string ProjectName = "CodeBrix.WebPlatform.Styling.Tailwind4.Sources";

    [Fact]
    public void package_holds_exactly_the_agreed_files()
    {
        //Arrange
        var expected = TailwindSources.All.Select(a => $"{TailwindSources.PackageFolder}/{a.FileName}")
            .Concat(new[]
            {
                $"{TailwindSources.PackageFolder}/SOURCES-README.txt",
                "SOURCES-LICENSES.txt",
                $"lib/net10.0/{ProjectName}.dll",
                $"lib/net10.0/{ProjectName}.xml",
                "README.md",
                "AGENT-README.txt",
                "THIRD-PARTY-NOTICES.txt",
                "icon-codebrix-128.png",
            })
            .ToArray();

        //Act
        string[] entries;
        using (var package = ZipFile.OpenRead(TestPaths.NewestPackage(ProjectName)))
        {
            entries = package.Entries.Select(e => e.FullName)
                .Where(n => !n.StartsWith("_rels/") && !n.StartsWith("package/") && n != "[Content_Types].xml" && !n.EndsWith(".nuspec"))
                .ToArray();
        }

        //Assert
        entries.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void packaged_archives_match_their_hashes()
    {
        //Arrange
        using var package = ZipFile.OpenRead(TestPaths.NewestPackage(ProjectName));

        //Assert
        foreach (var archive in TailwindSources.All)
        {
            using var stream = package.GetEntry($"{TailwindSources.PackageFolder}/{archive.FileName}").Open();
            FileHash.Sha256Of(stream).Should().Be(archive.Sha256);
        }
    }

    [Fact]
    public void package_licence_is_the_sources_licence_file()
    {
        //Arrange
        XDocument nuspec;
        using (var package = ZipFile.OpenRead(TestPaths.NewestPackage(ProjectName)))
        using (var stream = package.Entries.Single(e => e.FullName.EndsWith(".nuspec")).Open())
        {
            nuspec = XDocument.Load(stream);
        }

        var ns = nuspec.Root.Name.Namespace;
        var license = nuspec.Descendants(ns + "license").Single();

        //Assert
        license.Attribute("type").Value.Should().Be("file");
        license.Value.Should().Be("SOURCES-LICENSES.txt");
        nuspec.Descendants(ns + "id").Single().Value.Should().Be(ProjectName);
        nuspec.Descendants(ns + "dependency").Should().BeEmpty();
    }
}
