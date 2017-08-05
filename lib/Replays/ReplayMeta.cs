using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace lib.Replays
{
    public class ReplayMeta
    {
        public DateTime Timestamp;
        public string AiName;
        public int OurPunter;
        public int PunterCount;
        public ScoreModel[] Scores;
        
        public string DataId;

        public ReplayMeta()
        {
        }

        public ReplayMeta(DateTime timestamp, string aiName, int ourPunter, int punterCount, ScoreModel[] scores)
        {
            Timestamp = timestamp;
            AiName = aiName;
            OurPunter = ourPunter;
            PunterCount = punterCount;
            Scores = scores;
        }
    }

    public class ReplayData
    {
        public Map Map;
        public MoveJson[] Moves;

        public ReplayData()
        {
        }
        
        [JsonConstructor]
        public ReplayData(Map map, MoveJson[] moves)
        {
            Map = map;
            Moves = moves;
        }

        public ReplayData(Map map, IEnumerable<Move> moves)
        {
            Map = map;
            Moves = moves.Select(move => new MoveJson(move)).ToArray();
        }
    }

    public class MoveJson
    {
        public PassMove Pass;
        public ClaimMove Claim;

        public Move ToMove()
        {
            return (Move) Pass ?? Claim;
        }

        public MoveJson()
        {
        }

        public MoveJson(Move move)
        {
            switch (move)
            {
                case ClaimMove claimMove: Claim = claimMove; break;
                case PassMove passMove: Pass = passMove; break;
                default: throw new NotImplementedException();
            }
        }
    }
}
