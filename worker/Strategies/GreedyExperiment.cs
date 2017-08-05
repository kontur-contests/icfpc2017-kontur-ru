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
        public Result Play(Task task)
        {
            return ExperimentCommon.Run(
                task,
                player => new GreedyAi());
        }
    }
}
