using System.Collections.Generic;
using System.Linq;

namespace StatisticsService.Models
{
    public interface IReplaysStatisticsConverter
    {
        ReplaysStatistics Build(IList<ReportModel> reports);
    }

    public class ReplaysStatisticsConverter : IReplaysStatisticsConverter
    {
        public const int minutesGroupSize = 5;

        public ReplaysStatistics Build(IList<ReportModel> reports)
        {
            var minutes = reports.Select(e => e.Time).ToArray();

            var minuteFrom = (int)minutes.Min();
            var minuteTo = (int)minutes.Max();

            var replaysDictionary = reports.ToDictionary(e => e.Time, e => e.Count);
            var wonReplaysDictionary = reports.ToDictionary(e => e.Time, e => e.Wins);

            var allMinutes = Enumerable.Range(0, (minuteTo - minuteFrom) / minutesGroupSize).Select(e => minuteFrom + e * minutesGroupSize).ToArray();
            var replays = allMinutes.ToDictionary(e => e, e => replaysDictionary.ContainsKey(e) ? replaysDictionary[e] : 0);
            var replaysWon = allMinutes.ToDictionary(e => e, e => wonReplaysDictionary.ContainsKey(e) ? wonReplaysDictionary[e] : 0);

            var replaysByAi = reports
                .SelectMany(e => e.CountAi.Select(g => new { Ai = g.Key, Count = g.Value, e.Time }))
                .GroupBy(e => e.Ai, e => new { e.Count, e.Time })
                .ToDictionary(
                    e => e.Key,
                    e =>
                    {
                        var tempDictionary = e.ToDictionary(g => g.Time, g => g.Count);

                        return (IDictionary<int, int>)allMinutes.ToDictionary(
                            g => g, g => tempDictionary.ContainsKey(g) ? tempDictionary[g] : 0);
                    });

            var replaysByMapSize = reports
                .SelectMany(e => e.CountMap.Select(g => new { Ai = g.Key, Count = g.Value, e.Time }))
                .GroupBy(e => e.Ai, e => new { e.Count, e.Time })
                .ToDictionary(
                    e => e.Key,
                    e =>
                    {
                        var tempDictionary = e.ToDictionary(g => g.Time, g => g.Count);

                        return (IDictionary<int, int>)allMinutes.ToDictionary(
                            g => g, g => tempDictionary.ContainsKey(g) ? tempDictionary[g] : 0);
                    });

            var replaysByAiWon = reports
                .SelectMany(e => e.WinsAi.Select(g => new { Ai = g.Key, Count = g.Value, e.Time }))
                .GroupBy(e => e.Ai, e => new { e.Count, e.Time })
                .ToDictionary(
                    e => e.Key,
                    e =>
                    {
                        var tempDictionary = e.ToDictionary(g => g.Time, g => g.Count);

                        return (IDictionary<int, int>)allMinutes.ToDictionary(
                            g => g, g => tempDictionary.ContainsKey(g) ? tempDictionary[g] : 0);
                    });

            var replaysByMapSizeWon = reports
                .SelectMany(e => e.WinsMap.Select(g => new { Ai = g.Key, Count = g.Value, e.Time }))
                .GroupBy(e => e.Ai, e => new { e.Count, e.Time })
                .ToDictionary(
                    e => e.Key,
                    e =>
                    {
                        var tempDictionary = e.ToDictionary(g => g.Time, g => g.Count);

                        return (IDictionary<int, int>)allMinutes.ToDictionary(
                            g => g, g => tempDictionary.ContainsKey(g) ? tempDictionary[g] : 0);
                    });

            return new ReplaysStatistics
            {
                Minutes = allMinutes,
                Replays = replays,
                ReplaysWon = replaysWon,
                ReplaysByAi = replaysByAi,
                ReplaysByAiWon = replaysByAiWon,
                ReplaysByMapSize = replaysByMapSize,
                ReplaysByMapSizeWon = replaysByMapSizeWon,
                Min = new MinEntity
                {
                    Minute = minuteFrom
                },
                Max = new MaxEntity
                {
                    Minute = minuteTo,
                    Replays = reports.Max(e => e.Count)
                }
            };
        }
    }
}