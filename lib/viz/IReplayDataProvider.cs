using System.Collections.Generic;
using System.Linq;
using lib.Ai;
using lib.Structures;

namespace lib.viz
{
    public interface IReplayDataProvider
    {
        string[] PunterNames { get; }
        Future[] GetPunterFutures(int index);
        GameState NextMove();
    }

    public class SimulatorReplayDataProvider : IReplayDataProvider
    {
        private readonly List<IAi> ais;
        private readonly GameSimulator simulator;

        public SimulatorReplayDataProvider(List<IAi> ais, Map map, Settings settings)
        {
            this.ais = ais;
            simulator = new GameSimulator(map, settings);
            simulator.StartGame(ais);
        }

        public string[] PunterNames => ais.Select(ai => ai.Name).ToArray();

        public Future[] GetPunterFutures(int index)
        {
            return simulator.Futures[index];
        }

        public GameState NextMove()
        {
            return simulator.NextMove();
        }
    }
}