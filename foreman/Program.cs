using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using lib.Arena;
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
            var lockedPorts = new Dictionary<int, DateTime>();
            
            using (var producer = new Producer<Null, string>(config, null, new StringSerializer(Encoding.UTF8)))
            {
                while (!Console.KeyAvailable)
                {
                    ArenaMatch match;

                    do
                    {
                        match = OnlineArenaRunner.GetNextMatch();
                        Thread.Sleep(1000);
                    } while (lockedPorts.ContainsKey(match.Port) && lockedPorts[match.Port] > DateTime.UtcNow);
                    
                    lockedPorts[match.Port] = DateTime.UtcNow + TimeSpan.FromMinutes(1); 
                    
                    var deliveryReport = producer.ProduceAsync("matches", null, match.Port.ToString());

                    deliveryReport.ContinueWith(
                        x =>
                        {
                            logger.Info(
                                $"Sent port {match.Port} | Partition: {x.Result.Partition}, Offset: {x.Result.Offset}");
                        });
                }
                
                producer.Flush(TimeSpan.FromSeconds(10));
            }
        }
    }
}