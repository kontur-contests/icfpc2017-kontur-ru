using System;

namespace lib
{
    public class OfflineHighTransport
    {
        public OfflineHighTransport(ITransport transport1)
        {
			
        }

        public Setup ReadSetup()
        {
            throw new NotImplementedException();
        }

        public void WriteInitialState(string setupOurId, GameState state)
        {
            throw new NotImplementedException();
        }

        public Tuple<AbstractMove[], GameState> ReadMoves()
        {
            throw new NotImplementedException();
        }

        public void SendMove(AbstractMove resultItem1, GameState resultItem2)
        {
            throw new NotImplementedException();
        }

        public Tuple<AbstractMove[], Score[], GameState> ReadScore()
        {
            throw new NotImplementedException();
        }
    }
}