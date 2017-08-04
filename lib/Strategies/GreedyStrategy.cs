using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using lib.GraphImpl;

namespace lib.Strategies
{
	class GreedyStrategy : IStrategy
	{
		private Graph Graph;
		private SimpleScoreBuilder ScoreBuilder;

		public GreedyStrategy(Graph graph, SimpleScoreBuilder scorer)
		{
			Graph = graph;
		}

		public List<TurnResult> Turn(Graph graph, IList<int> activePoints)
		{
			throw new NotImplementedException();
		}
	}
}
