using System;
using Newtonsoft.Json;

namespace lib.Interaction.Internal
{
    internal class OnlineProtocol : ProtocolBase
    {
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
            var setup = JsonConvert.DeserializeObject<Setup>(transport.Read());
            transport.Write($"{{\"ready\":\"{setup.Id}\"}}");
            return setup;
        }

        public Move[] ReadMoves()
        {
            var moves = JsonConvert.DeserializeObject<MoveServerData>(transport.Read());
            return moves.LastRound.Moves;
        }

        public void WriteMove(Move move)
        {
            transport.Write(SerializeMove(move));
        }

        public Tuple<Move[], Score[]> ReadScore()
        {
            var scoreJsonResponse = JsonConvert.DeserializeObject<ScoreJsonResponse>(transport.Read());
            return Tuple.Create(scoreJsonResponse.ScoreData.Moves, scoreJsonResponse.ScoreData.Scores);
        }

        private readonly ITransport transport;
    }
}