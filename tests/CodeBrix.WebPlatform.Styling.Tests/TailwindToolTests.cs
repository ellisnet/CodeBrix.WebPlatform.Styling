using System;
using System.IO;
using System.Linq;
using SilverAssertions;
using Xunit;

namespace CodeBrix.WebPlatform.Styling.Tests;

public class TailwindToolTests
{
    private static readonly string[] SupportedIdentifiers = { "linux-x64", "linux-arm64", "osx-x64", "osx-arm64", "win-x64" };

    [Theory]
    [MemberData(nameof(PlatformCase.Platforms), MemberType = typeof(PlatformCase))]
    public void constants_agree_with_the_build_values(string platform)
    {
        //Arrange
        var tool = PlatformCase.For(platform);

        //Assert
        tool.Metadata("TailwindVersion").Should().Be(tool.TailwindVersion);
        tool.Metadata("TailwindRuntimeIdentifier").Should().Be(tool.RuntimeIdentifier);
        tool.Metadata("TailwindReleaseFileName").Should().Be(tool.ReleaseFileName);
        tool.Metadata("TailwindReleaseUrl").Should().Be(tool.ReleaseUrl);
        tool.Metadata("TailwindSha256").Should().Be(tool.Sha256);
        tool.Metadata("TailwindExecutableFileName").Should().Be(tool.ExecutableFileName);
    }

    [Theory]
    [MemberData(nameof(PlatformCase.Platforms), MemberType = typeof(PlatformCase))]
    public void TailwindVersion_is_4_3_5(string platform)
        => PlatformCase.For(platform).TailwindVersion.Should().Be("4.3.5");

    [Theory]
    [MemberData(nameof(PlatformCase.Platforms), MemberType = typeof(PlatformCase))]
    public void RuntimeIdentifier_is_one_of_the_five_supported(string platform)
        => SupportedIdentifiers.Should().Contain(PlatformCase.For(platform).RuntimeIdentifier);

    [Fact]
    public void every_platform_has_its_own_runtime_identifier()
        => PlatformCase.Names.Select(n => PlatformCase.For(n).RuntimeIdentifier).Should().BeEquivalentTo(SupportedIdentifiers);

    [Theory]
    [MemberData(nameof(PlatformCase.Platforms), MemberType = typeof(PlatformCase))]
    public void ReleaseUrl_names_the_release_and_file(string platform)
    {
        //Arrange
        var tool = PlatformCase.For(platform);

        //Assert
        tool.ReleaseUrl.Should().Be($"https://github.com/tailwindlabs/tailwindcss/releases/download/v{tool.TailwindVersion}/{tool.ReleaseFileName}");
    }

    [Theory]
    [MemberData(nameof(PlatformCase.Platforms), MemberType = typeof(PlatformCase))]
    public void ExecutableFileName_has_exe_only_on_windows(string platform)
    {
        //Arrange
        var tool = PlatformCase.For(platform);

        //Assert
        tool.ExecutableFileName.Should().Be(tool.RuntimeIdentifier == "win-x64" ? "tailwindcss.exe" : "tailwindcss");
    }

    [Theory]
    [MemberData(nameof(PlatformCase.Platforms), MemberType = typeof(PlatformCase))]
    public void Sha256_is_lower_case_hex_of_64_characters(string platform)
        => PlatformCase.For(platform).Sha256.Should().MatchRegex("^[0-9a-f]{64}$");

    [Theory]
    [MemberData(nameof(PlatformCase.Platforms), MemberType = typeof(PlatformCase))]
    public void Sha256_matches_the_release_checksum_file(string platform)
    {
        //Arrange
        var tool = PlatformCase.For(platform);
        var sums = File.ReadAllText(Path.Combine(TestPaths.InfoDirectory, "sha256sums.txt"));

        //Assert
        sums.Should().Contain($"{tool.Sha256}  ./{tool.ReleaseFileName}");
    }

    [Theory]
    [MemberData(nameof(PlatformCase.Platforms), MemberType = typeof(PlatformCase))]
    public void GetToolDirectory_combines_folder_and_identifier(string platform)
    {
        //Arrange
        var tool = PlatformCase.For(platform);
        var baseDirectory = Path.Combine(Path.GetTempPath(), "app");

        //Act
        var directory = tool.GetToolDirectory(baseDirectory);

        //Assert
        directory.Should().Be(Path.Combine(baseDirectory, "tailwind", tool.RuntimeIdentifier));
    }

    [Theory]
    [MemberData(nameof(PlatformCase.Platforms), MemberType = typeof(PlatformCase))]
    public void GetExecutablePath_ends_with_the_executable(string platform)
    {
        //Arrange
        var tool = PlatformCase.For(platform);
        var baseDirectory = Path.Combine(Path.GetTempPath(), "app");

        //Act
        var path = tool.GetExecutablePath(baseDirectory);

        //Assert
        path.Should().Be(Path.Combine(baseDirectory, "tailwind", tool.RuntimeIdentifier, tool.ExecutableFileName));
    }

    [Theory]
    [MemberData(nameof(PlatformCase.Platforms), MemberType = typeof(PlatformCase))]
    public void GetToolDirectory_rejects_a_blank_folder(string platform)
    {
        //Arrange
        var tool = PlatformCase.For(platform);

        //Act
        Action act = () => tool.GetToolDirectory("  ");

        //Assert
        act.Should().Throw<System.Reflection.TargetInvocationException>().WithInnerException<ArgumentException>();
    }

    [Fact]
    public void GetToolDirectory_without_argument_uses_the_application_base_folder()
        => Tailwind4.LinuxX64.TailwindTool.GetToolDirectory().Should().Be(Path.Combine(AppContext.BaseDirectory, "tailwind", "linux-x64"));

    [Fact]
    public void GetExecutablePath_without_argument_uses_the_application_base_folder()
        => Tailwind4.WindowsX64.TailwindTool.GetExecutablePath().Should().Be(Path.Combine(AppContext.BaseDirectory, "tailwind", "win-x64", "tailwindcss.exe"));
}
