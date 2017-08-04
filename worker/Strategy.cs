using System;
using System.Collections.Generic;
using System.Linq;

namespace worker
{
    public interface IPlayerStrategy
    {
        IEnumerable<Tuple<PlayerWithParams, int>> Play(List<PlayerWithParams> players);
    }
    
    public class DummySumPlayerStrategy : IPlayerStrategy
    {
        public IEnumerable<Tuple<PlayerWithParams, int>> Play(List<PlayerWithParams> players)
        {
            return players
                .OrderBy(player => player.Params.Values.Sum())
                .Select((player, i) => Tuple.Create(player, i));
        }
    }
}