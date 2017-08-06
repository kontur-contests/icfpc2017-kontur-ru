using JetBrains.Annotations;
using lib.Strategies.EdgeWeighting;

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
                (state, services) => new AllComponentsEWStrategy(
                    new MineGuardianEdgeWeighter(state, services, outEdgesWarningLevel, guardedComponentSizeThreshold), state, services),
                (state, services) => new BiggestComponentEWStrategy(new MaxVertextWeighter(mineMultiplier, state, services), state, services))
        {
        }

        public override string Version => "1.0";
    }
}