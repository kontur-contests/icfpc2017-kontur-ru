using lib.GraphImpl;

namespace lib.StateImpl
{
    public class MeetingPointService : IService
    {
        public class ServiceState
        {
            public int meetingPoint;
        }

        public int MeetingPoint { get; }

        public MeetingPointService(Graph graph, MineDistCalculator calculator, ServiceState serviceState, bool isSetupStage)
        {
            if (isSetupStage)
            {
                var bestPoint = -1;
                var bestValue = 0;
                var bestCount = 0;

                foreach (var vertex in graph.Vertexes)
                {
                    if (vertex.Value.IsMine)
                        continue;

                    var value = 0;
                    var count = 0;
                    foreach (var mine in graph.Mines)
                    {
                        var dist = calculator.GetDist(mine.Key, vertex.Key);
                        if (dist == -1)
                            continue;
                        count++;
                        value += dist;
                    }

                    if (bestPoint == -1 || bestCount < count || bestCount == count && value < bestValue)
                    {
                        bestPoint = vertex.Key;
                        bestCount = count;
                        bestValue = value;
                    }
                }
                serviceState.meetingPoint = bestPoint;
            }
            MeetingPoint = serviceState.meetingPoint;
        }
    }
}