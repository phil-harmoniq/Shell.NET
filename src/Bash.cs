using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Shell.NET.BashUtil;

namespace Shell.NET
{
    /// <summary>
    /// Handles boilerplate for Bash commands and stores output and exit codes</summary>
    public class Bash
    {
        private static readonly bool _linux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        private static readonly bool _macOS = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        private static readonly bool _windows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        private const string _subsystemBash = @"C:\Windows\System32\bash.exe";
        private const string _cygwinBash = @"C:\cygwin\bin\bash.exe";
        private static string _bashPath;

        /// <summary>
        /// Determines whether bash is running in a native OS (Linux/MacOS)</summary>
        /// <returns>True if in *nix, else false</returns>
        public static readonly bool Native = _linux || _macOS;

        /// <summary>
        /// Determines if using Windows and if Linux subsystem is installed
        /// </summary>
        /// <returns>True if in Windows and bash detected</returns>
        public static readonly bool Subsystem = _windows && File.Exists(_subsystemBash);

        /// <summary>
        /// Determines if using Windows and if Cygwin is installed
        /// </summary>
        /// <returns>True if in Windows and bash detected</returns>
        public static readonly bool Cygwin = _windows && File.Exists(_cygwinBash);

        /// <summary>
        /// Stores output of the previous command if redirected</summary>
        public string Output { get; private set; }

        /// <summary>
        /// Stores the exit code of the previous command</summary>
        public int ExitCode { get; private set; }

        /// <summary>
        /// Stores the error message of the previous command if redirected</summary>
        public string ErrorMsg { get; private set; }

        /// <summary>
        /// Validate that Bash exists and instantiate.
        /// </summary>
        public Bash()
        {
            if (Native)
                _bashPath = "bash";
            else if (Subsystem)
                _bashPath = _subsystemBash;
            else if (Cygwin)
                _bashPath = _cygwinBash;
            else
                throw new NotSupportedException("Neither Linux Subsystem nor Cygwin were detected");
        }

