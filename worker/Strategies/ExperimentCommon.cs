using lib;
using lib.Ai;
using lib.Scores.Simple;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace worker.Strategies
{
    class ExperimentCommon
    {
        public static List<PlayerResult> Run(List<PlayerWithParams> players, Func<PlayerWithParams,IAi> selector, string map)
        {
            var ais = players.Select(selector).ToList();
            var gameSimulator = new GameSimulatorRunner(new SimpleScoreCalculator());
            var results = gameSimulator.SimulateGame(ais, MapLoader.LoadMapByName(map).Map);
            return results.Select(z => new PlayerResult { Scores = z.Score }).ToList();
        }
    }
}
