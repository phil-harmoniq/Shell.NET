using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Shell.NET
{
    /// <summary>
    /// Handles boilerplate for Bash commands and stores output and exit codes</summary>
    public class Bash
    {
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
        /// Execute a new Bash command</summary>
        /// <param name="input">The command to execute</param>
        /// <param name="redirect">Redirect StdOut and StdError to internal properties</param>
        public void Command(string input, bool redirect = false)
        {
            using (var bash = new Process { StartInfo = BashInfo(redirect) })
            {
                bash.Start();
                bash.StandardInput.WriteLine($"{input}; exit");

                if (redirect)
                {
                    ErrorMsg = bash.StandardError.ReadToEnd();
                    Output = bash.StandardOutput.ReadToEnd();
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
        }

        /// <summary>
        /// Echo command wrapper</summary>
        public void Echo(string input)
        {
            Command($"echo \"{input}\"", redirect: false);
        }

        /// <summary>
        /// Echo command wrapper with flags</summary>
        public void Echo(string input, string flags)
        {
            Command($"echo {flags} \"{input}\"", redirect: false);
        }

        /// <summary>
        /// Echo command wrapper</summary>
        public void Echo(Object input)
        {
            Command($"echo \"{input.ToString()}\"", redirect: false);
        }

        /// <summary>
        /// Echo command wrapper with flags</summary>
        public void Echo(Object input, string flags)
        {
            Command($"echo {flags} \"{input.ToString()}\"", redirect: false);
        }

        private ProcessStartInfo BashInfo(bool redirectOutput)
        {
            return new ProcessStartInfo
            {
                FileName = "bash",
                RedirectStandardInput = true,
                RedirectStandardOutput = redirectOutput,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                ErrorDialog = false
            };
        }
    }
}
