using lib;
using lib.Ai;
using lib.Scores.Simple;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using lib.Ai.StrategicFizzBuzz;
using worker.Strategies;

namespace worker
{
    

    public class MaxReachableVertexWeightAIExperiment : IExperiment
    {
        public Result Play(Task task)
        {
            return ExperimentCommon.Run(task,
                player => new MaxReachableVertexWeightAi(player.Params["MineWeight"]));
        }
    }
}
