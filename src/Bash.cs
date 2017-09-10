using System;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Shell.NET.Util;

namespace Shell.NET
{
    /// <summary>Handles boilerplate for Bash commands and stores output information.</summary>
    public class Bash
    {
        private static readonly bool Linux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        private static readonly bool MacOs = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        private static readonly bool Windows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        private const string SubsystemBash = @"C:\Windows\sysnative\bash.exe";
        private const string CygwinBash = @"C:\cygwin\bin\bash.exe";
        private static readonly string BashPath = FindBash();

        /// <summary>Determines whether bash is running in a native OS (Linux/MacOS).</summary>
        /// <returns>True if in *nix, else false.</returns>
        public static bool Native => Linux || MacOs;

        /// <summary>Determines if using Windows and if Linux subsystem is installed.</summary>
        /// <returns>True if in Windows and bash detected.</returns>
        public static bool Subsystem => Windows && File.Exists(SubsystemBash);

        /// <summary>Determines if using Windows and if Cygwin is installed.</summary>
        /// <returns>True if in Windows and bash detected.</returns>
        public static bool Cygwin => Windows && File.Exists(CygwinBash);

        /// <summary>Stores output of the previous command if redirected.</summary>
        public string Output { get; private set; }

        /// <summary>
        /// Gets an array of the command output split by newline characters if redirected. </summary>
        public string[] Lines => Output?.Split(Environment.NewLine.ToCharArray());

        /// <summary>Stores the exit code of the previous command.</summary>
        public int ExitCode { get; private set; }

        /// <summary>Stores the error message of the previous command if redirected.</summary>
        public string ErrorMsg { get; private set; }

        private static string FindBash()
        {
            if (Native)
                return "bash";
            else if (Subsystem)
                return SubsystemBash;
            else if (Cygwin)
                return CygwinBash;
            else
                throw new NotSupportedException("Neither Linux Subsystem nor Cygwin were detected");
        }

        /// <summary>Execute a new Bash command.</summary>
        /// <param name="input">The command to execute.</param>
        /// <param name="redirect">Print output to terminal if false.</param>
        /// <returns>A `BashResult` containing the command's output information.</returns>
        public BashResult Command(string input, bool redirect = true)
        {
            using (var bash = new Process { StartInfo = BashInfo(redirect) })
            {
                bash.Start();
                bash.StandardInput.WriteLine($"{input}; exit");

                if (redirect)
                {
                    Output = bash.StandardOutput.ReadToEnd()
                        .TrimEnd(Environment.NewLine.ToCharArray());
                    ErrorMsg = bash.StandardError.ReadToEnd()
                        .TrimEnd(Environment.NewLine.ToCharArray());
                }
                else
                {
                    Output = null;
                    ErrorMsg = null;
                }

                bash.WaitForExit();
                ExitCode = bash.ExitCode;
                bash.Close();
            }

            if (redirect)
                return new BashResult(Output, ErrorMsg, ExitCode);
            else
                return new BashResult(null, null, ExitCode);
        }

        private ProcessStartInfo BashInfo(bool redirectOutput)
        {
            return new ProcessStartInfo
            {
                FileName = BashPath,
                RedirectStandardInput = true,
                RedirectStandardOutput = redirectOutput,
                RedirectStandardError = redirectOutput,
                UseShellExecute = false,
                CreateNoWindow = true,
                ErrorDialog = false
            };
        }

        /// <summary>Echo the given string to standard output.</summary>
        /// <param name="input">The string to print.</param>
        /// <param name="redirect">Print output to terminal if false.</param>
        /// <returns>A `BashResult` containing the command's output information.</returns>
        public BashResult Echo(string input, bool redirect = false) =>
            Command($"echo {input}", redirect: redirect);

        /// <summary>Echo the given string to standard output.</summary>
        /// <param name="input">The string to print.</param>
        /// <param name="flags">Optional `echo` arguments.</param>
        /// <param name="redirect">Print output to terminal if false.</param>
        /// <returns>A `BashResult` containing the command's output information.</returns>
        public void Echo(string input, string flags, bool redirect = false) =>
            Command($"echo {flags} {input}", redirect: redirect);

        /// <summary>Echo the given string to standard output.</summary>
        /// <param name="input">The string to print.</param>
        /// <param name="redirect">Print output to terminal if false.</param>
        /// <returns>A `BashResult` containing the command's output information.</returns>
        public BashResult Echo(object input, bool redirect = false) =>
            Command($"echo {input}", redirect: redirect);

        /// <summary>Echo the given string to standard output.</summary>
        /// <param name="input">The string to print.</param>
        /// <param name="flags">Optional `echo` arguments.</param>
        /// <param name="redirect">Print output to terminal if false.</param>
        /// <returns>A `BashResult` containing the command's output information.</returns>
        public BashResult Echo(object input, string flags, bool redirect = false) =>
            Command($"echo {flags} {input}", redirect: redirect);

        /// <summary>Search for `pattern` in each file in `location`.</summary>
        /// <param name="pattern">The pattern to match.</param>
        /// <param name="location">The files or directory to search.</param>
        /// <param name="redirect">Print output to terminal if false.</param>
        /// <returns>A `BashResult` containing the command's output information.</returns>
        public BashResult Grep(string pattern, string location, bool redirect = true) =>
            Command($"grep {pattern} {location}", redirect: redirect);

