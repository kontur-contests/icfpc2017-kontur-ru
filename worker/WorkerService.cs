using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using NLog;

namespace worker
{
    public class WorkerService
    {
        private readonly Dictionary<string, object> config;
        private readonly string inputTopicName;
        private readonly string outputTopicName;
        private bool cancelled;
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private Thread workerThread;
        
        public WorkerService(Dictionary<string, object> conf, string input, string output)
        {
            config = conf;
            inputTopicName = input;
            outputTopicName = output;
        }

        public void Start()
        {
            workerThread = new Thread(
                () =>
                {
                    using (var consumer = new Consumer<Null, string>(config, null, new StringDeserializer(Encoding.UTF8)))
                    using (var producer = new Producer<Null, string>(config, null, new StringSerializer(Encoding.UTF8)))
                    {
                        consumer.Assign(new List<TopicPartitionOffset> {new TopicPartitionOffset(inputTopicName, 0, 0)});

                        while (!cancelled)
                        {
                            Message<Null, string> msg;
                            if (!consumer.Consume(out msg, TimeSpan.FromSeconds(1))) continue;
                    
                            logger.Info($"Got message | Topic: {msg.Topic} Partition: {msg.Partition} Offset: {msg.Offset} {msg.Value}");

                            // hurr durr... making expensive computations...
                            Thread.Sleep(TimeSpan.FromSeconds(15));

                            var deliveryReport = producer.ProduceAsync(outputTopicName, null, msg.Value);
                        
                            deliveryReport.ContinueWith(task =>
                            {
                                // TODO сделать, чтобы эта запись попадала в логи, если сервис останавливают во время обработки 
                                logger.Info($"Sent result | Partition: {task.Result.Partition}, Offset: {task.Result.Offset}");
                            });
                        }
                    }                    
                });
            workerThread.Start();
        }

        public void Stop()
        {
            cancelled = true;
            workerThread.Join();
        }
    }
}