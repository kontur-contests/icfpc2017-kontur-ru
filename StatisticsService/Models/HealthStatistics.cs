using System.Collections.Generic;

namespace StatisticsService.Models
{
    public class ReplaysStatistics
    {
        public IDictionary<int, int> Replays { get; set; }
        public IDictionary<int, int> ReplaysWon { get; set; }
        public IDictionary<string, IDictionary<int, int>> ReplaysByAi { get; set; }
        public IDictionary<string, IDictionary<int, int>> ReplaysByAiWon { get; set; }
        public IDictionary<int, IDictionary<int, int>> ReplaysByMapSize { get; set; }
        public IDictionary<int, IDictionary<int, int>> ReplaysByMapSizeWon { get; set; }
        public int[] Minutes { get; set; }
        public MinEntity Min { get; set; }
        public MaxEntity Max { get; set; }
    }
}