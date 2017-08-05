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
        public IEnumerable<Tuple<PlayerWithParams, long>> Play(List<PlayerWithParams> players)
        {
            return players
            .OrderBy(player => player.Params.Values.Sum())
            .Select((player, i) => Tuple.Create(player, (long)i));
        }
    }
}