using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using lib.Ai;
using lib.Interaction.Internal;
using lib.Replays;
using lib.viz;
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
                futures = ai.StartRound(setup.Id, setup.PunterCount, setup.Map, setup.Settings);
            }
            catch
            {
                var handshake = JsonConvert.SerializeObject(new { you = CreateBotName() });
                var input = JsonConvert.SerializeObject(new
                {
                    punter = setup.Id,
                    punters = setup.PunterCount,
                    map = setup.Map,
                    settings = setup.Settings
                });
                File.WriteAllText($@"error-setup-{DateTime.UtcNow.ToString("O").Replace(":", "_")}.json", $@"{handshake.Length}:{handshake}{input.Length}:{input}");
                throw;
            }

            connection.WriteSetupReply(new SetupReply(setup.Id, futures));

            var map = setup.Map;

            var serverResponse = connection.ReadGameState();

            var allMoves = new List<Move>();
            
            while (!connection.IsGameOver)
            {
                var moves = connection.GetMoves(serverResponse);
                var gameplay = JsonConvert.SerializeObject(new
                {
                    move = new
                    {
                        moves = moves.Select(x => new
                        {
                            claim = x as ClaimMove,
                            pass = x as PassMove
                        }).ToArray()
                    },
                    state = new
                    {
                        ai = ai.SerializeGameState(),
                        punter = setup.Id,
                        punters = setup.PunterCount,
                        map
                    }
                }, new JsonSerializerSettings{NullValueHandling = NullValueHandling.Ignore});

                allMoves.AddRange(moves);
                foreach (var move in moves)
                    map = move.Execute(map);

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

                serverResponse = connection.ReadGameState();
            }
            var score = connection.GetScore(serverResponse);

            allMoves.AddRange(score.MoveModels.Select(ProtocolBase.MoveModel.GetMove));
            
            var meta = new ReplayMeta(DateTime.UtcNow, ai.Name, ai.Version, "", setup.Id, setup.PunterCount, score.Scores);
            var data = new ReplayData(setup.Map, allMoves, futures);
            
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