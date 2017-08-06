using System;
using System.Collections.Generic;
using lib.Ai;
using lib.Ai.StrategicFizzBuzz;

namespace lib.QualityControl
{
    public static class AiByGeneration
    {
        public static IEnumerable<Func<IAi>> Generations
        {
            get
            {
                yield return () => new GreedyAi();
                yield return () => new ConnectClosestMinesAi();
                yield return () => new LochKillerAi();
                yield return () => new MaxReachableVertexWeightAi();
                yield return () => new LochMaxVertexWeighterKillerAi();
                yield return () => new Podnaserator2000Ai();
                yield return () => new FutureIsNowAi();
                yield return () => new LochDinicKillerAi();
                yield return () => new MaxReachableVertexWeightAiWithMineGuardianAi();

            }
        }
    }
}