        /// <summary>
        /// Execute a new Bash command.</summary>
        /// <param name="input">The command to execute.</param>
        /// <param name="redirect">Redirect StdOut and StdError to internal properties.</param>
        public BashResult Command(string input, bool redirect = true)
        {
            using (var bash = new Process { StartInfo = BashInfo(redirect) })
            {
                bash.Start();
                bash.StandardInput.WriteLine($"{input}; exit");

                if (redirect)
                {
                    ErrorMsg = bash.StandardError.ReadToEnd();
                    Output = bash.StandardOutput
                        .ReadToEnd()
                        .TrimEnd(System.Environment.NewLine
                        .ToCharArray());
                }
                else
                {
                    ErrorMsg = null;
                    Output = null;
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

        /// <summary>
        /// Echo the given string to standard output.</summary>
        /// <param name="input">The string to print.</param>
        public BashResult Echo(string input)
        {
            return Command($"echo \"{input}\"", redirect: false);
        }

        /// <summary>
        /// Echo the given string to standard output.</summary>
        /// <param name="input">The string to print.</param>
        /// <param name="flags">Optional `echo` arguments.</param>
        public BashResult Echo(string input, string flags)
        {
            return Command($"echo {flags} \"{input}\"", redirect: false);
        }

        /// <summary>
        /// Echo the given string to standard output.</summary>
        /// 
        /// <param name="input">The string to print.</param>
        public BashResult Echo(Object input)
        {
            return Command($"echo \"{input.ToString()}\"", redirect: false);
        }

        /// <summary>
        /// Echo the given string to standard output.</summary>
        /// <param name="input">The string to print.</param>
        /// <param name="flags">Optional `echo` arguments.</param>
        public BashResult Echo(Object input, string flags)
        {
            return Command($"echo {flags} \"{input.ToString()}\"", redirect: false);
        }

        /// <summary>
        /// Search for `pattern` in each file in `location`.
        /// </summary>
        /// <param name="pattern">The pattern to match (enclosed in single-quotes).</param>
        /// <param name="location">The files or directory to search.</param>
        /// <param name="redirect">Will print output to terminal if false.</param>
        public BashResult Grep(string pattern, string location, bool redirect = true)
        {
            return Command($"grep '{pattern}' {location}", redirect: redirect);
        }

        /// <summary>
        /// Search for `pattern` in each file in `location`.
        /// </summary>
        /// <param name="pattern">The pattern to match (enclosed in single-quotes).</param>
        /// <param name="location">The files or directory to search.</param>
        /// <param name="flags">Optional `grep` arguments.</param>
        /// <param name="redirect">Will print output to terminal if false.</param>
        public BashResult Grep(string pattern, string location, string flags, bool redirect = true)
        {
            return Command($"grep '{pattern}' {flags} {location}", redirect: redirect);
        }

        /// <summary>
        /// List information about files in the current directory.
        /// </summary>
        /// <param name="redirect">Will print output to terminal if false.</param>
        public BashResult Ls(bool redirect = true)
        {
            return Command("ls", redirect: redirect);
        }

        /// <summary>
        /// List information about files in the current directory.
        /// </summary>
        /// <param name="flags">Optional `ls` arguments.</param>
        /// <param name="redirect">Will print output to terminal if false.</param>
        public BashResult Ls(string flags, bool redirect = true)
        {
            return Command($"ls {flags}", redirect: redirect);
        }

        /// <summary>
        /// Move `source` to `directory`.
        /// </summary>
        /// <param name="source">The file to be moved.</param>
        /// <param name="directory">The destination directory.</param>
        /// <param name="redirect">Will print output to terminal if false.</param>
        public BashResult Mv(string source, string directory, bool redirect = true)
        {
            return Command($"mv {source} {directory}", redirect: redirect);
        }

        /// <summary>
        /// Move `source` to `directory`.
        /// </summary>
        /// <param name="source">The file to be moved.</param>
        /// <param name="directory">The destination directory.</param>
        /// <param name="flags">Optional `mv` arguments.</param>
        /// <param name="redirect">Will print output to terminal if false.</param>
        public BashResult Mv(string source, string directory, string flags, bool redirect = true)
        {
            return Command($"mv {flags} {source} {directory}", redirect: redirect);
        }

        /// <summary>
        /// Copy `source` to `directory`.
        /// </summary>
        /// <param name="source">The file to be copied.</param>
        /// <param name="directory">The destination directory.</param>
        /// <param name="redirect">Will print output to terminal if false.</param>
        public BashResult Cp(string source, string directory, bool redirect = true)
        {
            return Command($"cp {source} {directory}", redirect: redirect);
        }

        /// <summary>
        /// Copy `source` to `directory`.
        /// </summary>
        /// <param name="source">The file to be copied.</param>
        /// <param name="directory">The destination directory.</param>
        /// <param name="flags">Optional `cp` arguments.</param>
        /// <param name="redirect">Will print output to terminal if false.</param>
        public BashResult Cp(string source, string directory, string flags, bool redirect = true)
        {
            return Command($"cp {flags} {source} {directory}", redirect: redirect);
        }

        /// <summary>
        /// Remove or unlink the given file.
        /// </summary>
        /// <param name="file">The file to be removed.</param>
        /// <param name="redirect">Will print output to terminal if false.</param>
        public BashResult Rm(string file, bool redirect = true)
        {
            return Command($"rm {file}", redirect: redirect);
        }

        /// <summary>
        /// Remove or unlink the given file.
        /// </summary>
        /// <param name="file">The file to be removed.</param>
        /// <param name="flags">Optional `rm` arguments.</param>
        /// <param name="redirect">Will print output to terminal if false.</param>
        public BashResult Rm(string file, string flags, bool redirect = true)
        {
            return Command($"rm {flags} {file}", redirect: redirect);
        }

        /// <summary>
        /// Concatenate `file` to standard input.
        /// </summary>
        /// <param name="file">The source file.</param>
        /// <param name="redirect">Will print output to terminal if false.</param>
        public BashResult Cat(string file, bool redirect = true)
        {
            return Command($"cat {file}", redirect: redirect);
        }

        /// <summary>
        /// Concatenate `file` to standard input.
        /// </summary>
        /// <param name="file">The source file.</param>
        /// <param name="flags">Optional `cat` arguments.</param>
        /// <param name="redirect">Will print output to terminal if false.</param>
        public BashResult Cat(string file, string flags, bool redirect = true)
        {
            return Command($"cat {flags} {file}", redirect: redirect);
        }

        private ProcessStartInfo BashInfo(bool redirectOutput)
        {
            return new ProcessStartInfo
            {
                FileName = _bashPath,
                RedirectStandardInput = true,
                RedirectStandardOutput = redirectOutput,
                RedirectStandardError = redirectOutput,
                UseShellExecute = false,
                CreateNoWindow = true,
                ErrorDialog = false
            };
        }
    }
}
