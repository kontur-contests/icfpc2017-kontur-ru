using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace lib
{
    internal class OnlineHighTransport
    {
        public OnlineHighTransport(ITransport transport)
        {
            this.transport = transport;
        }

        public void HandShake(string name)
        {
            transport.Write($"{{\"me\":\"{name}\"}}");
            var answer = transport.Read();
            if (!answer.Contains($"\"{name}\""))
                throw new InvalidOperationException($"Incorrect server handsnake: {answer}");
        }

        public Setup ReadSetup()
        {
            var data = JsonConvert.DeserializeObject<Setup>(transport.Read());
            transport.Write($"{{\"ready\":\"{data.OurId}\"}}");
            return data;
        }

        public AbstractMove[] ReadMoves()
        {
            var data = JsonConvert.DeserializeObject<MoveServerData>(transport.Read());
            return data.Moves.Moves.Select(DeserializeMove).ToArray();
        }

        private AbstractMove DeserializeMove(string move)
        {
            var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(move);
            if (data.Count != 1)
                throw new InvalidOperationException(move);
            switch (data.Keys.First().ToLowerInvariant())
            {
                case "claim":
                    return JsonConvert.DeserializeObject<ClaimSerializable>(move).Claim;
                case "pass":
                    return JsonConvert.DeserializeObject<PassSerializable>(move).Pass;
                default:
                    throw new InvalidOperationException(move);
            }
        }

        public void SendMove(AbstractMove abstractMove)
        {
            switch (abstractMove)
            {
                case Move move:
                    transport.Write(JsonConvert.SerializeObject(new ClaimSerializable(move)));
                    break;
                case Pass move:
                    transport.Write(JsonConvert.SerializeObject(new PassSerializable(move)));
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        private class MoveServerData
        {
            [JsonProperty("move")]
            public MoveDataData Moves { get; set; }

            public class MoveDataData
            {
                [JsonProperty("moves")]
                public string[] Moves { get; set; }
            }
        }

        private class ClaimSerializable
        {
            public ClaimSerializable(Move move) => Claim = move;

            [JsonProperty("claim")]
            public Move Claim { get; set; }
        }

        private class PassSerializable
        {
            public PassSerializable(Pass move) => Pass = move;

            [JsonProperty("pass")]
            public Move Pass { get; set; }
        }

        private readonly ITransport transport;

        public Tuple<Move[], Score[]> ReadScore()
        {
            throw new NotImplementedException();
        }
    }
}