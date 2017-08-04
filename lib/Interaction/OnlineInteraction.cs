using System;
using lib.Interaction.Internal;

namespace lib.Interaction
{
    public class OnlineInteraction : IServerInteraction
    {
        public OnlineInteraction(ITransport transport)
        {
            this.transport = new OnlineHighTransport(transport);
        }

        public void Start(string name, GameState state)
        {
            transport.HandShake(name);

            var setup = transport.ReadSetup();
            SetupRecieved?.Invoke(setup);

            while (true)
            {
                var moves = transport.ReadMoves();
                var result = HandleMove.Invoke(moves, state);
                state = result.Item2;
                transport.SendMove(result.Item1);
                if (moves.Length + 1 == setup.Map.Rivers.Length)
                    break;
            }

            var end = transport.ReadScore();
            GameEnded?.Invoke(end.Item1, end.Item2, state);
        }

        public event Action<Setup> SetupRecieved;
        public Func<AbstractMove[], GameState, Tuple<AbstractMove, GameState>> HandleMove { private get; set; }
        public event Action<AbstractMove[], Score[], GameState> GameEnded;

        private readonly OnlineHighTransport transport;
    }
}