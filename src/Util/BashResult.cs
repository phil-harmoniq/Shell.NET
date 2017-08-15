using System;

namespace Shell.NET.Util
{
    /// <summary>
    /// Simple container for the results of a Bash command.</summary>
    public class BashResult
    {
        /// <summary>
        /// The command's standard output as a string. (if redirected)</summary>
        public string Output { get; private set; }

        /// <summary>
        /// The command's error output as a string. (if redirected)</summary>
        public string ErrorMsg { get; private set; }

        /// <summary>
        /// The command's exit code as an integer.</summary>
        public int ExitCode { get; private set; }

        /// <summary>
        /// An array of the command's output split by newline characters. (if redirected)</summary>
        public string[] Lines => Output?.Split('\n');

        internal BashResult(string output, string errorMsg, int exitCode)
        {
            if (output != null)
                Output = output.TrimEnd(System.Environment.NewLine.ToCharArray());
            else
                Output = null;
            
            if (errorMsg != null)
                ErrorMsg = errorMsg.TrimEnd(System.Environment.NewLine.ToCharArray());
            else
                ErrorMsg = null;
            
            ExitCode = exitCode;
        }
    }
}