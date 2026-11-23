#:property TargetFramework=net10.0
#:property ImplicitUsings=disable
#:property Nullable=disable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;

// Makes one deterministic source zip for info/<version>/ (see SOURCES-README.txt there):
//
//   dotnet run tools/source-zips/MakeSourceZip.cs -- --root <folder> --prefix <name>/
//       --time <ISO-8601 instant> --out <zip> --include <path> [--include <path> ...]
//       [--exclude <path> ...]
//
// --root     the folder the source was checked out or extracted into
// --prefix   the folder every entry is placed under inside the zip (ends in '/')
// --time     the timestamp written for EVERY entry (the source commit's date)
// --include  a file or folder below --root to take (folders recursively)
// --exclude  a file or folder below --root to leave out
//
// The same input always gives the same bytes: entries are sorted by their path (ordinal, '/'
// separators), every entry gets the one --time timestamp and no file attributes, and every file
// is compressed at CompressionLevel.SmallestSize by the zlib that .NET itself carries. Symbolic
// links are not stored; each one found is listed on the console so SOURCES-README.txt can name it.

var options = ParseArguments(args);
var root = Path.GetFullPath(options.Root);
var files = new SortedDictionary<string, string>(StringComparer.Ordinal);
var links = new SortedSet<string>(StringComparer.Ordinal);

foreach (var include in options.Includes)
{
    var full = Path.GetFullPath(Path.Combine(root, include));
    if (File.Exists(full))
    {
        AddFile(full);
    }
    else if (Directory.Exists(full))
    {
        AddFolder(full);
    }
    else
    {
        throw new InvalidOperationException($"--include '{include}' does not exist below '{root}'.");
    }
}

var timestamp = DateTimeOffset.Parse(options.Time, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();
Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(options.Output)));
using (var stream = new FileStream(options.Output, FileMode.Create, FileAccess.Write))
using (var zip = new ZipArchive(stream, ZipArchiveMode.Create))
{
    foreach (var pair in files)
    {
        var entry = zip.CreateEntry(options.Prefix + pair.Key, CompressionLevel.SmallestSize);
        entry.LastWriteTime = timestamp;
        entry.ExternalAttributes = 0;
        using var input = File.OpenRead(pair.Value);
        using var output = entry.Open();
        input.CopyTo(output);
    }
}

Console.WriteLine($"{options.Output}: {files.Count} files");
foreach (var link in links)
{
    Console.WriteLine($"  symbolic link not stored: {link}");
}

return 0;

void AddFolder(string folder)
{
    foreach (var path in Directory.EnumerateFileSystemEntries(folder))
    {
        var relative = Relative(path);
        if (IsExcluded(relative))
        {
            continue;
        }

        var info = new FileInfo(path);
        if (info.LinkTarget != null)
        {
            links.Add(relative + " -> " + info.LinkTarget);
        }
        else if (Directory.Exists(path))
        {
            AddFolder(path);
        }
        else
        {
            files[relative] = path;
        }
    }
}

void AddFile(string path)
{
    var relative = Relative(path);
    if (IsExcluded(relative))
    {
        return;
    }

    var info = new FileInfo(path);
    if (info.LinkTarget != null)
    {
        links.Add(relative + " -> " + info.LinkTarget);
        return;
    }

    files[relative] = path;
}

string Relative(string path) => Path.GetRelativePath(root, path).Replace('\\', '/');

bool IsExcluded(string relative)
    => options.Excludes.Any(e => relative == e || relative.StartsWith(e + "/", StringComparison.Ordinal));

static Options ParseArguments(string[] arguments)
{
    var options = new Options();
    for (var i = 0; i < arguments.Length; i++)
    {
        var name = arguments[i];
        if (i + 1 >= arguments.Length)
        {
            throw new ArgumentException($"'{name}' needs a value.");
        }

        var value = arguments[++i];
        switch (name)
        {
            case "--root": options.Root = value; break;
            case "--prefix": options.Prefix = value; break;
            case "--time": options.Time = value; break;
            case "--out": options.Output = value; break;
            case "--include": options.Includes.Add(value.Replace('\\', '/').TrimEnd('/')); break;
            case "--exclude": options.Excludes.Add(value.Replace('\\', '/').TrimEnd('/')); break;
            default: throw new ArgumentException($"Unknown option '{name}'.");
        }
    }

    if (options.Root == null || options.Prefix == null || options.Time == null || options.Output == null || options.Includes.Count == 0)
    {
        throw new ArgumentException("--root, --prefix, --time, --out and at least one --include are required.");
    }

    if (!options.Prefix.EndsWith("/", StringComparison.Ordinal))
    {
        throw new ArgumentException("--prefix must end in '/'.");
    }

    return options;
}

sealed class Options
{
    public string Root { get; set; }
    public string Prefix { get; set; }
    public string Time { get; set; }
    public string Output { get; set; }
    public List<string> Includes { get; } = new List<string>();
    public List<string> Excludes { get; } = new List<string>();
}
