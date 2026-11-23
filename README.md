# CodeBrix.WebPlatform.Styling

The prebuilt standalone Tailwind CSS program, packaged for building CodeBrix.WebPlatform applications - so a build machine needs no Tailwind installed, and no Node.js.
CodeBrix.WebPlatform.Styling is provided as five per-platform NuGet packages, `CodeBrix.WebPlatform.Styling.Tailwind4.<Platform>.MitLicenseForever`, and the `CodeBrix.WebPlatform.Styling.Tailwind4.Sources` package they depend on.

CodeBrix.WebPlatform.Styling supports applications and assemblies that target Microsoft .NET version 10.0 and later.
Microsoft .NET version 10.0 is a Long-Term Supported (LTS) version of .NET, and was released on Nov 11, 2025; and will be actively supported by Microsoft until Nov 14, 2028.
Please update your C#/.NET code and projects to the latest LTS version of Microsoft .NET.

## Installation

Add the package of the machine the frontend is BUILT on:

```
dotnet add package CodeBrix.WebPlatform.Styling.Tailwind4.LinuxX64.MitLicenseForever
```

| Build machine | NuGet package ID | Assembly and namespace |
|---|---|---|
| Linux x64 | `CodeBrix.WebPlatform.Styling.Tailwind4.LinuxX64.MitLicenseForever` | `CodeBrix.WebPlatform.Styling.Tailwind4.LinuxX64` |
| Linux Arm64 | `CodeBrix.WebPlatform.Styling.Tailwind4.LinuxArm64.MitLicenseForever` | `CodeBrix.WebPlatform.Styling.Tailwind4.LinuxArm64` |
| macOS x64 (Intel) | `CodeBrix.WebPlatform.Styling.Tailwind4.MacOSX64.MitLicenseForever` | `CodeBrix.WebPlatform.Styling.Tailwind4.MacOSX64` |
| macOS Arm64 (Apple Silicon) | `CodeBrix.WebPlatform.Styling.Tailwind4.MacOSArm64.MitLicenseForever` | `CodeBrix.WebPlatform.Styling.Tailwind4.MacOSArm64` |
| Windows x64 | `CodeBrix.WebPlatform.Styling.Tailwind4.WindowsX64.MitLicenseForever` | `CodeBrix.WebPlatform.Styling.Tailwind4.WindowsX64` |

These are the only five platforms the standalone Tailwind CSS program is released for. Note that the NuGet package IDs and the namespaces are different - there is no package named plain `CodeBrix.WebPlatform.Styling.Tailwind4.LinuxX64`.

XML documentation (IntelliSense) ships alongside each assembly.

Every platform package depends on `CodeBrix.WebPlatform.Styling.Tailwind4.Sources` at one exact version, which NuGet brings in automatically; it has no other dependencies.

## CodeBrix.WebPlatform.Styling supports:

* The standalone Tailwind CSS v4 program (`tailwindcss`, `tailwindcss.exe` on Windows) for Linux x64, Linux Arm64, macOS x64, macOS Arm64 and Windows x64 build machines - exactly the files Tailwind Labs releases, checked against their published sha256 hashes
* Copying the program into the referencing project's output folder as `tailwind/<rid>/`, with `LICENSE-Tailwind.txt`, `THIRD-PARTY-NOTICES-Tailwind.txt` and a `version.txt` beside it - nothing to install, nothing on PATH
* A small `TailwindTool` class per package naming the program, its runtime identifier, its Tailwind release and its sha256, and finding it below an application's base folder
* The complete source of the programs, including the LGPL-licensed source they contain, in the `.Sources` package

## The source package

`CodeBrix.WebPlatform.Styling.Tailwind4.Sources` carries the source of the programs as zip archives under `sources/<version>/`: Tailwind CSS itself, the Bun runtime the programs are compiled with, and the JavaScriptCore (WebKit) and TinyCC source that the runtime contains. Nothing from it is copied into a build. Its `SOURCES-README.txt` says exactly what each archive holds and how it was made, and its `TailwindSources` class lists the archives and their sha256 hashes.

## Sample Code

### Find and run the packaged program

```csharp
using System;
using System.Diagnostics;
using System.IO;
using CodeBrix.WebPlatform.Styling.Tailwind4.LinuxX64;

var program = TailwindTool.GetExecutablePath();   // {AppContext.BaseDirectory}/tailwind/linux-x64/tailwindcss
if (!OperatingSystem.IsWindows())
{
    // NuGet records no file permissions: make sure the program is executable.
    File.SetUnixFileMode(program, File.GetUnixFileMode(program) | UnixFileMode.UserExecute);
}

var start = new ProcessStartInfo(program) { WorkingDirectory = "frontend" };
start.ArgumentList.Add("-i");
start.ArgumentList.Add("tailwind.css");
start.ArgumentList.Add("-o");
start.ArgumentList.Add(Path.Combine("assets", "tailwind.css"));
using var process = Process.Start(start);
process.WaitForExit();
```

## Documentation

The NuGet packages include `AGENT-README.txt`, a complete API reference and usage guide written for AI coding agents - point your agent at that file when it is writing code against these packages.

Additional sample code and usage examples are available in the `CodeBrix.WebPlatform.Styling.Tests` project:
https://github.com/ellisnet/CodeBrix.WebPlatform.Styling/tree/main/tests/CodeBrix.WebPlatform.Styling.Tests

## License

CodeBrix.WebPlatform.Styling is licensed under the MIT License - see the
[LICENSE](https://github.com/ellisnet/CodeBrix.WebPlatform.Styling/blob/main/LICENSE) file.

For licensing and provenance information about the open source code included in
these packages, see [THIRD-PARTY-NOTICES.txt](https://github.com/ellisnet/CodeBrix.WebPlatform.Styling/blob/main/THIRD-PARTY-NOTICES.txt).
