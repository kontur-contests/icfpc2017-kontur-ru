using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace lib.Interaction.Internal
{
    internal abstract class HightTransport
    {
        protected AbstractMove DeserializeMove(string move)
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

        protected string SerializeMove(AbstractMove abstractMove)
        {
            switch (abstractMove)
            {
                case Move move:
                    return JsonConvert.SerializeObject(new ClaimSerializable(move));
                case Pass move:
                    return JsonConvert.SerializeObject(new PassSerializable(move));
                default:
                    throw new InvalidOperationException();
            }
        }

        protected class MoveServerData
        {
            [JsonProperty("move")]
            public MoveDataData Moves { get; set; }

            public class MoveDataData
            {
                [JsonProperty("moves")]
                public string[] Moves { get; set; }
            }
        }

        protected class ScoreData
        {
            [JsonProperty("moves")]
            public string[] Moves { get; set; }

            [JsonProperty("score")]
            public Score[] Scores { get; set; }
        }

        private class ClaimSerializable
        {
            public ClaimSerializable(Move move) => Claim = move;

            [JsonProperty("claim")]
            public Move Claim { get; set; }
        }

        private class PassSerializable
        {
            public PassSerializable(Pass pass) => Pass = pass;

            [JsonProperty("pass")]
            public Pass Pass { get; set; }
        }
    }
}