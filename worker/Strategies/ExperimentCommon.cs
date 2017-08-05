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
        public static Result Run(Task task, Func<PlayerWithParams,IAi> selector)
        {
            var ais = task.Players.Select(selector).ToList();
            var gameSimulator = new GameSimulatorRunner(new SimpleScoreCalculator());
            var map = MapLoader.LoadMapByName(task.Map).Map;
            
            var results = gameSimulator.SimulateGame(ais, map, new Settings());
            var playerResults =  results.Select(z => new PlayerResult { Scores = z.Score, ServerName = z.Gamer.Name }).ToList();
            var result = new Result
            {
                Task = task,
                Results = playerResults,
                 RiversCount = map.Rivers.Length,
                  SitesCount = map.Sites.Length
            };
            return result;

        }
    }
}
