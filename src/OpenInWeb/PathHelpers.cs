using System;
using System.IO;

namespace OpenInWeb
{
    internal static class PathHelpers
    {
        public static string GetRelativePath(string basePath, string filePath)
        {
            if (!basePath.EndsWith("/"))
            {
                basePath += "/";
            }

            var fileUri = new Uri(filePath);
            var baseUri = new Uri(basePath, UriKind.Absolute);
            var relativeUri = baseUri.MakeRelativeUri(fileUri);
            return relativeUri.ToString().Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
        }

        public static string ToUnixPath(string windowsPath)
        {
            return windowsPath.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }

        public static string ToWindowsPath(string unixPath)
        {
            return unixPath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
        }
    }
}
