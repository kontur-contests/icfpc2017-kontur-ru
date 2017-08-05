﻿using System;
using System.Linq;
using System.Text;
using lib;
using Newtonsoft.Json;

namespace punter
{
    class Program
    {
        private static ConnectClosestMinesAi ai;

        private const string TeamName = "kontur.ru";

        static void Main()
        {
            ai = new ConnectClosestMinesAi();

            Write(new HandshakeOut { me = TeamName });
            var handshakeIn = Read<HandshakeIn>();
            if (handshakeIn.you != TeamName)
                throw new InvalidOperationException($"Couldn't pass handshake. handshakeIn.You = {handshakeIn.you}");
            var @in = Read<In>();
            if (@in.IsSetup())
            {
                Write(DoSetup(@in.punter.Value, @in.punters, @in.map));
            }
            else if (@in.IsGameplay())
            {
                Write(DoGameplay(@in.move.moves, @in.state));
            }
            else if (@in.IsScoring())
            {
                DoScoring(@in.stop.moves, @in.stop.scores, @in.state);
            }
            else
                throw new InvalidOperationException($"Invalid input: {@in.line}");
        }

        private static SetupOut DoSetup(int punter, int punters, Map map)
        {
            ai.StartRound(punter, punters, map);
            return new SetupOut
            {
                ready = punter,
                state = new State
                {
                    ai = ai.SerializeGameState(),
                    map = map
                }
            };
        }

        private static GameplayOut DoGameplay(MoveIn[] moves, State state)
        {
            var map = state.map;
            foreach (var moveIn in moves)
                ApplyMove(map, moveIn);
            ai.DeserializeGameState(state.ai);
            var nextMove = ai.GetNextMove(moves.Select(m => (Move) m.claim ?? m.pass).ToArray(), map);
            return new GameplayOut
            {
                claim = nextMove as ClaimMove,
                pass = nextMove as PassMove,
                state = new State
                {
                    ai = ai.SerializeGameState(),
                    map = map
                }
            };
        }

        private static void DoScoring(MoveIn[] moves, ScoreModel[] scores, State state)
        {
        }

        private static void ApplyMove(Map map, MoveIn moveIn)
        {
            if (moveIn.claim == null)
                return;

            var move = moveIn.claim;
            foreach (var river in map.Rivers)
            {
                if (river.Source == move.Source && river.Target == move.Target || river.Target == move.Source && river.Source == move.Target)
                {
                    if (river.Owner != -1)
                        throw new InvalidOperationException($"river.Owner != -1 for move: {move}");
                    river.Owner = move.PunterId;
                    return;
                }
            }

            throw new InvalidOperationException($"Couldn't find river for move: {move}");
        }

        private static void Write<T>(T obj)
        {
            var line = JsonConvert.SerializeObject(obj);
            Console.Out.Write($"{line.Length}:{line}");
        }

        private static T Read<T>() where T : InBase
        {
            var nBuilder = new StringBuilder();
            char ch;
            while ((ch = Convert.ToChar(Console.In.Read())) != ':')
            {
                nBuilder.Append(ch);
            }
            var n = int.Parse(nBuilder.ToString());
            var message = new StringBuilder();
            var buffer = new char[1024];
            while (message.Length < n)
            {
                var charsRead = Console.In.Read(buffer, 0, Math.Min(buffer.Length, n - message.Length));
                message.Append(buffer, 0, charsRead);
            }
            var line = message.ToString();
            var result = JsonConvert.DeserializeObject<T>(line);
            result.line = line;
            return result;
        }
    }

    public class State
    {
        public string ai;
        public Map map;
    }

    public abstract class InBase
    {
        [JsonIgnore] public string line;
    }

    public class HandshakeIn : InBase
    {
        public string you;
    }

    public class In : InBase
    {
        public bool IsSetup() => punter.HasValue;
        public int? punter;
        public int punters;
        public Map map;

        public bool IsGameplay() => move != null;
        public MovesIn move;

        public bool IsScoring() => stop != null;
        public StopIn stop;

        public State state;
    }

    public class StopIn
    {
        public MoveIn[] moves;
        public ScoreModel[] scores;
    }

    public class MovesIn
    {
        public MoveIn[] moves;
    }

    public class MoveIn
    {
        public ClaimMove claim;
        public PassMove pass;
    }

    public class HandshakeOut
    {
        public string me;
    }

    public class SetupOut
    {
        public int ready;
        public State state;
    }

    public class GameplayOut
    {
        public ClaimMove claim;
        public PassMove pass;
        public State state;
    }
}