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
        public List<PlayerResult> Play(Task task)
        {
            return ExperimentCommon.Run(task.Players,
                player => new DummyAi(player.Params["Param"]) { Name = player.Name },
                task.Map??"sample.json");
        }
    }
}