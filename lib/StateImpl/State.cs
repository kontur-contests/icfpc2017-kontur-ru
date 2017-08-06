using System.Collections.Concurrent;
using System.Collections.Generic;
using lib.Ai;
using lib.GraphImpl;
using lib.Structures;

namespace lib.StateImpl
{
    public class State
    {
        public int punter;
        public int punters;
        public Settings settings;
        public Map map;

        public AiInfoSetupDecision aiSetupDecision;
        public AiInfoMoveDecision lastAiMoveDecision;
        public List<TurnState> turns = new List<TurnState>();

        public MineDistCalculator.ServiceState mdc;
        public MeetingPointService.ServiceState mps;
    }
}