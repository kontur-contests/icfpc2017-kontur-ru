from pykafka import KafkaClient
import json

def execute_tasks(tasks):
    client = KafkaClient(hosts="icfpc-broker.dev.kontur.ru:9092")
    tasks = client.topics[b'tasks']
    results = client.topics[b'results']
    num_tasks = len(tasks)

    with tasks.get_sync_producer() as producer:
        for task in range(tasks):
            producer.produce(json.dumps(task).encode('utf-8'))

    consumer = results.get_balanced_consumer(
        consumer_group=b'icfpc2017-foreman',
        auto_commit_enable=True,
        zookeeper_connect='icfpc-broker.dev.kontur.ru:2181'
    )

    results = []

    remaining_answers = num_tasks
    for message in consumer:
        remaining_answers -= 1
        if message is not None:
            results.append()
            print (message.value)
        if not remaining_answers:
            break
