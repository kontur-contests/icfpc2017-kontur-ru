using System;
using System.Diagnostics;
using lib.Interaction.Internal;
using NLog;
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

        public void RunGame(IAi ai)
        {
            connection.HandShake("rutnok bks");

            var setup = connection.ReadSetup();
            ai.StartRound(setup.Id, setup.PunterCount, setup.Map);
            map = setup.Map;
            Console.WriteLine($"I'm {setup.Id}");
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
            Console.WriteLine(score);
        }
    }

    [TestFixture]
    public static class Program
    {
        [Test]
        [Explicit]
        public static void Main()
        {
            var onlineInteraction = new OnlineInteraction(901);
            onlineInteraction.RunGame(new ConnectClosestMinesAi());
        }
    }
}