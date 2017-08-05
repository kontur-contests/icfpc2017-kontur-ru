using System;
using System.Threading.Tasks;
using lib.Arena;
using lib.viz;

namespace ReplayCollector
{
    internal class Program
    {
        private static readonly ArenaApi ArenaApi = new ArenaApi();
        
        private const int ThreadCount = 4;
        private static readonly object Locker = new object();

        private static int completedTasksCount = 0;
        
        public static void Main(string[] args)
        {
            do
            {
                for (var i = 0; i < ThreadCount; i++)
                {
                    var index = i;
                    
                    Task.Run(() =>
                    {
                        lock (Locker)
                        {
                            var match = ArenaApi.GetNextMatch();
                            var ai = AiFactoryRegistry.GetNextAi();
                            
                            Console.WriteLine("Collector " + index + ": Found match on port " + match.Port + " for " + ai.Name + " AI");
                            
//                            var onlineInteraction = new OnlineInteraction(match.Port);
//                            onlineInteraction.RunGame();
                        }

                        ++completedTasksCount;
                        
                        Console.WriteLine("Collector " + index + ": " + completedTasksCount + " replays collected");
                    });
                }
            }
            while (!Console.KeyAvailable);
            
        }
    }
}