using System;
using System.Linq;
using System.Threading;
using lib;
using lib.Arena;
using lib.OnlineRunner;
using MoreLinq;
using NUnit.Framework;

namespace OnlineGameMonitor
{
    internal class Program
    {
        private static readonly PortLocker portLocker = new PortLocker();

        private static string GetMatchStatus(ArenaMatch match)
        {
            var teams = GetTeamsSummary(match.Players);
            return $"{match.Port}  -  {match.TakenSeats:00} / {match.TotalSeats:00}  -  {teams}";
        }

        private static string GetTeamDesc(IGrouping<string, string> x)
        {
            var name = x.Key.Length > 15 ? x.Key.Substring(0, 15) : x.Key;
            var names = x.ToArray();
            var count = names.Length > 1 ? $" x{names.Length}" : "";
            var teams = $"{name}{count}";
            return teams.Length > 50 ? teams.Substring(0, 50) : teams;
        }

        private static string GetTeamsSummary(string[] teams)
        {
            return string.Join(", ", teams.GroupBy(x => x).Select(GetTeamDesc));
        }
        
        public static void Main(string[] args)
        {
            do
            {
                var matches = new ArenaApi().GetArenaMatchesAsync()
                    .ConfigureAwait(false).GetAwaiter()
                    .GetResult();

                var suitableMatches = matches.Where(x => x.IsSuitableForOnlineGame()).ToArray();
                var ignoredMatches = matches.Where(x => !x.IsSuitableForOnlineGame()).ToArray();
                var playedByUsMatches = ignoredMatches.Where(x => x.Players.Any(y => y.IsOurBot())).ToArray();
                
                var gameStatus = $"Matches: {matches.Length} (suitable: {suitableMatches.Length}, played: {playedByUsMatches.Length}, ignored: {ignoredMatches.Length - playedByUsMatches.Length})";
                Console.WriteLine(gameStatus);
                Console.WriteLine("");

                var playedByUsStatuses = new[] {"PLAYED BY US:","---------------"}.Concat(playedByUsMatches.Select(GetMatchStatus)).ToArray();
                var suitableStatuses = new[] {"SUITABLE FOR US:","---------------"}.Concat(suitableMatches.Select(GetMatchStatus)).ToArray();

                playedByUsStatuses
                    .ZipLongest(suitableStatuses, (a, b) =>
                    {
                        var playedByUsStatus = $"{a ?? "",-80}";
                        var suitableStatus = $"{b ?? "",-80}";
                        return $"{playedByUsStatus}{suitableStatus}";
                    })
                    .ForEach(x => Console.WriteLine(x));

                Console.ReadKey();
                Console.Clear();
                Console.SetCursorPosition(0, 0);
            }
            while (true);
        }
    }

    [TestFixture]
    internal class OnlineGameMonitorTests
    {
        [Test]
        public void TestOurBotDetection()
        {
            Assert.True("kontur.ru_GA".IsOurBot());
        }
    }
}