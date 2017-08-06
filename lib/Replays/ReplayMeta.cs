using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using lib.Structures;
using MoreLinq;
using Newtonsoft.Json;

namespace lib.Replays
{
    public class ReplayMeta
    {
        public DateTime Timestamp;
        public string AiName;
        public string AiVersion;
        public string CommitHash;
        public string MapHash;
        public int OurPunter;
        public int PunterCount;
        public Score[] Scores;
        
        public string DataId;

        public ReplayMeta()
        {
        }

        public ReplayMeta(DateTime timestamp, string aiName, string aiVersion, string commitHash, int ourPunter, int punterCount, Score[] scores)
        {
            Timestamp = timestamp;
            AiName = aiName;
            AiVersion = aiVersion;
            CommitHash = commitHash;
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
        public Move[] Moves;
        public Future[] Futures;

        public override string ToString()
        {
            return $"{nameof(Moves)}: {Moves.Take(10).ToDelimitedString("; ")}";
        }

        public ReplayData()
        {
        }
        
        [JsonConstructor]
        public ReplayData(Map map, Move[] moves, Future[] futures)
        {
            Map = map;
            Moves = moves;
            Futures = futures;
        }

        public ReplayData(Map map, IEnumerable<Move> moves, Future[] futures)
        {
            Map = map;
            Moves = moves.ToArray();
            Futures = futures;
        }

        public string Encode()
        {
            var s = new MemoryStream();
            var w = new BinaryWriter(s);
            EncodeTo(w);
            return Convert.ToBase64String(s.ToArray());
        }
        public static ReplayData Decode(string data)
        {
            var s = new MemoryStream(Convert.FromBase64String(data));
            var r = new BinaryReader(s);
            return DecodeFrom(r);
        }
        public void EncodeTo(BinaryWriter writer)
        {
            foreach (var future in Futures)
                future.EncodeTo(writer);
            writer.Write((byte)0);
            foreach (var move in Moves)
                move.EncodeTo(writer);
            writer.Write((byte)0);
        }

        public static ReplayData DecodeFrom(BinaryReader reader)
        {
            var futures = new List<Future>();
            while (true)
            {
                var future = Future.DecodeFrom(reader);
                if (future == null) break;
                futures.Add(future);
            }
            var moves = new List<Move>();
            while (true)
            {
                var move = Move.DecodeFrom(reader);
                if (move == null) break;
                moves.Add(move);
            }
            return new ReplayData {Futures = futures.ToArray(), Moves = moves.ToArray()};

        }
    }
}
