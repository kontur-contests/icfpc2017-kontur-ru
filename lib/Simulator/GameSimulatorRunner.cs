using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using lib.Ai;
using lib.Scores;
using lib.Scores.Simple;
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
            
            var turnTime = new StatValue();
            
            while (!state.IsGameOver)
            {
                var lastMove = state.PreviousMoves.Last();

                if (!silent)
                    Console.WriteLine($"{lastMove}");

                var time = Stopwatch.StartNew();
                
                state = gameSimulator.NextMove();

                turnTime.Add(time.ElapsedMilliseconds);
            }
            map = state.CurrentMap;
            return gamers
                .Zip(gameSimulator.Futures, (ai, futures) => new {Gamer = ai, Futures = futures})
                .Select((e, i) => new GameSimulationResult(
                    e.Gamer, 
                    scoreCalculator.GetScoreData(i, state.CurrentMap, e.Futures), 
                    gameSimulator.GetLastException(e.Gamer),
                    (double)map.OptionsUsed.GetOrDefaultNoSideEffects(i, 0) / map.Mines.Length,
                    turnTime))
                .ToList();
        }
    }

    public class GameSimulationResult
    {
        public GameSimulationResult(IAi gamer, ScoreData gamerScore, Exception lastException, double optionsUsed, StatValue turnTime)
        {
            ScoreData = gamerScore;
            Gamer = gamer;
            LastException = lastException;
            TurnTime = turnTime;
            OptionsUsed = optionsUsed;
        }

        public IAi Gamer { get; }
        public ScoreData ScoreData { get; }
        public long Score => ScoreData.TotalScore;
        public int MatchScore { get; set; }
        public Exception LastException { get; }
        public StatValue TurnTime { get; }
        public double OptionsUsed { get; }
    }
}