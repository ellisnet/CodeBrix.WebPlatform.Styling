using System;
using System.IO;
using System.Security.Cryptography;

namespace CodeBrix.WebPlatform.Styling.Tests;

internal static class FileHash
{
    public static string Sha256Of(string path)
    {
        using var stream = File.OpenRead(path);
        return Convert.ToHexStringLower(SHA256.HashData(stream));
    }

    public static string Sha256Of(Stream stream) => Convert.ToHexStringLower(SHA256.HashData(stream));
}
