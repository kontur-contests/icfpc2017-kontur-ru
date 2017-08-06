using lib.GraphImpl;

namespace lib.StateImpl
{
    public class MeetingPointService : IService
    {
        public class ServiceState
        {
            public int meetingPoint;
        }

        public void Setup(State state, IServices services)
        {
            var bestPoint = -1;
            var bestValue = 0;
            var bestCount = 0;

            var graph = services.Get<GraphService>(state).Graph;
            var calculator = services.Get<MineDistCalculator>(state);

            foreach (var vertex in graph.Vertexes)
            {
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
            state.mps = new ServiceState {meetingPoint = bestPoint};
        }

        public void ApplyNextState(State state, IServices services)
        {
            
        }
    }
}