using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;
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

        public override string ToString()
        {
            return $"Meta {Timestamp} {AiName} dataId: {DataId}";
        }
    }

    public class ReplayData
    {
        public Map Map;
        public MoveJson[] Moves;
        public Future[] Futures;

        public override string ToString()
        {
            return $"{nameof(Moves)}: {Moves.Take(10).ToDelimitedString("; ")}";
        }

        public ReplayData()
        {
        }
        
        [JsonConstructor]
        public ReplayData(Map map, MoveJson[] moves, Future[] futures)
        {
            Map = map;
            Moves = moves;
            Futures = futures;
        }

        public ReplayData(Map map, IEnumerable<Move> moves, Future[] futures)
        {
            Map = map;
            Moves = moves.Select(move => new MoveJson(move)).ToArray();
            Futures = futures;
        }
    }

    public class MoveJson
    {
        public PassMove Pass;
        public ClaimMove Claim;

        public override string ToString()
        {
            return ToMove().ToString();
        }

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
