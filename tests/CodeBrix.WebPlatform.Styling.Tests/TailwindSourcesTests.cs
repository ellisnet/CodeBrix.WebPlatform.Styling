using System.IO;
using System.Linq;
using System.Reflection;
using CodeBrix.WebPlatform.Styling.Tailwind4.Sources;
using SilverAssertions;
using Xunit;

namespace CodeBrix.WebPlatform.Styling.Tests;

public class TailwindSourcesTests
{
    [Fact]
    public void TailwindVersion_is_4_3_5()
        => TailwindSources.TailwindVersion.Should().Be("4.3.5");

    [Fact]
    public void PackageFolder_names_the_version()
        => TailwindSources.PackageFolder.Should().Be("sources/v" + TailwindSources.TailwindVersion);

    [Fact]
    public void All_lists_the_four_archives_in_order()
        => TailwindSources.All.Select(a => a.FileName).Should().ContainInOrder(
            "tailwindcss-4.3.5.zip", "oven-sh-bun-v1.4.3.zip", "oven-sh-WebKit-0c06faad-JSCOnly.zip", "oven-sh-tinycc-05f0fafa.zip");

    [Fact]
    public void All_has_exactly_four_archives()
        => TailwindSources.All.Count.Should().Be(4);

    [Fact]
    public void archive_hashes_agree_with_the_build_values()
    {
        //Arrange
        var metadata = typeof(TailwindSources).Assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
            .Where(a => a.Key.StartsWith("SourceArchive:"))
            .ToDictionary(a => a.Key.Substring("SourceArchive:".Length), a => a.Value);

        //Assert
        metadata.Count.Should().Be(TailwindSources.All.Count);
        foreach (var archive in TailwindSources.All)
        {
            metadata[archive.FileName].Should().Be(archive.Sha256);
        }
    }

    [Fact]
    public void committed_archives_match_their_hashes()
    {
        foreach (var archive in TailwindSources.All)
        {
            FileHash.Sha256Of(Path.Combine(TestPaths.InfoDirectory, archive.FileName)).Should().Be(archive.Sha256);
        }
    }

    [Fact]
    public void SOURCES_README_records_every_hash_and_revision()
    {
        //Arrange
        var readme = File.ReadAllText(Path.Combine(TestPaths.InfoDirectory, "SOURCES-README.txt"));

        //Assert
        foreach (var archive in TailwindSources.All)
        {
            readme.Should().Contain(archive.FileName);
            readme.Should().Contain(archive.Sha256);
            readme.Should().Contain(archive.Revision);
        }
    }

    [Fact]
    public void SOURCES_LICENSES_names_every_archive()
    {
        //Arrange
        var licences = File.ReadAllText(Path.Combine(TestPaths.InfoDirectory, "SOURCES-LICENSES.txt"));

        //Assert
        foreach (var archive in TailwindSources.All)
        {
            licences.Should().Contain(archive.FileName);
        }
    }

    [Fact]
    public void every_archive_names_its_repository_on_github()
    {
        foreach (var archive in TailwindSources.All)
        {
            archive.Repository.Should().StartWith("https://github.com/");
        }
    }
}
