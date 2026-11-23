using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using SilverAssertions;
using Xunit;

namespace CodeBrix.WebPlatform.Styling.Tests;

/// <summary>Runs the packaged program of the machine the tests run on (the other four platforms' programs are never run).</summary>
public class HostProgram
{
    [Fact]
    public async Task program_reports_its_version()
    {
        //Arrange
        var program = HostExecutable();

        //Act
        var result = await RunAsync(program, Path.GetTempPath(), "--help");

        //Assert
        result.Output.Should().Contain("tailwindcss v4.3.5");
    }

    [Fact]
    public async Task program_builds_a_stylesheet()
    {
        //Arrange
        var program = HostExecutable();
        var folder = Path.Combine(Path.GetTempPath(), "codebrix-styling-tests-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(folder);
        try
        {
            await File.WriteAllTextAsync(Path.Combine(folder, "input.css"), "@import \"tailwindcss\";\n", TestContext.Current.CancellationToken);
            await File.WriteAllTextAsync(Path.Combine(folder, "index.html"), "<p class=\"text-red-500 underline\">x</p>\n", TestContext.Current.CancellationToken);

            //Act
            var result = await RunAsync(program, folder, "-i", "input.css", "-o", "output.css");

            //Assert
            result.ExitCode.Should().Be(0);
            var css = await File.ReadAllTextAsync(Path.Combine(folder, "output.css"), TestContext.Current.CancellationToken);
            css.Should().Contain(".text-red-500");
            css.Should().Contain(".underline");
        }
        finally
        {
            Directory.Delete(folder, true);
        }
    }

    private static string HostExecutable()
    {
        var platform = HostPlatform();
        if (platform == null)
        {
            Assert.Skip($"No Tailwind package for {RuntimeInformation.RuntimeIdentifier}.");
        }

        var path = PlatformCase.For(platform).GetExecutablePath(TestPaths.OutputDirectory);
        if (!OperatingSystem.IsWindows())
        {
            File.SetUnixFileMode(path, File.GetUnixFileMode(path) | UnixFileMode.UserExecute);
        }

        return path;
    }

    private static string HostPlatform()
    {
        var arm = RuntimeInformation.OSArchitecture == Architecture.Arm64;
        var x64 = RuntimeInformation.OSArchitecture == Architecture.X64;
        if (OperatingSystem.IsLinux())
        {
            return x64 ? "LinuxX64" : arm ? "LinuxArm64" : null;
        }

        if (OperatingSystem.IsMacOS())
        {
            return x64 ? "MacOSX64" : arm ? "MacOSArm64" : null;
        }

        return OperatingSystem.IsWindows() && x64 ? "WindowsX64" : null;
    }

    private static async Task<(int ExitCode, string Output)> RunAsync(string program, string workingDirectory, params string[] arguments)
    {
        var start = new ProcessStartInfo(program)
        {
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };
        //The program colours its output on Windows (and wherever CI is set) even when it is redirected; NO_COLOR keeps it plain text.
        start.Environment["NO_COLOR"] = "1";
        foreach (var argument in arguments)
        {
            start.ArgumentList.Add(argument);
        }

        using var process = Process.Start(start);
        var output = process.StandardOutput.ReadToEndAsync(TestContext.Current.CancellationToken);
        var error = process.StandardError.ReadToEndAsync(TestContext.Current.CancellationToken);
        await process.WaitForExitAsync(TestContext.Current.CancellationToken);
        return (process.ExitCode, await output + await error);
    }
}
