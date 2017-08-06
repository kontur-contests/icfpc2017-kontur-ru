using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace worker.Strategies
{
    class ExperimentSelector : IExperiment
    {
        public Result Play(Task task)
        {
            switch (task.Experiment)
            {
                case "DummyAI": return new DummyAIExperiment().Play(task);
                case "Greedy": return new GreedyExperiment().Play(task);
                case "Historical":return new HistoricalExperiment().Play(task);
                case "MRVW": return new MaxReachableVertexWeightAIExperiment().Play(task);
                default: throw new Exception("Experiment type '" + task.Experiment + "' is not recognized");
            }
        }
    }
}
