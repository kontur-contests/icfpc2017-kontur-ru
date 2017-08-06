using lib.Structures;

namespace lib.StateImpl
{
    public class AiInfoMoveDecision
    {
        public string name;
        public string version;
        public Move move;
        public string reason;

        public override string ToString()
        {
            return $"{name} {version}: {move} ({reason})";
        }
    }
}