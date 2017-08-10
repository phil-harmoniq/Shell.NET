using System;
using System.IO;
using Shell.NET;

namespace TestApp
{
    class Program
    {
        static readonly Bash bash = new Bash();
        static readonly string whoami = bash.Command("whoami").Output;
        static readonly string path = bash.Command("dirs -0").Output;
        static readonly string hostname = bash.Command("hostname").Output;
        static string os;
        static readonly string reset = @"\e[0m";
        static readonly string bold = @"\e[1m";
        static readonly string cyan = @"\e[36m";
        static readonly string blue = @"\e[34m";
        static readonly string green = @"\e[32m";

        static string prompt =
            $"{cyan}[ {blue}{bold}{whoami}@{hostname}{reset} {green}{path}{cyan} ]{reset}$";

        static void Main(string[] args)
        {
            // Because Travis doesn't have a ~/Desktop:
            bash.Command("mkdir -p $HOME/Desktop");
 
            SayPrompt("cp \"$HOME/.bashrc\" \"$HOME/Desktop/bashrc-backup\"");
            bash.Cp("$HOME/.bashrc", "$HOME/Desktop/bashrc-backup");
            CheckCommandOutput();

            SayPrompt("grep \"export\" \"$HOME/Desktop/bashrc-backup\"");
            bash.Grep("export", "$HOME/Desktop/bashrc-backup", redirect: false);
            CheckCommandOutput();

            // Commands return a BashResult that stores output information:
            SayPrompt("rm \"$HOME/Desktop/bashrc-backup\"");
            if (bash.Rm("$HOME/Desktop/bashrc-backup").ExitCode == 0)
                Console.WriteLine("Success!");
            CheckCommandOutput();

            // With redirect (default in most commands), access the command's output from BashResult.Output:
            SayPrompt("cat \"$HOME/.bashrc\"");
            Console.WriteLine(bash.Cat("$HOME/.bashrc").Output);
            CheckCommandOutput();

            // Without redirect, the command's output gets printed to the terminal:
            SayPrompt("cat \"$HOME/.bashrc\"");
            bash.Cat("$HOME/.bashrc", redirect: false);
            CheckCommandOutput();

            // BashResult.Lines splits BashResult.Output by new-lines and stores the result as an array:
            SayPrompt("ls -lhaF");
            foreach (var line in bash.Ls("-lhaF").Lines)
                Console.WriteLine(line);
            CheckCommandOutput();

            // Run custom commands using Bash.Command():
            SayPrompt("ldd /usr/bin/dotnet");
            bash.Command("ldd /usr/bin/dotnet", redirect: false);
            CheckCommandOutput();

            var netVersion = bash.Command("dotnet --version").Output;
            Console.WriteLine($".NET Core version: {netVersion}");
            Console.WriteLine($"OS: {bash.Command("uname -s").Output}");

            bash.Echo("\nAll Shell.NET tests passed! :)");
        }

        static void CheckCommandOutput(int errorCode = 1)
        {
            if (bash.ExitCode != 0)
            {
                bash.Echo(@"\e[1m[ \e[31mFAIL\e[39m ]\e[0m", "-e");
                Environment.Exit(1);
            }
        }

        static void SayPrompt(string cmd)
        {
            bash.Echo($"{prompt} ", "-ne");
            Console.WriteLine(cmd);
        }
    }
}
