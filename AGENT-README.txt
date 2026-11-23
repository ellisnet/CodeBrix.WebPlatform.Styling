================================================================================
AGENT-README: CodeBrix.WebPlatform.Styling
A Guide for AI Coding Agents — CONSUMING the CodeBrix.WebPlatform.Styling.Tailwind4.* NuGet packages
================================================================================

One file for every package of this repository:

  CodeBrix.WebPlatform.Styling.Tailwind4.<Platform>.MitLicenseForever
      (LinuxX64, LinuxArm64, MacOSX64, MacOSArm64, WindowsX64) - the prebuilt
      standalone Tailwind CSS program for one build machine.
  CodeBrix.WebPlatform.Styling.Tailwind4.Sources
      - the source of those programs; every platform package depends on it.


OVERVIEW
========
The standalone Tailwind CSS program is one executable that turns a Tailwind
input stylesheet into the generated stylesheet, with no Node.js or npm. These
packages carry that program exactly as Tailwind Labs releases it (the release
files are downloaded when the packages are built and checked against their
published sha256 hashes), so a machine that builds a CodeBrix.WebPlatform
frontend needs no Tailwind installed and nothing on PATH.

A platform package copies its program into the output folder of the project
that references it - normally the build program that calls
CodeBrix.WebPlatform.FrontendBuilder - as:

    {output folder}/tailwind/<rid>/tailwindcss        (tailwindcss.exe on Windows)
    {output folder}/tailwind/<rid>/LICENSE-Tailwind.txt
    {output folder}/tailwind/<rid>/THIRD-PARTY-NOTICES-Tailwind.txt
    {output folder}/tailwind/<rid>/version.txt        (the Tailwind release)

Each package also has a tiny assembly with one static class, TailwindTool, that
names the program and finds it below an application's base folder.

The program is built by Tailwind Labs with the Bun runtime, so it contains code
under several licences - among them LGPL-licensed JavaScriptCore (WebKit) and
TinyCC. THIRD-PARTY-NOTICES-Tailwind.txt, beside the program, lists all of it.
The complete source of the program, including that LGPL source, is in the
CodeBrix.WebPlatform.Styling.Tailwind4.Sources package, which every platform
package depends on at one exact version.

All assemblies target .NET 10 (net10.0).


INSTALLATION
============
Reference the package of the machine the frontend is BUILT on - not of the
machine the application runs on:

    dotnet add package CodeBrix.WebPlatform.Styling.Tailwind4.LinuxX64.MitLicenseForever

    Build machine                Package id
    ---------------------------  ------------------------------------------------------------
    Linux x64                    CodeBrix.WebPlatform.Styling.Tailwind4.LinuxX64.MitLicenseForever
    Linux Arm64                  CodeBrix.WebPlatform.Styling.Tailwind4.LinuxArm64.MitLicenseForever
    macOS x64 (Intel)            CodeBrix.WebPlatform.Styling.Tailwind4.MacOSX64.MitLicenseForever
    macOS Arm64 (Apple Silicon)  CodeBrix.WebPlatform.Styling.Tailwind4.MacOSArm64.MitLicenseForever
    Windows x64                  CodeBrix.WebPlatform.Styling.Tailwind4.WindowsX64.MitLicenseForever

These five are the only platforms the standalone Tailwind CSS program is
released for: there is no Windows Arm64, musl or other package.

A build program that is built on several kinds of machine can choose the
package with an MSBuild condition on the runtime identifier of the .NET SDK
doing the build, for example:

    <ItemGroup Condition="'$(NETCoreSdkRuntimeIdentifier)' == 'linux-x64'">
      <PackageReference Include="CodeBrix.WebPlatform.Styling.Tailwind4.LinuxX64.MitLicenseForever" Version="..." />
    </ItemGroup>
    <ItemGroup Condition="'$(NETCoreSdkRuntimeIdentifier)' == 'win-x64'">
      <PackageReference Include="CodeBrix.WebPlatform.Styling.Tailwind4.WindowsX64.MitLicenseForever" Version="..." />
    </ItemGroup>

Dependencies: CodeBrix.WebPlatform.Styling.Tailwind4.Sources (one exact
version, brought in by NuGet). Nothing else.

Licence: the packages are MIT licensed. The program they carry contains code
under other licences too; see THIRD-PARTY-NOTICES-Tailwind.txt beside the
program. The .Sources package's licence file is SOURCES-LICENSES.txt.


