================================================================================
EXTRAS-README: CodeBrix.WebPlatform.Styling
Samples, tools and other content in this repository that is not part of a NuGet
package
================================================================================

This repository ships no sample applications or demos. Six projects are
packable - the five src/CodeBrix.WebPlatform.Styling.Tailwind4.<Platform>
projects and src/CodeBrix.WebPlatform.Styling.Tailwind4.Sources - and
everything else listed below exists to build, test or document them.

For runnable, compilable usage of the packages, read the test project: the
"WORKING EXAMPLES ON GITHUB" section of AGENT-README.txt maps each feature area
to the test file that exercises it.


TEST PROJECT
============
    tests/CodeBrix.WebPlatform.Styling.Tests/

The only non-package project in the solution. xUnit v3; run it as described in
MAINTAINER-README.txt (TESTING). It references all five platform projects, so
its output folder holds all five Tailwind programs.


TOOLS
=====
    tools/source-zips/MakeSourceZip.cs

A .NET 10 file-based program (`dotnet run tools/source-zips/MakeSourceZip.cs --
...`) that makes one deterministic source zip: sorted entries, one fixed
timestamp, no file attributes, symbolic links listed rather than stored. It
made the Bun, WebKit and TinyCC archives in info/v4.3.5/;
info/v4.3.5/SOURCES-README.txt gives the exact command for each. It is not part
of any package and is not built with the solution.


REFERENCE MATERIAL
==================
    info/v4.3.5/

The Tailwind release's file list and checksums, and the committed source
archives that the .Sources package carries (MAINTAINER-README.txt, REPOSITORY
LAYOUT). Downloaded/ (git-ignored) holds the release files the build
downloads.

================================================================================
