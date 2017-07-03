using System;
using Shell.NET;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var bash = new Bash();
            bash.Command("netpkg-tool sdagadfd");
            Console.WriteLine(bash.ExitCode);
        }
    }
}
