using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
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

        public const string ourBotNamePrefix = "kontur.ru";

        private static readonly string[] orgBoxNames =
        {
            "eager punter"  
        };
        
        private static string GetBotName(string botTypeName)
        {
            return $"{ourBotNamePrefix}_{string.Join("", botTypeName.Where(char.IsUpper).ToArray())}";
        }

        public static bool IsOurBot(this string botName)
        {
            return botName.StartsWith(ourBotNamePrefix);
        }
        
        public static bool IsSuitableForOnlineGame(this ArenaMatch match)
        {
                   // Arena is "Waiting for punters."
            return match.Status == ArenaMatch.MatchStatus.Waiting
                   
                   // ...and there're no "kontur.ru_*" bots already connected to arena
                && match.Players.All(x => !x.IsOurBot())
                   
                   // ...and it's not a map fully populated with organizers' bots
                && match.Players.Count(x => orgBoxNames.Contains(x)) != match.TotalSeats - 1;
        }
        
        public static ArenaMatch GetNextMatch()
        {
            var matches = new ArenaApi().GetArenaMatchesAsync()
                .ConfigureAwait(false).GetAwaiter()
                .GetResult()
                .Where(x => x.IsSuitableForOnlineGame())
                .OrderBy(x => x.TotalSeats - x.TakenSeats)
                .ToArray();

            return matches
                //.OrderBy(x => Guid.NewGuid())
                .FirstOrDefault();
        }

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
                        match = GetNextMatch();
                        
                        if (match == null)
                        {
                            Thread.Sleep(5000);
                            log.Warn($"Collector {collectorId}: No matches found, sleeping 5 seconds...");
                            continue;
                        }
                        
                        isPortOpen = portLocker.TryAcquire(match.Port);
                        if (!isPortOpen)
                            log.Warn($"Collector {collectorId}: {match.Port} taken");
                    } while (!isPortOpen);

                    log.Info($"Collector {collectorId}: Take {match.Port}");

                    ai = AiFactoryRegistry.GetNextAi(true);

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

                log.Info($"Collector {collectorId}: Running game on port {match.Port}");
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
    }
}