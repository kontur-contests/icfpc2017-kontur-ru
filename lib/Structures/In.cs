using lib.StateImpl;

namespace lib.Structures
{
    public class In : InBase
    {
        public bool IsSetup() => punter != -1;
        public int punter = -1;
        public int punters;
        public Map map;
        public Settings settings;

        public bool IsGameplay() => move != null;
        public Moves move;
        public State state;

        public bool IsScoring() => stop != null;
        public StopIn stop;
    }
}