using System;
using System.Collections.Generic;
using System.Linq;
using lib.Ai;
using lib.Scores;
using lib.Structures;

namespace lib
{
    public class GameSimulatorRunner
    {
        private readonly IScoreCalculator scoreCalculator;
        private readonly bool silent;

        public GameSimulatorRunner(IScoreCalculator scoreCalculator, bool silent = false)
        {
            this.scoreCalculator = scoreCalculator;
            this.silent = silent;
        }

        public List<GameSimulationResult> SimulateGame(List<IAi> gamers, Map map, Settings settings)
        {
            var gameSimulator = new GameSimulator(map, settings);
            gameSimulator.StartGame(gamers);
            var state = gameSimulator.NextMove();
            while (!state.IsGameOver)
            {
                var lastMove = state.PreviousMoves.Last();

                if (!silent)
                    Console.WriteLine($"{lastMove}");

                state = gameSimulator.NextMove();
            }

            return gamers
                .Zip(gameSimulator.Futures, (ai, futures) => new {Gamer = ai, Futures = futures})
                .Select((e, i) => new GameSimulationResult(e.Gamer, scoreCalculator.GetScore(i, state.CurrentMap, e.Futures)))
                .ToList();
        }
    }

    public class GameSimulationResult
    {
        public GameSimulationResult(IAi gamer, long gamerScore)
        {
            Gamer = gamer;
            Score = gamerScore;
        }

        public IAi Gamer { get; }
        public long Score { get; }
    }
}