KEY NAMESPACES / USINGS
=======================
    using CodeBrix.WebPlatform.Styling.Tailwind4.LinuxX64;    // TailwindTool for linux-x64
    using CodeBrix.WebPlatform.Styling.Tailwind4.LinuxArm64;  // TailwindTool for linux-arm64
    using CodeBrix.WebPlatform.Styling.Tailwind4.MacOSX64;    // TailwindTool for osx-x64
    using CodeBrix.WebPlatform.Styling.Tailwind4.MacOSArm64;  // TailwindTool for osx-arm64
    using CodeBrix.WebPlatform.Styling.Tailwind4.WindowsX64;  // TailwindTool for win-x64
    using CodeBrix.WebPlatform.Styling.Tailwind4.Sources;     // TailwindSources, SourceArchive

Every platform namespace declares a class named TailwindTool; when a project
references more than one platform package, qualify the name (or use an alias).


CORE API REFERENCE
==================

TailwindTool (static class, one per platform package)
-----------------------------------------------------
Constants:
    string TailwindVersion      the Tailwind CSS release the program comes from
    string RuntimeIdentifier    "linux-x64", "linux-arm64", "osx-x64", "osx-arm64" or "win-x64"
    string ToolsFolderName      "tailwind"
    string ExecutableFileName   "tailwindcss" ("tailwindcss.exe" in the WindowsX64 package)
    string ReleaseFileName      the release file's name, e.g. "tailwindcss-linux-x64"
    string ReleaseUrl           where the release file was downloaded from
    string Sha256               the program's sha256, lower-case hex

Methods:
    string GetToolDirectory(string applicationBaseDirectory)
        {applicationBaseDirectory}/tailwind/<rid>. Throws ArgumentException for a
        null or blank argument.
    string GetToolDirectory()
        The same below AppContext.BaseDirectory.
    string GetExecutablePath(string applicationBaseDirectory)
        {applicationBaseDirectory}/tailwind/<rid>/<ExecutableFileName>. Throws
        ArgumentException for a null or blank argument. The file exists when the
        package was referenced and the build copied it.
    string GetExecutablePath()
        The same below AppContext.BaseDirectory.

TailwindSources (static class, .Sources package)
------------------------------------------------
    string TailwindVersion               the Tailwind CSS release the archives are the source of
    string PackageFolder                 the folder inside the package holding the archives
                                         and SOURCES-README.txt ("sources/v" + TailwindVersion)
    SourceArchive Tailwind               Tailwind CSS's own source
    SourceArchive Bun                    the Bun runtime's source
    SourceArchive WebKit                 JavaScriptCore, WTF and bmalloc from Bun's WebKit fork
    SourceArchive TinyCC                 Bun's TinyCC fork
    IReadOnlyList<SourceArchive> All     the four, in that order

SourceArchive (sealed record)
    string FileName     the archive's file name inside PackageFolder
    string Sha256       its sha256, lower-case hex
    string Repository   the repository the source was taken from
    string Revision     the git tag or commit it was taken at

The archives are not copied into a build; they stay in the NuGet package folder,
e.g. ~/.nuget/packages/codebrix.webplatform.styling.tailwind4.sources/<version>/sources/.

The program
-----------
The program is the standalone Tailwind CSS command line. The usual call, run in
the frontend folder:

    tailwindcss -i <input stylesheet> -o <output stylesheet> [--minify] [--watch]

`tailwindcss --help` prints the release (for example "tailwindcss v..." in its
first line) and every option.


COMPLETE EXAMPLES
=================

Run the packaged program from a build program
---------------------------------------------
    using System;
    using System.Diagnostics;
    using System.IO;
    using CodeBrix.WebPlatform.Styling.Tailwind4.LinuxX64;

    var program = TailwindTool.GetExecutablePath();
    if (!File.Exists(program))
    {
        throw new InvalidOperationException($"{program} is missing - is the package referenced?");
    }

    if (!OperatingSystem.IsWindows())
    {
        File.SetUnixFileMode(program, File.GetUnixFileMode(program) | UnixFileMode.UserExecute);
    }

    var start = new ProcessStartInfo(program) { WorkingDirectory = "frontend", UseShellExecute = false };
    start.ArgumentList.Add("-i");
    start.ArgumentList.Add("tailwind.css");
    start.ArgumentList.Add("-o");
    start.ArgumentList.Add(Path.Combine("assets", "tailwind.css"));
    using var process = Process.Start(start);
    process.WaitForExit();
    Console.WriteLine(process.ExitCode == 0 ? "stylesheet written" : "tailwindcss failed");

Put the program first on PATH for another tool
----------------------------------------------
A tool that looks for `tailwindcss` on PATH (such as the Dioxus CLI built
without downloads) finds the packaged one when its folder comes first:

    var start = new ProcessStartInfo("dx");
    start.Environment["PATH"] = TailwindTool.GetToolDirectory() + Path.PathSeparator
                                + Environment.GetEnvironmentVariable("PATH");

Check that the copied program is the packaged one
-------------------------------------------------
    using System.IO;
    using System.Security.Cryptography;
    using CodeBrix.WebPlatform.Styling.Tailwind4.LinuxX64;

    using var stream = File.OpenRead(TailwindTool.GetExecutablePath());
    var ok = Convert.ToHexStringLower(SHA256.HashData(stream)) == TailwindTool.Sha256;

