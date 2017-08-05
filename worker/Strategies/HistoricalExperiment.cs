using lib.QualityControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace worker.Strategies
{
    class HistoricalExperiment : IExperiment
    {
        public Result Play(Task task)
        {
            var ages = AiByGeneration.Generations.ToList();

            return ExperimentCommon.Run(
                task,
                player => ages[ages.Count - 1 - (int)Math.Round(player.Params["Age"])]()
                );
        }
    }
}
