using System.Collections.Generic;
using System.Linq;
using lib.Ai;

namespace lib.viz
{
    public interface IReplayDataProvider
    {
        string[] PunterNames { get; }
        GameState NextMove();
    }

    public class SimulatorReplayDataProvider : IReplayDataProvider
    {
        private readonly List<IAi> ais;
        private readonly GameSimulator simulator;

        public SimulatorReplayDataProvider(List<IAi> ais, Map map)
        {
            this.ais = ais;
            simulator = new GameSimulator(map);
            simulator.StartGame(ais);
        }

        public string[] PunterNames => ais.Select(ai => ai.Name).ToArray();
        public GameState NextMove()
        {
            return simulator.NextMove();
        }
    }
}