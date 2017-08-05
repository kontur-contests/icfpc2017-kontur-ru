using System;
using System.Linq;
using lib.Interaction.Internal;
using lib.Replays;
using NUnit.Framework;

namespace lib.Interaction
{
    public class OnlineInteraction : IServerInteraction
    {
        private readonly OnlineProtocol connection;
        private Map map;

        public OnlineInteraction(int port)
        {
            connection = new OnlineProtocol(new TcpTransport(port));
        }

        public void Start()
        {
            connection.HandShake(CreateBotName());
        }

        public string CreateBotName()
        {
            return "XXX"; // Sneaky fucker...
        }

        public Tuple<ReplayMeta, ReplayData> RunGame(IAi ai)
        {
            var setup = connection.ReadSetup();
            ai.StartRound(setup.Id, setup.PunterCount, setup.Map);
            map = setup.Map;
            
            var serverResponse = connection.ReadGameState();

            while (!connection.IsGameOver)
            {
                var moves = connection.GetMoves(serverResponse);
                foreach (var move in moves)
                    map = move.Execute(map);

                var nextMove = ai.GetNextMove(moves, map);
                connection.WriteMove(nextMove);

                serverResponse = connection.ReadGameState();
            }
            var score = connection.GetScore(serverResponse);

            var meta = new ReplayMeta(DateTime.UtcNow, ai.Name, setup.Id, setup.PunterCount, score.Scores);
            var data = new ReplayData(setup.Map, score.MoveModels.Select(ProtocolBase.MoveModel.GetMove));
            
            return Tuple.Create(meta, data);
        }
    }

    [TestFixture]
    public static class Program
    {
        [Test]
        [Explicit]
        public static void Main()
        {
            var interaction = new OnlineInteraction(901);
            interaction.Start();
            interaction.RunGame(new ConnectClosestMinesAi());
        }
    }
}