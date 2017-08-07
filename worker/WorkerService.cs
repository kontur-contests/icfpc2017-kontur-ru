using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using lib;
using lib.Ai.StrategicFizzBuzz;
using lib.Scores.Simple;
using lib.Structures;
using lib.viz;
using MoreLinq;
using Newtonsoft.Json;
using NLog;

namespace worker
{
    public class WorkerService
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly Dictionary<string, object> config;
        private bool cancelled;
        private Thread workerThread;
        static Random random = new Random();

        public WorkerService(Dictionary<string, object> conf)
        {
            config = conf;
        }

        public void Start()
        {
            workerThread = new Thread(
                () =>
                {
                    using (var producer = new Producer<Null, string>(config, null, new StringSerializer(Encoding.UTF8)))
                    {
                        while (!cancelled)
                        {
                            try
                            {
                                int minMapPlayersCount = 4;
                                int maxMapPlayersCount = 8;
                                int roundsCount = 10;
                                bool failOnExceptions = false;
                    
                                var ais = new List<AiFactory>()
                                    {
                                        //AiFactoryRegistry.CreateFactory<OptAntiLochDinicKillerAi>(),
                                        //AiFactoryRegistry.CreateFactory<AntiLochDinicKillerAi_0>(),
                                        //AiFactoryRegistry.CreateFactory<AntiLochDinicKillerAi_005>(),
                                        //AiFactoryRegistry.CreateFactory<AntiLochDinicKillerAi_01>(),
                                        //AiFactoryRegistry.CreateFactory<AntiLochDinicKillerAi_02>(),
                                        //AiFactoryRegistry.CreateFactory<AntiLochDinicKillerAi_03>(),
                                        //AiFactoryRegistry.CreateFactory<AntiLochDinicKillerAi_04>(),
                                        //AiFactoryRegistry.CreateFactory<AntiLochDinicKillerAi_05>(),
                                        //AiFactoryRegistry.CreateFactory<AntiLochDinicKillerAi_1>(),
                                        AiFactoryRegistry.CreateFactory<FutureIsNowAi>(),
                                        AiFactoryRegistry.CreateFactory<ConnectClosestMinesAi>(),
                                        //AiFactoryRegistry.CreateFactory<AntiLochDinicKillerAi>(),
                                        AiFactoryRegistry.CreateFactory<LochDinicKillerAi>(),
                                        AiFactoryRegistry.CreateFactory<LochMaxVertexWeighterKillerAi>(),
                                        //AiFactoryRegistry.CreateFactory<AllComponentsMaxReachableVertexWeightAi>(),
                                        //AiFactoryRegistry.CreateFactory<MaxReachableVertexWeightAi>(),
                                        //AiFactoryRegistry.CreateFactory<ConnectClosestMinesAi>(),
                                        AiFactoryRegistry.CreateFactory<GreedyAi>(),
                                        AiFactoryRegistry.CreateFactory<RandomEWAi>(),
                                        //AiFactoryRegistry.CreateFactory<TheUberfullessnessAi>(),
                                    }
                                    .Select(f => new PlayerTournamentResult(f)).ToList();
                                var maps = MapLoader.LoadOnlineMaps()
                                    .Where(map => map.PlayersCount.InRange(minMapPlayersCount, maxMapPlayersCount))
                                    .ToList();

                                logger.Info($"Start round");
                                
                                var r = Enumerable.Range(0, roundsCount)
                                    .AsParallel()
                                    .Select(
                                        i =>
                                        {
                                            return maps
                                                .AsParallel()
                                                .Select(
                                                    map =>
                                                    {
                                                        logger.Info($"map {map.Name}");
                                                        var matchPlayers = ais.Select(a => a.Clone()).Shuffle(random)
                                                            .Repeat().Take(map.PlayersCount).ToList();
                                                        var gameSimulator = new GameSimulatorRunner(
                                                            new SimpleScoreCalculator(), true, !failOnExceptions);
                                                        var gamers = matchPlayers.Select(p => p.Factory.Create())
                                                            .ToList();
                                                        var results = gameSimulator.SimulateGame(
                                                            gamers, map.Map, new Settings(true, true, true));
                                                        AssignMatchScores(results);
                                                        foreach (var res in results)
                                                        {
                                                            int index = gamers.IndexOf(res.Gamer);
                                                            var player = matchPlayers[index];
                                                            player.Maps.Add(map.Name);
                                                            player.GamesPlayed++;
                                                            player.OptionUsageRate.Add(res.OptionsUsed);
                                                            player.NormalizedMatchScores.Add(
                                                                (double) res.MatchScore / matchPlayers.Count);
                                                            player.GamesWon.Add(
                                                                res.MatchScore == matchPlayers.Count ? 1 : 0);
                                                            if (res.LastException != null)
                                                                player.ExceptionsCount++;
                                                            if (res.ScoreData.PossibleFuturesScore != 0)
                                                                player.GainFuturesScoreRate.Add(
                                                                    (double) res.ScoreData.GainedFuturesScore /
                                                                    res.ScoreData.PossibleFuturesScore);
                                                            if (res.ScoreData.TotalFuturesCount != 0)
                                                                player.GainFuturesCountRate.Add(
                                                                    (double) res.ScoreData.GainedFuturesCount /
                                                                    res.ScoreData.TotalFuturesCount);
                                                            player.TurnTime.AddAll(res.TurnTime);
                                                        }
                                                        return matchPlayers;
                                                    })
                                                .Aggregate(
                                                    new List<PlayerTournamentResult>(), (l, d) =>
                                                    {
                                                        l.AddRange(d);
                                                        return l;
                                                    });
                                        })
                                    .Aggregate(
                                        new List<PlayerTournamentResult>(), (l, d) =>
                                        {
                                            l.AddRange(d);
                                            return l;
                                        });

                                r = PlayerTournamentResult.Merge(r).ToList();
                                logger.Info($"Round complete");
                                producer.ProduceAsync("games", null, JsonConvert.SerializeObject(r));
                            }
                            catch (Exception e)
                            {
                                logger.Warn(e);
                            }
                        }
                        producer.Flush(TimeSpan.FromSeconds(10));
                    }
                });
            workerThread.Start();
        }

        public void Stop()
        {
            cancelled = true;
            workerThread.Join();
        }
        
        private static void AssignMatchScores(List<GameSimulationResult> results)
        {
            results = results.OrderByDescending(r => r.Score).ToList();
            var score = results.Count;
            results[0].MatchScore = score;
            for (int i = 1; i < results.Count; i++)
            {
                if (results[i].MatchScore < results[i - 1].MatchScore) score = results.Count - i;
                results[i].MatchScore = score;
            }
        }
    }
}