using System;
using Shell.NET;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var bash = new Bash();
            bash.Command("~/Desktop/Hello.npk one --npk-v two");
            bash.Command("netpkg-tool --help", redirect: true);
            Console.WriteLine(bash.Output);
        }
    }
}
