using System.Collections.Generic;

namespace StatisticsService.Models
{
    public class ReportModel
    {
        public ReportModel(long time, int count, IDictionary<string, int> countAi, IDictionary<int, int> countMap, int wins, IDictionary<string, int> winsAi, IDictionary<int, int> winsMap)
        {
            Time = time;
            Count = count;
            CountAi = countAi;
            Wins = wins;
            WinsAi = winsAi;
            WinsMap = winsMap;
            CountMap = countMap;
        }

        public long Time { get; }
        public int Count { get; }
        public IDictionary<string, int> CountAi { get; }
        public IDictionary<int, int> CountMap { get; }
        public int Wins { get; }
        public IDictionary<string, int> WinsAi { get; }
        public IDictionary<int, int> WinsMap { get; }
    }
}