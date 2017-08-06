using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using lib.Ai;
using lib.StateImpl;
using lib.Structures;
using MoreLinq;

namespace lib
{
    public class GameSimulator : ISimulator
    {
        private Map map;
        private readonly Settings settings;
        private readonly bool eatExceptions;
        private Dictionary<IAi, Exception> lastException = new Dictionary<IAi, Exception>();
        private List<Tuple<IAi, State>> punters;
        private int currentPunter = 0;
        private readonly List<Move> moves;
        private int turnsAmount;
        private Move[] turnMoves;

        [CanBeNull]
        public Exception GetLastException(IAi ai)
        {
            return lastException.TryGetValue(ai, out var ex) ? ex : null;
        }

        public IList<Future[]> Futures { get; }

        public GameSimulator(Map map, Settings settings, bool eatExceptions = false)
        {
            this.map = map;
            this.settings = settings;
            this.eatExceptions = eatExceptions;
            moves = new List<Move>();
            Futures = new List<Future[]>();
        }

        public void StartGame(List<IAi> gamers)
        {
            lastException.Clear();
            turnMoves = gamers.Select((_, i) => Move.Pass(i)).ToArray();
            punters = gamers.Select((g, i) => Tuple.Create(g, new State
            {
                map = map,
                punter = i,
                punters = gamers.Count,
                settings = settings
            })).ToList();
            for (int i = 0; i < punters.Count; i++)
            {
                var ai = punters[i].Item1;
                var state = punters[i].Item2;
                var services = new Services(state);
                var setupDecision = ai.Setup(state, services);
                Futures.Add(ValidateFutures(setupDecision.futures));
                state.aiSetupDecision = new AiInfoSetupDecision
                {
                    name = ai.Name,
                    version = ai.Version,
                    futures = setupDecision.futures,
                    reason = setupDecision.reason
                };
            }

            turnsAmount = map.Rivers.Length;
        }

        public GameState NextMove()
        {
            if (turnsAmount <= 0)
                return new GameState(map, moves.TakeLast(punters.Count).ToList(), true);

            var ai = punters[currentPunter].Item1;
            var state = punters[currentPunter].Item2;
            state.map = map;
            state.turns.Add(new TurnState { moves = turnMoves.ToArray(), aiMoveDecision = state.lastAiMoveDecision });
            var services = new Services(state);
            var moveDecision = GetNextMove(ai, state, services, eatExceptions, lastException);
            state.lastAiMoveDecision = new AiInfoMoveDecision
            {
                name = ai.Name,
                version = ai.Version,
                move = moveDecision.move,
                reason = moveDecision.reason
            };

            map = map.ApplyMove(state.lastAiMoveDecision);
            turnMoves[currentPunter] = moveDecision.move;
            moves.Add(moveDecision.move);
            currentPunter = (currentPunter + 1) % punters.Count;
            turnsAmount--;
            return new GameState(map, moves.TakeLast(punters.Count).ToList(), false);
        }

        private static AiMoveDecision GetNextMove(IAi ai, State state, IServices services, bool eatExceptions, Dictionary<IAi, Exception> lastException)
        {
            try
            {
                return ai.GetNextMove(state, services);
            }
            catch (Exception e)
            {
                lastException[ai] = e;
                if (eatExceptions)
                {
                    return AiMoveDecision.Pass(state.punter, e.ToString());
                }
                else throw;
            }
        }

        private Future[] ValidateFutures(Future[] futures)
            => futures
                .Where(e => map.Mines.Contains(e.source) && !map.Mines.Contains(e.target))
                .GroupBy(e => e.source)
                .Select(e => e.Last())
                .ToArray();
    }
}