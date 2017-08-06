using JetBrains.Annotations;
using lib.Strategies.EdgeWeighting;
using static lib.Strategies.EdgeWeighting.MetaStrategyHelpers;

namespace lib.Ai.StrategicFizzBuzz
{
    [UsedImplicitly]
    public class MaxReachableVertexWeightAiWithMineGuardianAi : CompositeStrategicAi
    {
        public MaxReachableVertexWeightAiWithMineGuardianAi()
            : this(3, 10, 100)
        {
        }

        public MaxReachableVertexWeightAiWithMineGuardianAi(int outEdgesWarningLevel, int guardedComponentSizeThreshold, int mineMultiplier)
            : base(
                AllComponentsEWStrategy((state, services) => new MineGuardianEdgeWeighter(state, services, outEdgesWarningLevel, guardedComponentSizeThreshold)),
                AllComponentsEWStrategy((state, services) => new MaxVertextWeighter(mineMultiplier, state, services)))
        {
        }

        public override string Version => "1.1";
    }
}