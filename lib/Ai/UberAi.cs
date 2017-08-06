using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using lib.Ai.StrategicFizzBuzz;
using lib.GraphImpl;
using lib.QualityControl;
using lib.StateImpl;
using MoreLinq;

namespace lib.Ai
{
    public class UberAi : IAi
    {
        public string Name => "UberAi";
        public string Version => "0.1";

        private IAi[] Bots =
            new IAi[]
            {
                new LochKillerAi(),
                new MaxReachableVertexWeightAi(),
                new LochMaxVertexWeighterKillerAi(),
                new Podnaserator2000Ai(),
                new FutureIsNowAi(),
                new LochDinicKillerAi(),
            };
        
        public int InitBot;
        public int StartBot;
        public int MiddleBot;
        public int EndBot;

        public UberAi(double initBot, double startBot, double middleBot, double endBot)
        {
            InitBot = (int)(initBot * Bots.Length);
            StartBot = (int)(startBot * Bots.Length);
            MiddleBot = (int)(middleBot * Bots.Length);
            EndBot = (int)(endBot * Bots.Length);
        }

        public AiSetupDecision Setup(State state, IServices services)
        {
            var startOptions = Bots.Select(bot => bot.Setup(state, services)).ToList();
            return startOptions[InitBot];
        }
        
        public AiMoveDecision GetNextMove(State state, IServices services)
        {
            var conqueredRiversCount = state.map.Rivers.Count(river => river.Owner >= 0) * 1.0 /
                                       state.map.Rivers.Length;
            var uberState = conqueredRiversCount < 0.2 ? UberState.Start :
                            conqueredRiversCount < 0.8 ? UberState.Middle: 
                            UberState.End;

            switch (uberState)
            {
                case UberState.Start:
                    return Bots[StartBot].GetNextMove(state, services);
                case UberState.Middle:
                    return Bots[MiddleBot].GetNextMove(state, services);
                default:
                    return Bots[EndBot].GetNextMove(state, services);
            }
        }
    }

    enum UberState
    {
        Start,
        Middle,
        End
    }
}
