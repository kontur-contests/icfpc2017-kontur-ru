using System;
using System.Linq;
using lib.GraphImpl;
using lib.StateImpl;
using lib.Strategies;
using lib.Strategies.EdgeWeighting;
using lib.viz;
using MoreLinq;
using static lib.Strategies.EdgeWeighting.MetaStrategyHelpers;

namespace lib.Ai.StrategicFizzBuzz
{
    public class PodnaseratorSettings
    {
        public PodnaseratorSettings(double enemyTurnEstimationDifferenceWeight, double myTurnEsimationWeight)
        {
            EnemyTurnEstimationDifferenceWeight = enemyTurnEstimationDifferenceWeight;
            MyTurnEsimationWeight = myTurnEsimationWeight;
        }

        public double EnemyTurnEstimationDifferenceWeight { get; }
        public double MyTurnEsimationWeight { get; }
    }

    public abstract class PodnaseratorAi : IAi
    {
        protected PodnaseratorAi(PodnaseratorSettings settings, Func<State, IServices, IStrategy> strategyProvider)
        {
            Settings = settings;
            StrategyProvider = strategyProvider;
        }

        public PodnaseratorSettings Settings { get; }
        private Func<State, IServices, IStrategy> StrategyProvider { get; }
        public string Name => GetType().Name;
        public abstract string Version { get; }

        public AiSetupDecision Setup(State state, IServices services)
        {
            services.Setup<Graph>();
            Enumerable.Range(0, state.punters)
                .Select(punterId => StrategyProvider(state, services))
                .Consume();
            return AiSetupDecision.Empty();
        }

        public AiMoveDecision GetNextMove(State state, IServices services)
        {
            var myStrategy = StrategyProvider(state, services);
            var enemyStrategies = Enumerable.Range(0, state.punters)
                .Except(new[] { state.punter })
                .Select(enemyId => StrategyProvider(state, services))
                .ToArray();
            var bestTurn = GetMyBestTurn(myStrategy);
            var enemyBestTurns = enemyStrategies
                .Select(s => s.NextTurns())
                .Where(ts => ts.Count >= 2)
                .Select(ts => ts.OrderByDescending(x => x.Estimation).Take(2).ToArray())
                .ToArray();
            if (enemyBestTurns.Any())
            {
                var bestestEnemyTurns = enemyBestTurns.MaxBy(ts => ts[0].Estimation - ts[1].Estimation);
                if (bestestEnemyTurns[0].Estimation > Settings.EnemyTurnEstimationDifferenceWeight *
                    bestestEnemyTurns[1].Estimation &&
                    bestestEnemyTurns[0].Estimation > Settings.MyTurnEsimationWeight * bestTurn.Estimation)
                    bestTurn = bestestEnemyTurns[0];
            }
            if (bestTurn.Estimation < 0)
                return AiMoveDecision.Pass(state.punter);
            return bestTurn.Move;
        }

        private TurnResult GetMyBestTurn(IStrategy myStrategy)
        {
            var turns = myStrategy.NextTurns();
            if (!turns.Any())
                return new TurnResult
                {
                    Estimation = -1
                };
            return turns.MaxBy(x => x.Estimation);
        }
    }

    [Obsolete("Broken. Can't run strategy for different punter ids")]
    [ShouldNotRunOnline] // T-16:30
    public class Podnaserator2000Ai : PodnaseratorAi
    {
        public Podnaserator2000Ai()
            : this(0, 2, 100)
        {
        }

        public Podnaserator2000Ai(
            int enemyTurnEstimationDifferenceWeight,
            int myTurnEsimationWeight,
            int mineMultiplier)
            : base(
                new PodnaseratorSettings(enemyTurnEstimationDifferenceWeight, myTurnEsimationWeight),
                BiggestComponentEWStrategy((state, services) => new MaxVertextWeighter(mineMultiplier, state, services)))
        {
        }

        public override string Version => "2000";
    }
}