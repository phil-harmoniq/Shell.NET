using System;
using Shell.NET;

namespace TestApp
{
    class Program
    {
        static Bash bash = new Bash();

        static void Main(string[] args)
        {
            bash.Command("ls -lhaF");
            CheckCommandOutput();
            bash.Command("echo $PATH");
            CheckCommandOutput();
            bash.Command("echo \"Hello Travis!\" >> $HOME/Shell.NET.Test");
            CheckCommandOutput();
            bash.Command("mv $HOME/Shell.NET.Test");
            CheckCommandOutput();
            bash.Command("cat /tmp/Shell.NET.Test");
            CheckCommandOutput();
        }

        static void CheckCommandOutput(int errorCode = 1)
        {
            if (bash.ExitCode != 0)
            {
                Console.WriteLine(@"\e[1m[ \e[31mFAIL\e[39m ]\e[0m");
                Environment.Exit(1);
            }
            Console.WriteLine(@"\e[1m[ \e[32mPass\e[39m ]\e[0m");
        }
    }
}
