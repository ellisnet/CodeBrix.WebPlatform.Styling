using System;
using System.IO;

namespace CodeBrix.WebPlatform.Styling.Tailwind4.WindowsX64;

/// <summary>
/// Names the standalone Tailwind CSS program this package ships for Windows x64 build machines
/// (runtime identifier <c>win-x64</c>) and where a consuming build finds it. The package copies it
/// into the output folder of the project that references it, as <c>tailwind/win-x64/</c>:
/// <c>{application base folder}/tailwind/win-x64/tailwindcss.exe</c>, with <c>LICENSE-Tailwind.txt</c>,
/// <c>THIRD-PARTY-NOTICES-Tailwind.txt</c> and <c>version.txt</c> beside it.
/// </summary>
/// <remarks>
/// The program is copied as a file. A NuGet package records no file permissions: on Linux and
/// macOS the executable permission is whatever the package extraction and the copy give, so code
/// that runs the program makes sure it is executable first.
/// </remarks>
public static class TailwindTool
{
    /// <summary>The Tailwind CSS release the program comes from.</summary>
    public const string TailwindVersion = "4.3.5";

    /// <summary>The runtime identifier of the build machines this package is for.</summary>
    public const string RuntimeIdentifier = "win-x64";

    /// <summary>The folder, below the application's base folder, that holds one folder per runtime identifier.</summary>
    public const string ToolsFolderName = "tailwind";

    /// <summary>The file name of the program in the package and in the output folder.</summary>
    public const string ExecutableFileName = "tailwindcss.exe";

    /// <summary>The file name of the program in the Tailwind CSS release.</summary>
    public const string ReleaseFileName = "tailwindcss-windows-x64.exe";

    /// <summary>Where the program was downloaded from when the package was built.</summary>
    public const string ReleaseUrl = "https://github.com/tailwindlabs/tailwindcss/releases/download/v4.3.5/tailwindcss-windows-x64.exe";

    /// <summary>The sha256 of the program (lower-case hex), the same as the release file's.</summary>
    public const string Sha256 = "2803583268c53f4713e89943e211081fddb186c0a4a7d07b5c342c6ae853ec95";

    /// <summary>
    /// The folder that holds this platform's program below an application's base folder:
    /// <c>{applicationBaseDirectory}/tailwind/win-x64</c>.
    /// </summary>
    /// <param name="applicationBaseDirectory">The application's base folder (normally <see cref="AppContext.BaseDirectory"/>).</param>
    /// <returns>The folder path.</returns>
    /// <exception cref="ArgumentException"><paramref name="applicationBaseDirectory"/> is null or blank.</exception>
    public static string GetToolDirectory(string applicationBaseDirectory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(applicationBaseDirectory);
        return Path.Combine(applicationBaseDirectory, ToolsFolderName, RuntimeIdentifier);
    }

    /// <summary>
    /// The folder that holds this platform's program below the running application's base folder
    /// (<see cref="AppContext.BaseDirectory"/>).
    /// </summary>
    /// <returns>The folder path.</returns>
    public static string GetToolDirectory() => GetToolDirectory(AppContext.BaseDirectory);

    /// <summary>
    /// The full path of the program below an application's base folder:
    /// <c>{applicationBaseDirectory}/tailwind/win-x64/tailwindcss.exe</c>.
    /// </summary>
    /// <param name="applicationBaseDirectory">The application's base folder (normally <see cref="AppContext.BaseDirectory"/>).</param>
    /// <returns>The file path; the file exists when the package was referenced and the build copied it.</returns>
    /// <exception cref="ArgumentException"><paramref name="applicationBaseDirectory"/> is null or blank.</exception>
    public static string GetExecutablePath(string applicationBaseDirectory)
        => Path.Combine(GetToolDirectory(applicationBaseDirectory), ExecutableFileName);

    /// <summary>
    /// The full path of the program below the running application's base folder
    /// (<see cref="AppContext.BaseDirectory"/>).
    /// </summary>
    /// <returns>The file path.</returns>
    public static string GetExecutablePath() => GetExecutablePath(AppContext.BaseDirectory);
}
