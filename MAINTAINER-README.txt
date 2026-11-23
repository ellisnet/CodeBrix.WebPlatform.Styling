================================================================================
MAINTAINER-README: CodeBrix.WebPlatform.Styling
Notes for people and agents MAINTAINING this repository - not for package
consumers
================================================================================

If you are CONSUMING the NuGet packages, stop reading and open AGENT-README.txt
instead. Everything below is about the repository itself: how it is laid out,
how it builds, how it is tested, how it is packaged, and the conventions the
source follows.


PURPOSE AND SCOPE
=================
This repository produces six NuGet packages:

    CodeBrix.WebPlatform.Styling.Tailwind4.LinuxX64.MitLicenseForever
    CodeBrix.WebPlatform.Styling.Tailwind4.LinuxArm64.MitLicenseForever
    CodeBrix.WebPlatform.Styling.Tailwind4.MacOSX64.MitLicenseForever
    CodeBrix.WebPlatform.Styling.Tailwind4.MacOSArm64.MitLicenseForever
    CodeBrix.WebPlatform.Styling.Tailwind4.WindowsX64.MitLicenseForever
        the prebuilt standalone Tailwind CSS program for one build machine
        (assembly = namespace = the package id without .MitLicenseForever)
    CodeBrix.WebPlatform.Styling.Tailwind4.Sources
        the source of those programs (no licence suffix: its archives are under
        several licences; its licence metadata is the file
        SOURCES-LICENSES.txt)

    License:    MIT (LICENSE) for this repository's own files
    Consumer documentation: AGENT-README.txt (one file for all six)

Tailwind is NOT built here. The platform packages carry the files Tailwind
Labs releases, unchanged; the build downloads them and checks them against
fixed sha256 hashes. The five platforms are exactly the ones the standalone
Tailwind program is released for, and the ones CodeBrix.WebPlatform supports
as build machines.

Every platform package depends on the .Sources package at ONE EXACT version
(the one built beside it), so the source always comes with the program - see
LICENCE OBLIGATIONS below for why.


REPOSITORY LAYOUT
=================
    src/CodeBrix.WebPlatform.Styling.Tailwind4.<Platform>/   (five of them)
        <name>.csproj            the release file name, URL, sha256 and program
                                 name for the platform, the package metadata,
                                 the reference to the .Sources project, and the
                                 import of the shared targets file below
        TailwindTool.cs          the same values as public constants, and the
                                 lookup methods
        InternalsVisibleTo.cs
    src/CodeBrix.WebPlatform.Styling.Tailwind4.Sources/
        <name>.csproj            the four source archives with their sha256
                                 hashes, the hash check, and the packing
        TailwindSources.cs, SourceArchive.cs, InternalsVisibleTo.cs
    src/CodeBrix.WebPlatform.Styling.Tailwind4.Platform.targets
                                 shared by the five platform projects: the
                                 download, the hash check, the rename, the
                                 packing and the exact .Sources dependency
    src/CodeBrix.WebPlatform.Styling.Tailwind4.buildTransitive.targets
                                 packed into each platform package as
                                 buildTransitive/<package id>.targets; copies
                                 tools/tailwind/<rid>/ into the consumer's
                                 output folder as tailwind/<rid>/
    src/notices/LICENSE-Tailwind.txt
                                 Tailwind's own LICENSE, verbatim from tag v4.3.5
    src/notices/THIRD-PARTY-NOTICES-Tailwind.txt
                                 everything inside the program, with licence
                                 texts, and where its source is; ships beside
                                 the program in every platform package
    info/v4.3.5/                 COMMITTED reference material and source:
        urls.txt                 the release files and their sha256 values
        sha256sums.txt           the release's own checksum file
        tailwindcss-4.3.5.zip, oven-sh-bun-v1.4.3.zip,
        oven-sh-WebKit-0c06faad-JSCOnly.zip, oven-sh-tinycc-05f0fafa.zip
                                 the source archives the .Sources package
                                 carries
        SOURCES-README.txt       what each archive holds, its sha256 and how it
                                 was made
        SOURCES-LICENSES.txt     the .Sources package's licence file
    Downloaded/v4.3.5/           NOT committed (git-ignored): the five release
                                 files, downloaded by the build
    tools/source-zips/MakeSourceZip.cs
                                 the .NET file-based program that makes the
                                 deterministic source zips (SOURCES-README.txt)
    tests/CodeBrix.WebPlatform.Styling.Tests/
                                 the xUnit v3 test project
    CodeBrix.WebPlatform.Styling.slnx
                                 the solution. Solution Items: .gitignore,
                                 AGENT-README.txt, EXTRAS-README.txt,
                                 global.json, icon-codebrix-128.png, LICENSE,
                                 MAINTAINER-README.txt, README-INDEX.txt,
                                 README.md, THIRD-PARTY-NOTICES.txt; a Tests
                                 folder with the test project
    global.json                  pins the .NET SDK to 10.0.404 with roll
                                 forward within 10.0 (nothing here targets
                                 Android), and selects the
                                 Microsoft.Testing.Platform test runner
    .gitattributes               LF for every text file on every platform;
                                 zips and other binaries never touched
    the eight AI-assistant pointer files (AGENTS.md, CLAUDE.md, .clinerules,
    .cursorrules, .cursor/rules/agent-readme.mdc, .windsurfrules,
    .github/copilot-instructions.md, .junie/guidelines.md)
                                 byte-for-byte the family's canonical copies.
                                 Never edit.


