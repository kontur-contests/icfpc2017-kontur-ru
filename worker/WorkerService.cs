using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using lib.Arena;
using lib.Interaction;
using lib.Replays;
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
        private readonly string[] inputTopicNames;
        private readonly string outputTopicName;
        private readonly IExperiment experiment;
        private bool cancelled;
        private Thread workerThread;
        private readonly string commitHash;

        public WorkerService(Dictionary<string, object> conf, string[] input, string output, IExperiment experiment)
        {
            config = conf;
            inputTopicNames = input;
            outputTopicName = output;
            this.experiment = experiment;
            try
            {
                commitHash = File.ReadAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "commit_hash.txt"))
                    .FirstOrDefault();
            }
            catch (Exception e)
            {
                commitHash = "Unknown";
                logger.Warn("Can't read commit_hash.txt");
                logger.Error(e);
            }
        }

        public void Start()
        {
            workerThread = new Thread(
                () =>
                {
                    using (var consumer = new Consumer<Null, string>(
                        config, null, new StringDeserializer(Encoding.UTF8)))
                    using (var producer = new Producer<Null, string>(config, null, new StringSerializer(Encoding.UTF8)))
                    {
                        consumer.OnPartitionEOF += (_, end) => logger.Info(
                            $"Reached end of topic {end.Topic} partition {end.Partition}, next message will be at offset {end.Offset}");

                        consumer.OnError += (_, error) => logger.Error($"Error: {error}");

                        consumer.OnPartitionsAssigned += (_, partitions) =>
                        {
                            logger.Info(
                                $"Assigned partitions: [{string.Join(", ", partitions)}], member id: {consumer.MemberId}");
                            consumer.Assign(partitions);
                        };

                        consumer.OnPartitionsRevoked += (_, partitions) =>
                        {
                            logger.Info($"Revoked partitions: [{string.Join(", ", partitions)}]");
                            consumer.Unassign();
                        };

                        consumer.OnStatistics += (_, json) => logger.Info($"Statistics: {json}");

                        consumer.Subscribe(inputTopicNames);

                        while (!cancelled)
                        {
                            Message<Null, string> msg;

                            if (!consumer.Consume(out msg, TimeSpan.FromSeconds(1)))
                            {
                                continue;
                            }

                            logger.Info(
                                $"Got message | Topic: {msg.Topic} Partition: {msg.Partition} Offset: {msg.Offset} {msg.Value}");

                            try
                            {
                                if (msg.Topic == "tasks")
                                    ProcessTask(msg, producer);

                                if (msg.Topic == "matches")
                                    ProcessMatch(msg);
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

        private void ProcessMatch(Message<Null, string> msg)
        {
            System.Threading.Tasks.Task.Run(
                () =>
                {
                    var ai = AiFactoryRegistry.GetNextAi();
                    var match = ArenaMatch.EmptyMatch;

                    if (!int.TryParse(msg.Value, out match.Port)) return;
                    logger.Info($"Match on port {match.Port} for {GetBotName(ai.Name)}");

                    var interaction = new OnlineInteraction(match.Port, GetBotName(ai.Name));
                    if (!interaction.Start()) return;

                    logger.Info($"Running game on port {match.Port}");
                    var metaAndData = interaction.RunGame(ai);
                    metaAndData.Item1.CommitHash = commitHash;
                    new ReplayRepo().SaveReplay(metaAndData.Item1, metaAndData.Item2);
                    logger.Info($"Saved replay {metaAndData.Item1.Scores.ToDelimitedString(", ")}");
                });
        }

        private void ProcessTask(Message<Null, string> msg, ISerializingProducer<Null, string> producer)
        {
            System.Threading.Tasks.Task.Run(
                () =>
                {
                    var task = JsonConvert.DeserializeObject<Task>(msg.Value);
                    Result result;
                    try
                    {
                        result = experiment.Play(task);
                    }
                    catch (Exception exception)
                    {
                        result = new Result
                        {
                            Error = exception.Message + "\n\n" + exception.StackTrace
                        };
                    }
                    result.Task = task;
                    result.Token = task.Token;
                    var resultString = JsonConvert.SerializeObject(result);

                    var deliveryReport = producer.ProduceAsync(outputTopicName, null, resultString);

                    deliveryReport.ContinueWith(
                        x =>
                        {
                            logger.Info(
                                $"Sent result | Partition: {x.Result.Partition}, Offset: {x.Result.Offset}");
                        });
                });
        }

        public void Stop()
        {
            cancelled = true;
            workerThread.Join();
        }

        private static string GetBotName(string botTypeName)
        {
            return $"kontur.ru_{string.Join("", botTypeName.Where(char.IsUpper).ToArray())}";
        }
    }
}