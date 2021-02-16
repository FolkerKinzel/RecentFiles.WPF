using System;
using System.Threading;

namespace MutexTester
{
    internal class Program
    {
        private static void Main()
        {
            Console.WriteLine("Press any key to take the mutex.");
            _ = Console.ReadKey(true);

            using (var _mutex = new Mutex(false, "Global\\MutexTester"))
            {
                try
                {
                    if (_mutex.WaitOne())
                    {
                        Console.WriteLine("Took mutex. Press any key to release the mutex.");
                        _ = Console.ReadKey(true);
                        _mutex.ReleaseMutex();

                        Console.WriteLine("Mutex released.");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            Console.WriteLine("Mutex disposed. Press any key to quit.");
            _ = Console.ReadKey();
        }
    }
}