BUILDING
========
    dotnet build --configuration Release CodeBrix.WebPlatform.Styling.slnx

The build must end with 0 warnings and 0 errors. GenerateDocumentationFile is
on, so a CS1591 is fixed by writing the doc comment, never with NoWarn.

Needs internet access the FIRST time: each platform project downloads its
release file (about 85 to 110 MB each, some 500 MB for all five) into
Downloaded/v4.3.5/. Later builds reuse the files and only check their hashes.
Nothing else is downloaded; NuGet restores only the test packages.

What each platform project does before it builds (PrepareTailwindProgram in
the shared targets file):
  1. creates Downloaded/v4.3.5/ when it does not exist;
  2. downloads its release file when it is missing - to "<name>.partial"
     first; the partial file is hash-checked and only then renamed, and a
     mismatching download is deleted and stops the build;
  3. checks the sha256 of the file in Downloaded/v4.3.5/, just downloaded or
     not, and stops the build on a mismatch ("Delete the file - the next build
     downloads it again");
  4. copies it, renamed to tailwindcss (tailwindcss.exe), with a version.txt,
     into obj/<Configuration>/net10.0/tailwind/<rid>/ - from where it is packed
     and copied to the project's own output folder.
Any other file in Downloaded/ is ignored.

The .Sources project checks the four committed archives in info/v4.3.5/
against their hashes before it builds (CheckTailwindSourceArchives).


TESTING
=======
    dotnet test --solution CodeBrix.WebPlatform.Styling.slnx

xUnit v3 with SilverAssertions. The test runner is Microsoft.Testing.Platform,
selected by global.json - do not delete it: without it `dotnet test` falls back
to the VSTest bridge, which fails on the .NET 10 SDK. If `dotnet test
--solution` ever reports zero tests, run the test assembly directly:

    dotnet tests/CodeBrix.WebPlatform.Styling.Tests/bin/Debug/net10.0/CodeBrix.WebPlatform.Styling.Tests.dll

No coverage collector is referenced.

The tests read the built packages (the newest .nupkg in each project's
bin/<Configuration>/), so build the solution - in the same configuration -
before testing. They check: the constants against the build values; the
hashes against the release's sha256sums.txt; the downloaded files; the
output-folder copy; every package's exact file list, licence metadata and
.Sources dependency; the source archives against TailwindSources and
SOURCES-README.txt. HostProgram.cs runs the program of the machine the tests
run on (`--help`, and a real stylesheet build); the other four programs are
never run.

The test project references all five platform projects, so its output folder
receives all five programs (about 500 MB, copied once).


PACKAGING / PUBLISHING
======================
Every project packs on build (GeneratePackageOnBuild), into
src/<project>/bin/<Configuration>/. All six packages can be built on ANY of
the five platforms - nothing is compiled for a platform - and Jeremy builds and
publishes them from his Windows x64 machine:

    git clone https://github.com/ellisnet/CodeBrix.WebPlatform.Styling.git
    cd CodeBrix.WebPlatform.Styling
    dotnet build --configuration Release CodeBrix.WebPlatform.Styling.slnx
    dotnet test --solution CodeBrix.WebPlatform.Styling.slnx --configuration Release

and then publishes the six newest .nupkg files of bin/Release/ together - the
.Sources package FIRST (or in the same push), because the platform packages
depend on that exact version.

Versions: every packable csproj carries the family's canonical date-stamped
version block (1.<years since 2026>.<day of year>.<minute of day>, UTC). The
platform packages depend on the .Sources package at exactly the version built
beside them ([version]); PinTailwindSourcesVersion in the shared targets turns
NuGet's usual "this version or later" into that exact range. NuGet reads that
version by evaluating the .Sources project again, so a build that crosses a
UTC minute boundary could name a version that was never packed: the build then
STOPS ("... the build crossed a minute boundary. Build again.") rather than
pack a package whose dependency cannot be found. Note that NuGet writes a
version whose last part is 0 without it (1.0.327.0 is 1.0.327).

Before publishing:
  1. the build and the tests are green (0 warnings);
  2. all six .nupkg files of the same build are there (same version, or the
     platform packages' dependency names the .Sources version);
  3. THIRD-PARTY-NOTICES-Tailwind.txt is the complete file (not a draft).

Never unlist a .Sources version while a platform package that depends on it is
still listed: the source has to stay available for as long as the programs
are.

The package contents:
    platform package
        tools/tailwind/<rid>/tailwindcss[.exe], LICENSE-Tailwind.txt,
        THIRD-PARTY-NOTICES-Tailwind.txt, version.txt
        buildTransitive/<package id>.targets
        lib/net10.0/<assembly>.dll and .xml
        README.md, AGENT-README.txt, THIRD-PARTY-NOTICES.txt, icon-codebrix-128.png
    .Sources package
        sources/v4.3.5/ (the four archives and SOURCES-README.txt)
        SOURCES-LICENSES.txt (the licence file)
        lib/net10.0/CodeBrix.WebPlatform.Styling.Tailwind4.Sources.dll and .xml
        README.md, AGENT-README.txt, THIRD-PARTY-NOTICES.txt, icon-codebrix-128.png

The programs are NOT packed under runtimes/<rid>/native (that convention is for
libraries a process loads, and NuGet merges and flattens that folder); like
the CodeBrix.WebPlatform.FrontendBuilder platform packages, each program has a
folder of its own at a fixed place.


LICENCE OBLIGATIONS
===================
The programs are `bun build --compile` executables: the Bun runtime with
Tailwind embedded. Bun statically links JavaScriptCore, WTF and bmalloc from
Bun's WebKit fork and TinyCC from Bun's TinyCC fork (LGPL), and Bun's own
source contains LGPL files copied from WebKit's WebCore; the programs also
embed Lightning CSS (MPL-2.0). One compiled-in WebKit file,
Source/WTF/wtf/text/Base64.cpp, is licensed under the LGPL version 2 ONLY, so
version 2's terms apply: the complete source of the library, and the "work that
uses the Library" so that a user can relink, must accompany the programs (or
be offered from the same place). That is why:
  - the source archives are committed in info/v4.3.5/ and published in the
    .Sources package on nuget.org, beside the programs;
  - every platform package depends on the .Sources package at one exact
    version;
  - THIRD-PARTY-NOTICES-Tailwind.txt travels beside each program and lists
    every bundled component, its licence and where its source is.
Included: the LGPL library source (the WebKit parts Bun's build compiles,
TinyCC, Bun's LGPL files), and the relinking material: all of Bun's source
(without tests, documentation and benchmarks - they are in nothing that is
distributed) and Tailwind's source. Not included: the permissively licensed
libraries Bun's build fetches itself (Bun's scripts/build/deps/*.ts name them
with their commits).


UPDATING TO A NEW TAILWIND RELEASE
==================================
A new release is a new info/v<version>/ folder and a rebuild of all six
packages - never of one alone.
  1. Find the release's Bun version (packages/@tailwindcss-standalone/
     package.json and pnpm-lock.yaml at the tag; `strings` on a program shows
     "Bun v..."), then the WebKit and TinyCC commits that Bun release pins
     (scripts/build/deps/webkit.ts and tinycc.ts).
  2. Re-check the licences: scan the LGPL files of the WebKit parts, TinyCC and
     Bun (the LGPL-2-only Base64.cpp may have changed), and rebuild the notices
     inventory (Bun's LICENSE.md, Tailwind's bundled packages and the crates
     of its native add-ons).
  3. Make info/v<version>/: urls.txt, sha256sums.txt, the Tailwind source zip
     (GitHub's tag zip), the three source zips with MakeSourceZip.cs, and
     SOURCES-README.txt and SOURCES-LICENSES.txt.
  4. Change TailwindVersion (the shared targets file and the .Sources csproj),
     each platform csproj's release URL and sha256, the .Sources archive
     list and hashes, and the TailwindTool and TailwindSources constants (the
     tests compare them).
  5. Replace src/notices/LICENSE-Tailwind.txt and
     THIRD-PARTY-NOTICES-Tailwind.txt.
The package ids keep Tailwind4 for every 4.x release; a Tailwind 5 would be
new packages (Tailwind5).


CODING CONVENTIONS
==================
The CodeBrix family conventions: one type per file; usings at the top
(System.* first), file-scoped namespaces, one blank line between header,
usings, namespace and the first type; no `?` on reference types and no `!`;
no global usings, no ImplicitUsings, no Nullable, no NoWarn; XML doc comments
on every public member. Tests: <ClassUnderTest>Tests.cs for one class (here
TailwindToolTests, TailwindSourcesTests), descriptive names for files that
check several things together (HostProgram, OutputFolder,
PlatformPackageContents, SourcesPackageContents, DownloadedPrograms); method
names Member_snake_case or plain snake_case; //Arrange //Act //Assert in
multi-statement tests, expression bodies for one-statement tests; every
cancellable call gets TestContext.Current.CancellationToken.


PROVENANCE / VENDORED SOURCES
=============================
No third-party code is compiled into the assemblies. Third-party material in
the repository and the packages: Tailwind's LICENSE (src/notices), the
release's sha256sums.txt and the four source archives (info/v4.3.5), and - in
the packages, not the repository - the downloaded Tailwind programs.
THIRD-PARTY-NOTICES.txt at the root describes each;
src/notices/THIRD-PARTY-NOTICES-Tailwind.txt lists what is inside the programs.


NOTES
=====
- A NuGet package records no file permissions. On Linux and macOS the program
  comes out of the package without a guaranteed executable bit; whoever runs it
  sets UserExecute first (the tests and AGENT-README.txt show how).
- The download uses MSBuild's DownloadFile and GetFileHash tasks, so it works
  the same on Windows, Linux and macOS without any script or extra tool.
- If an anti-virus program on Windows holds a freshly downloaded file, the
  rename of the .partial file fails and the build stops with that error; build
  again.

================================================================================
