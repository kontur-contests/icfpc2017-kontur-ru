using lib.Ai.StrategicFizzBuzz;

namespace worker.Strategies
{
    internal class GreedyExperiment : IExperiment
    {
        public Result Play(Task task)
        {
            return ExperimentCommon.Run(
                task,
                player => new GreedyAi());
        }
    }
}