using System.Collections.Generic;

namespace CodeBrix.WebPlatform.Styling.Tailwind4.Sources;

/// <summary>
/// Describes the source archives this package carries under <c>sources/v4.3.5/</c> in the
/// package: the source code of the prebuilt Tailwind CSS programs that the
/// CodeBrix.WebPlatform.Styling.Tailwind4 platform packages ship. <c>SOURCES-README.txt</c>,
/// beside the archives, says exactly what each archive holds and how it was made.
/// </summary>
/// <remarks>
/// Nothing from this package is copied into a consuming build: the archives stay in the NuGet
/// package folder (for example <c>~/.nuget/packages/codebrix.webplatform.styling.tailwind4.sources/&lt;version&gt;/sources/</c>).
/// </remarks>
public static class TailwindSources
{
    /// <summary>The Tailwind CSS release whose programs these archives are the source of.</summary>
    public const string TailwindVersion = "4.3.5";

    /// <summary>The folder inside the package that holds the archives and <c>SOURCES-README.txt</c>.</summary>
    public const string PackageFolder = "sources/v4.3.5";

    /// <summary>Tailwind CSS 4.3.5: the release's source (git tag <c>v4.3.5</c>).</summary>
    public static SourceArchive Tailwind { get; } = new SourceArchive(
        "tailwindcss-4.3.5.zip",
        "bb27af0df1b87280e62bff9bd71c1cdbd4a2a466d037f85c86e7914e65a3b687",
        "https://github.com/tailwindlabs/tailwindcss",
        "v4.3.5");

    /// <summary>The Bun runtime the programs are compiled with (git tag <c>bun-v1.4.3</c>), without its tests, documentation and benchmarks.</summary>
    public static SourceArchive Bun { get; } = new SourceArchive(
        "oven-sh-bun-v1.4.3.zip",
        "530261669eaa7061cd4c1c69ba2beccd44f271ea7ca99cbc19c7f50589d09935",
        "https://github.com/oven-sh/bun",
        "bun-v1.4.3");

    /// <summary>The parts of Bun's WebKit fork that Bun's JSCOnly build compiles: JavaScriptCore, WTF, bmalloc and their build files.</summary>
    public static SourceArchive WebKit { get; } = new SourceArchive(
        "oven-sh-WebKit-0c06faad-JSCOnly.zip",
        "495ecba149b6a436f1eb92f5cb06713c1dc5eafd941526fde7fa0de422158e47",
        "https://github.com/oven-sh/WebKit",
        "0c06faadf65bf8e8c8ad3a5a8aca83e1e9ed653f");

    /// <summary>Bun's TinyCC fork.</summary>
    public static SourceArchive TinyCC { get; } = new SourceArchive(
        "oven-sh-tinycc-05f0fafa.zip",
        "f141c6ad0b823bed68d21fa6b104e4a5df9de8eaecd1e2c3b5abbee24eff40be",
        "https://github.com/oven-sh/tinycc",
        "05f0fafaa3be31e31d7b4b5c17dc60f62c991171");

    /// <summary>Every archive the package carries, in the order <c>SOURCES-README.txt</c> lists them.</summary>
    public static IReadOnlyList<SourceArchive> All { get; } = new[] { Tailwind, Bun, WebKit, TinyCC };
}
