using System;
using System.Diagnostics;
using System.IO;

namespace Mjcheetham.WebLinks
{
    internal static class Git
    {
        public static string GetRepositoryUrl(string repositoryPath)
        {
            string originUrl = ExecuteGitCommand(repositoryPath, "config --local remote.origin.url").TrimEnd(null);

            if (originUrl.EndsWith(".git"))
            {
                return originUrl.Substring(0, originUrl.Length - 4);
            }

            return originUrl;
        }

        public static string GetCurrentRepositoryVersion(string repositoryPath, bool resolveRef)
        {
            string cmd = resolveRef
                       ? "rev-parse --abbrev-ref HEAD"
                       : "rev-parse HEAD";

            return ExecuteGitCommand(repositoryPath, cmd).TrimEnd(null);
        }

        public static string GetRepositoryPath(string filePath)
        {
            string parentPath = filePath;
            while ((parentPath = Path.GetDirectoryName(parentPath)) != null)
            {
                string gitDir = Path.Combine(parentPath, ".git");
                if (Directory.Exists(gitDir) || File.Exists(gitDir))
                {
                    break;
                }
            }

            return parentPath;
        }

        private static string ExecuteGitCommand(string repositoryPath, string command)
        {
            string gitExe = GetGitExe();

            string cmdline = $"--git-dir=\"{repositoryPath}\\.git\" {command}";

            var process = Process.Start(new ProcessStartInfo(gitExe, cmdline)
            {
                RedirectStandardOutput = true,
                RedirectStandardError  = true,
                RedirectStandardInput  = true,
                UseShellExecute        = false,
                CreateNoWindow         = true,
                WindowStyle            = ProcessWindowStyle.Hidden
            });

            if (process == null)
            {
                throw new Exception($"Failed to start git.exe with command-line \"{command}\"");
            }

            using (process)
            {
                process.WaitForExit(5000);

                if (!process.HasExited)
                {
                    throw new Exception("git.exe took longer than usual to respond");
                }

                if (process.ExitCode != 0)
                {
                    throw new Exception($"git.exe exited with error code: {process.ExitCode}");
                }

                string stdError = process.StandardError.ReadToEnd();
                if (!string.IsNullOrWhiteSpace(stdError))
                {
                    throw new Exception($"git.exe wrote to stderr:{Environment.NewLine}{stdError}");
                }

                return process.StandardOutput.ReadToEnd();
            }
        }

        private static string GetGitExe()
        {
            if (FindApplication("git", out string installation))
            {
                return installation;
            }

            throw new Exception("Failed to find git.exe");
        }

        public static bool FindApplication(string name, out string path)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                string pathext = Environment.GetEnvironmentVariable("PATHEXT");
                string envpath = Environment.GetEnvironmentVariable("PATH");

                // Combine %PATH% and %PATHEXT% to find the first match
                if (!string.IsNullOrEmpty(pathext) && !string.IsNullOrEmpty(envpath))
                {
                    string[] splitChars = {";"};

                    string[] exts  = pathext.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);
                    string[] paths = envpath.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);

                    for (int i = 0; i < paths.Length; i++)
                    {
                        if (paths[i] == null)
                            continue;

                        for (int j = 0; j < exts.Length; j++)
                        {
                            if (exts[j] == null)
                                continue;

                            // Concatenate the path together (value = $"{paths[0]}\\{name}{exts[j]}")
                            // then validate that it actually exists. If it does, it's "the one" otherwise
                            // keep trying until success or all potential values are exhausted.
                            string value = string.Concat(paths[i], Path.DirectorySeparatorChar, name, exts[j]);
                            if (File.Exists(value))
                            {
                                path = value;
                                return true;
                            }
                        }
                    }
                }
            }

            path = null;
            return false;
        }
    }
}
