using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace lib.Interaction.Internal
{
    internal class OfflineHighTransport : HightTransport
    {
        public OfflineHighTransport(ITransport transport)
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

        public Tuple<AbstractMove[], GameState> ReadMoves()
        {
            var data = JsonConvert.DeserializeObject<MoveServerData>(transport.Read());
            var moves = data.Moves.Moves.Select(DeserializeMove).ToArray();
            var state = data.State;
            return Tuple.Create(moves, state);
        }

        public void SendMove(AbstractMove resultItem1, GameState resultItem2)
        {
            throw new NotImplementedException();
        }

        public Tuple<AbstractMove[], Score[], GameState> ReadScore()
        {
            throw new NotImplementedException();
        }



        private readonly ITransport transport;

        private class MoveServerData
        {
            [JsonProperty("move")]
            public MoveDataData Moves { get; set; }

            [JsonProperty("state")]
            public GameState State { get; set; }

            public class MoveDataData
            {
                [JsonProperty("moves")]
                public string[] Moves { get; set; }
            }
        }
    }
}