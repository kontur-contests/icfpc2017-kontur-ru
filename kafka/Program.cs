using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace kafka
{
    class Program
    {
      
    
        static void Main(string[] args)
        {
            var inputTopicName = "test-topic";
            var outputTopicName = "test-topic";

            var config = new Dictionary<string, object>
                    {
                        { "group.id", "TEST-GROUP-ID" },
                        { "bootstrap.servers", "icfpc-broker.dev.kontur.ru" }
                    };

            using (var consumer = new Consumer<Null, string>(config, null, new StringDeserializer(Encoding.UTF8)))
            using (var producer = new Producer<Null, string>(config, null, new StringSerializer(Encoding.UTF8)))
            {
                //consumer.OnPartitionEOF += (_, end) => logger.Info(
                //    $"Reached end of topic {end.Topic} partition {end.Partition}, next message will be at offset {end.Offset}");

                //consumer.OnError += (_, error) => logger.Error($"Error: {error}");

                consumer.OnPartitionsAssigned += (_, partitions) =>
                {
                    //logger.Info($"Assigned partitions: [{string.Join(", ", partitions)}], member id: {consumer.MemberId}");
                    consumer.Assign(partitions);
                };

                consumer.OnPartitionsRevoked += (_, partitions) =>
                {
                   // logger.Info($"Revoked partitions: [{string.Join(", ", partitions)}]");
                    consumer.Unassign();
                };

                //consumer.OnStatistics += (_, json) => logger.Info($"Statistics: {json}");

                consumer.Subscribe(inputTopicName);

                //for (int i=0;i<10;i++)
                //{
                //    producer.ProduceAsync(outputTopicName, null, i.ToString());
                //}
                //producer.Flush(TimeSpan.FromSeconds(10));

                while (true)
                {
                    Message<Null, string> msg;
                    if (!consumer.Consume(out msg, TimeSpan.FromSeconds(1))) continue;

                    //logger.Info($"Got message | Topic: {msg.Topic} Partition: {msg.Partition} Offset: {msg.Offset} {msg.Value}");



                    Console.WriteLine(msg.Value);
                    //var task = JsonConvert.DeserializeObject<Task>(msg.Value);
                    //var result = player.Play(task);
                    //var resultString = JsonConvert.SerializeObject(result);



                 
                }

                
            }
        }
    }
}
