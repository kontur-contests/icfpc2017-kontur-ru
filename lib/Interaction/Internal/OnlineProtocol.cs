using System;
using System.Linq;
using System.Security.Cryptography;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace lib.Interaction.Internal
{
    internal class OnlineProtocol : ProtocolBase
    {
        public bool IsGameOver { get; private set; }

        public OnlineProtocol(ITransport transport)
        {
            this.transport = transport;
        }

        public void HandShake(string name)
        {
            transport.Write($"{{\"me\":\"{name}\"}}");
            transport.Read();
        }

        public Setup ReadSetup()
        {
            return JsonConvert.DeserializeObject<Setup>(transport.Read());
        }

        public void WriteSetupReply(SetupReply reply)
        {
            transport.Write(Serialize(reply));
        }

        public void WriteMove(Move move)
        {
            transport.Write(SerializeMove(move));
        }

        public JToken ReadGameState()
        {
            var state = JObject.Parse(transport.Read());
            if (state["stop"] != null)
            {
                IsGameOver = true;
                return state["stop"];
            }
            if (state["move"] != null)
            {
                return state["move"];
            }
            throw new TimeoutException("azaza");
        }

        public Move[] GetMoves(JToken moves)
        {
            return moves.ToObject<LastRoundModel>().MoveModels.Select(MoveModel.GetMove).ToArray();
        }

        public ScoreData GetScore(JToken score)
        {
            return score.ToObject<ScoreData>();
        }

        public Tuple<Move[], ScoreModel[]> ReadScore()
        {
            var scoreJsonResponse = JsonConvert.DeserializeObject<ScoreJsonResponse>(transport.Read());
            return Tuple.Create(scoreJsonResponse.ScoreData.MoveModels.Select(MoveModel.GetMove).ToArray(), scoreJsonResponse.ScoreData.Scores);
        }

        private readonly ITransport transport;
    }
}