List the source archives
------------------------
    using CodeBrix.WebPlatform.Styling.Tailwind4.Sources;

    foreach (var archive in TailwindSources.All)
    {
        Console.WriteLine($"{TailwindSources.PackageFolder}/{archive.FileName}  {archive.Repository} @ {archive.Revision}");
    }


MINIMUM VIABLE PROJECT TEMPLATE
===============================
A console build program for a Linux x64 build machine:

    <Project Sdk="Microsoft.NET.Sdk">
      <PropertyGroup>
        <OutputType>Exe</OutputType>
        <TargetFramework>net10.0</TargetFramework>
      </PropertyGroup>
      <ItemGroup>
        <PackageReference Include="CodeBrix.WebPlatform.Styling.Tailwind4.LinuxX64.MitLicenseForever" Version="..." />
      </ItemGroup>
    </Project>

Program.cs:

    using System;
    using CodeBrix.WebPlatform.Styling.Tailwind4.LinuxX64;

    Console.WriteLine($"Tailwind {TailwindTool.TailwindVersion}: {TailwindTool.GetExecutablePath()}");


PERFORMANCE TIPS
================
- Each platform package is large (the program is a single executable of around
  a hundred megabytes). Reference only the package(s) of the build machines you
  use; referencing all five copies all five programs into the output folder.
- The program is copied with PreserveNewest, so only the first build after a
  restore copies it.
- `tailwindcss --watch` keeps one process running and rebuilds on change, which
  is much faster than starting the program for every change.


COMMON PITFALLS TO AVOID
========================
- Referencing the package of the machine the application RUNS on instead of
  the machine the frontend is BUILT on. A Windows developer building a site that
  is deployed to Linux needs the WindowsX64 package.
- Running the program without making it executable first on Linux or macOS.
  NuGet records no file permissions; set UserExecute (as in the examples) before
  starting it.
- Looking for the program next to the assembly in the NuGet cache. Use
  TailwindTool.GetExecutablePath(): the build copies the program into the
  output folder, below tailwind/<rid>/.
- Expecting `tailwindcss` on PATH. The packages put nothing on PATH; put
  TailwindTool.GetToolDirectory() first on the PATH of the process that needs it.
- Expecting a Windows Arm64, Linux musl, FreeBSD or Android package. Tailwind
  releases the standalone program for the five platforms above only.
- Deleting THIRD-PARTY-NOTICES-Tailwind.txt or LICENSE-Tailwind.txt from a
  distribution of the program: the licences of the code inside it require the
  notices to travel with it.


WHAT THIS PACKAGE DOES NOT DO
=============================
- It does not build Tailwind: the programs are the files Tailwind Labs
  releases, unchanged.
- It does not run Tailwind for you; it ships the program and says where it is.
  CodeBrix.WebPlatform.FrontendBuilder (or your own code) runs it.
- It does not ship Tailwind's npm packages, plugins or a Node.js runtime.
- The .Sources package does not build anything; it only carries source archives.


WORKING EXAMPLES ON GITHUB
==========================
https://github.com/ellisnet/CodeBrix.WebPlatform.Styling/tree/main/tests/CodeBrix.WebPlatform.Styling.Tests
    TailwindToolTests.cs         every TailwindTool constant and method, all five packages
    HostProgram.cs               finding, preparing and running the program; building a stylesheet
    OutputFolder.cs              what a referencing project gets in its output folder
    PlatformPackageContents.cs   what each platform package contains and depends on
    TailwindSourcesTests.cs      TailwindSources and the archives
    SourcesPackageContents.cs    what the .Sources package contains


QUICK REFERENCE CARD
====================
    Package (build machine)   CodeBrix.WebPlatform.Styling.Tailwind4.<Platform>.MitLicenseForever
    Platforms                 LinuxX64, LinuxArm64, MacOSX64, MacOSArm64, WindowsX64
    Output                    {output}/tailwind/<rid>/tailwindcss[.exe]
                              + LICENSE-Tailwind.txt, THIRD-PARTY-NOTICES-Tailwind.txt, version.txt
    Find it                   TailwindTool.GetExecutablePath()   TailwindTool.GetToolDirectory()
    Facts                     TailwindTool.TailwindVersion / RuntimeIdentifier / ExecutableFileName
                              / ReleaseFileName / ReleaseUrl / Sha256
    Before running (Unix)     File.SetUnixFileMode(path, mode | UnixFileMode.UserExecute)
    Source                    CodeBrix.WebPlatform.Styling.Tailwind4.Sources
                              TailwindSources.All -> SourceArchive(FileName, Sha256, Repository, Revision)

================================================================================
