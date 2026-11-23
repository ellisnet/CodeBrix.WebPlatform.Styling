using System.IO;
using SilverAssertions;
using Xunit;

namespace CodeBrix.WebPlatform.Styling.Tests;

/// <summary>The release files the build downloaded into Downloaded/v4.3.5/.</summary>
public class DownloadedPrograms
{
    [Theory]
    [MemberData(nameof(PlatformCase.Platforms), MemberType = typeof(PlatformCase))]
    public void release_file_was_downloaded(string platform)
        => File.Exists(Path.Combine(TestPaths.DownloadDirectory, PlatformCase.For(platform).ReleaseFileName)).Should().BeTrue();

    [Theory]
    [MemberData(nameof(PlatformCase.Platforms), MemberType = typeof(PlatformCase))]
    public void release_file_matches_its_hash(string platform)
    {
        //Arrange
        var tool = PlatformCase.For(platform);

        //Act
        var hash = FileHash.Sha256Of(Path.Combine(TestPaths.DownloadDirectory, tool.ReleaseFileName));

        //Assert
        hash.Should().Be(tool.Sha256);
    }

    [Fact]
    public void no_partial_download_is_left_behind()
        => Directory.GetFiles(TestPaths.DownloadDirectory, "*.partial").Should().BeEmpty();
}
