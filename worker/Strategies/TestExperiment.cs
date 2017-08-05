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
        public IEnumerable<Tuple<PlayerWithParams, long>> Play(Task task)
        {
            return task.Players
            .OrderBy(player => player.Params.Values.Sum())
            .Select((player, i) => Tuple.Create(player, (long)i));
        }
    }
}