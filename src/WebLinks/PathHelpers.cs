using System;
using System.IO;

namespace Mjcheetham.WebLinks
{
    public static class PathHelpers
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

        public static string GetCorrectlyCasedPath(string path)
        {
            if (!(File.Exists(path) || Directory.Exists(path)))
            {
                return path;
            }

            var di = new DirectoryInfo(path);

            if (di.Parent != null)
            {
                return Path.Combine(
                    GetCorrectlyCasedPath(di.Parent.FullName),
                    di.Parent.GetFileSystemInfos(di.Name)[0].Name);
            }
            else
            {
                return di.Name.ToUpper();
            }
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
