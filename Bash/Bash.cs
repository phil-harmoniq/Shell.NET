using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Shell.NET
{
    public class Bash
    {
        public int ExitCode { get; private set; }
        public string ErrorMsg { get; private set; }

        public string Command(string input, bool redirect = false)
        {
            using (var bash = new Process { StartInfo = BashInfo(redirect) })
            {
                bash.Start();
                bash.StandardInput.WriteLine($"{input}; exit");
                ErrorMsg = bash.StandardError.ReadToEnd();
                ExitCode = bash.ExitCode;

                if (redirect)
                {
                    var output = bash.StandardOutput.ReadToEnd();
                    bash.WaitForExit();
                    bash.Close();
                    return output;
                }
                else
                {
                    bash.WaitForExit();
                    bash.Close();
                    return null;
                }
            }
        }

        public void Echo(string input)
        {
            Command($"echo '{input}'", redirect: false);
        }

        public void Echo(Object obj)
        {
            Command($"echo '{obj}'", redirect: false);
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
