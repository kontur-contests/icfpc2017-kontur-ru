using System;
using System.Diagnostics;
using System.Net.Sockets;
using lib.Ai;
using lib.Interaction;
using lib.Replays;
using lib.viz;
using MoreLinq;
using NLog;

namespace lib.Arena
{
    public class ArenaRunner
    {
        private static readonly object Locker = new object();
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        private static readonly ReplayRepo Repo = new ReplayRepo();

        public static bool TryCompeteOnArena(string collectorId, string commitHash = "manualRun")
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

                    if (match == null) return false;

                    log.Info($"Collector {collectorId}: Match on port {match.Port} for {ai.Name}");

                    try
                    {
                        interaction = new OnlineInteraction(match.Port);
                    }
                    catch (SocketException e)
                    {
                        log.Error(e);
                        return false;
                    }
                    if (!interaction.Start()) return false;
                }

                log.Info($"Collector {collectorId}: Running game");
                var metaAndData = interaction.RunGame(ai);

                metaAndData.Item1.CommitHash = commitHash;

                Repo.SaveReplay(metaAndData.Item1, metaAndData.Item2);
                log.Info($"Collector {collectorId}: Saved replay {metaAndData.Item1.Scores.ToDelimitedString(", ")}");
                log.Info($"Collector {collectorId}: Elapsed {sw.ElapsedMilliseconds}");
            }
            catch (Exception e)
            {
                log.Error(e, $"Collector {collectorId} failed: {e}");
            }
            return true;
        }

    }
}