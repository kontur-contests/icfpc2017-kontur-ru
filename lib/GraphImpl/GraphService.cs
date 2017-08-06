using lib.StateImpl;

namespace lib.GraphImpl
{
    public class GraphService : IService
    {
        public Graph Graph;

        public void Setup(State state, IServices services)
        {
            Graph = new Graph(state.map);
        }

        public void ApplyNextState(State state, IServices services)
        {
            Graph = new Graph(state.map);
        }
    }
}