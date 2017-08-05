using lib.Strategies;
using lib.Strategies.EdgeWeighting;
using lib.Structures;

namespace lib.Ai.StrategicFizzBuzz
{
    public abstract class EdgeWeightingStrategicAi : StrategicAi
    {
        private IStrategy strategy;
        protected override IStrategy Strategy => strategy;
        public override Future[] StartRound(int punterId, int puntersCount, Map map, Settings settings)
        {
            strategy = new EdgeWeightingStrategy(map, punterId, CreateEdgeWeighter(punterId, puntersCount, map, settings));
            return base.StartRound(punterId, puntersCount, map, settings);
        }

        protected abstract IEdgeWeighter CreateEdgeWeighter(int punterId, int puntersCount, Map map, Settings settings);
    }
}