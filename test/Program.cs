using System;
using System.IO;
using Shell.NET;

namespace TestApp
{
    class Program
    {
        static Bash bash = new Bash();
        static string whoami => bash.Command("whoami").Lines[0];
        static string path => bash.Command("dirs -0").Lines[0];
        static string hostname => bash.Command("hostname").Lines[0];
        static string reset = @"\e[0m";
        static string bold = @"\e[1m";
        static string cyan = @"\e[36m";
        static string blue = @"\e[34m";
        static string green = @"\e[32m";

        static string prompt =
            $"{cyan}[ {blue}{bold}{whoami}@{hostname}{reset} {green}{path}{cyan} ]{reset}$";

        static void Main(string[] args)
        {
            SayPrompt("ls -lhaF");
            Console.WriteLine(bash.Ls("-lhaF").Output);
            CheckCommandOutput();

            SayPrompt("echo \"C# + Linux = <3!\" >> ~/Shell.NET.Test");
            bash.Command("echo \"C# + Linux = <3!\" >> ~/Shell.NET.Test", redirect: false);
            CheckCommandOutput();

            SayPrompt("mv ~/Shell.NET.Test /tmp");
            bash.Mv("~/Shell.NET.Test", "/tmp", redirect: false);
            CheckCommandOutput();

            SayPrompt("echo $PATH");
            bash.Echo("$PATH");
            CheckCommandOutput();

            SayPrompt("cat /tmp/Shell.NET.Test");
            bash.Cat("/tmp/Shell.NET.Test", redirect: false);
            CheckCommandOutput();

            SayPrompt("cp /tmp/Shell.NET.Test ~/Desktop");
            bash.Cp("/tmp/Shell.NET.Test", "~/Desktop", redirect: false);
            CheckCommandOutput();

            SayPrompt("grep '+ Linux' -nrH ~/Desktop");
            bash.Grep("+ Linux", "~/Desktop", "-nrH", redirect: false);
            CheckCommandOutput();

            SayPrompt("grep '+ Linux' -nrH ~/Desktop");
            Console.WriteLine(bash.Grep("+ Linux", "~/Desktop", "-nrH").Output);
            CheckCommandOutput();

            SayPrompt("rm ~/Desktop/Shell.NET.Test /tmp/Shell.NET.Test");
            bash.Rm("~/Desktop/Shell.NET.Test /tmp/Shell.NET.Test", redirect: false);
            CheckCommandOutput();

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
