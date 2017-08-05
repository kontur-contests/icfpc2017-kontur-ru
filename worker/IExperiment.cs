using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace worker
{
    public interface IExperiment
    {
        IEnumerable<Tuple<PlayerWithParams, long>> Play(Task task);
    }
}
