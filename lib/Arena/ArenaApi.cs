using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp;

namespace lib.Arena
{
    public class ArenaApi
    {
        private const string Address = "http://punter.inf.ed.ac.uk/status.html";

        public ArenaMatch GetNextMatch()
        {
            var matches = new ArenaApi().GetArenaMatchesAsync()
                .ConfigureAwait(false).GetAwaiter()
                .GetResult();

            return matches.FirstOrDefault(x => x.IsSuitableForReplayCollection());
        }

        public async Task<ArenaMatch[]> GetArenaMatchesAsync()
        {
            var config = Configuration.Default.WithDefaultLoader();
            var document = await BrowsingContext.New(config).OpenAsync(Address);

            var matches = new List<ArenaMatch>();
            return document.QuerySelectorAll("tr").Skip(1).Select(
                x =>
                {
                    var match = new ArenaMatch();
                    var cells = x.QuerySelectorAll("td").ToArray();

                    var statusCellText = cells[0].TextContent.ToLower();
                    if (statusCellText.Contains("offline"))
                    {
                        match.Status = ArenaMatch.MatchStatus.Offline;
                    }
                    else if (statusCellText.Contains("progress"))
                    {
                        match.Status = ArenaMatch.MatchStatus.InProgress;
                    }
                    else if (statusCellText.Contains("waiting"))
                    {
                        match.Status = ArenaMatch.MatchStatus.Waiting;
                        var regex = new Regex(@"\((?<taken>\d+)/(?<total>\d+)\)$");
                        var players = regex.Match(statusCellText);
                        int.TryParse(players.Groups["taken"].Value, out match.TakenSeats);
                        int.TryParse(players.Groups["total"].Value, out match.TotalSeats);
                    }
                    else
                    {
                        match.Status = ArenaMatch.MatchStatus.Unknown;
                    }

                    match.Players = cells[1].TextContent.Split(',');
                    match.Extensions = cells[2].TextContent.Split(',');
                    int.TryParse(cells[3].TextContent, out match.Port);
                    match.MapName = cells[4].QuerySelector("a").TextContent;

                    return match;
                }).ToArray();
        }
    }
}