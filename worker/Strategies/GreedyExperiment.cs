using lib;
using lib.Ai;
using lib.Scores.Simple;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace worker.Strategies
{
    class GreedyExperiment : IExperiment
    {
        public IEnumerable<Tuple<PlayerWithParams, long>> Play(Task task)
        {
            return ExperimentCommon.Run(
                task.Players,
                player => new GreedyAi(),
                "sample.json");
        }
    }
}
