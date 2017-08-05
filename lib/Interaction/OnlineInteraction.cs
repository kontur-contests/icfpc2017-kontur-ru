using System;
using System.Collections.Generic;
using System.Linq;
using lib.Ai;
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
            connection = new OnlineProtocol(StreamTransport.TcpTransport(port));
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

            var allMoves = new List<Move>();

            while (!connection.IsGameOver)
            {
                Move[] moves = connection.GetMoves(serverResponse);
                allMoves.AddRange(moves);
                foreach (var move in moves)
                    map = move.Execute(map);

                var nextMove = ai.GetNextMove(moves, map);
                allMoves.Add(nextMove);
                connection.WriteMove(nextMove);

                serverResponse = connection.ReadGameState();
            }
            var score = connection.GetScore(serverResponse);
            allMoves.AddRange(score.MoveModels.Select(ProtocolBase.MoveModel.GetMove).ToArray());

            var meta = new ReplayMeta(DateTime.UtcNow, ai.Name, setup.Id, setup.PunterCount, score.Scores);
            var data = new ReplayData(setup.Map, allMoves);
            
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