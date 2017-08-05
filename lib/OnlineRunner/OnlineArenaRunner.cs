using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using lib.Ai;
using lib.Arena;
using lib.Interaction;
using lib.Replays;
using lib.viz;
using MoreLinq;
using NLog;

namespace lib.OnlineRunner
{
    public static class OnlineArenaRunner
    {
        private static readonly object Locker = new object();
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        private static readonly ReplayRepo Repo = new ReplayRepo();

        public static bool TryCompeteOnArena(string collectorId, string commitHash = "manualRun")
        {
            var portLocker = new PortLocker();
            var match = ArenaMatch.EmptyMatch;

            try
            {
                var sw = Stopwatch.StartNew();
                IAi ai;
                IServerInteraction interaction;

                lock (Locker)
                {
                    var isPortOpen = false;
                    do
                    {
                        match = ArenaApi.GetNextMatch();
                        if (match == null)
                            continue;
                        isPortOpen = portLocker.TryAcquire(match.Port);
                        if (!isPortOpen)
                            log.Warn($"Collector {collectorId}: {match.Port} taken");
                    } while (!isPortOpen);

                    log.Info($"Collector {collectorId}: Take {match.Port}");

                    ai = AiFactoryRegistry.GetNextAi();

                    log.Info($"Collector {collectorId}: Match on port {match.Port} for {GetBotName(ai.Name)}");
                    
                    try
                    {
                        interaction = new OnlineInteraction(match.Port, GetBotName(ai.Name));
                    }
                    catch (SocketException e)
                    {
                        log.Error(e);
                        portLocker.Free(match.Port);
                        return false;
                    }
                    if (!interaction.Start())
                    {
                        portLocker.Free(match.Port);
                        return false;
                    }
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
            
            portLocker.Free(match.Port);
            return true;
        }

        private static string GetBotName(string botTypeName)
        {
            return $"kontur.ru_{string.Join("", botTypeName.Where(char.IsUpper).ToArray())}";
        }
    }
}