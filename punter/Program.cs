using System;
using System.IO;
using System.Linq;
using System.Text;
using lib;
using lib.Ai;
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
            ai = new LochKillerAi();

            Write(new HandshakeOut {me = TeamName});
            var handshakeIn = Read<HandshakeIn>();
            if (handshakeIn.you != TeamName)
                throw new InvalidOperationException($"Couldn't pass handshake. handshakeIn.You = {handshakeIn.you}");

            var @in = Read<In>();
            if (@in.IsSetup())
                Write(DoSetup(@in.punter, @in.punters, @in.map, @in.settings));
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
            var futures = ai.StartRound(punter, punters, map, settings);
            return new SetupOut
            {
                ready = punter,
                futures = futures,
                state = new State
                {
                    ai = ai.SerializeGameState(),
                    punter = punter,
                    punters = punters,
                    map = map
                }
            };
        }

        private static GameplayOut DoGameplay(Move[] moves, State state)
        {
            var map = state.map;
            foreach (var moveIn in moves)
                ApplyMove(map, moveIn);
            ai.DeserializeGameState(state.ai);
            try
            {
                var nextMove = ai.GetNextMove(moves, map);
                return new GameplayOut(nextMove, new State
                {
                    ai = ai.SerializeGameState(),
                    punter = state.punter,
                    punters = state.punters,
                    map = map
                });
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return new GameplayOut(Move.Pass(state.punter), new State
                {
                    ai = state.ai,
                    punter = state.punter,
                    punters = state.punters,
                    map = map
                });
            }
        }

        private static void DoScoring(Move[] moves, Score[] scores)
        {
            foreach (var scoreModel in scores)
                Console.Error.WriteLine($"{scoreModel.punter}={scoreModel.score}");
        }

        private static void ApplyMove(Map map, Move move)
        {
            if (move.claim == null)
                return;

            foreach (var river in map.Rivers)
            {
                if (river.Source == move.claim.source && river.Target == move.claim.target || river.Target == move.claim.source && river.Source == move.claim.target)
                {
                    if (river.Owner != -1)
                        throw new InvalidOperationException($"river.Owner != -1 for move: {move}");
                    river.Owner = move.claim.punter;
                    return;
                }
            }

            throw new InvalidOperationException($"Couldn't find river for move: {move}");
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