        /// <summary>Search for `pattern` in each file in `location`.</summary>
        /// <param name="pattern">The pattern to match.</param>
        /// <param name="location">The files or directory to search.</param>
        /// <param name="flags">Optional `grep` arguments.</param>
        /// <param name="redirect">Print output to terminal if false.</param>
        public BashResult Grep(string pattern, string location, string flags, bool redirect = true) =>
            Command($"grep {pattern} {flags} {location}", redirect: redirect);

        /// <summary>List information about files in the current directory.</summary>
        /// <param name="redirect">Print output to terminal if false.</param>
        /// <returns>A `BashResult` containing the command's output information.</returns>
        public BashResult Ls(bool redirect = true) =>
            Command("ls", redirect: redirect);

        /// <summary>List information about files in the current directory.</summary>
        /// <param name="flags">Optional `ls` arguments.</param>
        /// <param name="redirect">Print output to terminal if false.</param>
        /// <returns>A `BashResult` containing the command's output information.</returns>
        public BashResult Ls(string flags, bool redirect = true) =>
            Command($"ls {flags}", redirect: redirect);

        /// <summary>List information about the given files.</summary>
        /// <param name="flags">Optional `ls` arguments.</param>
        /// <param name="files">Files or directory to search.</param>
        /// <param name="redirect">Print output to terminal if false.</param>
        /// <returns>A `BashResult` containing the command's output information.</returns>
        public BashResult Ls(string flags, string files, bool redirect = true) =>
            Command($"ls {flags} {files}", redirect: redirect);

        /// <summary>Move `source` to `directory`.</summary>
        /// <param name="source">The file to be moved.</param>
        /// <param name="directory">The destination directory.</param>
        /// <param name="redirect">Print output to terminal if false.</param>
        /// <returns>A `BashResult` containing the command's output information.</returns>
        public BashResult Mv(string source, string directory, bool redirect = true) =>
            Command($"mv {source} {directory}", redirect: redirect);

        /// <summary>Move `source` to `directory`.</summary>
        /// <param name="source">The file to be moved.</param>
        /// <param name="directory">The destination directory.</param>
        /// <param name="flags">Optional `mv` arguments.</param>
        /// <param name="redirect">Print output to terminal if false.</param>
        /// <returns>A `BashResult` containing the command's output information.</returns>
        public BashResult Mv(string source, string directory, string flags, bool redirect = true) =>
            Command($"mv {flags} {source} {directory}", redirect: redirect);

        /// <summary>Copy `source` to `directory`.</summary>
        /// <param name="source">The file to be copied.</param>
        /// <param name="directory">The destination directory.</param>
        /// <param name="redirect">Print output to terminal if false.</param>
        /// <returns>A `BashResult` containing the command's output information.</returns>
        public BashResult Cp(string source, string directory, bool redirect = true) =>
            Command($"cp {source} {directory}", redirect: redirect);

        /// <summary>Copy `source` to `directory`.</summary>
        /// <param name="source">The file to be copied.</param>
        /// <param name="directory">The destination directory.</param>
        /// <param name="flags">Optional `cp` arguments.</param>
        /// <param name="redirect">Print output to terminal if false.</param>
        /// <returns>A `BashResult` containing the command's output information.</returns>
        public BashResult Cp(string source, string directory, string flags, bool redirect = true) =>
            Command($"cp {flags} {source} {directory}", redirect: redirect);

        /// <summary>Remove or unlink the given file.</summary>
        /// <param name="file">The file(s) to be removed.</param>
        /// <param name="redirect">Print output to terminal if false.</param>
        /// <returns>A `BashResult` containing the command's output information.</returns>
        public BashResult Rm(string file, bool redirect = true) =>
            Command($"rm {file}", redirect: redirect);

        /// <summary>Remove or unlink the given file.</summary>
        /// <param name="file">The file(s) to be removed.</param>
        /// <param name="flags">Optional `rm` arguments.</param>
        /// <param name="redirect">Print output to terminal if false.</param>
        /// <returns>A `BashResult` containing the command's output information.</returns>
        public BashResult Rm(string file, string flags, bool redirect = true) =>
            Command($"rm {flags} {file}", redirect: redirect);

        /// <summary>Concatenate `file` to standard input.</summary>
        /// <param name="file">The source file.</param>
        /// <param name="redirect">Print output to terminal if false.</param>
        /// <returns>A `BashResult` containing the command's output information.</returns>
        public BashResult Cat(string file, bool redirect = true) =>
            Command($"cat {file}", redirect: redirect);

        /// <summary>Concatenate `file` to standard input.</summary>
        /// <param name="file">The source file.</param>
        /// <param name="flags">Optional `cat` arguments.</param>
        /// <param name="redirect">Print output to terminal if false.</param>
        /// <returns>A `BashResult` containing the command's output information.</returns>
        public BashResult Cat(string file, string flags, bool redirect = true) =>
            Command($"cat {flags} {file}", redirect: redirect);
    }
}
