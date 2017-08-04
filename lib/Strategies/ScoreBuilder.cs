using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using lib.GraphImpl;

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
			return Graph.vertexes
				.Where(vertex => vertex.Value.IsMine)
				.ToDictionary(mine => mine.Key, mine => GetScoresFromMine(mine.Key));
		}

		private Dictionary<int, MineNodeCharacteristic> GetScoresFromMine(int mine)
		{
			Dictionary<int, MineNodeCharacteristic> used = new Dictionary<int, MineNodeCharacteristic>();
			Queue<int> vertices = new Queue<int>();
			vertices.Enqueue(mine);
			used.Add(mine, new MineNodeCharacteristic{Vertex = mine, Mine = mine});
			while (vertices.Count > 0)
			{
				var vertex = vertices.Dequeue();
				var vertexNode = Graph.vertexes[vertex];
				var vertexCharct = used[vertex];

				foreach (var edge in vertexNode.Edges)
				{
					if (used.ContainsKey(edge.To))
					{
						if(used[edge.To].Distance == vertexCharct.Distance + 1)
							used[edge.To].PreviousOptimalEdges.Add(vertex);
						continue;
					}
					used[edge.To] = new MineNodeCharacteristic{Vertex = edge.To, Mine = mine, Distance = vertexCharct.Distance + 1};
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
	}
}
