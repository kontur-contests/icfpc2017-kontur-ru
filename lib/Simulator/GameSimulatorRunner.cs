using System;
using System.Collections.Generic;
using System.Linq;
using lib.Ai;
using lib.Scores;

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

        public List<GameSimulationResult> SimulateGame(List<IAi> gamers, Map map)
        {
            var gameSimulator = new GameSimulator(map);
            gameSimulator.StartGame(gamers);
            var state = gameSimulator.NextMove();
            while (!state.IsGameOver)
            {
                var lastMove = state.PreviousMoves.Last();

                if (lastMove is ClaimMove claimMove && !silent)
                    Console.WriteLine($"PunterId: {claimMove.PunterId} move source: {claimMove.Source} target: {claimMove.Target}");
                if (lastMove is PassMove passMove && !silent)
                    Console.WriteLine($"PunterId: {passMove.PunterId} pass");

                state = gameSimulator.NextMove();
            }

            var results = new List<GameSimulationResult>();
            for (int i = 0; i < gamers.Count; i++)
            {
                results.Add(new GameSimulationResult(gamers[i], scoreCalculator.GetScore(i, state.CurrentMap)));
            }

            return results;
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