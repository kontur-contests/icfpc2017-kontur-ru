using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using lib.Ai.StrategicFizzBuzz;
using lib.GraphImpl;
using lib.Strategies.EdgeWeighting;

namespace lib.Ai
{
    [UsedImplicitly]
    public class AgileMaxVertexWeighterAi : EdgeWeightingStrategicAi
    {
        public AgileMaxVertexWeighterAi() : this(100)
        {
        }

        public AgileMaxVertexWeighterAi(double mineWeight)
            : base((state, services) => new AgileMaxVertexWeighter(mineWeight, services.Get<MineDistCalculator>(state)))
        {
        }

        public override string Version => "1.0";
    }
}
