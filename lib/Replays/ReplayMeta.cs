using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lib.Replays
{
    public class ReplayMeta
    {
        public DateTime Timestamp;
        public string AiName;
        public int OurPunter;
        public List<Score> Scores;
        
        public string DataId;
    }

    public class ReplayData
    {
        public Map Map;
        public List<Move> Moves;
    }
}
