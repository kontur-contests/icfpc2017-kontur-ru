using lib.Strategies;
using lib.Strategies.EdgeWeighting;
using lib.Strategies.StrategiesCatalog;

namespace lib.Ai.StrategicFizzBuzz
{
    public class TheUberfullessnessAi : UberfullessnessAi
    {
        public TheUberfullessnessAi()
            : base(
                StrategyName.ForSetup<FutureIsNowSetupStrategy>(),
                StrategyName.For<FutureIsNowStrategy>(),
                StrategyName.ForEWStrategy<AllComponentsEWStrategy, MineGuardianEdgeWeighter>(),
                StrategyName.ForEWStrategy<AllComponentsEWStrategy, MaxVertextWeighter>())
        {
        }
    }
}