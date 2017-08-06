﻿using System.Collections.Generic;
using System.Linq;
using lib.StateImpl;
using lib.Strategies.EdgeWeighting;
using lib.Strategies.StrategiesCatalog;
using lib.viz;

namespace lib.Ai.StrategicFizzBuzz
{
    [ShouldNotRunOnline(DisableCompletely = true)]
    public class UberfullessnessAi : IAi
    {
        public UberfullessnessAi(string setup, string debut, string helper, string mittelspiel)
        {
            Name = $"{setup}{debut}{helper}{mittelspiel}UberfullessnessAi";
            Ai = new CompositeStrategicAi(
                Setups[setup].SetupStrategyProvider,
                Debuts[debut].StrategyProvider,
                Helpers[helper].StrategyProvider,
                Mittelspiels[mittelspiel].StrategyProvider,
                MetaStrategyHelpers.BiggestComponentEWStrategy((s, ss) => new RandomEdgeWeighter())); // Fall back to random in the end.
        }

        public static Dictionary<string, SetupStrategyFactory> Setups => SetupStrategies.Factories;
        public static Dictionary<string, StrategyFactory> Debuts => DebutStrategies.Factories;
        public static Dictionary<string, StrategyFactory> Helpers => HelperStrategies.Factories;
        public static Dictionary<string, StrategyFactory> Mittelspiels => MittelspielStrategies.Factories;

        public static UberfullessnessAi[] All =>
            Setups.Keys
                .SelectMany(
                    setup => Debuts.Keys
                        .SelectMany(
                            debut => Helpers.Keys
                                .SelectMany(
                                    helper => Mittelspiels.Keys
                                        .Select(mittelspiel => new UberfullessnessAi(setup, debut, helper, mittelspiel)))))
                .ToArray();

        private CompositeStrategicAi Ai { get; }

        public string Name { get; }

        public AiSetupDecision Setup(State state, IServices services)
        {
            return Ai.Setup(state, services);
        }

        public AiMoveDecision GetNextMove(State state, IServices services)
        {
            return Ai.GetNextMove(state, services);
        }

        public string Version => "1";
    }
}