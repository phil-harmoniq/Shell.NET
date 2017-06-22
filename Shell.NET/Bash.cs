using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Shell.NET
{
    public class Bash
    {
        public string Output { get; private set; }
        public int ExitCode { get; private set; }
        public string ErrorMsg { get; private set; }

        public void Command(string input, bool redirect = false)
        {
            using (var bash = new Process { StartInfo = BashInfo(redirect) })
            {
                bash.Start();
                bash.StandardInput.WriteLine($"{input}; exit");
                ErrorMsg = bash.StandardError.ReadToEnd();
                ExitCode = bash.ExitCode;

                if (redirect)
                    Output = bash.StandardOutput.ReadToEnd();
                else
                    Output = null;

                bash.WaitForExit();
                bash.Close();
            }
        }

        public void Echo(string input)
        {
            Command($"echo \"{input}\"", redirect: false);
        }

        public void Echo(string input, string flags)
        {
            Command($"echo {flags} \"{input}\"", redirect: false);
        }

        public void Echo(Object input)
        {
            Command($"echo \"{input.ToString()}\"", redirect: false);
        }

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
