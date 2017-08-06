using System;

namespace worker.Strategies
{
    internal class ExperimentSelector : IExperiment
    {
        public Result Play(Task task)
        {
            switch (task.Experiment)
            {
                case "Greedy": return new GreedyExperiment().Play(task);
                case "Historical": return new HistoricalExperiment().Play(task);
                case "MRVW": return new MaxReachableVertexWeightAIExperiment().Play(task);
                case "Uber": return new UberExperiment().Play(task);
                default: throw new Exception("Experiment type '" + task.Experiment + "' is not recognized");
            }
        }
    }
}