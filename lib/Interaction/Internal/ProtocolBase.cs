using System;
using Newtonsoft.Json;

namespace lib.Interaction.Internal
{
    internal abstract class ProtocolBase
    {
        protected class MoveServerData
        {
            [JsonProperty("move")]
            public LastRoundModel LastRound { get; set; }

            public class LastRoundModel
            {
                [JsonProperty("moves")]
                public Move[] Moves { get; set; }
            }
        }

        public class ScoreData
        {
            [JsonProperty("moves")]
            public Move[] Moves { get; set; }

            [JsonProperty("score")]
            public Score[] Scores { get; set; }
        }


        protected class ScoreJsonResponse
        {
            [JsonProperty("stop")]
            public ScoreData ScoreData { get; set; }
        }

        public static string SerializeMove(Move move)
        {
            switch (move)
            {
                case ClaimMove claimMove:
                    return JsonConvert.SerializeObject(new ClaimSerializable(claimMove));
                case PassMove passMove:
                    return JsonConvert.SerializeObject(new PassSerializable(passMove));
                default:
                    throw new NotImplementedException();
            }
        }

        private class PassSerializable
        {
            public PassSerializable(PassMove move) => Pass = move;

            [JsonProperty("pass")]
            public PassMove Pass { get; set; }
        }

        private class ClaimSerializable
        {
            public ClaimSerializable(ClaimMove move) => Claim = move;

            [JsonProperty("claim")]
            public ClaimMove Claim { get; set; }
        }
    }
}