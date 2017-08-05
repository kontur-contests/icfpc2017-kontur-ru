using System;
using System.Collections.Generic;
using System.IO;
using lib.Ai;
using lib.Interaction.Internal;
using lib.Replays;
using lib.Structures;
using Newtonsoft.Json;
using NUnit.Framework;

namespace lib.Interaction
{
    public class OnlineInteraction : IServerInteraction
    {
        private readonly OnlineProtocol connection;

        public OnlineInteraction(int port)
        {
            connection = new OnlineProtocol(new TcpTransport(port));
        }

        public bool Start()
        {
            return connection.HandShake(CreateBotName());
        }

        public string CreateBotName()
        {
            return "kontur.ru"; // Sneaky fucker...
        }

        public Tuple<ReplayMeta, ReplayData> RunGame(IAi ai)
        {
            var setup = connection.ReadSetup();

            Future[] futures;

            try
            {
                futures = ai.StartRound(setup.punter, setup.punters, setup.map, setup.settings);
            }
            catch
            {
                var handshake = JsonConvert.SerializeObject(new HandshakeIn { you = CreateBotName() });
                var input = JsonConvert.SerializeObject(setup, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                File.WriteAllText($@"error-setup-{DateTime.UtcNow.ToString("O").Replace(":", "_")}.json", $@"{handshake.Length}:{handshake}{input.Length}:{input}");
                throw;
            }

            connection.WriteSetupReply(new SetupOut { ready = setup.punter, futures = futures });

            var map = setup.map;

            var allMoves = new List<Move>();

            var serverResponse = connection.ReadNextTurn();

            while (!serverResponse.IsScoring())
            {
                var moves = serverResponse.move.moves;
                var gameplay = JsonConvert.SerializeObject(serverResponse, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore});

                allMoves.AddRange(moves);
                foreach (var move in moves)
                    map = map.ApplyMove(move);

                Move nextMove;
                try
                {
                    nextMove = ai.GetNextMove(moves, map);
                }
                catch
                {
                    var handshake = JsonConvert.SerializeObject(new { you = CreateBotName() });
                    File.WriteAllText($@"error-turn-{DateTime.UtcNow.ToString("O").Replace(":", "_")}.json", $@"{handshake.Length}:{handshake}{gameplay.Length}:{gameplay}");
                    throw;
                }

                allMoves.Add(nextMove);
                connection.WriteMove(nextMove);
                serverResponse = connection.ReadNextTurn();

            }

            var stopIn = serverResponse.stop;

            allMoves.AddRange(stopIn.moves);
            
            var meta = new ReplayMeta(DateTime.UtcNow, ai.Name, setup.punter, setup.punters, stopIn.scores);
            var data = new ReplayData(setup.map, allMoves, futures);
            
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
            var interaction = new OnlineInteraction(9001);
            interaction.Start();
            interaction.RunGame(new ConnectClosestMinesAi());
        }
    }
}