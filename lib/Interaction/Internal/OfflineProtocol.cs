using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace lib.Interaction.Internal
{
    internal class OfflineProtocol : ProtocolBase
    {
        public OfflineProtocol(ITransport transport)
        {
            this.transport = transport;
        }

        public Setup ReadSetup()
        {
            return JsonConvert.DeserializeObject<Setup>(transport.Read());
        }

        public void WriteInitialState(string ourId, GameState state)
        {
            var stateData = JsonConvert.SerializeObject(state);
            transport.Write($"{{\"ready\":\"{ourId}\", \"state\":\"{stateData}\"}}");
        }

        public Tuple<Move[], GameState> ReadMoves()
        {
            var data = JsonConvert.DeserializeObject<OfflineMoveServerData>(transport.Read());
            var moves = data.LastRound.Moves;
            var state = data.State;
            return Tuple.Create(moves, state);
        }

        public void WriteMove(Move move, GameState newState)
        {
            var moveData = SerializeMove(move);
            var stateData = JsonConvert.SerializeObject(newState);
            moveData = moveData.TrimEnd('}') + $"\"state\":\"{stateData}\"}}";
            transport.Write(moveData);
        }

        public Tuple<Move[], Score[], GameState> ReadScore()
        {
            var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(transport.Read());
            var scoreData = JsonConvert.DeserializeObject<ScoreData>(data["stop"]);
            var state = JsonConvert.DeserializeObject<GameState>(data["state"]);
            return Tuple.Create(scoreData.Moves, scoreData.Scores, state);
        }

        private class OfflineMoveServerData : MoveServerData
        {
            [JsonProperty("state")]
            public GameState State { get; set; }
        }

        private readonly ITransport transport;
    }
}