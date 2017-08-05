using System;
using System.Diagnostics;
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
        private static string commitHash;

        public static void Main(string[] args)
        {
            log.Info("Hello");
//            threadCount = args.Length == 1 ? int.Parse(args.First()) : 1;
            threadCount = 16;
            commitHash = args[0];

            for (var i = 0; i < threadCount; i++)
            {
                var index = i;

                Task.Run(
                    () =>
                    {
                        while (true)
                        {
                            if (TryCompeteOnArena(index)) return;
                        }
                    });
            }

            Console.ReadLine();
        }

        private static bool TryCompeteOnArena(int index)
        {
            try
            {
                var sw = Stopwatch.StartNew();
                IAi ai;
                IServerInteraction interaction;

                lock (Locker)
                {
                    var match = ArenaApi.GetNextMatch();
                    ai = AiFactoryRegistry.GetNextAi();

                    if (match == null) return true;

                    log.Info($"Collector {index}: Match on port {match.Port} for {ai.Name}");

                    try
                    {
                        interaction = new OnlineInteraction(match.Port);
                    }
                    catch (SocketException e)
                    {
                        log.Error(e);
                        return true;
                    }
                    if (!interaction.Start()) return true;
                }

                log.Info($"Collector {index}: Running game");
                var metaAndData = interaction.RunGame(ai);

                metaAndData.Item1.CommitHash = commitHash;

                Repo.SaveReplay(metaAndData.Item1, metaAndData.Item2);
                log.Info($"Collector {index}: Saved replay {metaAndData.Item1.Scores.ToDelimitedString(", ")}");

                ++completedTasksCount;

                log.Info($"Collector {index}: {completedTasksCount} replays collected");
                log.Info($"Collector {index}: Elapsed {sw.ElapsedMilliseconds}");
            }
            catch (Exception e)
            {
                log.Error(e, $"Collector {index} failed: {e}");
            }
            return false;
        }
    }
}