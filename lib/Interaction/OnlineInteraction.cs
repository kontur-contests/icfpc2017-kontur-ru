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
                setupDecision = ai.Setup(state, new Services(state));
            }
            catch
            {
                var handshake = JsonConvert.SerializeObject(new HandshakeIn { you = botName });
                var input = JsonConvert.SerializeObject(setup, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                File.WriteAllText($@"error-setup-{DateTime.UtcNow.ToString("O").Replace(":", "_")}.json", $@"{handshake.Length}:{handshake}{input.Length}:{input}");
                throw;
            }

            if (!state.settings.futures && setupDecision.futures?.Any() == true)
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
                var moves = serverResponse.move.moves.OrderBy(m => m.GetPunter()).ToArray();

                moves = moves.Skip(setup.punter).Concat(moves.Take(setup.punter)).ToArray();
                var gameplay = JsonConvert.SerializeObject(serverResponse, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore});

                allMoves.AddRange(moves);

                state.ApplyMoves(moves);

                AiMoveDecision moveDecision;
                try
                {
                    moveDecision = ai.GetNextMove(state, new Services(state));
                }
                catch
                {
                    var handshake = JsonConvert.SerializeObject(new { you = "kontur.ru" });
                    File.WriteAllText($@"error-turn-{DateTime.UtcNow.ToString("O").Replace(":", "_")}.json", $@"{handshake.Length}:{handshake}{gameplay.Length}:{gameplay}");
                    throw;
                }
                var aiInfoMoveDecision = new AiInfoMoveDecision
                {
                    name = ai.Name,
                    version = ai.Version,
                    move = moveDecision.move,
                    reason = moveDecision.reason
                };
                state.ValidateMove(aiInfoMoveDecision);
                state.lastAiMoveDecision = aiInfoMoveDecision;

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