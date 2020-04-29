using System;
using System.Threading;
using System.Threading.Tasks;

namespace nwrk.app
{
    public class TestController : bm.common.BaseController
    {
        public override int Run()
        {
            for (var i = 1; i <= 50; i++)
                TestTask(i);
            for (var i = 1; i <= 50; i++)
                TestThreadPool(i);
            for (var i = 1; i <= 50; i++)
                TestThread(i);
            Console.ReadLine();
            return 1;
        }

        private static void TestThread(int i)
        {
            Console.WriteLine("Thread {0} start.", i);
            new Thread(h =>
            {
                Thread.Sleep(5000);
                Console.WriteLine("-------------------Thread {0} end.", i);
            }).Start();
        }

        private static void TestThreadPool(int i)
        {
            Console.WriteLine("ThreadPool {0} start.", i);
            ThreadPool.QueueUserWorkItem(h =>
            {
                Thread.Sleep(5000);
                Console.WriteLine("-------------------ThreadPool {0} end.", i);
            });
        }

        private static void TestTask(int i)
        {
            Console.WriteLine("Task {0} start.", i);
            new Task(() =>
            {
                Thread.Sleep(5000);
                Console.WriteLine("-------------------Task {0} end.", i);
            }).Start();
        }
    }
}