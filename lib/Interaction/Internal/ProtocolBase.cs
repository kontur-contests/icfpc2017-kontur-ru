using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace lib.Interaction.Internal
{
    internal abstract class ProtocolBase
    {
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

        protected class MoveServerData
        {
            [JsonProperty("move")]
            public LastRoundModel LastRound { get; set; }
        }

        public class LastRoundModel
        {
            [JsonProperty("moves")]
            public MoveModel[] MoveModels { get; set; }
        }

        public class ScoreData
        {
            [JsonProperty("moves")]
            public MoveModel[] MoveModels { get; set; }

            [JsonProperty("scores")]
            public ScoreModel[] Scores { get; set; }

            public override string ToString()
            {
                return string.Join("\n", Scores.Select(s => $"{s.Punter} scored {s.Score}"));
            }
        }


        protected class ScoreJsonResponse
        {
            [JsonProperty("stop")]
            public ScoreData ScoreData { get; set; }
        }

        private class PassSerializable
        {
            public PassSerializable(PassMove move)
            {
                Pass = move;
            }

            [JsonProperty("pass")]
            public PassMove Pass { get; set; }
        }

        private class ClaimSerializable
        {
            public ClaimSerializable(ClaimMove move)
            {
                Claim = move;
            }

            [JsonProperty("claim")]
            public ClaimMove Claim { get; set; }
        }

        public class ServerResponseModel
        {
            [JsonProperty("move")]
            public JObject LastRound { get; set; }

            [JsonProperty("stop")]
            public JObject ScoreData { get; set; }
        }

        public class MoveModel
        {
            [JsonProperty("claim")] public ClaimMove ClaimMove;
            [JsonProperty("pass")] public PassMove PassMove;

            public static Move GetMove(MoveModel model)
            {
                if (model.ClaimMove != null)
                    return model.ClaimMove;
                if (model.PassMove != null)
                    return model.PassMove;
                throw new NotImplementedException();
            }
        }
    }
}