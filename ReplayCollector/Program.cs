using System;
using System.Threading.Tasks;
using lib.OnlineRunner;

namespace ReplayCollector
{
    internal class Program
    {
        private static int threadCount;

        public static void Main(string[] args)
        {
            threadCount = 1;
            for (var i = 0; i < threadCount; i++)
            {
                var index = i;

                Task.Run(
                    () =>
                    {
                        while (true)
                            if (!OnlineArenaRunner.TryCompeteOnArena(index.ToString())) return;
                    });
            }

            Console.ReadLine();
        }
    }
}