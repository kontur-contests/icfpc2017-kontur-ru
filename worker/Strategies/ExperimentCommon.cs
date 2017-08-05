using lib;
using lib.Ai;
using lib.Scores.Simple;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using lib.Structures;

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
            var rankings = Enumerable.Range(0, results.Count).OrderByDescending(z => results[z].Score).ToList();
            var playerResults = results
                .Zip(rankings, (res, ranking) => new PlayerResult
                {
                    Scores = res.Score,
                    ServerName = res.Gamer.Name,
                    Ranking = ranking,
                    TournamentScore = rankings.Count - ranking
                }).ToList();
            

            var result = new Result
            {
                Task = task,
                Results = playerResults,
                RiversCount = map.Rivers.Length,
                SitesCount = map.Sites.Length,
                MinesCount = map.Mines.Length
            };
            return result;

        }
    }
}
