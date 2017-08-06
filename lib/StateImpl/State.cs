using System.Collections.Generic;
using System.Linq;
using lib.GraphImpl;
using lib.Structures;

namespace lib.StateImpl
{
    public class State
    {
        public int punter;
        public int punters;
        public Settings settings = new Settings();
        public Map map;
        public Dictionary<int, int> credits = new Dictionary<int, int>();

        public AiInfoSetupDecision aiSetupDecision;
        public AiInfoMoveDecision lastAiMoveDecision;
        public List<TurnState> turns = new List<TurnState>();
        public bool IsSetupStage() => !turns.Any();

        public MineDistCalculator.ServiceState mdc = new MineDistCalculator.ServiceState();
        public MeetingPointService.ServiceState mps = new MeetingPointService.ServiceState();
    }
}