from pykafka import KafkaClient

client = KafkaClient(hosts="icfpc-broker.dev.kontur.ru:9092")
tasks = client.topics['tasks']
results = client.topics['results']

with tasks.get_sync_producer() as producer:
    producer.produce("{'Players': [{'Name': '0', 'Params': {'Param': 0.0}}, {'Name': '1', 'Params': {'Param': 0.2}}, {'Name': '2', 'Params': {'Param': 0.4}}, {'Name': '3', 'Params': {'Param': 0.6000000000000001}}]}")
    producer.produce("{'Players': [{'Name': '0', 'Params': {'Param': 0.0}}, {'Name': '1', 'Params': {'Param': 0.2}}, {'Name': '2', 'Params': {'Param': 0.4}}, {'Name': '3', 'Params': {'Param': 0.6000000000000001}}]}")
    producer.produce("{'Players': [{'Name': '0', 'Params': {'Param': 0.0}}, {'Name': '1', 'Params': {'Param': 0.2}}, {'Name': '2', 'Params': {'Param': 0.4}}, {'Name': '3', 'Params': {'Param': 0.6000000000000001}}]}")
    producer.produce("{'Players': [{'Name': '0', 'Params': {'Param': 0.0}}, {'Name': '1', 'Params': {'Param': 0.2}}, {'Name': '2', 'Params': {'Param': 0.4}}, {'Name': '3', 'Params': {'Param': 0.6000000000000001}}]}")
    producer.produce("{'Players': [{'Name': '0', 'Params': {'Param': 0.0}}, {'Name': '1', 'Params': {'Param': 0.2}}, {'Name': '2', 'Params': {'Param': 0.4}}, {'Name': '3', 'Params': {'Param': 0.6000000000000001}}]}")

consumer = results.get_balanced_consumer(
    consumer_group='icfpc2017-foreman',
    auto_commit_enable=True,
    zookeeper_connect='icfpc-broker.dev.kontur.ru:2181'
)
for message in consumer:
    if message is not None:
        print message.value
