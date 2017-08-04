using System;

namespace lib
{
    public class OffineInteraction : IServerInteraction
    {
        public OffineInteraction(ITransport transport)
        {
            this.transport = new OfflineHighTransport(transport);
        }

        public void Start(string name, GameState state)
        {
            var setup = transport.ReadSetup();
            transport.WriteInitialState(setup.OurId, state);
            SetupRecieved?.Invoke(setup);

            while (true)
            {
                var answer = transport.ReadMoves();
                var result = HandleMove.Invoke(answer.Item1, answer.Item2);
                transport.SendMove(result.Item1, result.Item2);
                if (answer.Item1.Length + 1 == setup.Map.Rivers.Length)
                    break;
            }

            var end = transport.ReadScore();
            GameEnded?.Invoke(end.Item1, end.Item2, end.Item3);
        }

        public event Action<Setup> SetupRecieved;
        public Func<AbstractMove[], GameState, Tuple<AbstractMove, GameState>> HandleMove { private get; set; }
        public event Action<AbstractMove[], Score[], GameState> GameEnded;

        private readonly OfflineHighTransport transport;
    }
}