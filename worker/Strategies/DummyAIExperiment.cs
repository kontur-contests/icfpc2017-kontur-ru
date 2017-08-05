using lib;
using lib.Ai;
using lib.Scores.Simple;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using worker.Strategies;

namespace worker
{
    

    public class DummyAIExperiment : IExperiment
    {
        public Result Play(Task task)
        {
            return ExperimentCommon.Run(task,
                player => new DummyAi(player.Params["Param"]) { Name = player.Name });
        }
    }
}