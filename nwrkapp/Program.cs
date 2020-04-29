using System;

namespace nwrk.app
{
    class Program
    {
        static void Main(string[] args)
        {
            var code = new StartUp().Start<MainController>();
            Console.WriteLine($"app exit with code {code}");
        }
    }
}
