using System.IO;
using SilverAssertions;
using Xunit;

namespace CodeBrix.WebPlatform.Styling.Tests;

/// <summary>What a project referencing a platform package gets in its output folder (this test project references all five).</summary>
public class OutputFolder
{
    [Theory]
    [MemberData(nameof(PlatformCase.Platforms), MemberType = typeof(PlatformCase))]
    public void program_is_copied_under_its_runtime_identifier(string platform)
    {
        //Arrange
        var tool = PlatformCase.For(platform);

        //Act
        var path = tool.GetExecutablePath(TestPaths.OutputDirectory);

        //Assert
        File.Exists(path).Should().BeTrue();
        FileHash.Sha256Of(path).Should().Be(tool.Sha256);
    }

    [Theory]
    [MemberData(nameof(PlatformCase.Platforms), MemberType = typeof(PlatformCase))]
    public void licence_notices_and_version_travel_beside_the_program(string platform)
    {
        //Arrange
        var directory = PlatformCase.For(platform).GetToolDirectory(TestPaths.OutputDirectory);

        //Assert
        File.ReadAllText(Path.Combine(directory, "LICENSE-Tailwind.txt")).Should().StartWith("MIT License");
        File.Exists(Path.Combine(directory, "THIRD-PARTY-NOTICES-Tailwind.txt")).Should().BeTrue();
        File.ReadAllText(Path.Combine(directory, "version.txt")).Trim().Should().Be("4.3.5");
    }

    [Theory]
    [MemberData(nameof(PlatformCase.Platforms), MemberType = typeof(PlatformCase))]
    public void tool_folder_holds_exactly_four_files(string platform)
        => Directory.GetFiles(PlatformCase.For(platform).GetToolDirectory(TestPaths.OutputDirectory)).Length.Should().Be(4);
}
