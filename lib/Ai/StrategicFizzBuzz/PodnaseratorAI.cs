using System;
using System.Linq;
using lib.GraphImpl;
using lib.StateImpl;
using lib.Strategies;
using lib.Strategies.EdgeWeighting;
using lib.Structures;
using MoreLinq;

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
        protected PodnaseratorAi(PodnaseratorSettings settings, Func<int, State, IServices, IStrategy> strategyProvider)
        {
            Settings = settings;
            StrategyProvider = strategyProvider;
        }

        public PodnaseratorSettings Settings { get; }
        private Func<int, State, IServices, IStrategy> StrategyProvider { get; }
        public abstract string Name { get; }
        public abstract string Version { get; }

        public AiSetupDecision Setup(State state, IServices services)
        {
            services.Setup<GraphService>(state);
            StrategyProvider(state.punter, state, services);
            Enumerable.Range(0, state.punters)
                .Except(new[] { state.punter })
                .Select(enemyId => StrategyProvider(enemyId, state, services))
                .ToArray();
            return AiSetupDecision.Empty();
        }

        public AiMoveDecision GetNextMove(State state, IServices services)
        {
            var myStrategy = StrategyProvider(state.punter, state, services);
            var enemyStrategies = Enumerable.Range(0, state.punters)
                .Except(new[] { state.punter })
                .Select(enemyId => StrategyProvider(enemyId, state, services))
                .ToArray();
            var bestTurn = GetMyBestTurn(state, services, myStrategy);
            var enemyBestTurns = enemyStrategies
                .Select(s => s.Turn(state, services))
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
            return AiMoveDecision.Claim(state.punter, bestTurn.River.Source, bestTurn.River.Target);
        }
        
        private TurnResult GetMyBestTurn(State state, IServices services, IStrategy myStrategy)
        {
            var turns = myStrategy.Turn(state, services);
            if (!turns.Any())
                return new TurnResult
                {
                    Estimation = -1
                };
            return turns.MaxBy(x => x.Estimation);
        }
    }

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
                (punterId, state, services) => 
                new BiggestComponentEWStrategy(
                    punterId,
                    new MaxVertextWeighter(mineMultiplier, state, services),
                    services.Get<MineDistCalculator>(state)))
        {
        }

        public override string Name => nameof(Podnaserator2000Ai);
        public override string Version => "2000";
    }
}