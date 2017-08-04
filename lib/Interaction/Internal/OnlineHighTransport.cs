using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace lib.Interaction.Internal
{
    internal class OnlineHighTransport : HightTransport
    {
        public OnlineHighTransport(ITransport transport)
        {
            this.transport = transport;
        }

        public void HandShake(string name)
        {
            transport.Write($"{{\"me\":\"{name}\"}}");
            var answer = transport.Read();
            if (!answer.Contains($"\"{name}\""))
                throw new InvalidOperationException($"Incorrect server handsnake: {answer}");
        }

        public Setup ReadSetup()
        {
            var data = JsonConvert.DeserializeObject<Setup>(transport.Read());
            transport.Write($"{{\"ready\":\"{data.OurId}\"}}");
            return data;
        }

        public AbstractMove[] ReadMoves()
        {
            var data = JsonConvert.DeserializeObject<MoveServerData>(transport.Read());
            return data.Moves.Moves.Select(DeserializeMove).ToArray();
        }

        public void SendMove(AbstractMove abstractMove)
        {
            transport.Write(SerializeMove(abstractMove));
        }

        public Tuple<Move[], Score[]> ReadScore()
        {
            throw new NotImplementedException();
        }

        private readonly ITransport transport;
    }
}