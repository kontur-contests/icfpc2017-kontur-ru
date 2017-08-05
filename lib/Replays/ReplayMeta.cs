using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using MoreLinq;
using Newtonsoft.Json;

namespace lib.Replays
{
    public class ReplayMeta
    {
        public DateTime Timestamp;
        public string AiName;
        public string MapHash;
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
            var moves = new List<MoveJson>();
            while (true)
            {
                var move = MoveJson.DecodeFrom(reader);
                if (move == null) break;
                moves.Add(move);
            }
            return new ReplayData(){Futures = futures.ToArray(), Moves = moves.ToArray()};

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

        public static MoveJson Parse(string arg)
        {
            var args = arg.Substring(1).Split('|');
            return new MoveJson(new ClaimMove(int.Parse(args[0]), int.Parse(args[1]), int.Parse(args[2])));

        }

        public static MoveJson DecodeFrom(BinaryReader reader)
        {
            byte marker = reader.ReadByte();
            if (marker == 0) return null;
            int punterId = reader.ReadInt32();
            int source = reader.ReadInt32();
            int target = reader.ReadInt32();
            return new MoveJson(new ClaimMove(punterId, source, target));
        }

        public void EncodeTo(BinaryWriter w)
        {
            if (Claim == null) return;
            w.Write((byte) 1);
            w.Write(Claim.PunterId);
            w.Write(Claim.Source);
            w.Write(Claim.Target);
        }
    }
}
