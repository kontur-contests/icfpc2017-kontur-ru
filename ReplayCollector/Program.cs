using System;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using lib;
using lib.Ai;
using lib.Arena;
using lib.Interaction;
using lib.Replays;
using lib.viz;
using MoreLinq;
using NLog;

namespace ReplayCollector
{
    internal class Program
    {
        private static readonly ArenaApi ArenaApi = new ArenaApi();
        private static readonly ReplayRepo Repo = new ReplayRepo();
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        
        private static int threadCount;
        private static readonly object Locker = new object();

        private static int completedTasksCount = 0;
        
        public static void Main(string[] args)
        {
            log.Info("Hello");
            threadCount = args.Length == 1 ? int.Parse(args.First()) : 1;
            
            for (var i = 0; i < threadCount; i++)
            {
                var index = i;
                
                Task.Run(() =>
                {
                    while (true)
                    {
                        try
                        {
                            ArenaMatch match;
                            IAi ai;

                            IServerInteraction interaction;

                            lock (Locker)
                            {
                                match = ArenaApi.GetNextMatch();
                                ai = AiFactoryRegistry.GetNextAi();

                                if (match == null) return;

                                log.Info(
                                    "Collector " + index + ": Match on port " + match.Port + " for " + ai.Name +
                                    " AI...");


                                try
                                {
                                    interaction = new OnlineInteraction(match.Port);
                                }
                                catch (SocketException e)
                                {
                                    log.Error(e);
                                    return;
                                }
                                if (!interaction.Start()) return;
                            }

                            log.Info("Collector " + index + ": Running game");
                            var metaAndData = interaction.RunGame(ai);

                            var tStart = DateTime.UtcNow;
                            Repo.SaveReplay(metaAndData.Item1, metaAndData.Item2);
                            var tEnd = DateTime.UtcNow;
                            log.Info("Collector " + index + ": Saved replay " + metaAndData.Item1.Scores.ToDelimitedString(", "));
                            log.Info("Collector " + index + ": time to save " + (tEnd - tStart).TotalSeconds);
                            
                            ++completedTasksCount;

                            log.Info("Collector " + index + ": " + completedTasksCount + " replays collected");
                        }
                        catch (Exception e)
                        {
                            log.Error(e, "Collector " + index + " failed: " + e);
                        }
                    }
                });
            }

            Console.ReadLine();
        }
    }
}