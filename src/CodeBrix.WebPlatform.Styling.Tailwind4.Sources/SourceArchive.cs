namespace CodeBrix.WebPlatform.Styling.Tailwind4.Sources;

/// <summary>One source archive the package carries.</summary>
/// <param name="FileName">The archive's file name inside <see cref="TailwindSources.PackageFolder"/>.</param>
/// <param name="Sha256">The archive's sha256 (lower-case hex), as recorded in <c>SOURCES-README.txt</c>.</param>
/// <param name="Repository">The repository the source was taken from.</param>
/// <param name="Revision">The git tag or commit the source was taken at.</param>
public sealed record SourceArchive(string FileName, string Sha256, string Repository, string Revision);
