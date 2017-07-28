using System;
using Shell.NET;

namespace TestApp
{
    class Program
    {
        static Bash bash = new Bash();

        static void Main(string[] args)
        {
            Console.WriteLine(" > ls -lhaF");
            bash.Command("ls -lhaF");
            CheckCommandOutput();
            Console.WriteLine(" > echo $PATH");
            bash.Command("echo $PATH");
            CheckCommandOutput();
            Console.WriteLine(" > echo \"Hello Travis!\" >> $HOME/Shell.NET.Test");
            bash.Command("echo \"Hello Travis!\" >> $HOME/Shell.NET.Test");
            CheckCommandOutput();
            Console.WriteLine(" > mv $HOME/Shell.NET.Test /tmp");
            bash.Command("mv $HOME/Shell.NET.Test /tmp");
            CheckCommandOutput();
            Console.WriteLine(" > cat /tmp/Shell.NET.Test");
            bash.Command("cat /tmp/Shell.NET.Test");
            CheckCommandOutput();
        }

        static void CheckCommandOutput(int errorCode = 1)
        {
            if (bash.ExitCode != 0)
            {
                bash.Echo(@"\e[1m[ \e[31mFAIL\e[39m ]\e[0m", "-e");
                Environment.Exit(1);
            }
            bash.Echo(@"\e[1m[ \e[32mPASS\e[39m ]\e[0m", "-e");
        }
    }
}
