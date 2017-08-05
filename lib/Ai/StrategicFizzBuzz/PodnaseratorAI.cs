using System;
using System.Linq;
using lib.GraphImpl;
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
        protected PodnaseratorAi(PodnaseratorSettings settings, Func<SuperSettings, IStrategy> strategyProvider)
        {
            Settings = settings;
            StrategyProvider = strategyProvider;
        }

        private int PunterId { get; set; }
        private IStrategy MyStrategy { get; set; }
        private IStrategy[] EnemyStrategies { get; set; }
        public PodnaseratorSettings Settings { get; }
        private Func<SuperSettings, IStrategy> StrategyProvider { get; }
        public abstract string Name { get; }
        public abstract string Version { get; }

        public virtual Future[] StartRound(int punterId, int puntersCount, Map map, Settings settings)
        {
            MyStrategy = StrategyProvider(new SuperSettings(punterId, puntersCount, map, settings));
            EnemyStrategies = Enumerable.Range(0, puntersCount)
                .Except(new[] {punterId})
                .Select(enemyId => StrategyProvider(new SuperSettings(enemyId, puntersCount, map, settings)))
                .ToArray();
            PunterId = punterId;
            return new Future[0];
        }

        public Move GetNextMove(Move[] prevMoves, Map map)
        {
            var graph = new Graph(map);
            var bestTurn = GetMyBestTurn(map);
            var enemyBestTurns = EnemyStrategies
                .Select(s => s.Turn(graph))
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
                return Move.Pass(PunterId);
            return Move.Claim(PunterId, bestTurn.River.Source, bestTurn.River.Target);
        }

        public string SerializeGameState()
        {
            return "";
        }

        public void DeserializeGameState(string gameState)
        {
        }

        private TurnResult GetMyBestTurn(Map map)
        {
            var turns = MyStrategy.Turn(new Graph(map));
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
                s => new EdgeWeightingStrategy(
                    s.Map, s.PunterId,
                    new MaxVertextWeighterWithConnectedComponents(new Graph(s.Map), mineMultiplier)))
        {
        }

        public override string Name => nameof(Podnaserator2000Ai);
        public override string Version => "2000";
    }
}