using System;
using System.Collections.Generic;
using System.Text;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using lib.OnlineRunner;
using NLog;

namespace foreman
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var logger = LogManager.GetCurrentClassLogger();
            var config = new Dictionary<string, object>
            {
                { "group.id", "icfpc2017-foreman" },
                { "bootstrap.servers", "icfpc-broker.dev.kontur.ru" }
            };
            using (var producer = new Producer<Null, string>(config, null, new StringSerializer(Encoding.UTF8)))
            {
                var match = OnlineArenaRunner.GetNextMatch();
                var deliveryReport = producer.ProduceAsync("matches", null, match.Port.ToString());

                deliveryReport.ContinueWith(
                    x =>
                    {
                        logger.Info(
                            $"Sent result | Partition: {x.Result.Partition}, Offset: {x.Result.Offset}");
                    });
                
                producer.Flush(TimeSpan.FromSeconds(10));
            }
        }
    }
}