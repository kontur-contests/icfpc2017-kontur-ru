using System;
using lib.Structures;

namespace lib.Interaction.Internal
{
    internal class OnlineProtocol
    {
        private readonly ITransport transport;

        public OnlineProtocol(ITransport transport)
        {
            this.transport = transport;
        }

        public bool HandShake(string name)
        {
            transport.Write(new HandshakeOut {me = name});
            try
            {
                var handshakeIn = transport.Read<HandshakeIn>(1000);
                if (handshakeIn.you != name)
                    throw new InvalidOperationException(
                        $"Couldn't pass handshake. handshakeIn.You = {handshakeIn.you}; me = {name}");
                return true;
            }
            catch (TimeoutException)
            {
                return false;
            }
        }

        public In ReadSetup()
        {
            var @in = transport.Read<In>();
            if (!@in.IsSetup())
                throw new InvalidOperationException("Invalid response. Expected setup");
            return @in;
        }

        public void WriteSetupReply(SetupOut reply)
        {
            transport.Write(reply);
        }

        public void WriteMove(Move move)
        {
            transport.Write(move);
        }

        public In ReadNextTurn()
        {
            var @in = transport.Read<In>();
            if (@in.IsGameplay() || @in.IsScoring())
                return @in;
            throw new InvalidOperationException("Invalid response. Expected gameplay or stop");
        }
    }
}