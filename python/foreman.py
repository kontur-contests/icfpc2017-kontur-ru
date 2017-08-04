from pykafka import KafkaClient

client = KafkaClient(hosts="icfpc-broker.dev.kontur.ru:9092")
tasks = client.topics['tasks']
results = client.topics['results']
num_tasks = 5

with tasks.get_sync_producer() as producer:
    for _ in xrange(num_tasks):
        producer.produce("{'Players': [{'Name': '0', 'Params': {'Param': 0.0}}, {'Name': '1', 'Params': {'Param': 0.2}}, {'Name': '2', 'Params': {'Param': 0.4}}, {'Name': '3', 'Params': {'Param': 0.6000000000000001}}]}")

consumer = results.get_balanced_consumer(
    consumer_group='icfpc2017-foreman',
    auto_commit_enable=True,
    zookeeper_connect='icfpc-broker.dev.kontur.ru:2181'
)
remaining_answers = num_tasks
for message in consumer:
    remaining_answers -= 1
    if message is not None:
        print message.value
    if not remaining_answers:
        break
