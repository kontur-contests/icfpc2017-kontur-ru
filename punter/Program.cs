using System;
using System.IO;
using System.Linq;
using System.Text;
using lib;
using lib.Ai;
using lib.StateImpl;
using lib.Structures;
using Newtonsoft.Json;

namespace punter
{
    class Program
    {
        private static IAi ai;
        private static TextReader inputReader;

        private const string TeamName = "kontur.ru";

        static void Main(string[] args)
        {
            if (!args.Any())
                inputReader = Console.In;
            else
                inputReader = new StreamReader(args[0]);
            ai = new ConnectClosestMinesAi();
        
            Write(new HandshakeOut {me = TeamName});
            var handshakeIn = Read<HandshakeIn>();
            if (handshakeIn.you != TeamName)
                throw new InvalidOperationException($"Couldn't pass handshake. handshakeIn.You = {handshakeIn.you}");

            var @in = Read<In>();
            if (@in.IsSetup())
                Write(DoSetup(@in.punter, @in.punters, @in.map, @in.settings ?? new Settings()));
            else if (@in.IsGameplay())
                Write(DoGameplay(@in.move.moves, @in.state));
            else if (@in.IsScoring())
                DoScoring(@in.stop.moves, @in.stop.scores);
            else
                throw new InvalidOperationException($"Invalid input: {@in.line}");
        }

        private static SetupOut DoSetup(int punter, int punters, Map map, Settings settings)
        {
            Console.Error.WriteLine($"punter={punter}/{punters}");
            var state = new State
            {
                map = map,
                punter = punter,
                punters = punters,
                settings = settings
            };
            var setupDecision = ai.Setup(state, new Services(state));
            if (!settings.futures && setupDecision.futures?.Any() == true)
            {
                Console.Error.WriteLine($"BUG in Ai {ai.Name} - futures are not supported");
                setupDecision = AiSetupDecision.Empty("futures are not supported");
            }
            state.aiSetupDecision = new AiInfoSetupDecision
            {
                name = ai.Name,
                version = ai.Version,
                futures = setupDecision.futures,
                reason = setupDecision.reason
            };
            return new SetupOut
            {
                ready = punter,
                futures = setupDecision.futures,
                state = state
            };
        }

        private static GameplayOut DoGameplay(Move[] moves, State state)
        {
            state.ApplyMoves(moves);
            try
            {
                var moveDecision = ai.GetNextMove(state, new Services(state));
                var aiInfoMoveDecision = new AiInfoMoveDecision
                {
                    name = ai.Name,
                    version = ai.Version,
                    move = moveDecision.move,
                    reason = moveDecision.reason
                };
                state.ValidateMove(aiInfoMoveDecision);
                state.lastAiMoveDecision = aiInfoMoveDecision;
                return new GameplayOut(moveDecision.move, state);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                var moveDecision = AiMoveDecision.Pass(state.punter, (e as InvalidDecisionException)?.Reason ?? "exception");
                state.lastAiMoveDecision = new AiInfoMoveDecision
                {
                    name = ai.Name,
                    version = ai.Version,
                    move = moveDecision.move,
                    reason = moveDecision.reason
                };
                return new GameplayOut(Move.Pass(state.punter), state);
            }
        }

        private static void DoScoring(Move[] moves, Score[] scores)
        {
            foreach (var scoreModel in scores)
                Console.Error.WriteLine($"{scoreModel.punter}={scoreModel.score}");
        }

        private static void Write<T>(T obj)
        {
            Console.Error.WriteLine($"Writing {typeof(T)}");
            var line = JsonConvert.SerializeObject(obj, new JsonSerializerSettings{NullValueHandling = NullValueHandling.Ignore});
            Console.Error.WriteLine($"Writing {typeof(T)} line: {line}");
            Console.Out.Write($"{line.Length}:{line}");
        }

        private static T Read<T>() where T : InBase
        {
            Console.Error.WriteLine($"Reading {typeof(T)}");
            var nBuilder = new StringBuilder();
            while (true)
            {
                var charCode = inputReader.Read();
                if (charCode == -1)
                    continue;
                var ch = Convert.ToChar(charCode);
                if (ch == ':')
                    break;
                nBuilder.Append(ch);
            }
            var n = int.Parse(nBuilder.ToString());
            var message = new StringBuilder();
            var buffer = new char[1024];
            while (message.Length < n)
            {
                var charsRead = inputReader.Read(buffer, 0, Math.Min(buffer.Length, n - message.Length));
                message.Append(buffer, 0, charsRead);
            }
            var line = message.ToString();
            Console.Error.WriteLine($"Read {typeof(T)} line: {line}");
            var result = JsonConvert.DeserializeObject<T>(line);
            result.line = line;
            return result;
        }
    }
}