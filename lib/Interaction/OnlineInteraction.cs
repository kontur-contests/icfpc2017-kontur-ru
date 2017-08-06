using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using lib.Ai;
using lib.Interaction.Internal;
using lib.Replays;
using lib.StateImpl;
using lib.Structures;
using Newtonsoft.Json;
using NUnit.Framework;

namespace lib.Interaction
{
    public class OnlineInteraction : IServerInteraction
    {
        private readonly string botName;
        private readonly OnlineProtocol connection;
        private string BotName => botName ?? "kontur.ru";
        public OnlineInteraction(int port, string botName=null)
        {
            this.botName = botName;
            connection = new OnlineProtocol(new TcpTransport(port));
        }

        public bool Start()
        {
            return connection.HandShake(BotName);
        }

        public Tuple<ReplayMeta, ReplayData> RunGame(IAi ai)
        {
            var setup = connection.ReadSetup();

            var state = new State
            {
                map = setup.map,
                punter = setup.punter,
                punters = setup.punters,
                settings = setup.settings
            };

            AiSetupDecision setupDecision;

            try
            {
                var services = new Services();
                setupDecision = ai.Setup(state, services);
            }
            catch
            {
                var handshake = JsonConvert.SerializeObject(new HandshakeIn { you = botName });
                var input = JsonConvert.SerializeObject(setup, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                File.WriteAllText($@"error-setup-{DateTime.UtcNow.ToString("O").Replace(":", "_")}.json", $@"{handshake.Length}:{handshake}{input.Length}:{input}");
                throw;
            }

            if (setup.settings?.futures != true && setupDecision.futures?.Any() == true)
                throw new InvalidOperationException($"BUG in Ai {ai.Name} - futures are not supported");
            state.aiSetupDecision = new AiInfoSetupDecision
            {
                name = ai.Name,
                version = ai.Version,
                futures = setupDecision.futures,
                reason = setupDecision.reason
            };

            connection.WriteSetupReply(new SetupOut { ready = setup.punter, futures = setupDecision.futures });

            var allMoves = new List<Move>();

            var serverResponse = connection.ReadNextTurn();

            while (!serverResponse.IsScoring())
            {
                var moves = serverResponse.move.moves;
                var gameplay = JsonConvert.SerializeObject(serverResponse, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore});

                allMoves.AddRange(moves);
                foreach (var move in moves)
                    state.map = state.map.ApplyMove(move);

                state.turns.Add(new TurnState
                {
                    moves = moves,
                    aiMoveDecision = state.lastAiMoveDecision
                });

                AiMoveDecision moveDecision;
                try
                {
                    var services = new Services();
                    moveDecision = ai.GetNextMove(state, services);
                }
                catch
                {
                    var handshake = JsonConvert.SerializeObject(new { you = "kontur.ru" });
                    File.WriteAllText($@"error-turn-{DateTime.UtcNow.ToString("O").Replace(":", "_")}.json", $@"{handshake.Length}:{handshake}{gameplay.Length}:{gameplay}");
                    throw;
                }
                state.lastAiMoveDecision = new AiInfoMoveDecision
                {
                    name = ai.Name,
                    version = ai.Version,
                    move = moveDecision.move,
                    reason = moveDecision.reason
                };

                connection.WriteMove(moveDecision.move);
                serverResponse = connection.ReadNextTurn();
            }

            var stopIn = serverResponse.stop;

            allMoves.AddRange(stopIn.moves);
            
            var meta = new ReplayMeta(DateTime.UtcNow, ai.Name, ai.Version, "", setup.punter, setup.punters, stopIn.scores);
            var data = new ReplayData(setup.map, allMoves, state.aiSetupDecision.futures);
            
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