using System;
using lib.Interaction.Internal;

namespace lib.Interaction
{
    public class OffineInteraction : IServerInteraction
    {
        public OffineInteraction(ITransport transport)
        {
            this.transport = new OfflineProtocol(transport);
        }

        public void Start(string name, GameState state)
        {
            var setup = transport.ReadSetup();
            transport.WriteInitialState(setup.Id, state);
            SetupRecieved?.Invoke(setup);

            while (true)
            {
                var answer = transport.ReadMoves();
                var result = HandleMove.Invoke(answer.Item1, answer.Item2);
                transport.WriteMove(result.Item1, result.Item2);
                if (answer.Item1.Length + 1 == setup.Map.Rivers.Length)
                    break;
            }

            var end = transport.ReadScore();
            GameEnded?.Invoke(end.Item1, end.Item2, end.Item3);
        }

        public event Action<Setup> SetupRecieved;
        public Func<Move[], GameState, Tuple<Move, GameState>> HandleMove { private get; set; }
        public event Action<Move[], Score[], GameState> GameEnded;

        private readonly OfflineProtocol transport;
    }
}