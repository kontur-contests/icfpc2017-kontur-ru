using lib.Ai;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lib.QualityControl
{
    public static class AiByGeneration
    {
        public static IEnumerable<Func<IAi>> Generations
        {
            get
            {
                yield return () => new DummyAi(0.8);
                yield return () => new GreedyAi();
                yield return () => new ConnectClosestMinesAi();
            }
        }
    }
}
