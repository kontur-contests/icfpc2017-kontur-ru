using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using lib.GraphImpl;
using ProbabilityMethods;

namespace lib.Strategies
{
	class SimpleScoreBuilder
	{
		public Graph Graph;
		public Dictionary<int, Dictionary<int, MineNodeCharacteristic>> DistanceFromMine = new Dictionary<int, Dictionary<int, MineNodeCharacteristic>>();

		public SimpleScoreBuilder(Graph graph)
		{
			Graph = graph;
			DistanceFromMine = GetScores();
		}

		private Dictionary<int, Dictionary<int, MineNodeCharacteristic>> GetScores()
		{
			return Graph.Vertexes
				.Where(vertex => vertex.Value.IsMine)
				.ToDictionary(mine => mine.Key, mine => GetScoresFromMine(mine.Key));
		}

		private Dictionary<int, MineNodeCharacteristic> GetScoresFromMine(int mine)
		{
			Dictionary<int, MineNodeCharacteristic> used = new Dictionary<int, MineNodeCharacteristic>();
			Queue<int> vertices = new Queue<int>();
			vertices.Enqueue(mine);
			used.Add(mine, new MineNodeCharacteristic{Vertex = mine, Mine = mine, ChainProbability = Probability.One});
			while (vertices.Count > 0)
			{
				var vertex = vertices.Dequeue();
				var vertexNode = Graph.Vertexes[vertex];
				var vertexCharct = used[vertex];

				const double saveProbability = 0.99;

				foreach (var edge in vertexNode.Edges)
				{
					if (used.TryGetValue(edge.To, out var nextChrct))
					{
						if (nextChrct.Distance == vertexCharct.Distance + 1)
						{
							nextChrct.PreviousOptimalEdges.Add(vertex);
							nextChrct.ChainProbability = Probability.From(1-(1-nextChrct.ChainProbability.GetProbability())*(1-vertexCharct.ChainProbability.GetProbability()* saveProbability));
						}
						continue;
					}
					used[edge.To] = new MineNodeCharacteristic
					{
						Vertex = edge.To,
						Mine = mine,
						Distance = vertexCharct.Distance + 1,
						ChainProbability = saveProbability * vertexCharct.ChainProbability
					};
					used[edge.To].PreviousOptimalEdges.Add(vertex);
				}
			}

			return used;
		}
	}

	class MineNodeCharacteristic
	{
		public int Mine;
		public int Vertex;
		public int Distance;
		public List<int> PreviousOptimalEdges = new List<int>();
		public Probability ChainProbability = Probability.Zero;
	}
}
