using lib;
using lib.Ai;
using lib.Scores.Simple;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace worker
{

    
    public class TestExperiment : IExperiment
    {
        public List<PlayerResult> Play(Task task)
        {
            return task.Players.Select(z => new PlayerResult { Scores = (long)(z.Params.Values.Sum() * 10000) }).ToList();
            
        }
    }
}