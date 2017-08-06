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
        private readonly bool eatExceptions;

        public GameSimulatorRunner(IScoreCalculator scoreCalculator, bool silent = false, bool eatExceptions = false)
        {
            this.scoreCalculator = scoreCalculator;
            this.silent = silent;
            this.eatExceptions = eatExceptions;
        }

        public List<GameSimulationResult> SimulateGame(List<IAi> gamers, Map map, Settings settings)
        {
            var gameSimulator = new GameSimulator(map, settings, eatExceptions);
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
                .Select((e, i) => new GameSimulationResult(e.Gamer, scoreCalculator.GetScore(i, state.CurrentMap, e.Futures), gameSimulator.GetLastException(e.Gamer)))
                .ToList();
        }
    }

    public class GameSimulationResult
    {
        public GameSimulationResult(IAi gamer, long gamerScore, Exception lastException)
        {
            Gamer = gamer;
            Score = gamerScore;
            LastException = lastException;
        }

        public IAi Gamer { get; }
        public long Score { get; }
        public Exception LastException { get; }
    }
}